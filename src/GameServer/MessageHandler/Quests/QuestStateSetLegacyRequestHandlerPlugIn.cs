// <copyright file="QuestStateSetLegacyRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler for the quest state set request for the legacy quest system (0xA2 identifier).
    /// </summary>
    [PlugIn("Quest state set request (legacy)", "Packet handler for the quest state set request for the legacy quest system (0xA2 identifier).")]
    [Guid("3D8D1510-E92C-4D0E-9282-1A932B1B8195")]
    public class QuestStateSetLegacyRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly QuestStartAction questStartAction = new QuestStartAction();
        private readonly QuestCompletionAction questCompletionAction = new QuestCompletionAction();
        private readonly QuestCancelAction questCancelAction = new QuestCancelAction();

        /// <inheritdoc />
        public byte Key => LegacyQuestStateSetRequest.Code;

        /// <inheritdoc />
        public bool IsEncryptionExpected => false;

        /// <inheritdoc />
        public void HandlePacket(Player player, Span<byte> packet)
        {
            LegacyQuestStateSetRequest request = packet;
            switch (request.NewState)
            {
                case LegacyQuestState.Active:
                    var state = player.GetQuestState(QuestConstants.LegacyQuestGroup, request.QuestNumber);
                    if (state is null)
                    {
                        this.questStartAction.StartQuest(player, QuestConstants.LegacyQuestGroup, request.QuestNumber);
                    }
                    else
                    {
                        this.questCompletionAction.CompleteQuest(player, QuestConstants.LegacyQuestGroup, request.QuestNumber);
                    }

                    break;
                case LegacyQuestState.Complete:
                    this.questCompletionAction.CompleteQuest(player, QuestConstants.LegacyQuestGroup, request.QuestNumber);
                    break;
                case LegacyQuestState.Inactive:
                    this.questCancelAction.CancelQuest(player, QuestConstants.LegacyQuestGroup, request.QuestNumber);
                    break;
                default:
                    player.Logger.LogError($"Invalid state value {request.NewState}, quest number {request.QuestNumber}, player {player}.");
                    break;
            }
        }
    }
}