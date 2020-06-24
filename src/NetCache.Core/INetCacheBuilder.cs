using Microsoft.Extensions.DependencyInjection;

namespace NetCache
{
    public interface INetCacheBuilder
    {
        IServiceCollection Services { get; }
    }
}
