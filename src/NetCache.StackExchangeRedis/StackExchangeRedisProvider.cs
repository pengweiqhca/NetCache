using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public class StackExchangeRedisProvider : IMultipleKeyCacheProvider
    {
        private readonly IDatabase _database;
        public string Name { get; }

        public StackExchangeRedisProvider(IDatabase database, string name)
        {
            Name = name;
            _database = database;
        }

        public ReadOnlyMemory<byte>? Get(string key, CancellationToken cancellationToken)
        {
            var value = _database.StringGet(key);
#if NET45
            return value.IsNull ? new ReadOnlyMemory<byte>?() : (byte[])value;
#else
            return value.IsNull ? new ReadOnlyMemory<byte>?() : value;
#endif
        }

        public async ValueTask<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken)
        {
            var value = await _database.StringGetAsync(key).ConfigureAwait(false);
#if NET45
            return value.IsNull ? new ReadOnlyMemory<byte>?() : (byte[])value;
#else
            return value.IsNull ? new ReadOnlyMemory<byte>?() : value;
#endif
        }

        public bool Set(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
#if NET45
            _database.StringSet(key, value.ToArray(), expiry, (StackExchange.Redis.When)when);
#else
            _database.StringSet(key, value, expiry, (StackExchange.Redis.When)when);
#endif
        public async ValueTask<bool> SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
#if NET45
            await _database.StringSetAsync(key, value.ToArray(), expiry, (StackExchange.Redis.When)when).ConfigureAwait(false);
#else
            await _database.StringSetAsync(key, value, expiry, (StackExchange.Redis.When)when).ConfigureAwait(false);
#endif
        public bool Remove(string key, CancellationToken cancellationToken) =>
            _database.KeyDelete(key);

        public async ValueTask<bool> RemoveAsync(string key, CancellationToken cancellationToken) =>
            await _database.KeyDeleteAsync(key).ConfigureAwait(false);

        public IReadOnlyList<ReadOnlyMemory<byte>?> Get(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var values = _database.StringGet(keys.Select(key => (RedisKey)key).ToArray());
#if NET45
            return values.Select(value => value.IsNull ? new ReadOnlyMemory<byte>?() : (byte[])value).ToArray();
#else
            return values.Select(value => value.IsNull ? new ReadOnlyMemory<byte>?() : value).ToArray();
#endif
        }

        public async ValueTask<IReadOnlyList<ReadOnlyMemory<byte>?>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var values = await _database.StringGetAsync(keys.Select(key => (RedisKey)key).ToArray()).ConfigureAwait(false);
#if NET45
            return values.Select(value => value.IsNull ? new ReadOnlyMemory<byte>?() : (byte[])value).ToArray();
#else
            return values.Select(value => value.IsNull ? new ReadOnlyMemory<byte>?() : value).ToArray();
#endif
        }

        public IReadOnlyList<bool> Set(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            var task = SetAsync(values, expiry, when, cancellationToken);

            return task.IsCompleted ? task.GetAwaiter().GetResult() : task.AsTask().GetAwaiter().GetResult();
        }

        public async ValueTask<IReadOnlyList<bool>> SetAsync(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            var batch = _database.CreateBatch();
#if NET45
            var tasks = values.Select(kv => batch.StringSetAsync(kv.Key, kv.Value.ToArray(), expiry, (StackExchange.Redis.When)when));
#else
            var tasks = values.Select(kv => batch.StringSetAsync(kv.Key, kv.Value, expiry, (StackExchange.Redis.When)when));
#endif
            batch.Execute();

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public long Remove(IEnumerable<string> keys, CancellationToken cancellationToken) =>
            _database.KeyDelete(keys.Select(key => (RedisKey)key).ToArray());

        public ValueTask<long> RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken) =>
            new(_database.KeyDeleteAsync(keys.Select(key => (RedisKey)key).ToArray()));
    }
}
