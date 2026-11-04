// <copyright file="AsyncLockExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Threading;
using Nito.AsyncEx;

/// <summary>
/// Extensions for <see cref="AsyncLock"/>.
/// </summary>
public static class AsyncLockExtensions
{
    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <param name="asyncLock">The asynchronous lock.</param>
    /// <param name="timeout">The timeout to take the lock.</param>
    /// <returns>A disposable that releases the lock when disposed. Null, if the lock couldn't be acquired within the timeout.</returns>
    public static async ValueTask<IDisposable?> LockAsync(this AsyncLock asyncLock, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            return await asyncLock.LockAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }
}