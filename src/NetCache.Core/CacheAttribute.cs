using System;

namespace NetCache
{
    /// <summary>Define cache name and cache default ttl</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CacheAttribute : Attribute
    {
        private int _ttlSecond;

        /// <summary>Cache name</summary>
        public string CacheName { get; }

        /// <summary>Default ttl if method don't have ttl parameter or ttl less than 1</summary>
        public int TtlSecond
        {
            get => _ttlSecond;
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), value, Res.Value_Must_Than_Zero);

                _ttlSecond = value;
            }
        }

        /// <summary>ctor</summary>
        /// <param name="cacheName">Cache name</param>
        public CacheAttribute(string cacheName)
        {
            if (string.IsNullOrEmpty(cacheName))
                throw new ArgumentNullException(nameof(cacheName));

            CacheName = cacheName;
        }
    }
}
