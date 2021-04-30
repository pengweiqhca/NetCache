using System.Collections.Generic;

namespace NetCache
{
#if BuildTask
    using Type = Mono.Cecil.TypeDefinition;
#else
    using System;
#endif

    internal class CacheType
    {
        public CacheType(string name, Type type, IReadOnlyList<CacheMethod> methods)
        {
            Name = name;
            Type = type;
            Methods = methods;
        }

        public string Name { get; }
        public Type Type { get; }
        public IReadOnlyList<CacheMethod> Methods { get; }
        public int DefaultTtl { get; set; }

        public override string ToString() => Type.ToString();
    }
}
