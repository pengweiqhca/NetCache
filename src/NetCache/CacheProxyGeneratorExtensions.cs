using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;

namespace NetCache
{
    public static class CacheProxyGeneratorExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type CreateProxyType<T>(this ICacheProxyGenerator generator)
           where T : class
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return generator.CreateProxyType(typeof(T));
        }

        public static T CreateProxy<T>(this ICacheProxyGenerator generator,
            CreateProxyOptions options, params object[] additionalParameters)
            where T : class
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (additionalParameters == null) throw new ArgumentNullException(nameof(additionalParameters));

            var args = new object[6 + additionalParameters.Length];

            Array.Copy(additionalParameters, args, additionalParameters.Length);

            args[args.Length - 6] = options.CacheProviderFactory;
            args[args.Length - 5] = options.DistributedLockFactory;
            args[args.Length - 4] = options.KeyFormatter;
            args[args.Length - 3] = options.ValueSerializer;
            args[args.Length - 2] = options.StreamManager;
            args[args.Length - 1] = new OptionMonitorWrapper(options.Options);

            return (T)Activator.CreateInstance(generator.CreateProxyType(typeof(T)), args);
        }

        private class OptionMonitorWrapper : IOptionsMonitor<CacheOptions>, IDisposable
        {
            public OptionMonitorWrapper(CacheOptions options) => CurrentValue = options;
#if NET45
            public IDisposable OnChange(Action<CacheOptions> listener) => this;
#else
            public CacheOptions Get(string name) => CurrentValue;

            public IDisposable OnChange(Action<CacheOptions, string> listener) => this;
#endif
            public CacheOptions CurrentValue { get; }

            public void Dispose() { }
        }
    }
}
