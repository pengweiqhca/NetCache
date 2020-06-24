using CSRedis;
using Microsoft.Extensions.Logging;

namespace NetCache
{
    public class CSRedisLockFactory : IDistributedLockFactory
    {
        private readonly CSRedisClient _client;
        private readonly ILogger<CSRedisLock>? _logger;

        public CSRedisLockFactory(CSRedisClient client, ILogger<CSRedisLock>? logger = null)
        {
            _client = client;
            _logger = logger;
        }

        public IDistributedLock CreateLock(string name, string key) =>
            new CSRedisLock($"{name}/{key}", _client, _logger);
    }
}
