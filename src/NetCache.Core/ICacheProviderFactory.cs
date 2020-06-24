namespace NetCache
{
    /// <summary>Create ICacheProvider</summary>
    public interface ICacheProviderFactory
    {
        /// <summary>Create ICacheProvider</summary>
        /// <param name="name">Cache name</param>
        /// <returns>ICacheProvider</returns>
        ICacheProvider Create(string name);
    }
}
