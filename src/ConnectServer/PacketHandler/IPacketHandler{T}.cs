// <copyright file="IPacketHandler{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler
{
    using System;

    /// <summary>
    /// The interface for a packet handler with a type which is passed as context argument.
    /// </summary>
    /// <typeparam name="T">Type of the context argument.</typeparam>
    internal interface IPacketHandler<in T>
    {
        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="packet">The packet.</param>
        void HandlePacket(T obj, Span<byte> packet);
    }
}
