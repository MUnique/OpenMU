// <copyright file="ConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Extensions for <see cref="IConnection"/>.
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Sends the specified serializeable.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="serializeable">The serializeable.</param>
        public static void Send(this IConnection connection, ISerializable serializeable)
        {
            connection.Send(serializeable.Serialize());
        }
    }
}