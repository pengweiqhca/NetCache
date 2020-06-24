using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NetCache
{
    public static class ValueSerializerExtensions
    {
        public static ReadOnlyMemory<byte> Serialize<TV>(this IValueSerializer serializer, [AllowNull] TV value)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            Stream stream = new MemoryStream();
            try
            {
                serializer.Serialize(value, ref stream);

                if (stream is MemoryStream ms) return ms.ToArray();
                if (stream is ReadOnlyMemoryStream roms) return roms.Memory;

                using var s = new MemoryStream();
                stream.CopyTo(s);
                return s.ToArray();
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}
