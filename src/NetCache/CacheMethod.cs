using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCache
{
    internal class CacheMethod
    {
        public CacheMethod(MethodInfo method, CacheOperation operation)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Operation = operation;

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
        }

        public CacheOperation Operation { get; }
        public MethodInfo Method { get; }

        public Type? AsyncType { get; }
        public Type ValueType { get; }
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
