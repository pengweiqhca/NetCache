using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    class CSRedisProvider : IMultipleKeyCacheProvider
    {
        private readonly CSRedisClient _client;

        public string Name { get; }

        public CSRedisProvider(string name, CSRedisClient client)
        {
            Name = name;
            _client = client;
        }

        private string GetKey(string key) => $"{Name}/{key}";

        public ReadOnlyMemory<byte>? Get(string key, CancellationToken cancellationToken)
        {
            var result = _client.Get<byte[]>(GetKey(key));

            // ReSharper disable once ConvertIfStatementToReturnStatement
            // ReSharper disable once UseNullPropagation
            if (result == null) return null;

            return result;
        }

        public async ValueTask<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken)
        {
            var result = await _client.GetAsync<byte[]>(GetKey(key)).ConfigureAwait(false);

            // ReSharper disable once ConvertIfStatementToReturnStatement
            // ReSharper disable once UseNullPropagation
            if (result == null) return null;

            return result;
        }

        private static RedisExistence? Convert(When when) => when switch
        {
            When.Exists => RedisExistence.Xx,
            When.NotExists => RedisExistence.Nx,
            _ => null
        };

        public bool Set(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
            _client.Set(GetKey(key), value.ToArray(), expiry, Convert(when));

        public ValueTask<bool> SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
            new ValueTask<bool>(_client.Set(GetKey(key), value.ToArray(), expiry, Convert(when)));

        public bool Remove(string key, CancellationToken cancellationToken) =>
            _client.Del(GetKey(key)) > 0;

        public async ValueTask<bool> RemoveAsync(string key, CancellationToken cancellationToken) =>
            await _client.DelAsync(GetKey(key)).ConfigureAwait(false) > 0;

        public IReadOnlyList<ReadOnlyMemory<byte>?> Get(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var values = _client.MGet<byte[]>(keys.Select(GetKey).ToArray());

            return values.Select(value => value == null ? new ReadOnlyMemory<byte>?() : new ReadOnlyMemory<byte>(value)).ToArray();
        }

        public async ValueTask<IReadOnlyList<ReadOnlyMemory<byte>?>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var values = await _client.MGetAsync<byte[]>(keys.Select(GetKey).ToArray()).ConfigureAwait(false);

            return values.Select(value => value == null ? new ReadOnlyMemory<byte>?() : new ReadOnlyMemory<byte>(value)).ToArray();
        }

        public IReadOnlyList<bool> Set(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            var pipe = _client.StartPipe();

            foreach (var kv in values)
                pipe.Set(GetKey(kv.Key), kv.Value.ToArray(), expiry, Convert(when));

            return pipe.EndPipe().Select(r => (bool)r).ToArray();
        }

        public ValueTask<IReadOnlyList<bool>> SetAsync(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
            new ValueTask<IReadOnlyList<bool>>(Set(values, expiry, when, cancellationToken));

        public long Remove(IEnumerable<string> keys, CancellationToken cancellationToken) =>
            _client.Del(keys.Select(GetKey).ToArray());

        public ValueTask<long> RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken) =>
            new ValueTask<long>(_client.DelAsync(keys.Select(GetKey).ToArray()));
    }
}
