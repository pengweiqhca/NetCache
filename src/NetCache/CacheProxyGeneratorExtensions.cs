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

            var args = new object[7 + additionalParameters.Length];

            args[0] = options.CacheProviderFactory;
            args[1] = options.DistributedLockFactory;
            args[2] = options.KeyFormatter;
            args[3] = options.ValueSerializer;
            args[4] = options.StreamManager;
            args[5] = new OptionMonitorWrapper(options.Options);

            for (var index = 0; index < additionalParameters.Length; index++)
                args[index + 6] = additionalParameters[index];

            return (T)Activator.CreateInstance(generator.CreateProxyType(typeof(T)), args);
        }

        private class OptionMonitorWrapper : IOptionsMonitor<CacheOptions>, IDisposable
        {
            public OptionMonitorWrapper(CacheOptions options) => CurrentValue = options;

            public IDisposable OnChange(Action<CacheOptions> listener) => this;

            public CacheOptions CurrentValue { get; }

            public void Dispose() { }
        }
    }
}
