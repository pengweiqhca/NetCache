using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NetCache
{
    /// <summary>The interface of value serializer</summary>
    public interface IValueSerializer
    {
        /// <summary>Deserialize</summary>
        [return: MaybeNull]
        TV Deserialize<TV>(ReadOnlyMemory<byte>? data);

        /// <summary>Serialize</summary>
        void Serialize<TV>([AllowNull] TV value, ref Stream stream);
    }
}
