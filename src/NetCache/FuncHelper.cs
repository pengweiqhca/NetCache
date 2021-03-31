using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NetCache
{
    internal partial class FuncHelper
    {
        private readonly IDictionary<FuncType, int> _wrapMethods = new Dictionary<FuncType, int>(new FuncTypeComparer());
        private readonly IDictionary<FuncType, int> _wrapAsyncMethods = new Dictionary<FuncType, int>(new FuncTypeComparer());
        public Type FuncAdapterType { get; }

        private struct FuncType
        {
            public Type? Arg1 { get; set; }
            public Type? Arg2 { get; set; }
            public Type? Arg3 { get; set; }
            public Type? ReturnArg { get; set; }

            public override string ToString() => $"{Arg1} {Arg2} {Arg3} {ReturnArg}";
        }

        private class FuncTypeComparer : IEqualityComparer<FuncType>
        {
            public bool Equals(FuncType x, FuncType y)
            {
                var a = x.Arg1 == y.Arg1 &&
                       x.Arg2 == y.Arg2 &&
                       x.Arg3 == y.Arg3 &&
                       x.ReturnArg == y.ReturnArg;

                return a;
            }

            public int GetHashCode(FuncType obj)
            {
#if NET45
                var code = (obj.Arg1 ?? typeof(Func<>)).GetHashCode() ^ 17;

                code ^= (obj.Arg2 ?? typeof(Func<,>)).GetHashCode() ^ 31;
                code ^= (obj.Arg3 ?? typeof(Func<,,>)).GetHashCode() ^ 37;
                code ^= (obj.ReturnArg ?? typeof(Func<,,,>)).GetHashCode() ^ 77;

                return code;
#else
                return HashCode.Combine(obj.Arg1 ?? typeof(Func<>), obj.Arg2 ?? typeof(Func<,>), obj.Arg3 ?? typeof(Func<,,>), obj.ReturnArg ?? typeof(Func<,,,>));
#endif
            }
        }

        public FuncHelper(ModuleBuilder module) => FuncAdapterType = CreateType(module);

        public int GetWrapMethod(bool sync, Type? arg1, Type? arg2, Type? arg3, Type? returnArg) =>
            (sync ? _wrapMethods : _wrapAsyncMethods)[new FuncType
            {
                Arg1 = arg1,
                Arg2 = arg2,
                Arg3 = arg3,
                ReturnArg = returnArg
            }];
    }
}
