﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace NetCache
{
    internal partial class FuncHelper
    {
        private readonly IDictionary<FuncType, MethodInfo> _wrapMethods;
        private readonly IDictionary<FuncType, MethodInfo> _wrapAsyncMethods;

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

        private FuncHelper(Type helperType)
        {
            _wrapMethods = new Dictionary<FuncType, MethodInfo>(new FuncTypeComparer());
            _wrapAsyncMethods = new Dictionary<FuncType, MethodInfo>(new FuncTypeComparer());

            foreach (var method in helperType.GetMethods()
                .Where(m => m.Name.StartsWith("Wrap", StringComparison.Ordinal)))
            {
                var args = method.GetParameters()[0].ParameterType.GenericTypeArguments;
                Type? arg1 = null, arg2 = null, arg3 = null, returnArg = null;

                if (args[args.Length - 1].IsGenericType)
                {
                    var type = args[args.Length - 1].GetGenericTypeDefinition();
                    if (type == typeof(Task<>) || type == typeof(ValueTask<>))
                        returnArg = type;
                }
                if (args.Length > 1) arg1 = args[0].IsGenericParameter ? typeof(object) : args[0];
                if (args.Length > 2) arg2 = args[1].IsGenericParameter ? typeof(object) : args[1];
                if (args.Length > 3) arg3 = args[2].IsGenericParameter ? typeof(object) : args[2];

                (method.Name == "Wrap" ? _wrapMethods : _wrapAsyncMethods)[new FuncType
                {
                    Arg1 = arg1,
                    Arg2 = arg2,
                    Arg3 = arg3,
                    ReturnArg = returnArg
                }] = method;
            }
        }

        public static FuncHelper CreateHelper(ModuleBuilder module) => new(CreateType(module));

        public MethodInfo GetWrapMethod(Type? arg1, Type? arg2, Type? arg3, Type? returnArg) =>
            _wrapMethods[new FuncType
            {
                Arg1 = arg1,
                Arg2 = arg2,
                Arg3 = arg3,
                ReturnArg = returnArg
            }];

        public MethodInfo GetWrapAsyncMethod(Type? arg1, Type? arg2, Type? arg3, Type? returnArg) =>
            _wrapAsyncMethods[new FuncType
            {
                Arg1 = arg1,
                Arg2 = arg2,
                Arg3 = arg3,
                ReturnArg = returnArg
            }];
    }
}
