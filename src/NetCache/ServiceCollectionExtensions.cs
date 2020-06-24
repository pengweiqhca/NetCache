using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IO;

namespace NetCache
{
    public static class ServiceCollectionExtensions
    {
        public static INetCacheBuilder AddNetCache(this IServiceCollection services, Action<CacheOptions>? configureAction = null) =>
            services.AddNetCache(new CacheProxyGenerator(), configureAction);

        public static INetCacheBuilder AddNetCache(this IServiceCollection services, ICacheProxyGenerator generator, Action<CacheOptions>? configureAction = null)
        {
            if (configureAction != null) services.Configure(configureAction);

            services.AddSingleton(generator);

            services.TryAddSingleton(new RecyclableMemoryStreamManager());

            var builder = new NetCacheBuilder(services, generator);

            builder.UseKeyFormatter<KeyFormatter>();

            return builder;
        }
    }
}
