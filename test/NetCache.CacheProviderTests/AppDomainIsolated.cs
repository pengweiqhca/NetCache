using System;
using System.Threading;

namespace NetCache.CacheProviderTests
{
    /// <see href="https://bitlush.com/blog/executing-code-in-a-separate-application-domain-using-c-sharp" />
    public class AppDomainIsolated<T> : IDisposable where T : MarshalByRefObject
    {
        private AppDomain? _domain;

        public AppDomainIsolated()
        {
            var domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
                null, AppDomain.CurrentDomain.SetupInformation);

            _domain = domain;

            Value = (T)domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName ?? throw new InvalidOperationException());
        }

        public T Value { get; }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                //释放托管资源，比如将对象设置为null
            }

            //释放非托管资源
            var domain = Interlocked.Exchange(ref _domain, null);
            if (domain != null) AppDomain.Unload(domain);

            _disposed = true;
        }

        ~AppDomainIsolated() => Dispose(false);

        #endregion
    }
}
