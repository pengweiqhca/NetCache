using NetCache.Tests.TestHelpers;
using Xunit;

namespace NetCache.Tests
{
    public class CacheProxyGeneratorTest
    {
        private static readonly ICacheProxyGenerator Generator = new CacheProxyGenerator();

        [Fact]
        public void GenericTest()
        {
            Generator.CreateProxyType<IGenericInterfaceCache<string>>();
            Generator.CreateProxyType<GenericMethodCache>();
            Generator.CreateProxyType<GenericTypeCache<string>>();
        }
    }
}
