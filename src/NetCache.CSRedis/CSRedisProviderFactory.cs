using CSRedis;

namespace NetCache
{
    public class CSRedisProviderFactory : ICacheProviderFactory
    {
        private readonly CSRedisClient _client;

        public CSRedisProviderFactory(CSRedisClient client) => _client = client;

        public ICacheProvider Create(string name) => new CSRedisProvider(name, _client);
    }
}
