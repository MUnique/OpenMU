// <copyright file="QuestProgressExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IQuestProgressPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(QuestProgressExtendedPlugIn), "The extended implementation of the IQuestProgressPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("30446944-29C6-48EE-A62E-3B724E7E444D")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class QuestProgressExtendedPlugIn : IQuestProgressPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestProgressExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestProgressExtendedPlugIn(RemotePlayer player)
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
            var size = QuestProgressExtendedRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            _ = new QuestProgressExtendedRef(span)
            {
                QuestGroup = (ushort)quest.Group,
            };

            var questState = this._player.SelectedCharacter?.QuestStates.FirstOrDefault(q => q.Group == quest.Group);

            // to write the quest state into the message, we can use the same logic as for the QuestState. The messages are equal in their content.
            QuestStateExtendedRef progress = span;
            progress.AssignActiveQuestData(questState, this._player);

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}