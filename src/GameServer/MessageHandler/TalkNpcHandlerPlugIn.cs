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
    using MUnique.OpenMU.Network;
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
        public byte Key => (byte)PacketType.TalkNPC;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ushort id = NumberConversionExtensions.MakeWord(packet[4], packet[3]);
            if (player.CurrentMap.GetObject(id) is NonPlayerCharacter npc)
            {
                this.talkNpcAction.TalkToNpc(player, npc);
            }
        }
    }
}
