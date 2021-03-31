namespace NetCache.Tests.TestHelpers
{
    public interface IGenericInterfaceCache<T>
    {
        public abstract void Set(string key, T value, int ttl);
    }
}
