using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public class LocalLock : IDistributedLock
    {
        private static readonly RefCounterPool<string, SemaphoreSlim> SemaphoreSlims
            = new();

        private readonly string _key;
        private readonly object _syncObj = new();

        public LocalLock(string key) => _key = key;
#pragma warning disable CA2213
        private volatile SemaphoreSlim? _semaphore;
#pragma warning restore CA2213
        private void GetOrCreate()
        {
            if (_semaphore != null) throw new DistributedLockException();

            lock (_syncObj)
            {
                if (_semaphore != null) throw new DistributedLockException();

                _semaphore = SemaphoreSlims.GetOrAdd(_key, key => new SemaphoreSlim(1, 1));
            }
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await ReleaseAsync().ConfigureAwait(false);
#pragma warning disable CA1816
            GC.SuppressFinalize(this);
#pragma warning restore CA1816
        }

        protected virtual void Dispose(bool disposing) => Release();

        ~LocalLock() => Dispose(false);

        #endregion Dispose

        private void InnerRelease()
        {
            var semaphore = Interlocked.Exchange(ref _semaphore, null);

            if (semaphore == null) return;

            semaphore.Release();

            SemaphoreSlims.TryRemove(_key)?.Dispose();
        }

        public virtual void Release() => InnerRelease();

        public virtual ValueTask ReleaseAsync()
        {
            InnerRelease();

            return default;
        }

        public virtual bool Lock(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            GetOrCreate();

            var locked = false;
            try
            {
                if (_semaphore != null) locked = _semaphore.Wait(millisecondsTimeout, cancellationToken);
            }
            finally
            {
                if (!locked) _semaphore = null;
            }

            return locked;
        }

        public virtual async ValueTask<bool> LockAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            GetOrCreate();

            var locked = false;
            try
            {
                if (_semaphore != null) locked = await _semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (!locked) _semaphore = null;
            }

            return locked;
        }
    }
}
