using System;
using Xunit;

namespace NetCache.SerializerTests
{
    public class JsonSerializerTest
    {
        [Fact]
        public void NewtonsoftJsonTest()
        {
            var serializer = new NewtonsoftJsonSerializer();

            var now = DateTimeOffset.Now;

            Assert.Equal(now, serializer.Deserialize<DateTimeOffset>(serializer.Serialize(now)));
        }
#if !NET46
        [Fact]
        public void SystemTextJsonTest()
        {
            var serializer = new SystemTextJsonSerializer();

            var now = DateTimeOffset.Now;

            Assert.Equal(now, serializer.Deserialize<DateTimeOffset>(serializer.Serialize(now)));
        }
#endif
    }
}
