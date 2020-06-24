using System;

namespace NetCache
{
    /// <summary>Define cache method if method name do not conform to the constraints</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheMethodAttribute : Attribute
    {
        /// <summary>Cache operation</summary>
        public CacheOperation Operation { get; }

        /// <summary>ctor</summary>
        /// <param name="operation">Cache operation</param>
        public CacheMethodAttribute(CacheOperation operation) => Operation = operation;
    }
}
