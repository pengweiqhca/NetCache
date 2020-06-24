namespace NetCache
{
    public class LocalLockFactory : IDistributedLockFactory
    {
        public IDistributedLock CreateLock(string name, string key) => new LocalLock($"{name}/{key}");
    }
}
