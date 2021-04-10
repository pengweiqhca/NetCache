using NetCache.Tests.TestHelpers;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace NetCache.BuildTask.Tests
{
    public class CeAssemblyTest
    {
        private readonly ITestOutputHelper _output;

        public CeAssemblyTest(ITestOutputHelper output) => _output = output;

        //public void A()
        //{
        //    var solutionDirectory = "<path to your solution>";
        //    var projectFilePath = "<path to the csproj in the solution>";

        //    var globalProperties = new Dictionary<string, string> {
        //        { "DesignTimeBuild", "true" },
        //        { "BuildProjectReferences", "false" },
        //        { "_ResolveReferenceDependencies", "true" },
        //        { "SolutionDir", solutionDirectory + Path.DirectorySeparatorChar }
        //    };

        //    var collection = new ProjectCollection();
        //    Project project = collection.LoadProject(projectFilePath);

        //}

        [Fact]
        public void Test()
        {
            var ass = new CacheAssembly(typeof(Int64Cache).Assembly.Location, Enumerable.Empty<string>(), new Logger(_output), false);

           Assert.Equal(2,  ass.GetCacheTypes().Count());
        }
    }
}
