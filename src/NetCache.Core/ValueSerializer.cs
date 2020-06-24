using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetCache
{
    /// <summary>The value serializer</summary>
    public abstract class ValueSerializer : IValueSerializer
    {
        private static readonly TypeInfo TaskInfo = typeof(Task).GetTypeInfo();

        /// <summary>Deserialize</summary>
        [return: MaybeNull]
        public TV Deserialize<TV>(ReadOnlyMemory<byte>? data)
        {
            if (data == null || data.Value.IsEmpty) return default!;

            var targetType = typeof(TV);

            if (TaskInfo.IsAssignableFrom(targetType))
                throw new SerializationException(Res.Serializer_Not_Support_Task);

            if (targetType.IsAssignableFrom(typeof(byte[])))
                return (TV)(object)data.Value.Span.ToArray();

            if (targetType == typeof(string))
#if NETSTANDARD2_1
                return (TV)(object)Encoding.UTF8.GetString(data.Value.Span);
#else
                return (TV)(object)Encoding.UTF8.GetString(data.Value.Span.ToArray());
#endif
            using var stream = new ReadOnlyMemoryStream(data.Value);

            return InternalDeserialize<TV>(stream);
        }

        /// <summary>Serialize</summary>
        public void Serialize<TV>([AllowNull] TV value, ref Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            switch (value)
            {
                case Task _:
                    throw new SerializationException(Res.Serializer_Not_Support_Task);
                case Memory<byte> m:
                    stream.Dispose();

                    stream = new ReadOnlyMemoryStream(m);

                    return;
                case ReadOnlyMemory<byte> rom:
                    stream.Dispose();

                    stream = new ReadOnlyMemoryStream(rom);

                    return;
                case ArraySegment<byte> s:
                    stream.Dispose();

                    stream = new ReadOnlyMemoryStream(s);

                    return;
                case byte[] data:
                    stream.Dispose();

                    stream = new ReadOnlyMemoryStream(data);

                    return;
                case string s:
                    {
                        using var sr = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, true);

                        sr.Write(s);

                        return;
                    }
                default:
                    InternalSerialize(value, stream);
                    break;
            }
        }

        /// <summary>Deserialize</summary>
        [return: MaybeNull]
        protected abstract TV InternalDeserialize<TV>(Stream stream);

        /// <summary>Serialize</summary>
        protected abstract void InternalSerialize<TV>([AllowNull] TV value, Stream stream);
    }
}
