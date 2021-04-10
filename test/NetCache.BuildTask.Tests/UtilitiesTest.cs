using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using Xunit;

namespace NetCache.BuildTask.Tests
{
    public class UtilitiesTest
    {
        public static AssemblyDefinition ReadAssembly(string fileName, params string[] searchDirectories) => ReadAssembly(fileName, (IEnumerable<string>)searchDirectories);

        public static AssemblyDefinition ReadAssembly(string fileName, IEnumerable<string> searchDirectories)
        {
            var resolver = new DefaultAssemblyResolver();

            foreach (var dir in searchDirectories)
            {
                resolver.AddSearchDirectory(dir);
            }

            var parameter = new ReaderParameters
            {
                ReadWrite = false,
                //ReadSymbols = true,
                AssemblyResolver = resolver
            };

            return AssemblyDefinition.ReadAssembly(fileName, parameter);
        }

        [Fact]
        public void GetUnderlyingTypeTest()
        {
            var module = ReadAssembly(typeof(int).Assembly.Location).MainModule;

            Assert.Null(module.GetType(typeof(int).FullName).GetUnderlyingType());

            Assert.Null(module.GetType(typeof(IEnumerable<>).FullName).MakeGenericInstanceType(module.GetType(typeof(int).FullName)).GetUnderlyingType());

            var type = module.GetType(typeof(Nullable<>).FullName).MakeGenericInstanceType(module.GetType(typeof(int).FullName)).GetUnderlyingType();

            Assert.NotNull(type);

            Assert.Equal(typeof(int).FullName, type!.FullName);
        }
    }
}
