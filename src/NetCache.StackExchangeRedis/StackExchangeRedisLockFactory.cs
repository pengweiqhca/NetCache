using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace NetCache
{
    public class StackExchangeRedisLockFactory : IDistributedLockFactory
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly ILogger<StackExchangeRedisLock>? _logger;

        public StackExchangeRedisLockFactory(ConnectionMultiplexer connection, ILogger<StackExchangeRedisLock>? logger = null)
        {
            _connection = connection;
            _logger = logger;
        }

        public IDistributedLock CreateLock(string name, string key) =>
            new StackExchangeRedisLock(key, _connection.GetDatabase().WithKeyPrefix(name), _logger);
    }
}
