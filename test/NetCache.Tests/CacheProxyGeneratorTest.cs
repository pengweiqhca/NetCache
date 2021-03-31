using NetCache.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;
using Xunit.Abstractions;

namespace NetCache.Tests
{
    public class CacheProxyGeneratorTest
    {
        private static readonly ICacheProxyGenerator Generator = new CacheProxyGenerator();
#if !NETCOREAPP2_1
        private readonly ITestOutputHelper _output;

        public CacheProxyGeneratorTest(ITestOutputHelper output) => _output = output;

        [Fact]
        public void ClassTest()
        {
            var type = Generator.CreateProxyType<Int64Cache>();

            foreach (var ctor in typeof(Int64CacheProxy).GetConstructors())
                AssertIl(_output, ctor, type.GetConstructor(ctor.GetParameters().Select(p => p.ParameterType).ToArray())!);

            foreach (var method in typeof(Int64CacheProxy).GetMethods().Where(m => m.DeclaringType == typeof(Int64CacheProxy)))
                AssertIl(_output, method, type.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())!);
        }

        private static void AssertIl(ITestOutputHelper output, MethodBase source, MethodBase target)
        {
            var raw = ReadIl(source, true);
            var generated = ReadIl(target, false);

            try
            {
                Assert.Equal(raw, generated.Replace($"{nameof(Int64Cache)}@Proxy@{typeof(Int64Cache).Assembly.GetHashCode()}", $"{nameof(Int64Cache)}Proxy").Replace("FuncAdapter@", "FuncAdapter`2"));
            }
            catch
            {
                if (source is MethodInfo method)
                    output.WriteLine($"{method.ReturnType.FullName} {source.Name}({string.Join(", ", source.GetParameters().Select(p => p.ParameterType.FullName))})");
                else
                    output.WriteLine($"{source.Name}({string.Join(", ", source.GetParameters().Select(p => p.ParameterType.FullName))})");

                output.WriteLine("");
                output.WriteLine(raw);
                output.WriteLine("");
                output.WriteLine("==⇈⇈==BuildTime==⇈⇈===========⇊⇊==RunTime==⇊⇊==");
                output.WriteLine("");
                output.WriteLine(generated);

                throw;
            }

            static string ReadIl(MethodBase method, bool raw)
            {
                try
                {
                    return string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(method)
                        .Where(il => il.opcode != OpCodes.Nop));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"{(raw ? "静态方法失败" : "动态方法失败")} {method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => GetAssemblyQualifiedName(p.ParameterType)))})", ex);
                }
            }
        }

        private static string GetAssemblyQualifiedName(Type type) =>
            type.IsConstructedGenericType ?
                $"{type.Namespace}.{type.Name}[[{string.Join("], [", type.GetGenericArguments().Select(GetAssemblyQualifiedName).ToArray())}]]"
                : $"{type.FullName}, {type.Assembly.GetName().Name}";

        [Fact]
        public void FuncAdapterTest()
        {
            var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = Guid.NewGuid().ToString("N") }, AssemblyBuilderAccess.Run);
            var helper = Activator.CreateInstance(typeof(ICacheProxyGenerator).Assembly.GetType("NetCache.FuncHelper")!, ab.DefineDynamicModule("Proxies"));
            var type = (Type?)helper?.GetType().GetProperty("FuncAdapterType")?.GetValue(helper);

            Assert.NotNull(type);

            var ex = new List<Exception>();
            foreach (var method in typeof(FuncAdapter<string, string>).GetMethods(BindingFlags.Static | BindingFlags.Public))
                try
                {
                    AssertIl(_output, method, type!.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())!);
                }
                catch (Exception e)
                {
                    _output.WriteLine(e.Message);

                    ex.Add(e);
                }

            Assert.Empty(ex);
        }
#endif
        [Fact]
        public void GenericTest()
        {
            Generator.CreateProxyType<IGenericInterfaceCache<string>>();
            Generator.CreateProxyType<GenericMethodCache>();
            Generator.CreateProxyType<GenericTypeCache<string>>();
        }
#if NETCOREAPP3_1
        [Fact]
        public void InterfaceTest()
        {
            var type = Generator.CreateProxyType<IDefaultImplementation>();

            var il = string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(type.GetMethod(nameof(IDefaultImplementation.Get))!)
                .Where(i => i.opcode != OpCodes.Nop));

            _output.WriteLine(il);

            Assert.Equal($@"ldarg.0 NULL
ldfld NetCache.CacheHelper {typeof(CacheProxyGeneratorTest).FullName}\+IDefaultImplementation@Proxy@{typeof(IDefaultImplementation).Assembly.GetHashCode()}::_helper
ldarg.1 NULL
ldarg.0 NULL
ldftn virtual System.Int64 NetCache.Tests.IDefaultImplementation::Get(System.String key)
newobj System.Void System.Func`2<System.String, System.Int64>::.ctor(System.Object object, System.IntPtr method)
newobj System.Void NetCache.FuncAdapter@<System.String, System.Int64>::.ctor(System.Object func)
ldftn System.Int64 NetCache.FuncAdapter@<System.String, System.Int64>::Wrap1(System.String key, System.TimeSpan expiry, System.Threading.CancellationToken cancellationToken)
newobj System.Void System.Func`4<System.String, System.TimeSpan, System.Threading.CancellationToken, System.Int64>::.ctor(System.Object object, System.IntPtr method)
ldloca.s 0 (System.TimeSpan)
initobj System.TimeSpan
ldloc.0 NULL
ldloca.s 1 (System.Threading.CancellationToken)
initobj System.Threading.CancellationToken
ldloc.1 NULL
callvirt System.Int64 NetCache.CacheHelper::GetOrSet(System.String key, System.Func`4<System.String, System.TimeSpan, System.Threading.CancellationToken, System.Int64> func, System.TimeSpan expiry, System.Threading.CancellationToken token)
ret NULL", il);
        }

        public interface IDefaultImplementation
        {
            long Get(string key) => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
#endif
    }
}
