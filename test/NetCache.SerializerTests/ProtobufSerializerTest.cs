using System;
using Hyper;
using Xunit;

namespace NetCache.SerializerTests
{
    public class ProtobufSerializerTest
    {
        [Fact]
        public void GoogleProtobufTest()
        {
            var serializer = new GoogleProtobufSerializer();

            Assert.Empty(serializer.Serialize((MultiplyRequest?) null).ToArray());

            var obj = new MultiplyRequest { X = 3, Y = 5 };

            var target = serializer.Deserialize<MultiplyRequest>(serializer.Serialize(obj))!;

            Assert.NotNull(target);
            Assert.Equal(obj.X, target.X);
            Assert.Equal(obj.Y, target.Y);
        }

        [Fact]
        public void GoogleProtobufErrorTest()
        {
            Assert.Throws<InvalidOperationException>(() => new GoogleProtobufSerializer().Serialize(new Address()));
        }

        [Fact]
        public void ProtobufNetTest()
        {
            var serializer = new ProtobufNetSerializer();

            Assert.Empty(serializer.Serialize((Address?) null).ToArray());

            Assert.Equal(3, serializer.Deserialize<int>(serializer.Serialize(3)));

            var source = new Address { Line1 = "abc", Line2 = "aaaaa" };

            var target = serializer.Deserialize<Address>(serializer.Serialize(source))!;

            Assert.NotNull(target);
            Assert.Equal(source.Line1, target.Line1);
            Assert.Equal(source.Line2, target.Line2);
        }

        [Fact]
        public void ProtobufNetErrorTest()
        {
            Assert.Throws<InvalidOperationException>(() => new ProtobufNetSerializer().Serialize(new MultiplyRequest()));
        }
    }
}
