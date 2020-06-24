using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace NetCache.CacheProviderTests
{
    public class MemoryCacheProviderTest : CacheProviderTest
    {
        private static readonly ICacheProviderFactory Factory = new MemoryCacheProviderFactory(new MemoryCache(Options.Create(new MemoryCacheOptions())));

        public MemoryCacheProviderTest() : base(Factory, false) { }
    }
}
