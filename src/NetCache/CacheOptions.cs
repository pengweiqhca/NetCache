using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetCache
{
    /// <summary>Cache options</summary>
    public class CacheOptions
    {
        private int _defaultTtl = 30;

        /// <summary>The max key length, default 256</summary>
        public int MaxKeyLength { get; set; } = 256;

        /// <summary>The max value length, default 1MB. If <see cref="CompressValue"/> is true, the length is compressed length.</summary>
        public int MaxValueLength { get; set; } = 1024 * 1024;

        /// <summary>Whether to use gzip compression value, default false.</summary>
        public bool CompressValue { get; set; }

        /// <summary>The key prefix, used for the isolation of different applications, default entry assembly name</summary>
        public string KeyPrefix { get; set; } = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name;

        /// <summary>Default GetOrSet lock timeout, default 30 second.</summary>
        public int LockTimeout { get; set; } = 30;

        /// <summary>Default ttl, default 30 second.</summary>
        public int DefaultTtl
        {
            get => _defaultTtl;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), Res.Value_Must_Than_Zero);

                _defaultTtl = value;
            }
        }

        /// <summary>Max ttl, default 30 day.</summary>
        public int MaxTll { get; set; } = 30 * 24 * 60 * 60;

        /// <summary>ttl per name.</summary>
        public Dictionary<string, int> Ttl { get; } = new();
    }
}
