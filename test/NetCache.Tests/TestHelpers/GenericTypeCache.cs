namespace NetCache.Tests.TestHelpers
{
    public abstract class GenericTypeCache<T>
    {
        public virtual T? Get(string key) => default;
        public abstract void Set(string key, T value, int ttl);
    }
}
