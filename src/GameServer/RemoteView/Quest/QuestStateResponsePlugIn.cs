// <copyright file="QuestStateResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.GameServer.MessageHandler.Quests;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IQuestStateResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - State response", "The default implementation of the IQuestStateResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("BBE63528-67DC-4D5F-8C9D-D09AB488CC55")]
    public class QuestStateResponsePlugIn : IQuestStateResponsePlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestStateResponsePlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public QuestStateResponsePlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ShowQuestState(CharacterQuestState questState)
        {
            if (questState == null || questState.Group == QuestConstants.LegacyQuestGroup)
            {
                questState.SendLegacyQuestState(this.player);
                return;
            }

            using var writer = this.player.Connection.StartSafeWrite(QuestState.HeaderType, QuestState.Length);
            var message = new QuestState(writer.Span)
            {
                QuestGroup = (ushort)questState.Group,
            };

            message.AssignActiveQuestData(questState, this.player);
            writer.Commit();
        }
    }
}