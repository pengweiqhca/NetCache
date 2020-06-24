namespace NetCache
{
    public static class NetCacheBuilderExtensions
    {
        public static INetCacheBuilder UseGoogleProtobufSerializer(this INetCacheBuilder builder) =>
            builder.UseValueSerializer<GoogleProtobufSerializer>();
    }
}
