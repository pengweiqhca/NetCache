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
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TV> func) =>
            new Adapter<TK, TV>(func).Wrap0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TK, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TK, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TimeSpan, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap7;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<CancellationToken, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap9;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TK, TimeSpan, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TK, CancellationToken, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap11;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TimeSpan, TK, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap12;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<TimeSpan, CancellationToken, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap13;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<CancellationToken, TK, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap14;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, TV> Wrap<TK, TV>(Func<CancellationToken, TimeSpan, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap15;

        #endregion

        #region Async Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TV> func) =>
            new Adapter<TK, TV>(func).Wrap16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap17;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap18;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap19;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap21;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap22;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap23;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap24;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap25;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap26;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap27;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, CancellationToken, TV> func) =>
            new Adapter<TK, TV>(func).Wrap28;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap29;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, TimeSpan, TV> func) =>
            new Adapter<TK, TV>(func).Wrap30;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, TK, TV> func) =>
            new Adapter<TK, TV>(func).Wrap31;

        #endregion

        #region Task Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap33;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap34;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap35;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap36;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap37;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap38;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap39;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap40;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap41;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, CancellationToken, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap42;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, TimeSpan, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap43;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, CancellationToken, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap44;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, TK, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap45;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, TimeSpan, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap46;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, TK, Task<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap47;

        #endregion

        #region ValueTask Wrap

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap48;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap49;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap50;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap51;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap52;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap53;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap54;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap55;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap56;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap57;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap58;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TK, CancellationToken, TimeSpan, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap59;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, TK, CancellationToken, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap60;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<TimeSpan, CancellationToken, TK, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap61;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TK, TimeSpan, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap62;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<TK, TimeSpan, CancellationToken, ValueTask<TV>> WrapAsync<TK, TV>(Func<CancellationToken, TimeSpan, TK, ValueTask<TV>> func) =>
            new Adapter<TK, TV>(func).Wrap63;

        #endregion

        private class Adapter<TK, TV>
        {
            private object _func;

            public Adapter(object func) => _func = func;

            #region Sync Wrap

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap0(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TV>)_func)();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap1(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, TV>)_func)(key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap2(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, TV>)_func)(expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap3(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TV>)_func)(cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap4(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, TimeSpan, TV>)_func)(key, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap5(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, CancellationToken, TV>)_func)(key, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap6(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, TK, TV>)_func)(expiry, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap7(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, CancellationToken, TV>)_func)(expiry, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap8(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TK, TV>)_func)(cancellationToken, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap9(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TimeSpan, TV>)_func)(cancellationToken, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap10(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, TimeSpan, CancellationToken, TV>)_func)(key, expiry, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap11(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, CancellationToken, TimeSpan, TV>)_func)(key, cancellationToken, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap12(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, TK, CancellationToken, TV>)_func)(expiry, key, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap13(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, CancellationToken, TK, TV>)_func)(expiry, cancellationToken, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap14(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TK, TimeSpan, TV>)_func)(cancellationToken, key, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TV Wrap15(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TimeSpan, TK, TV>)_func)(cancellationToken, expiry, key);

            #endregion

            #region Async Wrap

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap16(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TV>)_func)());

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap17(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, TV>)_func)(key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap18(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, TV>)_func)(expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap19(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TV>)_func)(cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap20(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, TimeSpan, TV>)_func)(key, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap21(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, CancellationToken, TV>)_func)(key, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap22(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, TK, TV>)_func)(expiry, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap23(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, CancellationToken, TV>)_func)(expiry, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap24(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TK, TV>)_func)(cancellationToken, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap25(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TimeSpan, TV>)_func)(cancellationToken, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap26(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, TimeSpan, CancellationToken, TV>)_func)(key, expiry, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap27(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, CancellationToken, TimeSpan, TV>)_func)(key, cancellationToken, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap28(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, TK, CancellationToken, TV>)_func)(expiry, key, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap29(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, CancellationToken, TK, TV>)_func)(expiry, cancellationToken, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap30(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TK, TimeSpan, TV>)_func)(cancellationToken, key, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap31(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TimeSpan, TK, TV>)_func)(cancellationToken, expiry, key));

            #endregion

            #region Task Wrap

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap32(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<Task<TV>>)_func)());

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap33(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, Task<TV>>)_func)(key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap34(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, Task<TV>>)_func)(expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap35(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, Task<TV>>)_func)(cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap36(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, TimeSpan, Task<TV>>)_func)(key, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap37(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, CancellationToken, Task<TV>>)_func)(key, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap38(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, TK, Task<TV>>)_func)(expiry, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap39(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, CancellationToken, Task<TV>>)_func)(expiry, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap40(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TK, Task<TV>>)_func)(cancellationToken, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap41(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TimeSpan, Task<TV>>)_func)(cancellationToken, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap42(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, TimeSpan, CancellationToken, Task<TV>>)_func)(key, expiry, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap43(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TK, CancellationToken, TimeSpan, Task<TV>>)_func)(key, cancellationToken, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap44(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, TK, CancellationToken, Task<TV>>)_func)(expiry, key, cancellationToken));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap45(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<TimeSpan, CancellationToken, TK, Task<TV>>)_func)(expiry, cancellationToken, key));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap46(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TK, TimeSpan, Task<TV>>)_func)(cancellationToken, key, expiry));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap47(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                new ValueTask<TV>(((Func<CancellationToken, TimeSpan, TK, Task<TV>>)_func)(cancellationToken, expiry, key));

            #endregion

            #region ValueTask Wrap

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap48(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<ValueTask<TV>>)_func)();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap49(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, ValueTask<TV>>)_func)(key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap50(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, ValueTask<TV>>)_func)(expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap51(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, ValueTask<TV>>)_func)(cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap52(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, TimeSpan, ValueTask<TV>>)_func)(key, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap53(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, CancellationToken, ValueTask<TV>>)_func)(key, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap54(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, TK, ValueTask<TV>>)_func)(expiry, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap55(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, CancellationToken, ValueTask<TV>>)_func)(expiry, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap56(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TK, ValueTask<TV>>)_func)(cancellationToken, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap57(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TimeSpan, ValueTask<TV>>)_func)(cancellationToken, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap58(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, TimeSpan, CancellationToken, ValueTask<TV>>)_func)(key, expiry, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap59(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TK, CancellationToken, TimeSpan, ValueTask<TV>>)_func)(key, cancellationToken, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap60(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, TK, CancellationToken, ValueTask<TV>>)_func)(expiry, key, cancellationToken);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap61(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<TimeSpan, CancellationToken, TK, ValueTask<TV>>)_func)(expiry, cancellationToken, key);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap62(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TK, TimeSpan, ValueTask<TV>>)_func)(cancellationToken, key, expiry);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ValueTask<TV> Wrap63(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
                ((Func<CancellationToken, TimeSpan, TK, ValueTask<TV>>)_func)(cancellationToken, expiry, key);

            #endregion
        }
    }
}
