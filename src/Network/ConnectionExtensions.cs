// <copyright file="ConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Threading;

/// <summary>
/// Extension methods for <see cref="IConnection"/>.
/// </summary>
public static class ConnectionExtensions
{
    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="packetBuilder">The packet builder which writes to the <see cref="IConnection.Output"/> and returns the length of the packet in bytes.</param>
    public static async ValueTask SendAsync(this IConnection connection, Func<int> packetBuilder)
    {
        if (!connection.Connected)
        {
            return;
        }

        try
        {
            using var l = await connection.OutputLock.LockAsync().ConfigureAwait(false);
            var length = packetBuilder();
            connection.Output.Advance(length);
            var result = await connection.Output.FlushAsync().ConfigureAwait(false);
            
            if (result.IsCanceled || result.IsCompleted)
            {
                // Connection might be closed, trigger disconnect if needed
                if (!connection.Connected)
                {
                    await connection.DisconnectAsync().ConfigureAwait(false);
                }
            }
        }
        catch (ObjectDisposedException)
        {
            // Connection already disposed, nothing to do
        }
        catch (InvalidOperationException)
        {
            // Output pipe might be completed, disconnect gracefully
            await connection.DisconnectAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Sends a message asynchronously with timeout and retry support.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="packetBuilder">The packet builder which writes to the <see cref="IConnection.Output"/> and returns the length of the packet in bytes.</param>
    /// <param name="timeout">The timeout for the send operation.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    public static async ValueTask<bool> SendAsyncWithRetryAsync(this IConnection connection, Func<int> packetBuilder, TimeSpan timeout = default, int retryCount = 1)
    {
        if (timeout == default)
        {
            timeout = TimeSpan.FromSeconds(5);
        }

        for (int attempt = 0; attempt <= retryCount; attempt++)
        {
            if (!connection.Connected)
            {
                return false;
            }

            try
            {
                using var cts = new CancellationTokenSource(timeout);
                using var l = await connection.OutputLock.LockAsync(cts.Token).ConfigureAwait(false);
                
                var length = packetBuilder();
                connection.Output.Advance(length);
                var result = await connection.Output.FlushAsync(cts.Token).ConfigureAwait(false);
                
                if (!result.IsCanceled && !result.IsCompleted)
                {
                    return true; // Success
                }
            }
            catch (OperationCanceledException) when (attempt < retryCount)
            {
                // Retry on timeout, but not on the last attempt
                await Task.Delay(100 * (attempt + 1), CancellationToken.None).ConfigureAwait(false);
                continue;
            }
            catch (Exception) when (attempt < retryCount)
            {
                // Retry on any other exception, but not on the last attempt
                await Task.Delay(100 * (attempt + 1), CancellationToken.None).ConfigureAwait(false);
                continue;
            }
        }

        // All attempts failed
        await connection.DisconnectAsync().ConfigureAwait(false);
        return false;
    }
}