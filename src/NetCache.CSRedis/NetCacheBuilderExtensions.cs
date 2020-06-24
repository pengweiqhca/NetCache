namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseCSRedis(this INetCacheBuilder builder) =>
            builder.UseCacheProviderFactory<CSRedisProviderFactory>()
                .UseDistributedLockFactory<CSRedisLockFactory>();
    }
}
