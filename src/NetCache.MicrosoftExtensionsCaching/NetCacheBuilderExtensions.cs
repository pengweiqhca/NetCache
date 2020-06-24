namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseMemoryCache(this INetCacheBuilder builder) =>
            builder.UseCacheProviderFactory<MemoryCacheProviderFactory>()
                .UseDistributedLockFactory<LocalLockFactory>();
    }
}
