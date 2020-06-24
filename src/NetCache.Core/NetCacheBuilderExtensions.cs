using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseCacheProviderFactory<T>(this INetCacheBuilder builder) where T : class, ICacheProviderFactory
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(ServiceDescriptor.Scoped<ICacheProviderFactory, T>());

            return builder;
        }

        public static INetCacheBuilder UseKeyFormatter<T>(this INetCacheBuilder builder) where T : class, IKeyFormatter
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(ServiceDescriptor.Scoped<IKeyFormatter, T>());

            return builder;
        }

        public static INetCacheBuilder UseValueSerializer<T>(this INetCacheBuilder builder)
            where T : class, IValueSerializer
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(ServiceDescriptor.Scoped<IValueSerializer, T>());

            return builder;
        }
    }
}
