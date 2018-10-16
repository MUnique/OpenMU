// <copyright file="ConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network
{
    using System;
    using System.Buffers;
    using System.Threading;

    /// <summary>
    /// Extension methods for <see cref="IConnection"/>.
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Sends the specified packet.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="packet">The packet.</param>
        public static void Send(this IConnection connection, ReadOnlySpan<byte> packet)
        {
            lock (connection)
            {
                connection.Output.Write(packet);
                connection.Output.FlushAsync();
            }
        }
    }
}
