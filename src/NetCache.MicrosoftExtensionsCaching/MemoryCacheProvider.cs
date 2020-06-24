using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly object _syncObj = new object();

        public string Name { get; }

        public MemoryCacheProvider(string name, IMemoryCache cache)
        {
            Name = name;
            _cache = cache;
        }

        private string GetKey(string key) => $"{Name}/{key}";

        public ReadOnlyMemory<byte>? Get(string key, CancellationToken cancellationToken) =>
            _cache.TryGetValue<ReadOnlyMemory<byte>>(GetKey(key), out var value) ? value : default(ReadOnlyMemory<byte>?);

        public ValueTask<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken) =>
        new ValueTask<ReadOnlyMemory<byte>?>(Get(key, cancellationToken));

        public bool Set(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            key = GetKey(key);

            if (when == When.Always)
            {
                _cache.Set(key, value, expiry);

                return true;
            }

            lock (_syncObj)
            {
                if (_cache.TryGetValue(key, out _))
                {
                    if (when == When.NotExists) return false;

                    _cache.Set(key, value, expiry);

                    return true;
                }

                if (when == When.Exists) return false;

                _cache.Set(key, value, expiry);

                return true;
            }
        }

        public ValueTask<bool> SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
            new ValueTask<bool>(Set(key, value, expiry, when, cancellationToken));

        public bool Remove(string key, CancellationToken cancellationToken)
        {
            key = GetKey(key);

            if (!_cache.TryGetValue(key, out _)) return false;

            lock (_syncObj)
            {
                if (!_cache.TryGetValue(key, out _)) return false;

                _cache.Remove(key);

                return true;
            }
        }

        public ValueTask<bool> RemoveAsync(string key, CancellationToken cancellationToken) =>
            new ValueTask<bool>(Remove(key, cancellationToken));
    }
}
