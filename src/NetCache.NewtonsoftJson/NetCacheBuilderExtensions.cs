using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;

namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseNewtonsoftJsonSerializer(this INetCacheBuilder builder, JsonSerializerSettings? settings = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.Replace(new ServiceDescriptor(typeof(IValueSerializer), new NewtonsoftJsonSerializer(settings)));

            return builder;
        }
    }
}
