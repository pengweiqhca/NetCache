using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache.CacheProviderTests
{
    public abstract class LockTimeoutTest : MarshalByRefObject
    {
        public async Task<List<bool>> TestAsync(string key, TimeSpan timeout)
        {
            using var client = CreateFactory().CreateLock("Test", key);

            using var cts = new CancellationTokenSource(timeout);
            var list = new List<bool>();
            var random = new Random();
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(random.Next(10, 50), cts.Token).ConfigureAwait(false);

                    list.Add(await client.LockAsync(30, cts.Token).ConfigureAwait(false));

                    await Task.Delay(random.Next(10, 50), cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    await client.ReleaseAsync().ConfigureAwait(false);
                }
            }

            return list;
        }

        protected abstract IDistributedLockFactory CreateFactory();

        public List<bool> Test(string key, TimeSpan timeout) => TestAsync(key, timeout).GetAwaiter().GetResult();
    }
}
