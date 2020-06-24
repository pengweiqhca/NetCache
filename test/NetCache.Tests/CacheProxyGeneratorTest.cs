using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NetCache.Tests.TestHelpers;
using System;
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
            {
                AssertIl(_output, ctor, type.GetConstructor(ctor.GetParameters().Select(p => p.ParameterType).ToArray())!);
            }

            foreach (var method in typeof(Int64CacheProxy).GetMethods().Where(m => m.DeclaringType == typeof(Int64CacheProxy)))
            {
                AssertIl(_output, method, type.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())!);
            }

            static void AssertIl(ITestOutputHelper output, MethodBase source, MethodBase target)
            {
                var raw = ReadIl(source);
                var generated = ReadIl(target);

                try
                {
                    Assert.Equal(raw, generated.Replace($"{nameof(Int64Cache)}@Proxy@{typeof(Int64Cache).Assembly.GetHashCode()}", $"{nameof(Int64Cache)}Proxy"));
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
                    output.WriteLine("====⇈⇈BuildTime⇈⇈===============⇊⇊RunTime⇊⇊====");
                    output.WriteLine("");
                    output.WriteLine(generated);

                    throw;
                }
            }

            static string ReadIl(MethodBase method) =>
                string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(method)
                    .Where(il => il.opcode != OpCodes.Nop));
        }
#endif
#if NETCOREAPP3_1
        [Fact]
        public void InterfaceTest()
        {
            var type = Generator.CreateProxyType<IDefaultImplementation>();

            var il = string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(type.GetMethod(nameof(IDefaultImplementation.Get))!)
                .Where(il => il.opcode != OpCodes.Nop));

            _output.WriteLine(il);

            Assert.Equal($@"ldarg.0 NULL
ldfld NetCache.CacheHelper {typeof(CacheProxyGeneratorTest).FullName}\+IDefaultImplementation@Proxy@{typeof(IDefaultImplementation).Assembly.GetHashCode()}::_helper
ldarg.1 NULL
ldarg.0 NULL
ldftn virtual System.Int64 NetCache.Tests.IDefaultImplementation::Get(System.String key)
newobj System.Void System.Func`2<System.String, System.Int64>::.ctor(System.Object object, System.IntPtr method)
call static System.Func`4<System.String, System.TimeSpan, System.Threading.CancellationToken, System.Threading.Tasks.ValueTask`1<System.Int64>> NetCache.FuncHelper::Wrap(System.Func`2<System.String, System.Int64> func)
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
