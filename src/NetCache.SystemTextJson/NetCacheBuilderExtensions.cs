using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseSystemTextJsonSerializer(this INetCacheBuilder builder, JsonSerializerOptions? options = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(new ServiceDescriptor(typeof(IValueSerializer), new SystemTextJsonSerializer(options)));

            return builder;
        }
    }
}
