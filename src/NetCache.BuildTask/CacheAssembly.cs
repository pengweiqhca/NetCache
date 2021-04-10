using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetCache
{
    public class CacheAssembly : IDisposable
    {
        private readonly ILogger _logger;

        private readonly bool _readSymbols;

        private readonly AssemblyDefinition _assembly;

        private readonly FuncHelper _helper;

        private readonly IReadOnlyDictionary<string, MethodReference> _cacheHelperMethods;

        public TypeDefinition[] KnowTypes { get; }

        public ModuleDefinition MainModule { get; }

        public CacheAssembly(string fileName, IEnumerable<string> searchDirectories, ILogger logger, bool readWrite, bool readSymbols = false)
        {
            var resolver = new DefaultAssemblyResolver();
            foreach (var dir in searchDirectories)
            {
                logger.Message("Add search directory", dir);

                resolver.AddSearchDirectory(dir);
            }

            var parameter = new ReaderParameters
            {
                ReadWrite = readWrite,
                ReadSymbols = readSymbols,
                AssemblyResolver = resolver
            };

            _logger = logger;
            _readSymbols = readSymbols;
            _assembly = AssemblyDefinition.ReadAssembly(fileName, parameter);
            _helper = new FuncHelper(_assembly.MainModule);

            MainModule = _assembly.MainModule;
            KnowTypes = MainModule
                .AssemblyReferences
                .Select(asm => ResolveAssemblyNameReference(resolver, asm))
                .Where(item => item != null)
                .SelectMany(item => item!.GetTypes())
                .ToArray();

            _cacheHelperMethods = typeof(CacheHelper).GetMethods()
                .Where(m => m.DeclaringType == typeof(CacheHelper))
                .GroupBy(m => m.Name)
                .ToDictionary(g => g.Key, g => ImportMethod(g.First()));
        }

        private ModuleDefinition? ResolveAssemblyNameReference(DefaultAssemblyResolver resolver, AssemblyNameReference assembly)
        {
            try
            {
                return resolver.Resolve(assembly).MainModule;
            }
            catch (Exception ex)
            {
                _logger.Message(ex.Message);

                return null;
            }
        }

        public bool WriteProxyTypes()
        {
            var willSave = false;
            foreach (var @interface in GetCacheTypes())
            {
                var proxyType = new CacheProxyType(this, @interface, _helper, _cacheHelperMethods).Build();

                if (IsDefined(proxyType)) continue;

                _logger.Message("Writing IL", proxyType.FullName);

                @interface.Type.NestedTypes.Add(proxyType);

                willSave = true;
            }

            if (!willSave) return willSave;

            _logger.Message("Saving", _assembly.FullName);
#if NETCOREAPP
            //PrivateCoreLibFixer.FixReferences(MainModule);
#endif
            _assembly.Write(new WriterParameters { WriteSymbols = _readSymbols });

            return willSave;
        }

        public TypeDefinition CreateProxyType<T>() => new CacheProxyType(this, new CacheInterface(this, MainModule.ImportReference(typeof(T)).Resolve()), _helper, _cacheHelperMethods).Build();

        public IEnumerable<CacheInterface> GetCacheTypes() =>
            _assembly
                .MainModule
                .GetTypes()
                .Select(item => new CacheInterface(this, item))
                .Where(item => item.IsCache());

        public bool IsDefined(TypeDefinition typeDefinition) => _assembly.MainModule.GetType(typeDefinition.FullName) != null;

        protected MethodReference ImportMethod(MethodInfo method, params TypeReference[] typeArguments)
        {
            if (typeArguments.Length < 1) return MainModule.ImportReference(method);

            var m = new GenericInstanceMethod(MainModule.ImportReference(method));

            foreach (var arg in typeArguments) m.GenericArguments.Add(arg);

            return m;
        }

        public void Dispose()
        {
            _assembly.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
