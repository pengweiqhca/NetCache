using MessagePack;
using ProtoBuf;

namespace NetCache.SerializerTests
{
    [ProtoContract]
    [MessagePackObject]
    public class Address
    {
        [ProtoMember(1)]
        [Key(0)]
        public string? Line1 { get; set; }

        [ProtoMember(2)]
        [Key(1)]
        public string? Line2 { get; set; }
    }
}
