using System;

namespace NetCache.Tests.TestHelpers
{
    public abstract class StringCache
    {
        public virtual string Get(string key) => DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString("D", null);
        public abstract void Set(string key, string value, int ttl);
        [CacheMethod(CacheOperation.Remove)]
        public abstract void Delete(string key);
    }
}
