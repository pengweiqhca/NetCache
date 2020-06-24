using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseDistributedLockFactory<T>(this INetCacheBuilder builder) where T : class, IDistributedLockFactory
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(ServiceDescriptor.Scoped<IDistributedLockFactory, T>());

            return builder;
        }
    }
}
