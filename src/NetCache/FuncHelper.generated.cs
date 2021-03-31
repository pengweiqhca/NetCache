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
        private Type CreateType(ModuleBuilder module)
        {
            var proxy = module.GetType("NetCache.FuncAdapter@");
            if (proxy != null) throw new InvalidOperationException("TYpe 'NetCache.FuncAdapter@' has exits.");

            var adapter = module.DefineType("NetCache.FuncAdapter@", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.Public | TypeAttributes.Sealed, typeof(object));

            var gp = adapter.DefineGenericParameters("TK", "TV");

            var func = adapter.DefineField("_func", typeof(object), FieldAttributes.Private);

            var ctor = adapter.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new[] { typeof(object) });

            ctor.DefineParameter(1, ParameterAttributes.None, "func");

            var il = ctor.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructors().First());
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, func);
            il.Emit(OpCodes.Ret);

            MethodBuilder am;
            var key = gp[0].UnderlyingSystemType;

            var aggressiveInlining = new CustomAttributeBuilder(typeof(MethodImplAttribute).GetConstructor(new[] { typeof(MethodImplOptions) }), new object[] { MethodImplOptions.AggressiveInlining });

            var taskReturnType = typeof(Task<>).MakeGenericType(gp[1].UnderlyingSystemType);
            var valueTaskReturnType = typeof(ValueTask<>).MakeGenericType(gp[1].UnderlyingSystemType);

            var syncCtor = typeof(ValueTask<>).GetConstructors().First(c => !c.GetParameters()[0].ParameterType.Name.Contains("Task`1"));
            var asyncCtor = typeof(ValueTask<>).GetConstructors().First(c => c.GetParameters()[0].ParameterType.Name.Contains("Task`1"));

            var invoke0 = typeof(Func<>).GetMethod("Invoke");
            var invoke1 = typeof(Func<,>).GetMethod("Invoke");
            var invoke2 = typeof(Func<,,>).GetMethod("Invoke");
            var invoke3 = typeof(Func<,,,>).GetMethod("Invoke");

            am = adapter.DefineMethod("Wrap0", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
            }] = 0;

            am = adapter.DefineMethod("Wrap1", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(object),
            }] = 1;

            am = adapter.DefineMethod("Wrap2", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
            }] = 2;

            am = adapter.DefineMethod("Wrap3", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
            }] = 3;

            am = adapter.DefineMethod("Wrap4", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
            }] = 4;

            am = adapter.DefineMethod("Wrap5", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
            }] = 5;

            am = adapter.DefineMethod("Wrap6", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
            }] = 6;

            am = adapter.DefineMethod("Wrap7", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
            }] = 7;

            am = adapter.DefineMethod("Wrap8", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
            }] = 8;

            am = adapter.DefineMethod("Wrap9", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
            }] = 9;

            am = adapter.DefineMethod("Wrap10", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(CancellationToken),
            }] = 10;

            am = adapter.DefineMethod("Wrap11", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(TimeSpan),
            }] = 11;

            am = adapter.DefineMethod("Wrap12", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                Arg3 = typeof(CancellationToken),
            }] = 12;

            am = adapter.DefineMethod("Wrap13", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(object),
            }] = 13;

            am = adapter.DefineMethod("Wrap14", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                Arg3 = typeof(TimeSpan),
            }] = 14;

            am = adapter.DefineMethod("Wrap15", MethodAttributes.Public | MethodAttributes.HideBySig, gp[1].UnderlyingSystemType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(object),
            }] = 15;

            am = adapter.DefineMethod("Wrap16", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
            }] = 16;

            am = adapter.DefineMethod("Wrap17", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
            }] = 17;

            am = adapter.DefineMethod("Wrap18", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
            }] = 18;

            am = adapter.DefineMethod("Wrap19", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), gp[1].UnderlyingSystemType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, syncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
            }] = 19;

            am = adapter.DefineMethod("Wrap20", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
            }] = 20;

            am = adapter.DefineMethod("Wrap21", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
            }] = 21;

            am = adapter.DefineMethod("Wrap22", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
            }] = 22;

            am = adapter.DefineMethod("Wrap23", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
            }] = 23;

            am = adapter.DefineMethod("Wrap24", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
            }] = 24;

            am = adapter.DefineMethod("Wrap25", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
            }] = 25;

            am = adapter.DefineMethod("Wrap26", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(CancellationToken),
            }] = 26;

            am = adapter.DefineMethod("Wrap27", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(TimeSpan),
            }] = 27;

            am = adapter.DefineMethod("Wrap28", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                Arg3 = typeof(CancellationToken),
            }] = 28;

            am = adapter.DefineMethod("Wrap29", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(object),
            }] = 29;

            am = adapter.DefineMethod("Wrap30", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                Arg3 = typeof(TimeSpan),
            }] = 30;

            am = adapter.DefineMethod("Wrap31", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(object),
            }] = 31;

            am = adapter.DefineMethod("Wrap32", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(taskReturnType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                ReturnArg = typeof(Task<>),
            }] = 32;

            am = adapter.DefineMethod("Wrap33", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, taskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                ReturnArg = typeof(Task<>),
            }] = 33;

            am = adapter.DefineMethod("Wrap34", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), taskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                ReturnArg = typeof(Task<>),
            }] = 34;

            am = adapter.DefineMethod("Wrap35", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), taskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Newobj, asyncCtor);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                ReturnArg = typeof(Task<>),
            }] = 35;

            am = adapter.DefineMethod("Wrap36", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                ReturnArg = typeof(Task<>),
            }] = 36;

            am = adapter.DefineMethod("Wrap37", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                ReturnArg = typeof(Task<>),
            }] = 37;

            am = adapter.DefineMethod("Wrap38", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                ReturnArg = typeof(Task<>),
            }] = 38;

            am = adapter.DefineMethod("Wrap39", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                ReturnArg = typeof(Task<>),
            }] = 39;

            am = adapter.DefineMethod("Wrap40", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                ReturnArg = typeof(Task<>),
            }] = 40;

            am = adapter.DefineMethod("Wrap41", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                ReturnArg = typeof(Task<>),
            }] = 41;

            am = adapter.DefineMethod("Wrap42", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(CancellationToken),
                ReturnArg = typeof(Task<>),
            }] = 42;

            am = adapter.DefineMethod("Wrap43", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(TimeSpan),
                ReturnArg = typeof(Task<>),
            }] = 43;

            am = adapter.DefineMethod("Wrap44", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                Arg3 = typeof(CancellationToken),
                ReturnArg = typeof(Task<>),
            }] = 44;

            am = adapter.DefineMethod("Wrap45", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(object),
                ReturnArg = typeof(Task<>),
            }] = 45;

            am = adapter.DefineMethod("Wrap46", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                Arg3 = typeof(TimeSpan),
                ReturnArg = typeof(Task<>),
            }] = 46;

            am = adapter.DefineMethod("Wrap47", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(object),
                ReturnArg = typeof(Task<>),
            }] = 47;

            am = adapter.DefineMethod("Wrap48", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<>).MakeGenericType(valueTaskReturnType));
            il.Emit(OpCodes.Callvirt, invoke0);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                ReturnArg = typeof(ValueTask<>),
            }] = 48;

            am = adapter.DefineMethod("Wrap49", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                ReturnArg = typeof(ValueTask<>),
            }] = 49;

            am = adapter.DefineMethod("Wrap50", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                ReturnArg = typeof(ValueTask<>),
            }] = 50;

            am = adapter.DefineMethod("Wrap51", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,>).MakeGenericType(typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke1);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                ReturnArg = typeof(ValueTask<>),
            }] = 51;

            am = adapter.DefineMethod("Wrap52", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                ReturnArg = typeof(ValueTask<>),
            }] = 52;

            am = adapter.DefineMethod("Wrap53", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(key, typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                ReturnArg = typeof(ValueTask<>),
            }] = 53;

            am = adapter.DefineMethod("Wrap54", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                ReturnArg = typeof(ValueTask<>),
            }] = 54;

            am = adapter.DefineMethod("Wrap55", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(TimeSpan), typeof(CancellationToken), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                ReturnArg = typeof(ValueTask<>),
            }] = 55;

            am = adapter.DefineMethod("Wrap56", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), key, valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                ReturnArg = typeof(ValueTask<>),
            }] = 56;

            am = adapter.DefineMethod("Wrap57", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

            am.SetCustomAttribute(aggressiveInlining);

            il = am.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, func);
            il.Emit(OpCodes.Castclass, typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), typeof(TimeSpan), valueTaskReturnType));
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, invoke2);
            il.Emit(OpCodes.Ret);

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                ReturnArg = typeof(ValueTask<>),
            }] = 57;

            am = adapter.DefineMethod("Wrap58", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(CancellationToken),
                ReturnArg = typeof(ValueTask<>),
            }] = 58;

            am = adapter.DefineMethod("Wrap59", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(object),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(TimeSpan),
                ReturnArg = typeof(ValueTask<>),
            }] = 59;

            am = adapter.DefineMethod("Wrap60", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(object),
                Arg3 = typeof(CancellationToken),
                ReturnArg = typeof(ValueTask<>),
            }] = 60;

            am = adapter.DefineMethod("Wrap61", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(TimeSpan),
                Arg2 = typeof(CancellationToken),
                Arg3 = typeof(object),
                ReturnArg = typeof(ValueTask<>),
            }] = 61;

            am = adapter.DefineMethod("Wrap62", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(object),
                Arg3 = typeof(TimeSpan),
                ReturnArg = typeof(ValueTask<>),
            }] = 62;

            am = adapter.DefineMethod("Wrap63", MethodAttributes.Public | MethodAttributes.HideBySig, valueTaskReturnType, new [] { key, typeof(TimeSpan), typeof(CancellationToken) });

            am.DefineParameter(1, ParameterAttributes.None, "key");
            am.DefineParameter(2, ParameterAttributes.None, "expiry");
            am.DefineParameter(3, ParameterAttributes.None, "cancellationToken");

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

            _wrapAsyncMethods[new FuncType
            {
                Arg1 = typeof(CancellationToken),
                Arg2 = typeof(TimeSpan),
                Arg3 = typeof(object),
                ReturnArg = typeof(ValueTask<>),
            }] = 63;

#if NETSTANDARD2_0
            return adapter.CreateTypeInfo()!;
#else
            return adapter.CreateType()!;
#endif
        }
    }
}
