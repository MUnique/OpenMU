// <copyright file="IPacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Interface for a packet handler.
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// Handles the packet for the specified player.
        /// </summary>
        /// <param name="player">The player for which the packet should be handled.</param>
        /// <param name="packet">The packet which should be handled.</param>
        void HandlePacket(Player player, Span<byte> packet);
    }

    /// <summary>
    /// A main packet handler, which needs initialization.
    /// </summary>
    public interface IMainPacketHandler : IPacketHandler
    {
        /// <summary>
        /// Gets the client version.
        /// </summary>
        byte[] ClientVersion { get; }
    }
}
