using BenchmarkDotNet.Attributes;
using Microsoft.IO;

namespace NetCache.Benchmark
{
    public class Cache
    {
        private static readonly CreateProxyOptions CreateProxyOptions = new CreateProxyOptions(
            new RuntimeCachingProviderFactory(),
            new LocalLockFactory(),
            new KeyFormatter(),
            new NewtonsoftJsonSerializer(),
            new RecyclableMemoryStreamManager(),
            new CacheOptions());
        private static readonly ICacheProxyGenerator Generator = new CacheProxyGenerator();

        [Benchmark]
        public void Get() => Generator.CreateProxy<Test>(CreateProxyOptions).Get(3);
    }

    public abstract class Test
    {
        public abstract string Get(int key);
    }
}
