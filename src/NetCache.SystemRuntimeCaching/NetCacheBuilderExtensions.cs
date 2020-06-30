namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseRuntimeCaching(this INetCacheBuilder builder) =>
            builder.UseCacheProviderFactory<RuntimeCachingProviderFactory>()
                .UseDistributedLockFactory<LocalLockFactory>();
    }
}
