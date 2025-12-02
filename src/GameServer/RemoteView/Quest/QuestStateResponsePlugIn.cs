// <copyright file="QuestStateResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IQuestStateResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - State response", "The default implementation of the IQuestStateResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("BBE63528-67DC-4D5F-8C9D-D09AB488CC55")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class QuestStateResponsePlugIn : IQuestStateResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestStateResponsePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestStateResponsePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowQuestStateAsync(CharacterQuestState? questState)
    {
        if (questState is null || questState.Group == QuestConstants.LegacyQuestGroup)
        {
            await this.ShowLegacyQuestStateAsync(this._player.SelectedCharacter?.QuestStates.FirstOrDefault(state => state.Group == QuestConstants.LegacyQuestGroup)).ConfigureAwait(false);
            return;
        }

        await this.ShowNewQuestStateAsync(questState).ConfigureAwait(false);
    }

    /// <summary>
    /// Shows the legacy quest state.
    /// </summary>
    /// <param name="questState">State of the quest.</param>
    protected virtual async ValueTask ShowLegacyQuestStateAsync(CharacterQuestState? questState)
    {
        await questState.SendLegacyQuestStateAsync(this._player).ConfigureAwait(false);
    }

    /// <summary>
    /// Shows the quest state of the new quest system.
    /// </summary>
    /// <param name="questState">State of the quest.</param>
    protected virtual async ValueTask ShowNewQuestStateAsync(CharacterQuestState questState)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var size = QuestStateRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new QuestStateRef(span)
            {
                QuestGroup = (ushort)questState.Group,
            };

            packet.AssignActiveQuestData(questState, this._player);

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}