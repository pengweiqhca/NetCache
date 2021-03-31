using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    internal partial class FuncHelper
    {
        public static Type CreateType(ModuleBuilder module)
        {
            var proxy = module.GetType("NetCache.FuncAdapter@");
            if (proxy != null) return proxy;

            var type = module.DefineType("NetCache.FuncAdapter@", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Abstract | TypeAttributes.Public | TypeAttributes.Sealed, typeof(object));

            var adapter = type.DefineNestedType("Adapter", TypeAttributes.NestedPrivate | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, typeof(object));

            GenericTypeParameterBuilder[] mp;
            var gp = adapter.DefineGenericParameters("TK", "TV");

            var func = adapter.DefineField("_func", typeof(object), FieldAttributes.Private);

            MethodBuilder am, m;
            var ctor = adapter.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new[] { typeof(object) });

            var il = ctor.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, func);
            il.Emit(OpCodes.Ret);

            var aggressiveInlining = new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor(new[] { typeof(MethodImplOptions) }), new object[] { MethodImplOptions.AggressiveInlining });
            var taskReturnType = typeof(Task<>).MakeGenericType(gp[1].UnderlyingSystemType);
            var valueTaskReturnType = typeof(ValueTask<>).MakeGenericType(gp[1].UnderlyingSystemType);
            var syncCtor = typeof(ValueTask<>).GetConstructors().First(c => !c.GetParameters()[0].ParameterType.Name.Contains("Task`1"));
            var asyncCtor = typeof(ValueTask<>).GetConstructors().First(c => c.GetParameters()[0].ParameterType.Name.Contains("Task`1"));

            var funcCtor = typeof(Func<,,,>).GetConstructors().First();
            var invoke0 = typeof(Func<>).GetMethod("Invoke");
            var invoke1 = typeof(Func<,>).GetMethod("Invoke");
            var invoke2 = typeof(Func<,,>).GetMethod("Invoke");
            var invoke3 = typeof(Func<,,,>).GetMethod("Invoke");

            Type key;

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap0", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<>).MakeGenericType(mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap1", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,>).MakeGenericType(key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap2", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap3", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap4", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap5", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap6", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap7", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap8", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap9", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap10", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap11", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap12", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap13", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap14", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap15", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("Wrap", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap16", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<>).MakeGenericType(mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap17", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap18", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap19", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap20", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap21", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap22", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap23", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap24", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap25", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap26", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap27", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap28", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap29", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap30", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap31", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, mp[1].UnderlyingSystemType));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap32", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(taskReturnType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<>).MakeGenericType(typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap33", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(key, typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap34", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(TimeSpan), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap35", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap36", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap37", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap38", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap39", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap40", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap41", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap42", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap43", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap44", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap45", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap46", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap47", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, typeof(Task<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap48", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(valueTaskReturnType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<>).MakeGenericType(typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap49", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(key, typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap50", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(TimeSpan), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap51", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap52", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap53", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap54", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap55", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap56", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap57", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap58", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap59", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(key, typeof(CancellationToken), typeof(TimeSpan), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap60", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), key, typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap61", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), key, typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap62", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), key, typeof(TimeSpan), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

            key = gp[0].UnderlyingSystemType;

            am = adapter.DefineMethod("Wrap63", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke3);
            il.Emit(OpCodes.Ret);

            m = type.DefineMethod("WrapAsync", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static);

            mp = m.DefineGenericParameters("TK", "TV");

            key = mp[0].UnderlyingSystemType;

            m.SetReturnType(typeof(Func<,,,>).MakeGenericType(key, typeof(TimeSpan), typeof(CancellationToken), typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.SetParameters(typeof(Func<,,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), key, typeof(ValueTask<>).MakeGenericType(mp[1].UnderlyingSystemType)));
            m.DefineParameter(1, ParameterAttributes.None, "func");

            m.SetCustomAttribute(aggressiveInlining);

            il = m.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ldftn, am);
            il.Emit(OpCodes.Newobj, funcCtor);

            il.Emit(OpCodes.Ret);

#if NETSTANDARD2_0
            adapter.CreateTypeInfo();

            return type.CreateTypeInfo()!;
#else
            adapter.CreateType();

            return type.CreateType()!;
#endif
        }
    }
}
