using Mono.Cecil;
using System.Linq;

namespace NetCache
{
    public class CacheInterface : CacheMetadata
    {
        public TypeDefinition Type { get; }

        public CacheInterface(CacheAssembly assembly, TypeDefinition @interface)
            : base(assembly)
        {
            Type = @interface;
        }

        public bool IsCache() =>
            (Type.IsInterface || Type.IsClass && !Type.IsSealed)
            && Type.GetCustomAttribute("NetCache.CacheAttribute, NetCache.Core") != null;

        public TypeDefinition MakeProxyType()
        {
            var proxyType = new TypeDefinition("", "@Proxy", TypeAttributes.NestedPublic | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, !Type.IsInterface ? Type : TypeSystem.Object)
            {
                DeclaringType = Type
            };

            if (!Type.IsInterface) return proxyType;

            TypeReference type = Type;

            if (Type.HasGenericParameters)
            {
                var genericInterface = new GenericInstanceType(Type);

                foreach (var arg in Type.GenericParameters.Select(p => MakeGenericParameter(p, proxyType)))
                {
                    proxyType.GenericParameters.Add(arg);

                    genericInterface.GenericArguments.Add(arg);
                }

                type = genericInterface;
            }

            proxyType.Interfaces.Add(new InterfaceImplementation(type));

            return proxyType;
        }

        private static GenericParameter MakeGenericParameter(GenericParameter source, IGenericParameterProvider proxyType)
        {
            var parameter = new GenericParameter(source.Name, proxyType)
            {
                Attributes = source.Attributes
                & ~GenericParameterAttributes.Covariant
                & ~GenericParameterAttributes.Contravariant
            };

            foreach (var type in source.Constraints)
            {
                parameter.Constraints.Add(type);
            }
            return parameter;
        }
    }
}
