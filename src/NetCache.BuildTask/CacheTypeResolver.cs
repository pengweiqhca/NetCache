using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    public static class CacheTypeResolver
    {
        public static readonly TypeDefinition Void;
        public static readonly TypeDefinition Boolean;

        static CacheTypeResolver()
        {
            var module = AssemblyDefinition.ReadAssembly(typeof(void).Assembly.Location).MainModule;

            Void = module.GetType(typeof(void).FullName);
            Boolean = module.GetType(typeof(bool).FullName);
        }

        /// <exception cref="AggregateException"></exception>
        public static CacheType Resolve(TypeDefinition type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var exceptions = new List<Exception>();
            var methods = new List<CacheMethod>();

            foreach (var method in type.GetMethods())
            {
                var canBeOverride = method.IsVirtual && !method.IsFinal;
                if (!canBeOverride || method.DeclaringType.FullName == typeof(object).FullName) continue;

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

        private static CacheOperation GetOperation(IMemberDefinition method)
        {
            var attr = method.GetCustomAttribute("NetCache.CacheMethodAttribute, NetCache.Core");
            if (attr != null) return (CacheOperation)attr.ConstructorArguments[0].Value;

            if (method.Name.StartsWith(nameof(CacheOperation.Set), StringComparison.OrdinalIgnoreCase)) return CacheOperation.Set;

            if (method.Name.StartsWith(nameof(CacheOperation.Get), StringComparison.OrdinalIgnoreCase)) return CacheOperation.Get;

            return method.Name.StartsWith(nameof(CacheOperation.Remove), StringComparison.OrdinalIgnoreCase) ||
                   method.Name.StartsWith("Delete", StringComparison.OrdinalIgnoreCase)
                ? CacheOperation.Remove
                : CacheOperation.Ignore;
        }

        public static CacheMethod? ResolveGet(MethodReference method)
        {
            var valueType = GetSyncType(method.ReturnType, out var a);
            if (valueType.IsType(typeof(void))) return null;

            var parameters = method.Parameters;
            if (parameters.Count < 1) return null;

            if (parameters.Count < 2) return new CacheMethod(method, CacheOperation.Get);

            int ttl = 0, value = 0, token = 0;

            for (var index = 1; index < parameters.Count; index++)
            {
                if (parameters[index].ParameterType.IsType<CancellationToken>())
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

                if (!parameters[index].ParameterType.IsGenericInstance) return null;

                var args = ((GenericInstanceType)parameters[index].ParameterType).GenericArguments;
                if (valueType != GetSyncType(args[args.Count - 1], out var b) || !a && b ||
                    args.Count == 1 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<>)) ||
                    args.Count == 2 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,>)) ||
                    args.Count == 3 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,,>)) ||
                    args.Count == 4 && !parameters[index].ParameterType.GetElementType().IsType(typeof(Func<,,,>)) ||
                    args.Count > 4 || value > 0 || !method.Resolve().IsAbstract) return null;

                var type = 0;
                for (var i = 0; i < args.Count - 2; i++)
                {
                    if (args[i] == parameters[0].ParameterType)
                    {
                        if ((type & 1) == 1) return null;

                        type |= 1;

                        continue;
                    }

                    if (args[i].IsType<TimeSpan>())
                    {
                        if ((type & 2) == 2) return null;

                        type |= 2;

                        continue;
                    }

                    if (args[i].IsType<CancellationToken>())
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

        public static CacheMethod? ResolveSet(MethodReference method)
        {
            var returnType = GetSyncType(method.ReturnType, out _);
            if (!returnType.IsType(typeof(void)) && !returnType.IsType<bool>()) return null;

            var parameters = method.Parameters;
            if (parameters.Count < 2) return null;

            if (parameters.Count == 2) return new CacheMethod(method, CacheOperation.Set)
            {
                Value = 1,
                RawType = Boolean
            };

            int ttl = 0, value = 0, when = 0, token = 0, ttlType = 0; //0: auto, 1: implicit, 2: explicit

            for (var index = 1; index < parameters.Count; index++)
            {
                var attr = parameters[index].GetCustomAttribute("NetCache.CacheExpiryAttribute, NetCache.Core");

                if (parameters[index].ParameterType.IsType<CancellationToken>())
                {
                    if (token > 0 || attr != null) return null;

                    token = index;

                    continue;
                }

                if (parameters[index].ParameterType.IsType("NetCache.When, NetCache.Core"))
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

        private static bool IsTtlType(TypeReference type)
        {
            if (!type.IsValueType) return false;

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
        }

        public static CacheMethod? ResolveRemove(MethodReference method)
        {
            var returnType = GetSyncType(method.ReturnType, out _);
            if (!returnType.IsType(typeof(void)) && !returnType.IsType<bool>()) return null;

            return method.Parameters.Count == 1 ? new CacheMethod(method, CacheOperation.Remove) { RawType = Boolean } : null;
        }

        internal static Exception ParameterException(TypeReference type, MethodReference method) => new NotSupportedException($"{type.FullName}.{method.Name}参数异常");

        public static string GetCacheName(TypeDefinition type, out int defaultTtl)
        {
            defaultTtl = 0;

            var attr = type.GetCustomAttribute("NetCache.CacheAttribute, NetCache.Core");
            if (attr == null) return type.FullName ?? type.Name;

            defaultTtl = attr.Properties.FirstValue("TtlSecond", 0);

            return attr.ConstructorArguments.FirstValue(0, type.FullName ?? type.Name);
        }

        private static TypeReference GetSyncType(TypeReference type, out bool isAsync)
        {
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

            isAsync = false;

            return type;
        }
    }
}
