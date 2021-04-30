using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public class RuntimeCachingProvider : ICacheProvider
    {
        private readonly ObjectCache _cache;
        private readonly object _syncObj = new();

        public string Name { get; }

        public RuntimeCachingProvider(string name, ObjectCache cache)
        {
            _cache = cache;
            Name = name;
        }

        private string GetKey(string key) => $"{Name}/{key}";

        public ReadOnlyMemory<byte>? Get(string key, CancellationToken cancellationToken) =>
            (ReadOnlyMemory<byte>?)_cache.Get(GetKey(key));

        public ValueTask<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken) =>
            new(Get(key, cancellationToken));

        public bool Set(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken)
        {
            key = GetKey(key);

            if (when == When.Always) return SetValue();

            lock (_syncObj)
                return _cache.Get(key) == null
                    ? when != When.Exists && SetValue()
                    : when != When.NotExists && SetValue();

            bool SetValue()
            {
                _cache.Set(key, value, DateTimeOffset.Now.Add(expiry));

                return true;
            }
        }

        public ValueTask<bool> SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken) =>
            new(Set(key, value, expiry, when, cancellationToken));

        public bool Remove(string key, CancellationToken cancellationToken)
        {
            key = GetKey(key);

            if (_cache.Get(key) == null) return false;

            lock (_syncObj)
            {
                if (_cache.Get(key) == null) return false;

                _cache.Remove(key);

                return true;
            }
        }

        public ValueTask<bool> RemoveAsync(string key, CancellationToken cancellationToken) =>
            new(Remove(key, cancellationToken));
    }
}
