using Microsoft.Extensions.DependencyInjection;

namespace NetCache
{
    internal class NetCacheBuilder : INetCacheBuilder
    {
        public NetCacheBuilder(IServiceCollection services, ICacheProxyGenerator generator)
        {
            Services = services;
            Generator = generator;
        }

        public IServiceCollection Services { get; }
        public ICacheProxyGenerator Generator { get; }
    }
}
