using System;
using System.Threading.Tasks;

namespace NetCache
{
#if BuildTask
    using Mono.Cecil;
    using MethodInfo = Mono.Cecil.MethodReference;
    using Type = Mono.Cecil.TypeReference;
#else
    using System.Reflection;
#endif
    internal class CacheMethod
    {
        public CacheMethod(MethodInfo method, CacheOperation operation)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Operation = operation;
#if BuildTask
            var returnType = method.ReturnType;
            if (returnType.IsGenericInstance) returnType = returnType.GetElementType();

            if (method.ReturnType.IsType<Task>() ||
                method.ReturnType.IsType<ValueTask>())
            {
                AsyncType = returnType;
                ValueType = CacheTypeResolver.Void;
            }
            else if (returnType.IsType(typeof(Task<>)) ||
                     returnType.IsType(typeof(ValueTask<>)))
            {
                AsyncType = returnType;
                ValueType = ((GenericInstanceType)method.ReturnType).GenericArguments[0];
            }
            else ValueType = method.ReturnType;

            WarpedValue = ValueType.IsGenericInstance && ValueType.GetElementType().IsType("NetCache.ICacheResult`1, NetCache.Core");
#else
            var returnType = method.ReturnType;
            if (returnType.IsGenericType) returnType = returnType.GetGenericTypeDefinition();

            if (typeof(Task) == method.ReturnType ||
                typeof(ValueTask) == method.ReturnType)
            {
                AsyncType = returnType;
                ValueType = typeof(void);
            }
            else if (typeof(Task<>) == returnType ||
                     typeof(ValueTask<>) == returnType)
            {
                AsyncType = returnType;
                ValueType = method.ReturnType.GetGenericArguments()[0];
            }
            else ValueType = method.ReturnType;

            WarpedValue = ValueType.IsGenericType && ValueType.GetGenericTypeDefinition() == typeof(ICacheResult<>);
#endif
        }

        public CacheOperation Operation { get; }
        public MethodInfo Method { get; }

        public Type? AsyncType { get; }
        public Type ValueType { get; }
        public bool WarpedValue { get; }
        public Type? RawType { get; set; }

        /// <summary>ttl parameter index</summary>
        public int Ttl { get; set; }

        public int When { get; set; }

        /// <summary>CancellationToken parameter index</summary>
        public int CancellationToken { get; set; }
        /// <summary>value parameter index</summary>
        public int Value { get; set; }
    }
}
