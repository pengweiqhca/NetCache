using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public class CacheProxyGenerator : ICacheProxyGenerator
    {
        private static readonly MethodInfo ConvertMethod = typeof(CacheProxyGenerator).GetMethod(nameof(Convert), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly IReadOnlyDictionary<string, MethodInfo> CacheHelperMethods =
            typeof(CacheHelper).GetMethods()
                .Where(m => m.DeclaringType == typeof(CacheHelper))
                .GroupBy(m => m.Name)
                .ToDictionary(g => g.Key, g => g.First());

        private readonly ModuleBuilder _mb = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = "NetCache.Proxies" }, AssemblyBuilderAccess.Run).DefineDynamicModule("Proxies");
        private readonly ConcurrentDictionary<Type, Type> _proxies = new ConcurrentDictionary<Type, Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type CreateProxyType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!type.IsInterface)
            {
                if (!type.IsClass) throw new InvalidOperationException($"{type.FullName} is not a class.");

                if (type.IsSealed) throw new InvalidOperationException($"{type.FullName} is sealed.");
            }

            return _proxies.GetOrAdd(type, t => CreateProxy(_mb, t));
        }

        public static Type CreateProxy(ModuleBuilder module, Type type)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var tb = module.DefineType($"{type.FullName}@Proxy@{type.Assembly.GetHashCode()}", TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.NotPublic);

            var cacheType = CacheTypeResolver.Resolve(type);
            var fb = MakeField(tb, cacheType);

            foreach (var method in cacheType.Methods)
            {
                var mb = tb.DefineMethod(method.Method.Name,
                    type.IsInterface
                        ? MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot
                        : MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig,
                    method.Method.ReturnType,
                    method.Method.GetParameters().Select(p => p.ParameterType).ToArray());
                var il = mb.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fb);
                il.Emit(OpCodes.Ldarg_1);

                switch (method.Operation)
                {
                    case CacheOperation.Get:
                        BuildGet(method, il, cacheType.DefaultTtl);
                        break;
                    case CacheOperation.Set:
                        BuildSet(method, il, cacheType.DefaultTtl);
                        break;
                    case CacheOperation.Remove:
                        BuildRemove(method, il);
                        break;
                    default:
                        throw new InvalidOperationException($"{mb.Name}不支持的操作{method.Operation}");
                }

                il.Emit(OpCodes.Ret);

                if (type.IsInterface) tb.DefineMethodOverride(mb, method.Method);
            }
#if NETSTANDARD2_0
            return tb.CreateTypeInfo()!;
#else
            return tb.CreateType()!;
#endif
        }

        private static FieldBuilder MakeField(TypeBuilder tb, CacheType type)
        {
            var filed = tb.DefineField("_helper", typeof(CacheHelper), FieldAttributes.Private);

            IEnumerable<ConstructorInfo> ctors;
            if (type.Type.IsInterface)
            {
                tb.AddInterfaceImplementation(type.Type);

                ctors = typeof(object).GetConstructors();
            }
            else
            {
                tb.SetParent(type.Type);

                ctors = type.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(ctor => ctor.IsFamily || ctor.IsPublic);
            }

            foreach (var method in ctors)
                foreach (var ctor in typeof(CacheHelper).GetConstructors().Take(1))
                    BuildConstructor(tb, type.Name, filed, method, ctor);

            return filed;
        }

        private static void BuildConstructor(TypeBuilder tb, string cacheName, FieldInfo field, ConstructorInfo cacheCtor, ConstructorInfo helperCtor)
        {
            var p1 = cacheCtor.GetParameters();
            var p2 = helperCtor.GetParameters();
            var parameterTypes = p1.Union(p2.Skip(1)).Select(p => p.ParameterType).ToArray();
            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes)
                .GetILGenerator();

            //base.ctor
            ctor.Emit(OpCodes.Ldarg_0);
            var index = 1;
            for (; index <= p1.Length; index++) Ldarg(ctor, index);

            ctor.Emit(OpCodes.Call, cacheCtor);

            //new CacheHelper
            ctor.Emit(OpCodes.Ldarg_0);
            ctor.Emit(OpCodes.Ldstr, cacheName);
            for (; index < p1.Length + p2.Length; index++) Ldarg(ctor, index);

            ctor.Emit(OpCodes.Newobj, helperCtor);
            ctor.Emit(OpCodes.Stfld, field);
            ctor.Emit(OpCodes.Ret);
        }

        private static void BuildGet(CacheMethod method, ILGenerator il, int defaultTtl)
        {
            var keyType = method.Method.GetParameters()[0].ParameterType;

            if (method.Method.IsAbstract && method.Value < 1)
            {
                var locals = 0;
                if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

                BuildCacheMethod(method, locals, il, nameof(CacheHelper.Get), nameof(CacheHelper.GetAsync), keyType, method.ValueType!);
            }
            else
            {
                BuildFunc(method, il, keyType, method.ValueType!);

                var locals = 0;
                if (LdTtl(method, il, locals, defaultTtl)) locals++;
                if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

                BuildCacheMethod(method, locals, il, nameof(CacheHelper.GetOrSet), nameof(CacheHelper.GetOrSetAsync), keyType, method.ValueType!);
            }
        }

        private static void BuildFunc(CacheMethod method, ILGenerator il, Type keyType, Type valueType)
        {
            Type funcType;
            if (method.Method.IsAbstract)
            {
                Ldarg(il, method.Value + 1);

                funcType = method.Method.GetParameters()[method.Value].ParameterType;
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, method.Method);

                var result = method.Method.GetParameters().Select(p => p.ParameterType).Union(new[] { valueType }).ToArray();
                funcType = result.Length switch
                {
                    1 => typeof(Func<>).MakeGenericType(result),
                    2 => typeof(Func<,>).MakeGenericType(result),
                    3 => typeof(Func<,,>).MakeGenericType(result),
                    4 => typeof(Func<,,,>).MakeGenericType(result),
                    _ => throw CacheTypeResolver.ParameterException(method.Method.DeclaringType, method.Method)
                };

                il.Emit(OpCodes.Newobj, funcType.GetConstructors()[0]);
            }

            var args = funcType.GetGenericArguments();

            Type? arg1 = null, arg2 = null, arg3 = null, returnArg = null;
            if (args.Length > 1) arg1 = args[0] == keyType ? typeof(object) : args[0];
            if (args.Length > 2) arg2 = args[1] == keyType ? typeof(object) : args[1];
            if (args.Length > 3) arg3 = args[2] == keyType ? typeof(object) : args[2];
            if (args.Length > 4) throw CacheTypeResolver.ParameterException(method.Method.DeclaringType, method.Method);

            if (args[args.Length - 1].IsGenericType)
            {
                var type = args[args.Length - 1].GetGenericTypeDefinition();

                if (type == typeof(Task<>) || type == typeof(ValueTask<>)) returnArg = type;
            }

            if (arg1 != typeof(object) || arg2 != typeof(TimeSpan) || arg3 != typeof(CancellationToken) || method.AsyncType != null && returnArg != typeof(ValueTask<>))
                il.Emit(OpCodes.Call, (method.AsyncType == null
                        ? FuncHelper.GetWrapMethod(arg1, arg2, arg3, returnArg)
                        : FuncHelper.GetWrapAsyncMethod(arg1, arg2, arg3, returnArg))
                    .MakeGenericMethod(keyType, method.ValueType));
        }

        private static void BuildSet(CacheMethod method, ILGenerator il, int defaultTtl)
        {
            Ldarg(il, method.Value + 1);

            var locals = 0;
            if (LdTtl(method, il, locals, defaultTtl)) locals++;

            if (method.When > 0) Ldarg(il, method.When + 1);
            else il.Emit(OpCodes.Ldc_I4_0);

            if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

            BuildCacheMethod(method, locals, il,
                nameof(CacheHelper.Set), nameof(CacheHelper.SetAsync),
                method.Method.GetParameters()[0].ParameterType,
                method.Method.GetParameters()[method.Value].ParameterType);
        }

        private static void BuildRemove(CacheMethod method, ILGenerator il)
        {
            var locals = 0;
            if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

            BuildCacheMethod(method, locals, il, nameof(CacheHelper.Remove), nameof(CacheHelper.RemoveAsync), method.Method.GetParameters()[0].ParameterType);
        }

        private static void BuildCacheMethod(CacheMethod method, int locals, ILGenerator il,
            string syncMethod, string asyncMethod, params Type[] typeArguments)
        {
            if (method.AsyncType == null)
            {
                il.Emit(OpCodes.Callvirt, CacheHelperMethods[syncMethod].MakeGenericMethod(typeArguments));

                if (method.Method.ReturnType == typeof(void)) il.Emit(OpCodes.Pop);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, CacheHelperMethods[asyncMethod].MakeGenericMethod(typeArguments));

                Cast(il, method, locals);
            }
        }

        private static void Ldarg(ILGenerator il, int index)
        {
            if (index == 0)
                il.Emit(OpCodes.Ldarg_0);
            else if (index == 1)
                il.Emit(OpCodes.Ldarg_1);
            else if (index == 2)
                il.Emit(OpCodes.Ldarg_2);
            else if (index == 3)
                il.Emit(OpCodes.Ldarg_3);
            else if (index <= 255)
                il.Emit(OpCodes.Ldarg_S, (byte)index);
            else
                il.Emit(OpCodes.Ldarg, index);
        }
        private static void Ldloc(ILGenerator il, int index)
        {
            if (index == 0)
                il.Emit(OpCodes.Ldloc_0);
            else if (index == 1)
                il.Emit(OpCodes.Ldloc_1);
            else if (index == 2)
                il.Emit(OpCodes.Ldloc_2);
            else if (index == 3)
                il.Emit(OpCodes.Ldloc_3);
            else if (index <= 255)
                il.Emit(OpCodes.Ldloc_S, (byte)index);
            else
                il.Emit(OpCodes.Ldloc, index);
        }
        private static void Stloc(ILGenerator il, int index)
        {
            if (index == 0)
                il.Emit(OpCodes.Stloc_0);
            else if (index == 1)
                il.Emit(OpCodes.Stloc_1);
            else if (index == 2)
                il.Emit(OpCodes.Stloc_2);
            else if (index == 3)
                il.Emit(OpCodes.Stloc_3);
            else if (index <= 255)
                il.Emit(OpCodes.Stloc_S, (byte)index);
            else
                il.Emit(OpCodes.Stloc, index);
        }

        private static bool LdTtl(CacheMethod method, ILGenerator il, int locals, int defaultTtl)
        {
            if (method.Ttl < 1)
            {
                if (defaultTtl < 1) return InitValue<TimeSpan>(method.Ttl, il, locals);

                il.Emit(OpCodes.Ldc_R8, (double)defaultTtl);
            }
            else
            {
                var type = method.Method.GetParameters()[method.Ttl].ParameterType;
                var rawType = Nullable.GetUnderlyingType(type);

                if (rawType == null) Ldarg(il, method.Ttl + 1);
                else il.Emit(OpCodes.Ldarga_S, method.Ttl + 1);

                if (type == typeof(DateTime) ||
                    type == typeof(DateTimeOffset))
                {
                    il.Emit(OpCodes.Call, type.GetProperty(nameof(DateTime.Now)).GetMethod);
                    il.Emit(OpCodes.Call, type.GetMethod("op_Subtraction", new[] { type, type }));

                    return false;
                }

                if (rawType == typeof(DateTime) ||
                    rawType == typeof(DateTimeOffset))
                {
                    var falseLabel = il.DefineLabel();
                    var endLabel = il.DefineLabel();

                    il.Emit(OpCodes.Call, type.GetProperty(nameof(Nullable<TimeSpan>.HasValue)).GetMethod);
                    il.Emit(OpCodes.Brfalse_S, falseLabel);

                    il.Emit(OpCodes.Ldarga_S, method.Ttl + 1);
                    il.Emit(OpCodes.Call, type.GetProperty(nameof(Nullable<TimeSpan>.Value)).GetMethod);

                    il.Emit(OpCodes.Call, rawType.GetProperty(nameof(DateTime.Now)).GetMethod);
                    il.Emit(OpCodes.Call, rawType.GetMethod("op_Subtraction", new[] { rawType, rawType }));
                    il.Emit(OpCodes.Br_S, endLabel);

                    il.MarkLabel(falseLabel);
                    il.DeclareLocal(typeof(TimeSpan));
                    il.Emit(OpCodes.Ldloca_S, locals);
                    il.Emit(OpCodes.Initobj, typeof(TimeSpan));
                    Ldloc(il, locals);
                    il.MarkLabel(endLabel);

                    return true;
                }

                if (rawType != null)
                    il.Emit(OpCodes.Call,
                        type.GetMethod(nameof(Nullable<TimeSpan>.GetValueOrDefault),
#if NET45
                            new Type[0]));
#else
                            Array.Empty<Type>()));
#endif
                if ((rawType ?? type) == typeof(TimeSpan)) return false;

                if ((rawType ?? type) != typeof(double))
                {
                    if ((rawType ?? type) == typeof(uint) ||
                        (rawType ?? type) == typeof(ulong))
                        il.Emit(OpCodes.Conv_R_Un);
                    else if ((rawType ?? type) == typeof(decimal))
                        il.Emit(OpCodes.Call, typeof(decimal).GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .First(m => m.Name == "op_Explicit" &&
                                        m.ReturnType == typeof(double)));

                    il.Emit(OpCodes.Conv_R8);
                }
            }

            il.Emit(OpCodes.Call, typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds)));

            return false;
        }

        private static bool InitValue<T>(int index, ILGenerator il, int locals) where T : struct
        {
            if (index > 0) Ldarg(il, index + 1);
            else
            {
                il.DeclareLocal(typeof(T));

                il.Emit(OpCodes.Ldloca_S, locals);

                il.Emit(OpCodes.Initobj, typeof(T));

                Ldloc(il, locals);
            }

            return index < 1;
        }

        private static void Cast(ILGenerator il, CacheMethod method, int locals)
        {
            if (method.AsyncType == null) return;

            var returnType = (method.RawType ?? method.ValueType) == typeof(void) ? typeof(ValueTask) : typeof(ValueTask<>).MakeGenericType(method.RawType ?? method.ValueType);

            if (returnType == method.Method.ReturnType && !(method.Method.ReturnType == typeof(ValueTask) && method.RawType != null)) return;

            if (method.Method.ReturnType == typeof(ValueTask))
                il.Emit(OpCodes.Call, ConvertMethod.MakeGenericMethod(method.RawType));
            else
            {
                il.DeclareLocal(returnType);

                Stloc(il, locals);

                il.Emit(OpCodes.Ldloca_S, locals);

                il.Emit(OpCodes.Call, returnType.GetMethod(nameof(ValueTask.AsTask)));
            }
        }

        internal static async ValueTask Convert<T>(ValueTask<T> task) => await task.ConfigureAwait(false);
    }
}
