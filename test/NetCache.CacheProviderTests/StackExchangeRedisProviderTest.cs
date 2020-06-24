using StackExchange.Redis;

namespace NetCache.CacheProviderTests
{
    public class StackExchangeRedisProviderTest : CacheProviderTest
    {
        private static readonly ICacheProviderFactory Factory = new StackExchangeRedisProviderFactory(ConnectionMultiplexer.Connect("localhost"));

        public StackExchangeRedisProviderTest() : base(Factory, true) { }
    }
}
