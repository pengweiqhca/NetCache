using ProtoBuf;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NetCache
{
    public class ProtobufNetSerializer : ValueSerializer
    {
        protected override TV InternalDeserialize<TV>(Stream stream) =>
            Serializer.Deserialize<TV>(stream);

        protected override void InternalSerialize<TV>([AllowNull]TV value, Stream stream) =>
            Serializer.Serialize(stream, value!);
    }
}
