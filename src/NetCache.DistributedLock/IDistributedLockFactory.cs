namespace NetCache
{
    public interface IDistributedLockFactory
    {
        IDistributedLock CreateLock(string name, string key);
    }
}
