using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace NetCache.CacheProviderTests
{
    public class StackExchangeRedisLockTest : DistributedLockTest
#if NETFRAMEWORK
        <StackExchangeRedisLockTimeoutTest>
#endif
    {
        public static IDistributedLockFactory Factory = new StackExchangeRedisLockFactory(ConnectionMultiplexer.Connect("localhost"));

        public StackExchangeRedisLockTest() : base(Factory, new LoggerFactory()) { }
    }
#if NETFRAMEWORK
    public class StackExchangeRedisLockTimeoutTest : LockTimeoutTest
    {
        protected override IDistributedLockFactory CreateFactory() => StackExchangeRedisLockTest.Factory;
    }
#endif
}
