using System;

namespace NetCache
{
    /// <summary>Define cache expiry if parameter name do not conform to the constraints</summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CacheValueAttribute : Attribute { }
}
