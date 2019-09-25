// <copyright file="TalkNpcHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for talk npc request packets.
    /// </summary>
    [PlugIn("TalkNpcHandlerPlugIn", "Handler for talk npc request packets.")]
    [Guid("b196fd5e-706d-41a2-ba07-72a3b184151d")]
    internal class TalkNpcHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TalkNpcAction talkNpcAction = new TalkNpcAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => TalkToNpcRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            TalkToNpcRequest message = packet;
            if (player.CurrentMap.GetObject(message.NpcId) is NonPlayerCharacter npc)
            {
                this.talkNpcAction.TalkToNpc(player, npc);
            }
        }
    }
}
