using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public static class CacheTypeResolver
    {
        /// <exception cref="AggregateException"></exception>
        public static CacheType Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var exceptions = new List<Exception>();
            var methods = new List<CacheMethod>();

            foreach (var method in type.GetMethods())
            {
                var canBeOverride = method.IsVirtual && !method.IsFinal;
                if (!canBeOverride || method.DeclaringType == typeof(object)) continue;

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
            var attr = method.GetCustomAttribute<CacheMethodAttribute>(true);
            if (attr != null) return attr.Operation;

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
            if (valueType == typeof(void)) return null;

            var parameters = method.GetParameters();
            if (parameters.Length < 1) return null;

            if (parameters.Length < 2) return new CacheMethod(method, CacheOperation.Get);

            int ttl = 0, value = 0, token = 0;

            for (var index = 1; index < parameters.Length; index++)
            {
                if (parameters[index].ParameterType == typeof(CancellationToken))
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

                if (!parameters[index].ParameterType.IsGenericType) return null;

                var args = parameters[index].ParameterType.GetGenericArguments();
                if (valueType != GetSyncType(args[args.Length - 1], out var b) || !a && b ||
                    args.Length == 1 && typeof(Func<>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 2 && typeof(Func<,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 3 && typeof(Func<,,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length == 4 && typeof(Func<,,,>) != parameters[index].ParameterType.GetGenericTypeDefinition() ||
                    args.Length > 4 || value > 0 || !method.IsAbstract) return null;

                var type = 0;
                for (var i = 0; i < args.Length - 2; i++)
                {
                    if (args[i] == parameters[0].ParameterType)
                    {
                        if ((type & 1) == 1) return null;

                        type |= 1;

                        continue;
                    }

                    if (args[i] == typeof(TimeSpan))
                    {
                        if ((type & 2) == 2) return null;

                        type |= 2;

                        continue;
                    }

                    if (args[i] == typeof(CancellationToken))
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
            if (returnType != typeof(void) && returnType != typeof(bool)) return null;

            var parameters = method.GetParameters();
            if (parameters.Length < 2) return null;

            if (parameters.Length == 2) return new CacheMethod(method, CacheOperation.Set)
            {
                Value = 1,
                RawType = typeof(bool)
            };

            int ttl = 0, value = 0, when = 0, token = 0, ttlType = 0; //0: auto, 1: implicit, 2: explicit

            for (var index = 1; index < parameters.Length; index++)
            {
                var attr = parameters[index].GetCustomAttribute<CacheExpiryAttribute>();

                if (parameters[index].ParameterType == typeof(CancellationToken))
                {
                    if (token > 0 || attr != null) return null;

                    token = index;

                    continue;
                }

                if (parameters[index].ParameterType == typeof(When))
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
                RawType = typeof(bool),
                Value = value
            };

            return null;
        }

        private static bool IsTtlType(Type type)
        {
            if (type.IsClass) return false;

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
        }

        public static CacheMethod? ResolveRemove(MethodInfo method)
        {
            var returnType = GetSyncType(method.ReturnType, out _);
            if (returnType != typeof(void) && returnType != typeof(bool)) return null;

            return method.GetParameters().Length == 1 ? new CacheMethod(method, CacheOperation.Remove) { RawType = typeof(bool) } : null;
        }

        internal static Exception ParameterException(Type type, MethodInfo method) => new NotSupportedException($"{type.FullName}.{method.Name}参数异常");

        public static string GetCacheName(Type type, out int defaultTtl)
        {
            defaultTtl = 0;

            var attr = type.GetCustomAttribute<CacheAttribute>(true);
            if (attr == null) return type.FullName ?? type.Name;

            defaultTtl = attr.TtlSecond;

            return attr.CacheName ?? type.FullName ?? type.Name;
        }

        private static Type GetSyncType(Type type, out bool isAsync)
        {
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

            isAsync = false;

            return type;
        }
    }
}
