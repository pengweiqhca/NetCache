using NetCache.BuildTask.Tests;
using NetCache.Tests.TestHelpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace NetCache.IL.Tests
{
    public class StaticTest
    {
        private readonly ITestOutputHelper _output;

        public StaticTest(ITestOutputHelper output) => _output = output;

        [Fact]
        public void ClassTest()
        {
            var file = Path.GetTempFileName();

            File.Copy(typeof(Int64Cache).Assembly.Location, file, true);

            using (var generator = new CacheAssembly(file, Enumerable.Empty<string>(), new Logger(_output), true))
                generator.WriteProxyTypes();

            _output.WriteLine(file);

            var type = Assembly.LoadFile(file).GetType(typeof(Int64Cache).FullName!)!.GetNestedTypes()[0];

            foreach (var ctor in typeof(Int64CacheProxy).GetConstructors())
                AssertIl(_output, ctor, type.GetConstructor(ctor.GetParameters().Select(p => p.ParameterType).ToArray())!);

            foreach (var method in typeof(Int64CacheProxy).GetMethods().Where(m => m.DeclaringType == typeof(Int64CacheProxy)))
                AssertIl(_output, method, type.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())!);

            static void AssertIl(ITestOutputHelper output, MethodBase source, MethodBase target)
            {
                var raw = ReadIl(source);
                var generated = ReadIl(target);

                try
                {
                    Assert.Equal(Regex.Replace( raw.Replace(nameof(Int64CacheProxy), "@Proxy"), @" \[Label\d+\]", "", RegexOptions.None), generated.Replace("FuncAdapter@`2", "FuncAdapter`2"));
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
            }

            static string ReadIl(MethodBase method) =>
                string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(method)
                    .Where(il => il.opcode != OpCodes.Nop));
        }
#if NETCOREAPP3_1
        [Fact]
        public void InterfaceTest()
        {
            var file = Path.GetTempFileName();

            File.Copy(GetType().Assembly.Location, file, true);

            using (var generator = new CacheAssembly(file, Enumerable.Empty<string>(), new Logger(_output), true))
                generator.WriteProxyTypes();

            _output.WriteLine(file);

            var type = Assembly.LoadFile(file).GetType(GetType().FullName!)!.GetNestedType(nameof(IDefaultImplementation))!.GetNestedTypes()[0];

            var il = string.Join(Environment.NewLine, HarmonyLib.PatchProcessor.GetOriginalInstructions(type.GetMethod(nameof(IDefaultImplementation.Get))!)
                .Where(i => i.opcode != OpCodes.Nop));

            _output.WriteLine(il);

            Assert.Equal($@"ldarg.0 NULL
ldfld NetCache.CacheHelper {typeof(StaticTest).Namespace}.@Proxy::_helper
ldarg.1 NULL
ldarg.0 NULL
ldftn virtual System.Int64 NetCache.IL.Tests.IDefaultImplementation::Get(System.String key)
newobj System.Void System.Func`2<System.String, System.Int64>::.ctor(System.Object object, System.IntPtr method)
newobj System.Void NetCache.FuncAdapter@`2<System.String, System.Int64>::.ctor(System.Object func)
ldftn System.Int64 NetCache.FuncAdapter@`2<System.String, System.Int64>::Wrap1(System.String key, System.TimeSpan expiry, System.Threading.CancellationToken cancellationToken)
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

        [Cache]
        public interface IDefaultImplementation
        {
            long Get(string key) => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
#endif
    }
}
