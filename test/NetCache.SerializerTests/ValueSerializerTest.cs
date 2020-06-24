using Moq;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace NetCache.SerializerTests
{
    public class ValueSerializerTest
    {
        [Fact]
        public void SerializeTest()
        {
            var serializer = new Mock<ValueSerializer>().Object;

            Assert.Throws<SerializationException>(() => serializer.Serialize(Task.CompletedTask));

            var data = Guid.NewGuid().ToByteArray();

            Assert.Equal(data, serializer.Serialize(data).ToArray());
        }

        [Fact]
        public void DeserializeTest()
        {
            var serializer = new Mock<ValueSerializer>().Object;

            Assert.ThrowsAsync<SerializationException>(() => serializer.Deserialize<Task>(new ReadOnlyMemory<byte>(new byte[] { 3 })));

            Assert.Null(serializer.Deserialize<string>(null));
            Assert.Null(serializer.Deserialize<string>(new ReadOnlyMemory<byte>(Array.Empty<byte>())));
        }
    }
}
