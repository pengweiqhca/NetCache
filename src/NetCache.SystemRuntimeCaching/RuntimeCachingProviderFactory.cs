using System.Runtime.Caching;

namespace NetCache
{
    public class RuntimeCachingProviderFactory : ICacheProviderFactory
    {
        private readonly ObjectCache _cache;

        public RuntimeCachingProviderFactory() : this(MemoryCache.Default) { }

        public RuntimeCachingProviderFactory(ObjectCache cache) => _cache = cache;

        public ICacheProvider Create(string name) => new RuntimeCachingProvider(name, _cache);
    }
}
