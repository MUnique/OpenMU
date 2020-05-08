// <copyright file="QuestStartRequestHandlerPlugIn.cs" company="MUnique">
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
    /// Packet handler for quest start request packets (0xF6, 0x0A identifier).
    /// </summary>
    [PlugIn("Quest - Start Request", "Packet handler for quest start request packets (0xF6, 0x0A identifier)")]
    [Guid("EF771EB5-9BC6-4DF3-BB0E-EADAB4295292")]
    [BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
    public class QuestStartRequestHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly QuestStartAction questStartAction = new QuestStartAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => QuestStartRequest.SubCode;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            QuestStartRequest request = packet;
            this.questStartAction.StartQuest(player, (short)request.QuestGroup, (short)request.QuestNumber, false);
        }
    }
}