namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseStackExchangeRedis(this INetCacheBuilder builder) =>
            builder.UseCacheProviderFactory<StackExchangeRedisProviderFactory>()
                .UseDistributedLockFactory<StackExchangeRedisLockFactory>();
    }
}
