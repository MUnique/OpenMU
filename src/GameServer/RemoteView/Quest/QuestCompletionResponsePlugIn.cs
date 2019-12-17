// <copyright file="QuestCompletionResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.GameServer.MessageHandler.Quests;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <seealso cref="IQuestCompletionResponsePlugIn" /> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Completion Response", "The default implementation of the IQuestCompletionResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("4BCA9C67-A695-4244-9F5A-3B7CAC049DB4")]
    public class QuestCompletionResponsePlugIn : IQuestCompletionResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestCompletionResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public QuestCompletionResponsePlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc />
        public void QuestCompleted(QuestDefinition quest)
        {
            if (quest.Group == QuestConstants.LegacyQuestGroup)
            {
                using var writer = this.player.Connection.StartSafeWrite(LegacySetQuestStateResponse.HeaderType, LegacySetQuestStateResponse.Length);
                _ = new LegacySetQuestStateResponse(writer.Span)
                {
                    NewState = LegacyQuestState.Complete,
                    QuestIndex = (byte)quest.Number,
                };

                writer.Commit();
            }
            else
            {
                using var writer = this.player.Connection.StartSafeWrite(QuestCompletionResponse.HeaderType, QuestCompletionResponse.Length);
                _ = new QuestCompletionResponse(writer.Span)
                {
                    QuestNumber = (ushort)quest.Number,
                    QuestGroup = (ushort)quest.Group,
                    IsQuestCompleted = true,
                };

                writer.Commit();
            }
        }
    }
}