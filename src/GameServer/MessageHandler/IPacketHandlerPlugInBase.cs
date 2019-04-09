// <copyright file="IPacketHandlerPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Base interface for a packet handler plugin.
    /// </summary>
    public interface IPacketHandlerPlugInBase : IStrategyPlugIn<byte>
    {
        /// <summary>
        /// Gets a value indicating whether this packet handler instance expects that the handled packet was checked to be encrypted.
        /// </summary>
        bool IsEncryptionExpected { get; }

        /// <summary>
        /// Handles the packet for the specified player.
        /// </summary>
        /// <param name="player">The player for which the packet should be handled.</param>
        /// <param name="packet">The packet which should be handled.</param>
        void HandlePacket(Player player, Span<byte> packet);
    }
}