// <copyright file="CurrentlyActiveQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest
{
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Quest;
    using MUnique.OpenMU.GameServer.MessageHandler.Quests;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ICurrentlyActiveQuestsPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("Quest - Currently Active Quests", "The default implementation of the ICurrentlyActiveQuestsPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9851157D-97CA-42F3-840C-8448D02B49A4")]
    [MinimumClient(6, 0, ClientLanguage.Invariant)]
    public class CurrentlyActiveQuestsPlugIn : ICurrentlyActiveQuestsPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentlyActiveQuestsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public CurrentlyActiveQuestsPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc />
        public void ShowActiveQuests()
        {
            var connection = this.player.Connection;
            if (connection is null || this.player.SelectedCharacter is null)
            {
                return;
            }

            var activeQuests = this.player.SelectedCharacter.QuestStates.Where(state => state.Group != QuestConstants.LegacyQuestGroup && state.ActiveQuest != null).Select(s => s.ActiveQuest!).ToList();
            using var writer = connection.StartSafeWrite(
                QuestStateList.HeaderType,
                QuestStateList.GetRequiredSize(activeQuests.Count));
            var message = new QuestStateList(writer.Span);
            int i = 0;
            foreach (var activeQuest in activeQuests)
            {
                var questIdentification = message[i];
                questIdentification.Number = (ushort)activeQuest.Number;
                questIdentification.Group = (ushort)activeQuest.Group;
                i++;
            }

            writer.Commit();
        }
    }
}