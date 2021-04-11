using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if BuildTask
using MethodInfo = Mono.Cecil.MethodReference;
using ModuleBuilder = Mono.Cecil.ModuleDefinition;
using Type = Mono.Cecil.TypeReference;
#else
using System.Reflection;
using System.Reflection.Emit;
#endif

namespace NetCache
{
    internal partial class FuncHelper
    {
        private readonly IDictionary<int, MethodInfo> _wrapMethods = new Dictionary<int, MethodInfo>();

        public Type FuncAdapterType { get; }

        public FuncHelper(ModuleBuilder module) => FuncAdapterType = CreateType(module);

        private static int GetIndex(Type? arg1, Type? arg2, Type? arg3, Type? returnArg)
        {
            return (Index(arg1) << 24) + (Index(arg2) << 16) + (Index(arg3) << 8) + Index(returnArg);

            static int Index(Type? type)
            {
                if (type == null) return 0;
#if BuildTask
                if (type.FullName == typeof(object).FullName) return 1;
                if (type.FullName == typeof(TimeSpan).FullName) return 2;
                if (type.FullName == typeof(CancellationToken).FullName) return 3;
                if (type.FullName == typeof(Task<>).FullName) return 4;
                if (type.FullName == typeof(ValueTask<>).FullName) return 5;
#else
                if (type == typeof(object)) return 1;
                if (type == typeof(TimeSpan)) return 2;
                if (type == typeof(CancellationToken)) return 3;
                if (type == typeof(Task<>)) return 4;
                if (type == typeof(ValueTask<>)) return 5;
#endif
                throw new NotSupportedException($"Invalid type `{type.FullName}`");
            }
        }

        public MethodInfo GetWrapMethod(bool sync, Type? arg1, Type? arg2, Type? arg3, Type? returnArg) =>
            _wrapMethods[(sync ? -1 : 1) * GetIndex(arg1, arg2, arg3, returnArg)];
    }
}
