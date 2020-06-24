using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    /// <summary>A distributed lock</summary>
    public interface IDistributedLock : IDisposable
#if NET45
    {
        /// <summary>Dipose</summary>
        ValueTask DisposeAsync();
#else
        , IAsyncDisposable
    {
#endif
        /// <summary>Lock the resource</summary>
        /// <param name="millisecondsTimeout">The max wait timeout for the lock</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>The locked resource, if succeed, else return null</returns>
        bool Lock(int millisecondsTimeout, CancellationToken cancellationToken = default);

        /// <summary>Lock the resource</summary>
        /// <param name="millisecondsTimeout">The max wait timeout for the lock</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The locked resource, if succeed, else return null.</returns>
        ValueTask<bool> LockAsync(int millisecondsTimeout, CancellationToken cancellationToken = default);

        /// <summary>Release the resource</summary>
        void Release();

        /// <summary>Release the resource</summary>
        ValueTask ReleaseAsync();
    }
}
