// <copyright file="QuestProgressPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IQuestProgressPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(QuestProgressPlugIn), "The default implementation of the IQuestProgressPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("5D2B7F90-FEFA-4889-B339-D64512471613")]
public class QuestProgressPlugIn : IQuestProgressPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestProgressPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestProgressPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public async ValueTask ShowQuestProgressAsync(QuestDefinition quest, bool wasProgressionRequested)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var size = QuestProgressRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new QuestProgressRef(span)
            {
                QuestGroup = (ushort)quest.Group,
            };

            var questState = this._player.SelectedCharacter?.QuestStates.FirstOrDefault(q => q.Group == quest.Group);

            // to write the quest state into the message, we can use the same logic as for the QuestState. The messages are equal in their content.
            QuestStateRef progress = span;
            progress.AssignActiveQuestData(questState, this._player);

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}