using NetCache.Tests.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NetCache.BuildTask.Tests
{
    public class ProxyTypeBuildTest
    {
        private readonly ITestOutputHelper _output;

        public ProxyTypeBuildTest(ITestOutputHelper output) => _output = output;

        [Fact]
        public void GenericTest()
        {
            var generator = new CacheAssembly(typeof(Int64Cache).Assembly.Location, Enumerable.Empty<string>(), new Logger(_output), false);

            generator.CreateProxyType<IGenericInterfaceCache<string>>();
            generator.CreateProxyType<GenericMethodCache>();
            generator.CreateProxyType<GenericTypeCache<string>>();
        }
    }
}
