using System;

namespace NetCache
{
    /// <summary>Define cache name and cache default ttl</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CacheAttribute : Attribute
    {
        /// <summary>Cache name</summary>
        public string CacheName { get; }

        /// <summary>Default ttl if method don't have ttl parameter or ttl less than 1</summary>
        public int Ttl { get; set; }

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
