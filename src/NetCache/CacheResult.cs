using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace NetCache
{
    internal class CacheResult<T> : ICacheResult<T>
    {
        public CacheResult([AllowNull] T value) => Value = value;

        public ExceptionDispatchInfo? Exception { get; set; }
        public float Elapsed { get; set; }

        [AllowNull]
        public T Value { get; }
    }
}
