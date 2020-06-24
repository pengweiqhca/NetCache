using System;
using System.Diagnostics.CodeAnalysis;

namespace NetCache
{
    /// <summary>The interface of key formatter</summary>
    public interface IKeyFormatter
    {
        /// <summary>Format the key.</summary>
        /// <returns>Stringify key.</returns>
        public string Format<TK>([NotNull] TK key);
    }
}
