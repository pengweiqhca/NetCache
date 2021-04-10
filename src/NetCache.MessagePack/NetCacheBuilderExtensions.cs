using System;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
#if NETFRAMEWORK
        public static INetCacheBuilder UseMessagePack(this INetCacheBuilder builder)
#else
        public static INetCacheBuilder UseMessagePack(this INetCacheBuilder builder, MessagePackSerializerOptions? options = null)
#endif
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
#if NETFRAMEWORK
            builder.Services.Replace(new ServiceDescriptor(typeof(IValueSerializer), new MessagePackSerializer()));
#else
            builder.Services.Replace(new ServiceDescriptor(typeof(IValueSerializer), new MessagePackSerializer(options)));
#endif
            return builder;
        }
    }
}
