using Microsoft.Extensions.Caching.Memory;

namespace NetCache
{
    public class MemoryCacheProviderFactory : ICacheProviderFactory
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheProviderFactory(IMemoryCache cache) => _cache = cache;

        public ICacheProvider Create(string name) => new MemoryCacheProvider(name, _cache);
    }
}
