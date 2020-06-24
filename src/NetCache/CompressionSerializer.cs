using Microsoft.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace NetCache
{
    public class CompressionSerializer : IValueSerializer
    {
        private static readonly byte[] GZipHeaderBytes = { 0x1f, 0x8b, 8, 0, 0, 0, 0, 0 };
        private readonly IValueSerializer _serializer;
        private readonly RecyclableMemoryStreamManager _manager;

        public CompressionSerializer(IValueSerializer serializer, RecyclableMemoryStreamManager manager)
        {
            _serializer = serializer;
            _manager = manager;
        }

        public virtual bool IsPossiblyGZipped(ReadOnlyMemory<byte>? buffer) =>
            buffer != null && buffer.Value.Length >= 10 &&
            buffer.Value.Span.Slice(0, GZipHeaderBytes.Length).SequenceEqual(GZipHeaderBytes);

        [return: MaybeNull]
        public TV Deserialize<TV>(ReadOnlyMemory<byte>? data)
        {
            if (IsPossiblyGZipped(data))
                try
                {
                    using var stream = new MemoryStream(data!.Value.ToArray());

                    using var dms = new MemoryStream();
                    using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                        gzip.CopyTo(dms);

                    data = dms.ToArray();
                }
                catch (InvalidDataException)
                {
                }

            return _serializer.Deserialize<TV>(data);
        }

        public void Serialize<TV>([AllowNull] TV value, ref Stream stream)
        {
            _serializer.Serialize(value, ref stream);
            if (stream.Length <= 10) return;

            var cms = _manager.GetStream();
            using (var gzip = new GZipStream(cms, CompressionMode.Compress))
                stream.CopyTo(gzip);

            if (cms.Length < stream.Length)
            {
                stream.Dispose();

                stream = cms;
            }
            else cms.Dispose();
        }
    }
}
