using MessagePack;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Mps = MessagePack.MessagePackSerializer;

namespace NetCache
{
    public class MessagePackSerializer : ValueSerializer
    {
#if NETFRAMEWORK
        protected override TV InternalDeserialize<TV>(Stream stream) =>
            Mps.Deserialize<TV>(stream);

        protected override void InternalSerialize<TV>([AllowNull] TV value, Stream stream) =>
            Mps.Serialize(stream, value!);
#else
        private readonly MessagePackSerializerOptions? _options;

        public MessagePackSerializer(MessagePackSerializerOptions? options = null) => _options = options;

        protected override TV InternalDeserialize<TV>(Stream stream) =>
            Mps.Deserialize<TV>(stream, _options);

        protected override void InternalSerialize<TV>([AllowNull] TV value, Stream stream) =>
            Mps.Serialize(stream, value!, _options);
#endif
    }
}
