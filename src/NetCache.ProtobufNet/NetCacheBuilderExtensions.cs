namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseProtobufNetSerializer(this INetCacheBuilder builder) =>
            builder.UseValueSerializer<ProtobufNetSerializer>();
    }
}
