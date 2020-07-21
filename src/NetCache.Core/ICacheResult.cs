using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace NetCache
{
    /// <typeparam name="T">The value returned by the operation.</typeparam>
    public interface ICacheResult<out T>
    {
        ExceptionDispatchInfo? Exception { get; }

        float Elapsed { get; }

        [MaybeNull] T Value { get; }
    }
}
