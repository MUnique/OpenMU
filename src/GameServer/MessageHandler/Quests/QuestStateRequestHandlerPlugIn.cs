// <copyright file="QuestStateRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for quest state request packets (0xF6, 0x1B identifier).
    /// </summary>
    [PlugIn("Quest - Request quest state", "Packet handler for quest state request packets (0xF6, 0x1B identifier)")]
    [Guid("AEA553A5-06A7-43D6-9A59-7C64AAC768C7")]
    [BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
    internal class QuestStateRequestHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => QuestStateRequest.SubCode;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            QuestStateRequest request = packet;
            var questState = player.GetQuestState((short)request.QuestGroup, (short)request.QuestNumber);
            if (questState is null)
            {
                player.Logger.LogError($"Quest state not found. Group {request.QuestGroup}, Number {request.QuestNumber}, Player {player}");
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IQuestStateResponsePlugIn>()?.ShowQuestState(questState);
            }
        }
    }
}
