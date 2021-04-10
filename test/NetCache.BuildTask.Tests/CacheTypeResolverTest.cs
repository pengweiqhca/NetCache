using Mono.Cecil;
using Mono.Cecil.Rocks;
using NetCache.Tests.TestHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetCache.BuildTask.Tests
{
    public class CacheTypeResolverTest
    {
        private static readonly AssemblyDefinition TestAssembly = UtilitiesTest.ReadAssembly(typeof(Int64Cache).Assembly.Location);

        private static TypeDefinition GetType(string type, params TypeReference[] arguments) =>
            arguments.Length > 1
                ? TestAssembly.MainModule.GetType(type).MakeGenericInstanceType(arguments).Resolve()
                : TestAssembly.MainModule.GetType(type);

        [Fact]
        public void TypeTest()
        {
            var info = CacheTypeResolver.Resolve(GetType("NetCache.Tests.CacheTypeResolverTest/TypeTestClass"));

            Assert.Equal("NetCache.Tests.CacheTypeResolverTest/TypeTestClass", info.Name);
            Assert.Equal(GetType("NetCache.Tests.CacheTypeResolverTest/TypeTestClass"), info.Type);
            Assert.Equal(4, info.Methods.Count);

            Assert.Equal(CacheOperation.Get, info.Methods[0].Operation);
            Assert.Equal(CacheOperation.Set, info.Methods[1].Operation);
            Assert.Equal(CacheOperation.Remove, info.Methods[2].Operation);
            Assert.Equal(CacheOperation.Remove, info.Methods[3].Operation);

            var agg = Assert.Throws<AggregateException>(() => CacheTypeResolver.Resolve(GetType("NetCache.Tests.CacheTypeResolverTest/TypeTestClass2")));
            Assert.Single((IEnumerable)agg.InnerExceptions);
            Assert.Equal("不支持的抽象方法Test", agg.InnerException!.Message);
        }

        [Theory]
        [MemberData(nameof(GenericTestSource))]
        public void GenericTest(TypeDefinition type)
        {
            var info = CacheTypeResolver.Resolve(type);

            Assert.Equal(2, info.Methods.Count);

            Assert.Equal(CacheOperation.Get, info.Methods[0].Operation);
            Assert.Equal(CacheOperation.Set, info.Methods[1].Operation);
        }

        public static IEnumerable<object[]> GenericTestSource()
        {
            yield return new object[] { GetType("NetCache.Tests.TestHelpers.GenericMethodCache") };
            yield return new object[] { GetType("NetCache.Tests.TestHelpers.GenericTypeCache`1", TestAssembly.MainModule.ImportReference(typeof(string))) };
        }

        [Fact]
        public void CacheNameTest()
        {
            Assert.Equal("NetCache.Tests.TestHelpers.StringCache", CacheTypeResolver.GetCacheName(GetType("NetCache.Tests.TestHelpers.StringCache"), out var defaultTtl));
            Assert.Equal(0, defaultTtl);
            Assert.Equal("long", CacheTypeResolver.GetCacheName(GetType("NetCache.Tests.TestHelpers.Int64Cache"), out defaultTtl));
            Assert.Equal(10, defaultTtl);
        }

        [Fact]
        public void GetTest()
        {
            var type = GetType("NetCache.Tests.CacheTypeResolverTest/TestClass");

            Assert.Null(CacheTypeResolver.ResolveGet(type.GetMethod("Get")!));
            Assert.Null(CacheTypeResolver.ResolveGet(type.GetMethod("GetError")!));

            var method = CacheTypeResolver.ResolveGet(type.GetMethod("GetValue")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetTll")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetTtlWithToken")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("TokenAndTtl")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(1, method.CancellationToken);
        }

        [Fact]
        public void GetOrSetTest()
        {
            var type = GetType("NetCache.Tests.CacheTypeResolverTest/TestClass");

            Assert.Null(CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSetError")!));
            Assert.Null(CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet9")!));

            var method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet0")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet1")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet2")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(1, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet3")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet4")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet5")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet6")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet7")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(3, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet8")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(3, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);
        }

        [Fact]
        public void SetTest()
        {
            var type = GetType("NetCache.Tests.CacheTypeResolverTest/TestClass");

            Assert.Null(CacheTypeResolver.ResolveSet(type.GetMethod("Set")!));

            Assert.Null(CacheTypeResolver.ResolveSet(type.GetMethod("Set9")!));

            var method = CacheTypeResolver.ResolveSet(type.GetMethod("Set0")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set1")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set2")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(1, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set3")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set4")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set5")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set6")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set7")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(3, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(type.GetMethod("Set8")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(3, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);
        }

        [Fact]
        public void RemoveTest()
        {
            var type = GetType("NetCache.Tests.CacheTypeResolverTest/TestClass");

            Assert.Null(CacheTypeResolver.ResolveRemove(type.GetMethod("Delete")!));

            var method = CacheTypeResolver.ResolveRemove(type.GetMethod("Remove")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Remove, method.Operation);
            Assert.Equal(0, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);
        }

        [Fact]
        public void AsyncTest()
        {
            var type = GetType("NetCache.Tests.CacheTypeResolverTest/TestClass");

            Assert.Null(CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet3Async")!));

            var method = CacheTypeResolver.ResolveGet(type.GetMethod("GetAsync")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(0, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSetAsync")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet1Async")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(type.GetMethod("GetOrSet2Async")!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);
        }

        [Fact]
        public void GetOrSetTtlTest()
        {
            var methods = GetType("NetCache.Tests.CacheTypeResolverTest/TtlClass").GetMethods().Where(m => m.Name == "Get").ToArray();

            for (var index = 0; index < methods.Length - 1; index++)
            {
                var method = CacheTypeResolver.ResolveGet(methods[index])!;

                Assert.NotNull(method);
                Assert.Equal(1, method.Ttl);
                Assert.Equal(0, method.CancellationToken);
            }
#if NETCOREAPP3_1_OR_GREATER
            Assert.Null(CacheTypeResolver.ResolveGet(methods[^1]));
#else
            Assert.Null(CacheTypeResolver.ResolveGet(methods[methods.Length - 1]));
#endif
        }

        [Fact]
        public void SetTtlTest()
        {
            foreach (var m in GetType("NetCache.Tests.CacheTypeResolverTest/TtlClass").GetMethods().Where(m => m.Name == "Set"))
            {
                var method = CacheTypeResolver.ResolveSet(m)!;

                Assert.NotNull(method);
                Assert.Equal(1, method.Value);
                Assert.Equal(2, method.Ttl);
                Assert.Equal(0, method.CancellationToken);
            }
        }
    }
}
