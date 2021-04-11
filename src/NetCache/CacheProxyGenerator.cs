using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
#if BuildTask
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Rocks;
    using ConstructorInfo = Mono.Cecil.MethodReference;
    using FieldInfo = Mono.Cecil.FieldReference;
    using ILGenerator = Mono.Cecil.Cil.ILProcessor;
    using MethodBuilder = Mono.Cecil.MethodDefinition;
    using MethodInfo = Mono.Cecil.MethodReference;
    using ParameterBuilder = Mono.Cecil.ParameterDefinition;
    using ParameterInfo = Mono.Cecil.ParameterDefinition;
    using Type = Mono.Cecil.TypeReference;
    using TypeBuilder = Mono.Cecil.TypeDefinition;

    internal class CacheProxyType : CacheMetadata
    {
        private readonly IReadOnlyDictionary<string, MethodInfo> _cacheHelperMethods;

        private readonly CacheInterface _interface;

        private readonly FuncHelper _helper;

        public CacheProxyType(CacheAssembly assembly, CacheInterface @interface, FuncHelper helper, IReadOnlyDictionary<string, MethodInfo> cacheHelperMethods)
            : base(assembly)
        {
            _interface = @interface;
            _helper = helper;
            _cacheHelperMethods = cacheHelperMethods;
        }

        /// <see href="https://cecilifier.me/" />
        public TypeBuilder Build()
        {
            var type = _interface.Type;
            var proxyType = _interface.MakeProxyType();
#else
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Reflection.Emit;

    public class CacheProxyGenerator : ICacheProxyGenerator
    {
        private static readonly IReadOnlyDictionary<string, MethodInfo> CacheHelperMethods =
            typeof(CacheHelper).GetMethods()
                .Where(m => m.DeclaringType == typeof(CacheHelper))
                .GroupBy(m => m.Name)
                .ToDictionary(g => g.Key, g => g.First());

        private readonly ModuleBuilder _mb = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = "NetCache.Proxies" }, AssemblyBuilderAccess.Run).DefineDynamicModule("Proxies");
        private readonly ConcurrentDictionary<Type, Type> _proxies = new();
        private readonly FuncHelper _helper;

        public CacheProxyGenerator() => _helper = new FuncHelper(_mb);

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

        private Type CreateProxy(ModuleBuilder module, Type type)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var proxyType = module.DefineType($"{type.FullName}@Proxy@{type.Assembly.GetHashCode()}", TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.NotPublic);
#endif
            var cacheType = CacheTypeResolver.Resolve(type);
            var filed = proxyType.DefineField("_helper", typeof(CacheHelper), FieldAttributes.Private | FieldAttributes.InitOnly);

            BuildConstructors(proxyType, cacheType, filed);

            foreach (var method in cacheType.Methods)
            {
                var mb = proxyType.DefineMethod(method.Method.Name,
                    type.IsInterface
                        ? MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot
                        : MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    method.Method.ReturnType,
                    method.Method.GetParameters().Select(p => p.ParameterType).ToArray());

                var il = mb.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, filed);
                il.Emit(OpCodes.Ldarg_1);

                DefineGenericParameters(mb, method.Method);

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
#if !BuildTask
                if (type.IsInterface) proxyType.DefineMethodOverride(mb, method.Method);
#endif
                BuildParameters(mb.DefineParameter, method.Method.GetParameters());
            }
#if BuildTask
            return proxyType;
#elif NETSTANDARD2_0
            return proxyType.CreateTypeInfo()!;
#else
            return proxyType.CreateType()!;
#endif
        }

        private static void DefineGenericParameters(MethodBuilder mb, MethodInfo method)
        {
#if BuildTask
            if (!method.IsGenericInstance) return;

            foreach (var gp in method.GenericParameters) mb.GenericParameters.Add(gp);
#else
            if (!method.IsGenericMethod) return;

            var gas = method.GetGenericArguments();

            var gtpb = mb.DefineGenericParameters(gas.Select((_, index) => "T" + index).ToArray());
            for (var index = 0; index < gas.Length; index++)
            {
                gtpb[index].SetGenericParameterAttributes(gas[index].GenericParameterAttributes);

                gtpb[index].SetBaseTypeConstraint(gas[index].BaseType);

                gtpb[index].SetInterfaceConstraints(gas[index].GetInterfaces());

                foreach (var attr in gas[index].GetCustomAttributesData())
                {
                    var cab = attr.NamedArguments == null ?
                    new CustomAttributeBuilder(
                            attr.Constructor,
                            attr.ConstructorArguments.Select(a => a.Value).ToArray())
                        : new CustomAttributeBuilder(
                            attr.Constructor,
                            attr.ConstructorArguments.Select(a => a.Value).ToArray(),
                            attr.NamedArguments.Where(a => !a.IsField).Select(a => (PropertyInfo)a.MemberInfo).ToArray(),
                            attr.NamedArguments.Where(a => !a.IsField).Select(a => a.TypedValue.Value).ToArray(),
                            attr.NamedArguments.Where(a => a.IsField).Select(a => (FieldInfo)a.MemberInfo).ToArray(),
                            attr.NamedArguments.Where(a => a.IsField).Select(a => a.TypedValue.Value).ToArray()
                        );

                    gtpb[index].SetCustomAttribute(cab);
                }
            }
#endif
        }

        private void BuildConstructors(TypeBuilder tb, CacheType type, FieldInfo filed)
        {
#if BuildTask
            var ctors = _interface.Type.IsInterface
                ? TypeSystem.Object.Resolve().GetConstructors()
                : _interface.Type.GetConstructors().Where(ctor => ctor.IsFamily || ctor.IsPublic);

            foreach (var method in ctors)
                foreach (var ctor in ImportType<CacheHelper>().Resolve().GetConstructors().Take(1))
                    BuildConstructor(tb, type, filed, Module.ImportReference(method), Module.ImportReference(ctor));
#else
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
                    BuildConstructor(tb, type, filed, method, ctor);
#endif
        }

        private void BuildConstructor(TypeBuilder tb, CacheType type, FieldInfo field, ConstructorInfo baseCtor, ConstructorInfo helperCtor)
        {
            var p1 = baseCtor.GetParameters();
            var p2 = helperCtor.GetParameters();
            var parameterTypes = p1.Union(p2.Skip(1)).Take(p1.Length + p2.Length - 2).Select(p => p.ParameterType).ToArray();
#if BuildTask
            var ctor = tb.DefineMethod(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, TypeSystem.Void, parameterTypes);

            ctor.HasThis = true;
#else
            var ctor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, parameterTypes);
#endif
            BuildParameters(ctor.DefineParameter, p1.Union(p2.Skip(1)).Take(p1.Length + p2.Length - 2));

            var il = ctor.GetILGenerator();

            //base.ctor
            il.Emit(OpCodes.Ldarg_0);
            var index = 1;
            for (; index <= p1.Length; index++) Ldarg(il, index);

            il.Emit(OpCodes.Call, baseCtor);

            //new CacheHelper
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, type.Name);
            for (; index < p1.Length + p2.Length - 1; index++) Ldarg(il, index);

            Ldc(il, type.DefaultTtl);

            il.Emit(OpCodes.Newobj, helperCtor);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);
        }

        private static void BuildParameters(Func<int, ParameterAttributes, string, ParameterBuilder> defineParameter, IEnumerable<ParameterInfo> parameters)
        {
            var index = 1;
            foreach (var p in parameters)
            {
                defineParameter(index++, p.Attributes, p.Name);
                //TODO AttributeData;
            }
        }

        private void BuildGet(CacheMethod method, ILGenerator il, int defaultTtl)
        {
            var keyType = method.Method.GetParameters()[0].ParameterType;
#if BuildTask
            if (method.Method.Resolve().IsAbstract && method.Value < 1)
#else
            if (method.Method.IsAbstract && method.Value < 1)
#endif
            {
                var locals = 0;
                if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

                BuildCacheMethod(method, locals, il, nameof(CacheHelper.Get), keyType, method.WarpedValue ? method.ValueType.GetGenericArguments()[0] : method.ValueType);
            }
            else
            {
                BuildFunc(method, il, keyType, method.ValueType);

                var locals = 0;
                if (LdTtl(method, il, locals, defaultTtl)) locals++;
                if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

                BuildCacheMethod(method, locals, il, nameof(CacheHelper.GetOrSet), keyType, method.WarpedValue ? method.ValueType.GetGenericArguments()[0] : method.ValueType);
            }
        }

        private void BuildFunc(CacheMethod method, ILGenerator il, Type keyType, Type valueType)
        {
#if BuildTask
            GenericInstanceType funcType;
            if (method.Method.Resolve().IsAbstract)
#else
            Type funcType;
            if (method.Method.IsAbstract)
#endif
            {
                Ldarg(il, method.Value + 1);
#if BuildTask
                funcType = (GenericInstanceType)method.Method.GetParameters()[method.Value].ParameterType;
#else
                funcType = method.Method.GetParameters()[method.Value].ParameterType;
#endif
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldftn, method.Method);

                var result = method.Method.GetParameters().Select(p => p.ParameterType).Union(new[] { valueType }).ToArray();
                funcType = result.Length switch
                {
#if BuildTask
                    1 => new GenericInstanceType(ImportType(typeof(Func<>))),
                    2 => new GenericInstanceType(ImportType(typeof(Func<,>))),
                    3 => new GenericInstanceType(ImportType(typeof(Func<,,>))),
                    4 => new GenericInstanceType(ImportType(typeof(Func<,,,>))),
#else
                    1 => typeof(Func<>).MakeGenericType(result),
                    2 => typeof(Func<,>).MakeGenericType(result),
                    3 => typeof(Func<,,>).MakeGenericType(result),
                    4 => typeof(Func<,,,>).MakeGenericType(result),
#endif
                    _ => throw CacheTypeResolver.ParameterException(method.Method.DeclaringType!, method.Method)
                };
#if BuildTask
                foreach (var p in result) funcType.GenericArguments.Add(p);

                il.Emit(OpCodes.Newobj, Module.ImportReference(funcType.Resolve().GetConstructors().First()).CopyTo(funcType));
#else
                il.Emit(OpCodes.Newobj, funcType.GetConstructors()[0]);
#endif
            }

            var args = funcType.GetGenericArguments();

            Type? arg1 = null, arg2 = null, arg3 = null, returnArg = null;
#if BuildTask
            if (args.Length > 1) arg1 = args[0] == keyType ? TypeSystem.Object : args[0];
            if (args.Length > 2) arg2 = args[1] == keyType ? TypeSystem.Object : args[1];
            if (args.Length > 3) arg3 = args[2] == keyType ? TypeSystem.Object : args[2];
            if (args.Length > 4) throw CacheTypeResolver.ParameterException(method.Method.DeclaringType!, method.Method);

            if (args[args.Length - 1].IsGenericInstance)
            {
                var type = args[args.Length - 1].GetElementType();

                if (type.IsType(typeof(Task<>)) || type.IsType(typeof(ValueTask<>))) returnArg = type;
            }

            if (arg1 != null && arg1.IsType<object>() &&
                arg2 != null && arg2.IsType<TimeSpan>() &&
                arg3 != null && arg3.IsType<CancellationToken>() &&
                (method.AsyncType == null || returnArg != null && returnArg.IsType(typeof(ValueTask<>)))) return;

            var fat = _helper.FuncAdapterType.MakeGenericInstanceType(keyType, method.ValueType);

            il.Emit(OpCodes.Newobj, new MethodInfo(".ctor", TypeSystem.Void, fat) { Parameters = { new ParameterBuilder(TypeSystem.Object) }, HasThis = true });
            il.Emit(OpCodes.Ldftn, Module.ImportReference(_helper.GetWrapMethod(method.AsyncType == null, arg1, arg2, arg3, returnArg)).CopyTo(fat));
            il.Emit(OpCodes.Newobj, Module.ImportReference(typeof(Func<,,,>).GetConstructors()[0]).CopyTo(ImportType(typeof(Func<,,,>)).MakeGenericInstanceType(keyType.GetElementType(), ImportType<TimeSpan>(), ImportType<CancellationToken>(), method.AsyncType == null ? method.ValueType : ImportType(typeof(ValueTask<>)).MakeGenericInstanceType(method.ValueType.GetElementType()))));
#else
            if (args.Length > 1) arg1 = args[0] == keyType ? typeof(object) : args[0];
            if (args.Length > 2) arg2 = args[1] == keyType ? typeof(object) : args[1];
            if (args.Length > 3) arg3 = args[2] == keyType ? typeof(object) : args[2];
            if (args.Length > 4) throw CacheTypeResolver.ParameterException(method.Method.DeclaringType!, method.Method);

            if (args[args.Length - 1].IsGenericType)
            {
                var type = args[args.Length - 1].GetGenericTypeDefinition();

                if (type == typeof(Task<>) || type == typeof(ValueTask<>)) returnArg = type;
            }

            if (arg1 == typeof(object) && arg2 == typeof(TimeSpan) && arg3 == typeof(CancellationToken)
                && (method.AsyncType == null || returnArg == typeof(ValueTask<>))) return;

            var fat = _helper.FuncAdapterType.MakeGenericType(keyType, method.ValueType);

            il.Emit(OpCodes.Newobj, fat.GetConstructors()[0]);
            il.Emit(OpCodes.Ldftn, fat.GetMethod(_helper.GetWrapMethod(method.AsyncType == null, arg1, arg2, arg3, returnArg).Name)!);
            il.Emit(OpCodes.Newobj, typeof(Func<,,,>).MakeGenericType(keyType, typeof(TimeSpan), typeof(CancellationToken), method.AsyncType == null ? method.ValueType : typeof(ValueTask<>).MakeGenericType(method.ValueType)).GetConstructors()[0]);
#endif
        }

        private void BuildSet(CacheMethod method, ILGenerator il, int defaultTtl)
        {
            Ldarg(il, method.Value + 1);

            var locals = 0;
            if (LdTtl(method, il, locals, defaultTtl)) locals++;

            if (method.When > 0) Ldarg(il, method.When + 1);
            else Ldc(il, 0);

            if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

            BuildCacheMethod(method, locals, il, nameof(CacheHelper.Set),
                method.Method.GetParameters()[0].ParameterType,
                method.Method.GetParameters()[method.Value].ParameterType);
        }

        private void BuildRemove(CacheMethod method, ILGenerator il)
        {
            var locals = 0;
            if (InitValue<CancellationToken>(method.CancellationToken, il, locals)) locals++;

            BuildCacheMethod(method, locals, il, nameof(CacheHelper.Remove), method.Method.GetParameters()[0].ParameterType);
        }

        private void BuildCacheMethod(CacheMethod method, int locals, ILGenerator il,
            string methodName, params Type[] typeArguments)
        {
            if (method.WarpedValue) methodName += "2";

            if (method.AsyncType == null)
            {
#if BuildTask
                il.Emit(OpCodes.Callvirt, GetCacheMethod(methodName, typeArguments));

                if (method.Method.ReturnType.IsType(typeof(void))) il.Emit(OpCodes.Pop);
#else
                il.Emit(OpCodes.Callvirt, CacheHelperMethods[methodName].MakeGenericMethod(typeArguments));

                if (method.Method.ReturnType == typeof(void)) il.Emit(OpCodes.Pop);
#endif
            }
            else
            {
#if BuildTask
                il.Emit(OpCodes.Callvirt, GetCacheMethod(methodName + "Async", typeArguments));
#else
                il.Emit(OpCodes.Callvirt, CacheHelperMethods[methodName + "Async"].MakeGenericMethod(typeArguments));
#endif
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

        private static void Ldloca(ILGenerator il, int index)
        {
            if (index <= 255)
                il.Emit(OpCodes.Ldloca_S, (byte)index);
            else
                il.Emit(OpCodes.Ldloca, index);
        }

        private static void Ldc(ILGenerator il, int index)
        {
            if (index == -1)
                il.Emit(OpCodes.Ldc_I4_M1);
            else if (index == 0)
                il.Emit(OpCodes.Ldc_I4_0);
            else if (index == 1)
                il.Emit(OpCodes.Ldc_I4_1);
            else if (index == 2)
                il.Emit(OpCodes.Ldc_I4_2);
            else if (index == 3)
                il.Emit(OpCodes.Ldc_I4_3);
            else if (index == 4)
                il.Emit(OpCodes.Ldc_I4_4);
            else if (index == 5)
                il.Emit(OpCodes.Ldc_I4_5);
            else if (index == 6)
                il.Emit(OpCodes.Ldc_I4_6);
            else if (index == 7)
                il.Emit(OpCodes.Ldc_I4_7);
            else if (index == 8)
                il.Emit(OpCodes.Ldc_I4_8);
            else if (index is <= sbyte.MaxValue and >= sbyte.MinValue)
                il.Emit(OpCodes.Ldc_I4_S, (sbyte)index);
            else
                il.Emit(OpCodes.Ldc_I4, index);
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

        private bool LdTtl(CacheMethod method, ILGenerator il, int locals, int defaultTtl)
        {
            if (method.Ttl < 1)
            {
                if (defaultTtl < 1) return InitValue<TimeSpan>(method.Ttl, il, locals);

                il.Emit(OpCodes.Ldc_R8, (double)defaultTtl);
            }
            else
            {
                var type = method.Method.GetParameters()[method.Ttl].ParameterType;
#if BuildTask
                var rawType = type.GetUnderlyingType();
#else
                var rawType = Nullable.GetUnderlyingType(type);
#endif
                if (rawType == null) Ldarg(il, method.Ttl + 1);
                else il.Emit(OpCodes.Ldarga_S, (byte)(method.Ttl + 1));
#if BuildTask
                if (type.IsType<DateTime>() ||
                    type.IsType<DateTimeOffset>())
#else
                if (type == typeof(DateTime) ||
                    type == typeof(DateTimeOffset))
#endif
                {
                    il.Emit(OpCodes.Call, type.GetGetMethod(nameof(DateTime.Now))!);
#if BuildTask
                    il.Emit(OpCodes.Call, type.GetMethod(m => m.Name == "op_Subtraction" && m.ReturnType.IsType<TimeSpan>())!);
#else
                    il.Emit(OpCodes.Call, type.GetMethod("op_Subtraction", new[] { type, type })!);
#endif
                    return false;
                }
#if BuildTask
                if (rawType != null && (rawType.IsType<DateTime>() || rawType.IsType<DateTimeOffset>()))
#else
                if (rawType == typeof(DateTime) ||
                    rawType == typeof(DateTimeOffset))
#endif
                {
                    var falseLabel = il.DefineLabel();
                    var endLabel = il.DefineLabel();

                    il.Emit(OpCodes.Call, type.GetGetMethod(nameof(Nullable<TimeSpan>.HasValue))!);
                    il.Emit(OpCodes.Brfalse_S, falseLabel);

                    il.Emit(OpCodes.Ldarga_S, (byte)(method.Ttl + 1));
                    il.Emit(OpCodes.Call, type.GetGetMethod(nameof(Nullable<TimeSpan>.Value))!);

                    il.Emit(OpCodes.Call, rawType.GetGetMethod(nameof(DateTime.Now))!);
#if BuildTask
                    il.Emit(OpCodes.Call, rawType.GetMethod(m => m.Name == "op_Subtraction" && m.ReturnType.IsType<TimeSpan>())!);
#else
                    il.Emit(OpCodes.Call, rawType.GetMethod("op_Subtraction", new[] { rawType, rawType })!);
#endif
                    il.Emit(OpCodes.Br_S, endLabel);

                    il.MarkLabel(falseLabel);
                    InitValue<TimeSpan>(-1, il, locals);
                    il.MarkLabel(endLabel);

                    return true;
                }

                if (rawType != null)
                    il.Emit(OpCodes.Call,
#if BuildTask
                        Module.ImportReference(type.GetMethod("GetValueOrDefault")));

                if ((rawType ?? type).IsType<TimeSpan>()) return false;

                if (!(rawType ?? type).IsType<double>())
                {
                    if ((rawType ?? type).IsType<uint>() || (rawType ?? type).IsType<ulong>())
                        il.Emit(OpCodes.Conv_R_Un);
                    else if ((rawType ?? type).IsType<decimal>())
                        il.Emit(OpCodes.Call, Module.ImportReference(Module.ImportReference(typeof(decimal))
                            .GetMethod(m => m.Name == "op_Explicit" && m.ReturnType.IsType<double>())));
#else
                        type.GetMethod(nameof(Nullable<TimeSpan>.GetValueOrDefault),
#if NET45
                            new Type[0])!);
#else
                            Array.Empty<Type>())!);
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
#endif
                    il.Emit(OpCodes.Conv_R8);
                }
            }
#if BuildTask
            il.Emit(OpCodes.Call, _interface.Type.Module.ImportReference(typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!));
#else
            il.Emit(OpCodes.Call, typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromSeconds))!);
#endif
            return false;
        }

        private bool InitValue<T>(int index, ILGenerator il, int locals) where T : struct
        {
            if (index > 0) Ldarg(il, index + 1);
            else
            {
                il.DeclareLocal(typeof(T));

                Ldloca(il, locals);
#if BuildTask
                il.Emit(OpCodes.Initobj, ImportType<T>());
#else
                il.Emit(OpCodes.Initobj, typeof(T));
#endif
                Ldloc(il, locals);
            }

            return index < 1;
        }

        private void Cast(ILGenerator il, CacheMethod method, int locals)
        {
            if (method.AsyncType == null) return;
#if BuildTask
            var returnType = (method.RawType ?? method.ValueType).IsType(typeof(void)) ? ImportType<ValueTask>() : ImportType(typeof(ValueTask<>)).MakeGenericInstanceType(method.RawType ?? method.ValueType);

            if (returnType.IsType(method.Method.ReturnType) && !(method.Method.ReturnType.IsType<ValueTask>() && method.RawType != null)) return;
#else
            var returnType = (method.RawType ?? method.ValueType) == typeof(void) ? typeof(ValueTask) : typeof(ValueTask<>).MakeGenericType(method.RawType ?? method.ValueType);

            if (returnType == method.Method.ReturnType && !(method.Method.ReturnType == typeof(ValueTask) && method.RawType != null)) return;
#endif
            il.DeclareLocal(returnType);

            Stloc(il, locals);

            Ldloca(il, locals);
#if BuildTask
            if (method.Method.ReturnType.IsType<ValueTask>())
#else
            if (method.Method.ReturnType == typeof(ValueTask))
#endif
            {
                var trueLabel = il.DefineLabel();
                var endLabel = il.DefineLabel();
                var endLabel2 = il.DefineLabel();

                il.Emit(OpCodes.Call, returnType.GetGetMethod(nameof(ValueTask.IsCompletedSuccessfully))!);
                il.Emit(OpCodes.Brtrue_S, trueLabel);

                Ldloca(il, locals);
                il.Emit(OpCodes.Call, returnType.GetMethod(nameof(ValueTask.AsTask))!);
#if BuildTask
                il.Emit(OpCodes.Newobj, Module.ImportReference(typeof(ValueTask).GetConstructor(new[] { typeof(Task) })!));
#else
                il.Emit(OpCodes.Newobj, typeof(ValueTask).GetConstructor(new[] { typeof(Task) })!);
#endif
                il.Emit(OpCodes.Br_S, endLabel);

                il.MarkLabel(trueLabel);
                InitValue<ValueTask>(-1, il, ++locals);

                il.MarkLabel(endLabel);
                il.DeclareLocal(typeof(ValueTask));
                Stloc(il, ++locals);
                il.Emit(OpCodes.Br_S, endLabel2);

                il.MarkLabel(endLabel2);
                Ldloc(il, locals);
            }
            else
            {
                il.Emit(OpCodes.Call, returnType.GetMethod(nameof(ValueTask.AsTask))!);
            }
        }
#if BuildTask
        private MethodInfo GetCacheMethod(string name, params Type[] typeArguments)
        {
            if (typeArguments.Length < 1) return _cacheHelperMethods[name];

            var m = new GenericInstanceMethod(_cacheHelperMethods[name]);

            foreach (var arg in typeArguments) m.GenericArguments.Add(arg);

            return m;
        }
#endif
    }
}
