using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetCache.CacheProviderTests
{
    public abstract class DistributedLockTest
#if NETFRAMEWORK
        <T> where T : LockTimeoutTest
#endif
    {
        private readonly IDistributedLockFactory _lockFactory;
        private readonly ILoggerFactory _loggerFactory;

        protected DistributedLockTest(IDistributedLockFactory lockFactory, ILoggerFactory loggerFactory)
        {
            _lockFactory = lockFactory;
            _loggerFactory = loggerFactory;
        }

        [Fact]
        public async Task ReleaseTestMethod()
        {
            using var @lock = _lockFactory.CreateLock("test", Guid.NewGuid().ToString());
            Assert.True(await @lock.LockAsync(60000).ConfigureAwait(false));

            await @lock.ReleaseAsync().ConfigureAwait(false);
        }

        [Fact]
        public void LockTestMethod()
        {
            var key = Guid.NewGuid().ToString();
            var timeout = 1000;
            EventWaitHandle handle = new AutoResetEvent(false);

            Task.WaitAll(Task.Run(() => LockTestMethod1(key, timeout, handle)), Task.Run(() => LockTestMethod2(key, timeout, handle)));
        }

        private async Task LockTestMethod1(string key, int timeout, EventWaitHandle handle)
        {
            using var @lock = _lockFactory.CreateLock("test", key);
            Assert.True(await @lock.LockAsync(timeout).ConfigureAwait(false));

            handle.Set();

            await Task.Delay(timeout * 2).ConfigureAwait(false);

            await @lock.ReleaseAsync().ConfigureAwait(false);
        }

        private async Task LockTestMethod2(string key, int timeout, EventWaitHandle handle)
        {
            using var @lock = _lockFactory.CreateLock("test", key);
            handle.WaitOne();

            Assert.False(await @lock.LockAsync(timeout).ConfigureAwait(false));
        }

        [Fact]
        public async Task DupleLockTestMethod()
        {
            const int timeout = 200;
            var logger = _loggerFactory.CreateLogger("DupleLockTestMethod");

            var lck = _lockFactory.CreateLock("test", "abc");
            Assert.True(await lck.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            try
            {
                Assert.False(await lck.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                Assert.True(false, "Fail");
            }
            catch (DistributedLockException ex)
            {
                Assert.Equal("Please don't repeat Wait before Release", ex.Message);
            }
            finally
            {
                await lck.ReleaseAsync().ConfigureAwait(false);
            }

            Assert.True(await lck.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            var lck2 = _lockFactory.CreateLock("test", "def");
            Assert.True(await lck2.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            try
            {
                Assert.False(await lck2.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                Assert.True(false, "Fail");
            }
            catch (DistributedLockException ex)
            {
                Assert.Equal("Please don't repeat Wait before Release", ex.Message);
            }
            finally
            {
                await lck.ReleaseAsync().ConfigureAwait(false);
                await lck2.ReleaseAsync().ConfigureAwait(false);
            }

            var lck3 = _lockFactory.CreateLock("test", "ghi");
            Assert.True(await lck3.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            Assert.True(await lck.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            try
            {
                Assert.False(await lck3.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                Assert.True(false, "Fail");
            }
            catch (DistributedLockException ex)
            {
                Assert.Equal("Please don't repeat Wait before Release", ex.Message);
            }
            finally
            {
                await lck.ReleaseAsync().ConfigureAwait(false);
                await lck3.ReleaseAsync().ConfigureAwait(false);
            }

            var lck4 = _lockFactory.CreateLock("test", "jkl");
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                lck4.Lock(timeout, CancellationToken.None);
            }).Start();
            Assert.True(await lck.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
            Thread.Sleep(10);
            try
            {
                Assert.False(await lck4.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                Assert.True(false, "Fail");
            }
            catch (DistributedLockException ex)
            {
                Assert.Equal("Please don't repeat Wait before Release", ex.Message);
            }
            finally
            {
                Thread.Sleep(timeout);
                await lck.ReleaseAsync().ConfigureAwait(false);
                await lck4.ReleaseAsync().ConfigureAwait(false);
            }

            var lck5 = _lockFactory.CreateLock("test", "mnq");
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                logger.LogDebug("Start Lock1");
                lck5.Lock(timeout, CancellationToken.None);
                Thread.Sleep(10);
                logger.LogDebug("Start release1");
                lck5.Release();
            }).Start();
            try
            {
                Thread.Sleep(10);
                logger.LogDebug("Start Lock2");
                Assert.False(await lck5.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                Assert.True(false, "Fail");
            }
            catch (DistributedLockException ex)
            {
                Assert.Equal("Please don't repeat Wait before Release", ex.Message);
            }
            finally
            {
                Thread.Sleep(700);
                try
                {
                    logger.LogDebug("Start Lock3");
                    Assert.True(await lck5.LockAsync(timeout, CancellationToken.None).ConfigureAwait(false));
                }
                catch (DistributedLockException ex)
                {
                    Assert.True(false, ex.Message);
                }
                finally
                {
                    logger.LogDebug("Start release2");
                    await lck5.ReleaseAsync().ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public async Task LockWithCancellationTokenTestMethod()
        {
            using var lock1 = _lockFactory.CreateLock("test", nameof(LockWithCancellationTokenTestMethod));
            using var lock2 = _lockFactory.CreateLock("test", nameof(LockWithCancellationTokenTestMethod));
            using var cts = new CancellationTokenSource();
            Assert.True(await lock1.LockAsync(100, cts.Token).ConfigureAwait(false));

            Assert.Throws<OperationCanceledException>(() =>
            {
                static async Task Run(CancellationTokenSource c)
                {
                    await Task.Delay(10, c.Token).ConfigureAwait(false);
                    c.Cancel();
                }

                var unused = Run(cts);

                Assert.False(lock2.Lock(200, cts.Token));
            });
        }
#if NETFRAMEWORK
        [Fact]
        public async Task TimeoutTest()
        {
            var results = await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(TimeoutTest))).ConfigureAwait(false);

            foreach (var result in results)
            {
                Assert.NotEmpty(result);

                Assert.False(result.All(_ => _), "TimeoutTest should have false");
            }

            Assert.False(results.Any(result => result.All(_ => !_)), "TimeoutTest should have no any false" + string.Join(Environment.NewLine, results.Select(result => string.Join(",", result))));

            static IReadOnlyCollection<bool> TimeoutTest()
            {
                using var isolated = new AppDomainIsolated<T>();
                return isolated.Value.Test("aaa", TimeSpan.FromSeconds(5));
            }
        }
#endif
    }
}
