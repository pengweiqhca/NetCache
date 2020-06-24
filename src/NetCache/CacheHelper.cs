using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    internal class CacheHelper
    {
        private readonly IDistributedLockFactory _factory;
        private readonly IKeyFormatter _formatter;
        private readonly RecyclableMemoryStreamManager _manager;
        private readonly IValueSerializer _serializer;
        private readonly string _name;
        private readonly CacheOptions _options;
        private readonly ICacheProvider _provider;

        public CacheHelper(string name,
            ICacheProviderFactory cpf,
            IDistributedLockFactory factory,
            IKeyFormatter formatter,
            IValueSerializer serializer,
            RecyclableMemoryStreamManager manager,
            IOptionsMonitor<CacheOptions> options)
        {
            if (cpf == null) throw new ArgumentNullException(nameof(cpf));
            if (options == null) throw new ArgumentNullException(nameof(options));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _formatter = formatter;
            _manager = manager;

            _options = options.CurrentValue;
            _provider = cpf.Create($"{_options.KeyPrefix}/Cache/{_name = name}");
            _serializer = _options.CompressValue ? new CompressionSerializer(serializer, manager) : serializer;
        }

        public CacheHelper(string name,
            ICacheProviderFactory cpf,
            IDistributedLockFactory factory,
            IKeyFormatter formatter,
            IValueSerializer serializer,
            RecyclableMemoryStreamManager manager,
            CacheOptions options)
        {
            if (cpf == null) throw new ArgumentNullException(nameof(cpf));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _formatter = formatter;
            _manager = manager;
            _provider = cpf.Create($"{_options.KeyPrefix}/Cache/{_name = name}");
            _serializer = _options.CompressValue ? new CompressionSerializer(serializer, manager) : serializer;
        }

        private TimeSpan GetDefaultTtl() => TimeSpan.FromSeconds(_options.Ttl.TryGetValue(_name, out var ttl) ? ttl : _options.DefaultTtl);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TimeSpan GetTtl(TimeSpan expiry)
        {
            if (expiry.TotalSeconds < 1) return GetDefaultTtl();

            return expiry.TotalSeconds > _options.MaxTll ? TimeSpan.FromSeconds(_options.MaxTll) : expiry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string ParseKey<TK>(TK key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var str = _formatter.Format(key);

            if (_provider.Name.Length + 1 + str.Length > _options.MaxKeyLength)
                throw new InvalidOperationException(string.Format(null, Res.Key_is_Too_Long, $"{_provider.Name}/{str}", _options.MaxKeyLength));

            return str;
        }

        private ReadOnlyMemory<byte> Serialize<TV>(TV value)
        {
            Stream stream = _manager.GetStream();
            try
            {
                _serializer.Serialize(value, ref stream);

                if (stream.Length > _options.MaxValueLength)
                    throw new InvalidOperationException(string.Format(null, Res.Value_is_Too_Large, stream.Length, _options.MaxKeyLength));

                if (stream is MemoryStream ms) return ms.ToArray();
                if (stream is ReadOnlyMemoryStream roms) return roms.Memory;

                using var s = new MemoryStream();
                stream.CopyTo(s);
                return s.ToArray();
            }
            finally
            {
                stream.Dispose();
            }
        }

        [return: MaybeNull]
        public TV Get<TK, TV>(TK key, CancellationToken token) =>
            _serializer.Deserialize<TV>(_provider.Get(ParseKey(key), token));

        public async ValueTask<TV> GetAsync<TK, TV>(TK key, CancellationToken token) =>
            _serializer.Deserialize<TV>((await _provider.GetAsync(ParseKey(key), token).ConfigureAwait(false)))!;

        [return: MaybeNull]
        public TV GetOrSet<TK, TV>(TK key, Func<TK, TimeSpan, CancellationToken, TV> func, TimeSpan expiry, CancellationToken token)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var formattedKey = ParseKey(key);

            var result = _provider.Get(formattedKey, token);
            if (result.HasValue) return _serializer.Deserialize<TV>(result.Value);

            using var @lock = _factory.CreateLock($"{_options.KeyPrefix}/Lock/{_name}", formattedKey);

            if (!@lock.Lock(_options.LockTimeout * 1000, token)) throw new TimeoutException(Res.Wait_Lock_Timeout);

            result = _provider.Get(formattedKey, token)!;
            if (result.HasValue) return _serializer.Deserialize<TV>(result.Value);

            var value = func(key, expiry, token);

            _provider.Set(formattedKey, Serialize(value), GetTtl(expiry), When.Always, token);

            @lock.Release();

            return value;
        }

        public async ValueTask<TV> GetOrSetAsync<TK, TV>(TK key, Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> func, TimeSpan expiry, CancellationToken token)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var formattedKey = ParseKey(key);

            var result = await _provider.GetAsync(formattedKey, token).ConfigureAwait(false);
            if (result.HasValue) return _serializer.Deserialize<TV>(result.Value)!;

            var @lock = _factory.CreateLock($"{_options.KeyPrefix}/Lock/{_name}", formattedKey);
            try
            {
                if (!await @lock.LockAsync(_options.LockTimeout * 1000, token).ConfigureAwait(false))
                    throw new TimeoutException(Res.Wait_Lock_Timeout);

                result = await _provider.GetAsync(formattedKey, token).ConfigureAwait(false);
                if (result.HasValue) return _serializer.Deserialize<TV>(result.Value)!;

                var value = await func(key, expiry, token).ConfigureAwait(false);

                await _provider.SetAsync(formattedKey, Serialize(value), GetTtl(expiry), When.Always, token).ConfigureAwait(false);

                await @lock.ReleaseAsync().ConfigureAwait(false);

                return value;
            }
            finally
            {
                await @lock.DisposeAsync().ConfigureAwait(false);
            }
        }

        public bool Set<TK, TV>(TK key, TV value, TimeSpan expiry, When when, CancellationToken token) =>
            _provider.Set(ParseKey(key), Serialize(value), GetTtl(expiry), when, token);

        public ValueTask<bool> SetAsync<TK, TV>(TK key, TV value, TimeSpan expiry, When when, CancellationToken token) =>
            _provider.SetAsync(ParseKey(key), Serialize(value), GetTtl(expiry), when, token);

        public bool Remove<TK>(TK key, CancellationToken token) => _provider.Remove(ParseKey(key), token);

        public ValueTask<bool> RemoveAsync<TK>(TK key, CancellationToken token) => _provider.RemoveAsync(ParseKey(key), token);

        public IReadOnlyDictionary<TK, TV> Gets<TK, TV>(IReadOnlyList<TK> keys, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<TK, TV>();
            if (keys.Count == 0) return new ReadOnlyDictionary<TK, TV>(dic);

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var values = mkcp.Get((IEnumerable<string>)keys.Select(ParseKey), cancellationToken);

                if (values.Count != keys.Count)
                    throw new InvalidOperationException(string.Format(null, Res.Output_Length_Not_Match_Input_Length, values.Count, keys.Count));

                for (var index = 0; index < keys.Count; index++)
                    dic[keys[index]] = _serializer.Deserialize<TV>(values[index])!;
            }
            else
                foreach (var key in keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dic[key] = _serializer.Deserialize<TV>(_provider.Get(ParseKey(key), cancellationToken))!;
                }

            return new ReadOnlyDictionary<TK, TV>(dic);
        }

        public async ValueTask<IReadOnlyDictionary<TK, TV>> GetsAsync<TK, TV>(IReadOnlyList<TK> keys, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<TK, TV>();
            if (keys.Count == 0) return new ReadOnlyDictionary<TK, TV>(dic);

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var values = await mkcp.GetAsync((IEnumerable<string>)keys.Select(ParseKey), cancellationToken).ConfigureAwait(false);

                if (values.Count != keys.Count)
                    throw new InvalidOperationException(string.Format(null, Res.Output_Length_Not_Match_Input_Length, values.Count, keys.Count));

                for (var index = 0; index < keys.Count; index++)
                    dic[keys[index]] = _serializer.Deserialize<TV>(values[index])!;
            }
            else
                foreach (var key in keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dic[key] = _serializer.Deserialize<TV>(await _provider.GetAsync(ParseKey(key), cancellationToken).ConfigureAwait(false))!;
                }

            return new ReadOnlyDictionary<TK, TV>(dic);
        }

        public IReadOnlyDictionary<TK, bool> Sets<TK, TV>(IReadOnlyDictionary<TK, TV> values, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<TK, bool>();
            if (values.Count == 0) return new ReadOnlyDictionary<TK, bool>(dic);

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                var keys = values.Keys.ToArray();

                cancellationToken.ThrowIfCancellationRequested();

                var result = mkcp.Set(keys.ToDictionary(ParseKey, key => Serialize(values[key])),
                    GetTtl(expiry), when, cancellationToken);

                if (result.Count != keys.Length)
                    throw new InvalidOperationException(string.Format(null, Res.Output_Length_Not_Match_Input_Length, result.Count, keys.Length));

                for (var index = 0; index < keys.Length; index++)
                    dic[keys[index]] = result[index];
            }
            else
                foreach (var kv in values)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dic[kv.Key] = _provider.Set(ParseKey(kv.Key), Serialize(kv.Value), GetTtl(expiry), when, cancellationToken);
                }

            return dic;
        }

        public async ValueTask<IReadOnlyDictionary<TK, bool>> SetsAsync<TK, TV>(IReadOnlyDictionary<TK, TV> values, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<TK, bool>();
            if (values.Count == 0) return new ReadOnlyDictionary<TK, bool>(dic);

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                var keys = values.Keys.ToArray();

                cancellationToken.ThrowIfCancellationRequested();

                var result = await mkcp.SetAsync(keys.ToDictionary(ParseKey, key => Serialize(values[key])),
                    GetTtl(expiry), when, cancellationToken).ConfigureAwait(false);

                if (result.Count != keys.Length)
                    throw new InvalidOperationException(string.Format(null, Res.Output_Length_Not_Match_Input_Length, result.Count, keys.Length));

                for (var index = 0; index < keys.Length; index++)
                    dic[keys[index]] = result[index];
            }
            else
                foreach (var kv in values)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    dic[kv.Key] = await _provider.SetAsync(ParseKey(kv.Key), Serialize(kv.Value), GetTtl(expiry), when, cancellationToken).ConfigureAwait(false);
                }

            return dic;
        }

        public long Removes<TK>(IReadOnlyCollection<TK> keys, CancellationToken cancellationToken)
        {
            if (keys.Count == 0) return 0;

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return mkcp.Remove((IEnumerable<string>)keys.Select(ParseKey), cancellationToken);
            }

            var count = 0L;
            foreach (var key in keys)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_provider.Remove(ParseKey(key), cancellationToken)) count++;
            }

            return count;
        }

        public async ValueTask<long> RemovesAsync<TK>(IReadOnlyCollection<TK> keys, CancellationToken cancellationToken)
        {
            if (keys.Count == 0) return 0;

            if (_provider is IMultipleKeyCacheProvider mkcp)
            {
                cancellationToken.ThrowIfCancellationRequested();

                return await mkcp.RemoveAsync((IEnumerable<string>)keys.Select(ParseKey), cancellationToken).ConfigureAwait(false);
            }

            var count = 0L;
            foreach (var key in keys)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await _provider.RemoveAsync(ParseKey(key), cancellationToken).ConfigureAwait(false)) count++;
            }

            return count;
        }
    }
}
