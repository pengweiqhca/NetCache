using System;

namespace NetCache
{
    public interface ICacheProxyGenerator
    {
        Type CreateProxyType(Type type);
    }
}
