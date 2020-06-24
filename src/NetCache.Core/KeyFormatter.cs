using System;
using System.Diagnostics.CodeAnalysis;

namespace NetCache
{
    /// <summary>The default key formatter</summary>
    public class KeyFormatter : IKeyFormatter
    {
        /// <summary>Format the key.</summary>
        public string Format<TK>([NotNull] TK key) => key switch
        {
            null => throw new ArgumentNullException(nameof(key)),
            string s => s,
            _ => InternalFormat(key)
        };

        /// <summary>Format the key.</summary>
        protected virtual string InternalFormat<TK>([NotNull] TK key) => key!.ToString();
    }
}
