using System;
using System.Collections.Generic;
using System.Threading;

namespace NetCache
{
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

        public override string ToString() => Type.ToString();
    }
}
