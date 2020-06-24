using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace NetCache
{
    public class StackExchangeRedisProviderFactory : ICacheProviderFactory
    {
        private readonly ConnectionMultiplexer _connection;

        public StackExchangeRedisProviderFactory(ConnectionMultiplexer connection) => _connection = connection;

        public ICacheProvider Create(string name) =>
            new StackExchangeRedisProvider(_connection.GetDatabase().WithKeyPrefix(name), name);
    }
}
