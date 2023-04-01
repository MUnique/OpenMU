// <copyright file="CurrentlyActiveQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

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
[MinimumClient(5, 0, ClientLanguage.Invariant)]
public class CurrentlyActiveQuestsPlugIn : ICurrentlyActiveQuestsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentlyActiveQuestsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CurrentlyActiveQuestsPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask ShowActiveQuestsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.SelectedCharacter is not { } character)
        {
            return;
        }

        int Write()
        {
            var activeQuests = character.QuestStates.Where(state => state.Group != QuestConstants.LegacyQuestGroup && state.ActiveQuest != null).Select(s => s.ActiveQuest!).ToList();
            var size = QuestStateListRef.GetRequiredSize(activeQuests.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var message = new QuestStateListRef(span);
            int i = 0;
            foreach (var activeQuest in activeQuests)
            {
                var questIdentification = message[i];
                questIdentification.Number = (ushort)activeQuest.Number;
                questIdentification.Group = (ushort)activeQuest.Group;
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}