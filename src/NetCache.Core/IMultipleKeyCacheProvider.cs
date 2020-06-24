using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    /// <summary>Multiple cache provider</summary>
    public interface IMultipleKeyCacheProvider : ICacheProvider
    {
        /// <summary>Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the special value nil is returned.</summary>
        /// <param name="keys">The keys of the strings.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>The values of the strings with nil for keys do not exist.</returns>
        IReadOnlyList<ReadOnlyMemory<byte>?> Get(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>Returns the values of all specified keys. For every key that does not hold a string value or does not exist, the special value nil is returned.</summary>
        /// <param name="keys">The keys of the strings.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The values of the strings with nil for keys do not exist.</returns>
        ValueTask<IReadOnlyList<ReadOnlyMemory<byte>?>> GetAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the given keys to their respective values. If "not exists" is specified, this will not perform any operation at all even if just a single key already exists.
        /// </summary>
        /// <param name="values">The keys and values to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (detaults to always).</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>True if the keys were set, else False</returns>
        IReadOnlyList<bool> Set(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the given keys to their respective values. If "not exists" is specified, this will not perform any operation at all even if just a single key already exists.
        /// </summary>
        /// <param name="values">The keys and values to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (detaults to always).</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>True if the keys were set, else False</returns>
        ValueTask<IReadOnlyList<bool>> SetAsync(IEnumerable<KeyValuePair<string, ReadOnlyMemory<byte>>> values, TimeSpan expiry, When when, CancellationToken cancellationToken);

        /// <summary>Removes the specified keys. A key is ignored if it does not exist.</summary>
        /// <param name="keys">The keys to delete.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>The number of keys that were removed.</returns>
        long Remove(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>Removes the specified keys. A key is ignored if it does not exist.</summary>
        /// <param name="keys">The keys to delete.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The number of keys that were removed.</returns>
        ValueTask<long> RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
