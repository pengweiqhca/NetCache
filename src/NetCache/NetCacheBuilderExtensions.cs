using Microsoft.Extensions.DependencyInjection;
using System;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder AddProxy<T>(this INetCacheBuilder builder, Func<IServiceProvider, object[]>? getParameters = null,
            ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : class
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Add(getParameters == null && builder is NetCacheBuilder ncb
                ? new ServiceDescriptor(typeof(T), ncb.Generator.CreateProxyType<T>(), lifetime)
                : new ServiceDescriptor(typeof(T), provider => ActivatorUtilities.CreateInstance(provider,
                        provider.GetRequiredService<ICacheProxyGenerator>().CreateProxyType<T>(),
#if NET45
                        getParameters == null ? new object[0] : getParameters(provider)),
#else
                        getParameters == null ? Array.Empty<object>() : getParameters(provider)),
#endif
                    lifetime));

            return builder;
        }
    }
}
