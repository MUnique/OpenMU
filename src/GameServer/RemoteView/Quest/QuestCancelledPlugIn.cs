// <copyright file="QuestCancelledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.GameServer.MessageHandler.Quests;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IQuestCancelledPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Cancelled", "The default implementation of the IQuestCancelledPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("733C8E1A-A663-4804-96E3-1EA955438970")]
    public class QuestCancelledPlugIn : IQuestCancelledPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestCancelledPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public QuestCancelledPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc/>
        public void QuestCancelled(QuestDefinition quest)
        {
            if (quest.Group == QuestConstants.LegacyQuestGroup)
            {
                this.player.SelectedCharacter?.QuestStates.FirstOrDefault(q => q.Group == QuestConstants.LegacyQuestGroup)?.SendLegacyQuestState(this.player);
                return;
            }

            this.player.Connection?.SendQuestCancelled((ushort)quest.Number, (ushort)quest.Group);
        }
    }
}