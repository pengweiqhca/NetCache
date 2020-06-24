using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache
{
    /// <summary>Cache provider</summary>
    public interface ICacheProvider
    {
        /// <summary>Cache full name</summary>
        string Name { get; }

        /// <summary>Get the value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.</summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        ReadOnlyMemory<byte>? Get(string key, CancellationToken cancellationToken);

        /// <summary>Get the value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.</summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        ValueTask<ReadOnlyMemory<byte>?> GetAsync(string key, CancellationToken cancellationToken);

        /// <summary>Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.</summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (detaults to always).</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        bool Set(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken);

        /// <summary>Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.</summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="when">Which condition to set the value under (detaults to always).</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        ValueTask<bool> SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan expiry, When when, CancellationToken cancellationToken);

        /// <summary>Removes the specified key. A key is ignored if it does not exist.</summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
        /// <returns>True if the key was removed.</returns>
        bool Remove(string key, CancellationToken cancellationToken);

        /// <summary>Removes the specified key. A key is ignored if it does not exist.</summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>True if the key was removed.</returns>
        ValueTask<bool> RemoveAsync(string key, CancellationToken cancellationToken);
    }
}
