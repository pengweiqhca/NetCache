namespace NetCache
{
    /// <summary>Cache operation</summary>
    public enum CacheOperation
    {
        /// <summary>This is not a cache method</summary>
        Ignore = 0,
        /// <summary>Get or GetOrSet cache</summary>
        Get = 1,
        /// <summary>Store cache</summary>
        Set = 2,
        /// <summary>Remove cache</summary>
        Remove = 3,
        /// <summary>Get or GetOrSet cache</summary>
        Gets = 4,
        /// <summary>Store cache</summary>
        Sets = 5,
        /// <summary>Remove cache</summary>
        Removes = 6
    }
}
