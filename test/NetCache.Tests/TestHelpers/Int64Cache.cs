using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache.Tests.TestHelpers
{
    [Cache(CacheName, TtlSecond = DefaultTtl)]
    public abstract class Int64Cache
    {
        public const string CacheName = "long";
        public const int DefaultTtl = 10;

        protected Int64Cache(object any) { }

        public virtual long? Get(string key) => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public virtual long? Get(string key, TimeSpan ttl, CancellationToken cancellationToken) => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public abstract Task<ICacheResult<long>> GetAsync(string key);
        public abstract Task<long> GetAsync(string key, Func<long> func);
        public abstract Task<long> GetAsync(string key, TimeSpan ttl, CancellationToken cancellationToken, Func<string, TimeSpan, CancellationToken, ValueTask<long>> func);
        public abstract Task<long> GetAsync(string key, Func<ValueTask<long>> func);
        public abstract void Set(string key, string value, int ttlSecond);
        public abstract void Set(string key, int ttlSecond, string value);
        public abstract void Set(string key, string value, int ttlSecond, CancellationToken cancellationToken);
        public abstract ValueTask<bool> SetAsync(string key, string value);
        public abstract ValueTask SetAsync(string key, string value, CancellationToken cancellationToken);
        public abstract void Delete(string key);
        public abstract Task RemoveAsync(string key);

        public abstract void Set(string key, int value, byte ttl);
        public abstract void Set(string key, int value, sbyte ttl);
        public abstract void Set(string key, int value, short ttl);
        public abstract void Set(string key, int value, ushort ttl);
        public abstract void Set(string key, int value, uint ttl);
        public abstract void Set(string key, int? value, long expiry);
        public abstract void Set(string key, int value, ulong expiry);
        public abstract void Set(string key, int value, decimal expiry);
        public abstract void Set(string key, int value, float expiry);
        public abstract void Set(string key, int value, double expiry);
        public abstract void Set(string key, int value, TimeSpan expiry);
        public abstract void Set(string key, int value, DateTime expiry);
        public abstract void Set(string key, int value, DateTimeOffset ttl);
        public abstract void Set(string key, int ttl, [CacheExpiry] int value);

        public abstract void Set(string key, int value, byte? ttl);
        public abstract void Set(string key, int? value, sbyte? ttl);
        public abstract void Set(string key, int value, short? ttl);
        public abstract void Set(string key, int value, ushort? ttl);
        public abstract void Set(string key, int? value, uint? ttl);
        public abstract void Set(string key, int value, long? expiry);
        public abstract void Set(string key, int value, ulong? expiry);
        public abstract void Set(string key, int value, decimal? expiry);
        public abstract void Set(string key, int value, float? expiry);
        public abstract void Set(string key, int value, double? expiry);
        public abstract void Set(string key, int value, TimeSpan? expiry);
        public abstract void Set(string key, int value, DateTime? expiry);
        public abstract void Set(string key, int value, DateTimeOffset? ttl);
        public abstract void Set(string key, string p, int? value);
        public abstract void Set(string key, int ttl, [CacheExpiry] int? value);
    }
}
