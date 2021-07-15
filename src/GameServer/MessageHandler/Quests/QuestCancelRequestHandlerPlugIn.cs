// <copyright file="QuestCancelRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for quest cancel request packets (0xF6, 0x0F identifier).
    /// </summary>
    [PlugIn("Quest - Cancel Request", "Packet handler for quest cancel request packets (0xF6, 0x0F identifier)")]
    [Guid("996E33ED-14B7-4165-9E6C-AE583974D0B7")]
    [BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
    public class QuestCancelRequestHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly QuestCancelAction questCancelAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => QuestCancelRequest.SubCode;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            QuestCancelRequest request = packet;
            this.questCancelAction.CancelQuest(player, (short)request.QuestGroup, (short)request.QuestNumber);
        }
    }
}