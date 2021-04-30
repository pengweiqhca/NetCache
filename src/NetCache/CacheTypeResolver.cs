using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if BuildTask
using Mono.Cecil;
using Mono.Cecil.Rocks;
using MemberInfo = Mono.Cecil.IMemberDefinition;
using MethodInfo = Mono.Cecil.MethodReference;
using Type = Mono.Cecil.TypeReference;
#else
using System.Reflection;
#endif

namespace NetCache
{
    internal static class CacheTypeResolver
    {
#if BuildTask
        public static readonly TypeDefinition Void;
        private static readonly TypeDefinition Boolean;

        static CacheTypeResolver()
        {
            var module = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location).MainModule;

            Void = module.GetType(typeof(void).FullName);
            Boolean = module.GetType(typeof(bool).FullName);
        }
#else
        private static readonly Type Boolean = typeof(bool);
#endif
        /// <exception cref="AggregateException"></exception>
#if BuildTask
        public static CacheType Resolve(TypeDefinition type)
#else
        public static CacheType Resolve(Type type)
#endif
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var exceptions = new List<Exception>();
            var methods = new List<CacheMethod>();

            foreach (var method in type.GetMethods())
            {
                var canBeOverride = method.IsVirtual && !method.IsFinal;
#if BuildTask
                if (!canBeOverride || method.DeclaringType == method.Module.TypeSystem.Object) continue;
#else
                if (!canBeOverride || method.DeclaringType == typeof(object)) continue;
#endif
                var operation = GetOperation(method);

                if (operation == CacheOperation.Ignore)
                {
                    if (method.IsAbstract) exceptions.Add(new InvalidOperationException($"不支持的抽象方法{method.Name}"));

                    continue;
                }

                CacheMethod? cm = null;
                switch (operation)
                {
                    case CacheOperation.Get:
                        cm = ResolveGet(method);
                        if (cm == null) exceptions.Add(ParameterException(type, method));
                        break;
                    case CacheOperation.Set:
                        cm = ResolveSet(method);
                        if (cm == null) exceptions.Add(ParameterException(type, method));
                        break;
                    case CacheOperation.Remove:
                        cm = ResolveRemove(method);
                        if (cm == null) exceptions.Add(ParameterException(type, method));
                        break;
                    default:
                        exceptions.Add(new InvalidOperationException($"{method.Name}不支持的操作{operation}"));
                        break;
                }

                if (cm != null) methods.Add(cm);
            }

            if (exceptions.Count > 0) throw new AggregateException(exceptions);

            return new CacheType(GetCacheName(type, out var defaultTtl), type, methods) { DefaultTtl = defaultTtl };
        }

        private static CacheOperation GetOperation(MemberInfo method)
        {
#if BuildTask
            var attr = method.GetCustomAttribute("NetCache.CacheMethodAttribute, NetCache.Core");
            if (attr != null) return (CacheOperation)attr.ConstructorArguments[0].Value;
#else
            var attr = method.GetCustomAttribute<CacheMethodAttribute>(true);
            if (attr != null) return attr.Operation;
#endif
            if (method.Name.StartsWith(nameof(CacheOperation.Set), StringComparison.OrdinalIgnoreCase)) return CacheOperation.Set;

            if (method.Name.StartsWith(nameof(CacheOperation.Get), StringComparison.OrdinalIgnoreCase)) return CacheOperation.Get;

            return method.Name.StartsWith(nameof(CacheOperation.Remove), StringComparison.OrdinalIgnoreCase) ||
                   method.Name.StartsWith("Delete", StringComparison.OrdinalIgnoreCase)
                ? CacheOperation.Remove
                : CacheOperation.Ignore;
        }

        public static CacheMethod? ResolveGet(MethodInfo method)
        {
            var valueType = GetSyncType(method.ReturnType, out var a);
#if BuildTask
            if (valueType.IsType(typeof(void))) return null;
#else
            if (valueType == typeof(void)) return null;
#endif
            var parameters = method.GetParameters();
            if (parameters.Length < 1) return null;

            if (parameters.Length < 2) return new CacheMethod(method, CacheOperation.Get);

            int ttl = 0, value = 0, token = 0;

            for (var index = 1; index < parameters.Length; index++)
            {
#if BuildTask
                if (parameters[index].ParameterType.IsType<CancellationToken>())
#else
                if (parameters[index].ParameterType == typeof(CancellationToken))
#endif
                {
                    if (token > 0) return null;

                    token = index;

                    continue;
                }

                if (IsTtlType(parameters[index].ParameterType))
                {
                    if (ttl > 0) return null;

                    ttl = index;

                    continue;
                }
#if BuildTask
                if (!parameters[index].ParameterType.IsGenericInstance) return null;
#else
                if (!parameters[index].ParameterType.IsGenericType) return null;
#endif
                var args = parameters[index].ParameterType.GetGenericArguments();
                if (valueType != GetSyncType(args[args.Length - 1], out var b) || !a && b ||
#if BuildTask
                    args.Length == 1 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<>)) ||
                    args.Length == 2 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,>)) ||
                    args.Length == 3 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,,>)) ||
                    args.Length == 4 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,,,>)) ||
                    args.Length > 4 || value > 0 || !method.Resolve().IsAbstract) return null;
#else
                    args.Length == 1 && typeof(Func<>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 2 && typeof(Func<,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 3 && typeof(Func<,,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 4 && typeof(Func<,,,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length > 4 || value > 0 || !method.IsAbstract) return null;
#endif
                var type = 0;
                for (var i = 0; i < args.Length - 2; i++)
                {
                    if (args[i] == parameters[0].ParameterType)
                    {
                        if ((type & 1) == 1) return null;

                        type |= 1;

                        continue;
                    }
#if BuildTask
                    if (args[i].IsType<TimeSpan>())
#else
                    if (args[i] == typeof(TimeSpan))
#endif
                    {
                        if ((type & 2) == 2) return null;

                        type |= 2;

                        continue;
                    }
#if BuildTask
                    if (args[i].IsType<CancellationToken>())
#else
                    if (args[i] == typeof(CancellationToken))
#endif
                    {
                        if ((type & 4) == 4) return null;

                        type |= 4;

                        continue;
                    }

                    return null;
                }

                value = index;
            }

            return new CacheMethod(method, CacheOperation.Get)
            {
                Ttl = ttl,
                CancellationToken = token,
                Value = value
            };
        }

        public static CacheMethod? ResolveSet(MethodInfo method)
        {
            var returnType = GetSyncType(method.ReturnType, out _);
#if BuildTask
            if (!returnType.IsType(typeof(void)) && !returnType.IsType<bool>()) return null;
#else
            if (returnType != typeof(void) && returnType != typeof(bool)) return null;
#endif
            var parameters = method.GetParameters();
            if (parameters.Length < 2) return null;

            if (parameters.Length == 2) return new CacheMethod(method, CacheOperation.Set)
            {
                Value = 1,
                RawType = Boolean
            };

            int ttl = 0, value = 0, when = 0, token = 0, ttlType = 0; //0: auto, 1: implicit, 2: explicit

            for (var index = 1; index < parameters.Length; index++)
            {
#if BuildTask
                var attr = parameters[index].GetCustomAttribute("NetCache.CacheExpiryAttribute, NetCache.Core");

                if (parameters[index].ParameterType.IsType<CancellationToken>())
#else
                var attr = parameters[index].GetCustomAttribute<CacheExpiryAttribute>();

                if (parameters[index].ParameterType == typeof(CancellationToken))
#endif
                {
                    if (token > 0 || attr != null) return null;

                    token = index;

                    continue;
                }
#if BuildTask
                if (parameters[index].ParameterType.IsType("NetCache.When, NetCache.Core"))
#else
                if (parameters[index].ParameterType == typeof(When))
#endif
                {
                    if (when > 0 || attr != null) return null;

                    when = index;

                    continue;
                }

                if (IsTtlType(parameters[index].ParameterType))
                {
                    if (ttl > 0)
                    {
                        if (attr != null)
                        {
                            if (ttlType > 1 || value > 0) return null;

                            value = ttl;
                            ttl = index;
                        }
                        else if (string.Compare(parameters[index].Name, "ttl", StringComparison.OrdinalIgnoreCase) == 0 ||
                                 string.Compare(parameters[index].Name, "expiry", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (ttlType > 0 || value > 0) return null;

                            value = ttl;
                            ttl = index;
                        }
                        else if (value > 0) return null;
                        else value = index;
                    }
                    else
                    {
                        ttl = index;

                        if (attr != null) ttlType = 2;
                        else if (string.Compare(parameters[index].Name, "ttl", StringComparison.OrdinalIgnoreCase) == 0 ||
                                 string.Compare(parameters[index].Name, "expiry", StringComparison.OrdinalIgnoreCase) == 0)
                            ttlType = 1;
                    }
                }
                else
                {
                    if (value > 0 || attr != null) return null;

                    value = index;
                }
            }

            if (value == 0)
            {
                if (ttlType > 0) return null;

                value = ttl;
                ttl = 0;
            }

            if (value > 0) return new CacheMethod(method, CacheOperation.Set)
            {
                Ttl = ttl,
                When = when,
                CancellationToken = token,
                RawType = Boolean,
                Value = value
            };

            return null;
        }

        private static bool IsTtlType(Type type)
        {
            if (!type.IsValueType) return false;
#if BuildTask
            type = type.GetUnderlyingType() ?? type;

            return type.IsType<byte>() ||
                   type.IsType<sbyte>() ||
                   type.IsType<short>() ||
                   type.IsType<ushort>() ||
                   type.IsType<int>() ||
                   type.IsType<uint>() ||
                   type.IsType<long>() ||
                   type.IsType<ulong>() ||
                   type.IsType<float>() ||
                   type.IsType<double>() ||
                   type.IsType<decimal>() ||
                   type.IsType<TimeSpan>() ||
                   type.IsType<DateTime>() ||
                   type.IsType<DateTimeOffset>();
#else
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset);
#endif
        }

        public static CacheMethod? ResolveRemove(MethodInfo method)
        {
            var returnType = GetSyncType(method.ReturnType, out _);
#if BuildTask
            if (!returnType.IsType(typeof(void)) && !returnType.IsType<bool>()) return null;
#else
            if (returnType != typeof(void) && returnType != typeof(bool)) return null;
#endif
            return method.GetParameters().Length == 1 ? new CacheMethod(method, CacheOperation.Remove) { RawType = Boolean } : null;
        }

        internal static Exception ParameterException(Type type, MethodInfo method) => new NotSupportedException($"{type.FullName}.{method.Name}参数异常");
#if BuildTask
        public static string GetCacheName(TypeDefinition type, out int defaultTtl)
#else
        public static string GetCacheName(Type type, out int defaultTtl)
#endif
        {
            defaultTtl = 0;
#if BuildTask
            var attr = type.GetCustomAttribute("NetCache.CacheAttribute, NetCache.Core");
            if (attr == null) return type.FullName ?? type.Name;

            defaultTtl = attr.Properties.FirstValue("TtlSecond", 0);

            return attr.ConstructorArguments.FirstValue(0, type.FullName ?? type.Name);
#else
            var attr = type.GetCustomAttribute<CacheAttribute>(true);
            if (attr == null) return type.FullName ?? type.Name;

            defaultTtl = attr.TtlSecond;

            return attr.CacheName ?? type.FullName ?? type.Name;
#endif
        }

        private static Type GetSyncType(Type type, out bool isAsync)
        {
#if BuildTask
            if (type.IsType<Task>() || type.IsType<ValueTask>())
            {
                isAsync = true;

                return Void;
            }

            if (type.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)type;

                if (genericInstance.GetElementType().IsType(typeof(Task<>)) ||
                    genericInstance.GetElementType().IsType(typeof(ValueTask<>)))
                {
                    isAsync = true;

                    return genericInstance.GenericArguments[0];
                }
            }
#else
            if (type == typeof(Task) || type == typeof(ValueTask))
            {
                isAsync = true;

                return typeof(void);
            }

            if (type.IsGenericType &&
                (typeof(Task).IsAssignableFrom(type) ||
                 type.GetGenericTypeDefinition() == typeof(ValueTask<>)))
            {
                isAsync = true;

                return type.GetGenericArguments()[0];
            }
#endif
            isAsync = false;

            return type;
        }
    }
}
