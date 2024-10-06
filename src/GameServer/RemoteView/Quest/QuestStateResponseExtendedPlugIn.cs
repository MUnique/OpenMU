// <copyright file="QuestStateResponseExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IQuestStateResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(QuestStateResponseExtendedPlugIn), "The extended implementation of the IQuestStateResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("5EA4F6F0-BF20-48AD-B491-11E707052E95")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class QuestStateResponseExtendedPlugIn : QuestStateResponsePlugIn, IQuestStateResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestStateResponseExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestStateResponseExtendedPlugIn(RemotePlayer player) : base(player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    protected override async ValueTask ShowNewQuestStateAsync(CharacterQuestState questState)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var size = QuestStateExtendedRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new QuestStateExtendedRef(span)
            {
                QuestGroup = (ushort)questState.Group,
            };

            packet.AssignActiveQuestData(questState, this._player);

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}