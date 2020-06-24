using Microsoft.IO;

namespace NetCache
{
    public class CreateProxyOptions
    {
        public CreateProxyOptions(ICacheProviderFactory cacheProviderFactory,
            IDistributedLockFactory distributedLockFactory,
            IKeyFormatter keyFormatter,
            IValueSerializer valueSerializer,
            RecyclableMemoryStreamManager streamManager,
            CacheOptions options)
        {
            CacheProviderFactory = cacheProviderFactory;
            DistributedLockFactory = distributedLockFactory;
            KeyFormatter = keyFormatter;
            ValueSerializer = valueSerializer;
            StreamManager = streamManager;
            Options = options;
        }

        public ICacheProviderFactory CacheProviderFactory { get; }
        public IDistributedLockFactory DistributedLockFactory { get; }
        public IKeyFormatter KeyFormatter { get; }
        public IValueSerializer ValueSerializer { get; }
        public RecyclableMemoryStreamManager StreamManager { get; }
        public CacheOptions Options { get; }
    }
}
