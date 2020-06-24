using CSRedis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace NetCache.CacheProviderTests
{
    public class CSRedisProviderTest : DistributedLockTest
#if NETFRAMEWORK
        <CSRedisLockTimeoutTest>
#endif
    {
        public static IDistributedLockFactory Factory = new CSRedisLockFactory(new CSRedisClient("localhost"));

        public CSRedisProviderTest() : base(Factory, new LoggerFactory()) { }
    }
#if NETFRAMEWORK
    public class CSRedisLockTimeoutTest : LockTimeoutTest
    {
        protected override IDistributedLockFactory CreateFactory() => CSRedisProviderTest.Factory;
    }
#endif
}
