// <copyright file="TalkNpcHandlerPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;

    /// <summary>
    /// Handler for talk npc request packets.
    /// </summary>
    internal abstract class TalkNpcHandlerPlugInBase : IPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public virtual bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => TalkToNpcRequest.Code;

        /// <summary>
        /// Gets the talk NPC action.
        /// </summary>
        protected abstract TalkNpcAction TalkNpcAction { get; }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            TalkToNpcRequest message = packet;
            if (player.CurrentMap?.GetObject(message.NpcId) is NonPlayerCharacter npc)
            {
                this.TalkNpcAction.TalkToNpc(player, npc);
            }
        }
    }
}