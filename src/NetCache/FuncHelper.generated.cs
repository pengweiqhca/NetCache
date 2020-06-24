using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    internal static partial class FuncHelper
    {
        #region Sync Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, TimeSpan, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, TimeSpan, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, cancellationToken, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, CancellationToken, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, key, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TK, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, cancellationToken, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, TimeSpan, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, key, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TK, TV> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, expiry, key));

        #endregion

        #region Task Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, TimeSpan, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, TimeSpan, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(key, cancellationToken, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, CancellationToken, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, key, cancellationToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TK, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(expiry, cancellationToken, key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, TimeSpan, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, key, expiry));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TK, Task<TV>> func) =>
            (key, expiry, cancellationToken) => new ValueTask<TV>(func(cancellationToken, expiry, key));

        #endregion

        #region ValueTask Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(expiry);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, TimeSpan, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(key, expiry);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(key, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(expiry, key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(expiry, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(cancellationToken, key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(cancellationToken, expiry);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TK, CancellationToken, TimeSpan, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(key, cancellationToken, expiry);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, TK, CancellationToken, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(expiry, key, cancellationToken);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TK, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(expiry, cancellationToken, key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TK, TimeSpan, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(cancellationToken, key, expiry);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TK, ValueTask<TV>> func) =>
            (key, expiry, cancellationToken) => func(cancellationToken, expiry, key);

        #endregion
    }
}
