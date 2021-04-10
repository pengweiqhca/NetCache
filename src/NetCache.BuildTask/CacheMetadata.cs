using Mono.Cecil;
using System;
using System.Linq;

namespace NetCache
{
    public abstract class CacheMetadata
    {
        private readonly TypeDefinition[] _knowTypes;

        public ModuleDefinition Module { get; }

        public TypeSystem TypeSystem => Module.TypeSystem;

        protected CacheMetadata(CacheAssembly assembly)
        {
            Module = assembly.MainModule;

            _knowTypes = assembly.KnowTypes;
        }

        protected TypeReference ImportType<T>() => ImportType(typeof(T));

        protected TypeReference ImportType(Type type)
        {
            if (type.IsArray) throw new NotSupportedException("不支持数组类型");

            var knowType = _knowTypes.FirstOrDefault(item => item.IsType(type));
            if (knowType != null) return Module.ImportReference(knowType);

            // 本程序集的类型不作直接导入
            if (type.Assembly == GetType().Assembly) throw new TypeLoadException($"找不到类型：{type.FullName}");

            return Module.ImportReference(type);
        }
    }
}
