using System;
using System.Collections.Generic;

namespace NetCache
{
    public class CacheType
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
