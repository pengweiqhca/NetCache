using NetCache.Tests.TestHelpers;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetCache.Tests
{
    public class CacheTypeResolverTest
    {
        [Fact]
        public void TypeTest()
        {
            var info = CacheTypeResolver.Resolve(typeof(TypeTestClass));

            Assert.Equal(typeof(TypeTestClass).FullName, info.Name);
            Assert.Equal(typeof(TypeTestClass), info.Type);
            Assert.Equal(4, info.Methods.Count);

            Assert.Equal(CacheOperation.Get, info.Methods[0].Operation);
            Assert.Equal(CacheOperation.Set, info.Methods[1].Operation);
            Assert.Equal(CacheOperation.Remove, info.Methods[2].Operation);
            Assert.Equal(CacheOperation.Remove, info.Methods[3].Operation);

            var agg = Assert.Throws<AggregateException>(() => CacheTypeResolver.Resolve(typeof(TypeTestClass2)));
            Assert.Single((IEnumerable)agg.InnerExceptions);
            Assert.Equal("不支持的抽象方法Test", agg.InnerException!.Message);
        }

        [Theory]
        [InlineData(typeof(GenericMethodCache))]
        [InlineData(typeof(GenericTypeCache<string>))]
        public void GenericTest(Type type)
        {
            var info = CacheTypeResolver.Resolve(type);

            Assert.Equal(2, info.Methods.Count);

            Assert.Equal(CacheOperation.Get, info.Methods[0].Operation);
            Assert.Equal(CacheOperation.Set, info.Methods[1].Operation);
        }

        public abstract class TypeTestClass
        {
            public virtual int Get(string key) => 3;
            public abstract void Set(string key, int ttl);
            public abstract void Delete(string key);
            public abstract void Remove(string key);
            public void Test() { }
        }

        public abstract class TypeTestClass2
        {
            public abstract void Test();
        }

        [Fact]
        public void CacheNameTest()
        {
            Assert.Equal(typeof(StringCache).FullName, CacheTypeResolver.GetCacheName(typeof(StringCache), out var defaultTtl));
            Assert.Equal(0, defaultTtl);
            Assert.Equal("long", CacheTypeResolver.GetCacheName(typeof(Int64Cache), out defaultTtl));
            Assert.Equal(10, defaultTtl);
        }

        [Fact]
        public void GetTest()
        {
            Assert.Null(CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.Get))!));
            Assert.Null(CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetError))!));

            var method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetValue))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetTll))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetTtlWithToken))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.TokenAndTtl))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(1, method.CancellationToken);
        }

        [Fact]
        public void GetOrSetTest()
        {
            Assert.Null(CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSetError))!));
            Assert.Null(CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet9))!));

            var method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet0))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet1))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet2))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(1, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet3))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet4))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet5))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet6))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet7))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(3, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet8))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(3, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);
        }

        [Fact]
        public void SetTest()
        {
            Assert.Null(CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set))!));

            Assert.Null(CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set9))!));

            var method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set0))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set1))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set2))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(1, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set3))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(2, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set4))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set5))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set6))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(2, method.Ttl);
            Assert.Equal(3, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set7))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(3, method.Ttl);
            Assert.Equal(2, method.CancellationToken);

            method = CacheTypeResolver.ResolveSet(typeof(TestClass).GetMethod(nameof(TestClass.Set8))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Set, method.Operation);
            Assert.Equal(3, method.Value);
            Assert.Equal(1, method.Ttl);
            Assert.Equal(2, method.CancellationToken);
        }

        [Fact]
        public void RemoveTest()
        {
            Assert.Null(CacheTypeResolver.ResolveRemove(typeof(TestClass).GetMethod(nameof(TestClass.Delete))!));

            var method = CacheTypeResolver.ResolveRemove(typeof(TestClass).GetMethod(nameof(TestClass.Remove))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Remove, method.Operation);
            Assert.Equal(0, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);
        }

        [Fact]
        public void AsyncTest()
        {
            Assert.Null(CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet3Async))!));

            var method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetAsync))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(0, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSetAsync))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet1Async))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);

            method = CacheTypeResolver.ResolveGet(typeof(TestClass).GetMethod(nameof(TestClass.GetOrSet2Async))!)!;
            Assert.NotNull(method);
            Assert.Equal(CacheOperation.Get, method.Operation);
            Assert.Equal(1, method.Value);
            Assert.Equal(0, method.Ttl);
            Assert.Equal(0, method.CancellationToken);
        }

        public abstract class TestClass
        {
            public abstract int Get();
            public abstract int GetValue(string key);
            public abstract long GetTll(string key, int ttl);
            public abstract long GetTtlWithToken(string key, int ttl, CancellationToken token);
            [CacheMethod(CacheOperation.Get)]
            public abstract long TokenAndTtl(string key, CancellationToken token, int ttl);
            public abstract long GetError(string key, int ttl, CancellationToken token, string value);

            public abstract int GetOrSetError(string key, Func<string, string> func);
            public abstract int GetOrSet0(string key, Func<string, int> func);
            public abstract int GetOrSet1(string key, Func<string, int> abc, CancellationToken token);
            public abstract int GetOrSet2(string key, CancellationToken token, Func<string, int> abc);
            public abstract int GetOrSet3(string key, int ttl, Func<string, int> func);
            public abstract int GetOrSet4(string key, Func<string, int> func, int ttl);
            public abstract int GetOrSet5(string key, Func<string, int> func, int ttl, CancellationToken token);
            public abstract string GetOrSet6(int key, Func<int, string> func, int abc, CancellationToken token);
            public abstract string GetOrSet7(int key, Func<int, string> func, CancellationToken token, int abc);
            public abstract string GetOrSet8(int key, int abc, CancellationToken token, Func<int, string> func);
            public abstract string GetOrSet9(int key, string value, Func<int, string> func, int ttl, CancellationToken token);

            public abstract void Set(string key);
            public abstract void Set0(string key, int value);
            public abstract void Set1(string key, int abc, CancellationToken token);
            public abstract void Set2(string key, CancellationToken token, int abc);
            public abstract void Set3(string key, int ttl, int value);
            public abstract void Set4(string key, int value, int ttl);
            public abstract void Set5(string key, int value, int ttl, CancellationToken token);
            public abstract void Set6(string key, string value, int abc, CancellationToken token);
            public abstract void Set7(string key, string value, CancellationToken token, int abc);
            public abstract void Set8(string key, int abc, CancellationToken token, string value);
            public abstract void Set9(string key, string value, string other, int ttl, CancellationToken token);

            public abstract void Delete();
            public abstract void Remove(string key);

            public abstract Task<int> GetAsync(string key);
            public abstract Task<int> GetOrSetAsync(string key, Func<string, Task<int>> func);
            public abstract Task<int> GetOrSet1Async(string key, Func<CancellationToken, Task<int>> func);
            public abstract Task<int> GetOrSet2Async(string key, Func<string, CancellationToken, Task<int>> func);
            public abstract Task<int> GetOrSet3Async(string key, Func<Task<object>> func);
        }

        [Fact]
        public void GetOrSetTtlTest()
        {
            var methods = typeof(TtlClass).GetMethods().Where(m => m.Name == nameof(TtlClass.Get)).ToArray();

            for (var index = 0; index < methods.Length - 1; index++)
            {
                var method = CacheTypeResolver.ResolveGet(methods[index])!;

                Assert.NotNull(method);
                Assert.Equal(1, method.Ttl);
                Assert.Equal(0, method.CancellationToken);
            }

            Assert.Null(CacheTypeResolver.ResolveGet(methods[methods.Length - 1]));
        }

        [Fact]
        public void SetTtlTest()
        {
            foreach (var m in typeof(TtlClass).GetMethods().Where(m => m.Name == nameof(TtlClass.Set)))
            {
                var method = CacheTypeResolver.ResolveSet(m)!;

                Assert.NotNull(method);
                Assert.Equal(1, method.Value);
                Assert.Equal(2, method.Ttl);
                Assert.Equal(0, method.CancellationToken);
            }
        }

        public abstract class TtlClass
        {
            public virtual int Get(string key, byte a) => default;
            public virtual int Get(string key, sbyte b) => default;
            public virtual int Get(string key, short c) => default;
            public virtual int Get(string key, ushort d) => default;
            public virtual int Get(string key, int e) => default;
            public virtual int Get(string key, uint f) => default;
            public virtual int Get(string key, long g) => default;
            public virtual int Get(string key, ulong h) => default;
            public virtual int Get(string key, decimal j) => default;
            public virtual int Get(string key, float k) => default;
            public virtual int Get(string key, double l) => default;
            public virtual int Get(string key, TimeSpan m) => default;
            public virtual int Get(string key, DateTime m) => default;
            public virtual int Get(string key, DateTimeOffset o) => default;
            public virtual int Get(string key, byte? a) => default;
            public virtual int Get(string key, sbyte? b) => default;
            public virtual int Get(string key, short? c) => default;
            public virtual int Get(string key, ushort? d) => default;
            public virtual int Get(string key, int? e) => default;
            public virtual int Get(string key, uint? f) => default;
            public virtual int Get(string key, long? g) => default;
            public virtual int Get(string key, ulong? h) => default;
            public virtual int Get(string key, decimal? j) => default;
            public virtual int Get(string key, float? k) => default;
            public virtual int Get(string key, double? l) => default;
            public virtual int Get(string key, TimeSpan? m) => default;
            public virtual int Get(string key, DateTime? m) => default;
            public virtual int Get(string key, DateTimeOffset? o) => default;
            public virtual int Get(string key, string p) => default;

            public abstract void Set(string key, int value, byte ttl);
            public abstract void Set(string key, int value, sbyte ttl);
            public abstract void Set(string key, int value, short ttl);
            public abstract void Set(string key, int value, ushort ttl);
            public abstract void Set(string key, int value, uint ttl);
            public abstract void Set(string key, int value, long expiry);
            public abstract void Set(string key, int value, ulong expiry);
            public abstract void Set(string key, int value, decimal expiry);
            public abstract void Set(string key, int value, float expiry);
            public abstract void Set(string key, int value, double expiry);
            public abstract void Set(string key, int value, TimeSpan expiry);
            public abstract void Set(string key, int value, DateTime expiry);
            public abstract void Set(string key, int value, DateTimeOffset ttl);
            public abstract void Set(string key, string p, int value);
            public abstract void Set(string key, int ttl, [CacheExpiry] int value);
        }
    }
}
