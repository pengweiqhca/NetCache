namespace NetCache.CacheProviderTests
{
    public class RuntimeCachingProviderTest : CacheProviderTest
    {
        public RuntimeCachingProviderTest() : base(new RuntimeCachingProviderFactory(), false) { }
    }
}
