// <copyright file="ConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

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

        using var l = await connection.OutputLock.LockAsync().ConfigureAwait(false);
        var length = packetBuilder();
        connection.Output.Advance(length);
        await connection.Output.FlushAsync().ConfigureAwait(false);
    }
}