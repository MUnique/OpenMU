// <copyright file="QuestEventResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IQuestEventResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - Event Quests Response", "The default implementation of the IQuestEventResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("5EAE1634-9589-4DEB-AEBA-93D0AC8AC5DF")]
public class QuestEventResponsePlugIn : IQuestEventResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestEventResponsePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestEventResponsePlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public async ValueTask ShowActiveEventQuestsAsync()
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        // Hints: * This is only sent for new characters (level <= 1, no MG or DL).
        //        * I also found a struct which contains the following fields (each 2 bytes: NpcNumber (0), Count (1), QuestGroup (1), QuestNumber (0)).
        //        * Always: C1 0C F6 03 00 00 01 00 00 00 01 00
        int Write()
        {
            var size = QuestEventResponseRef.Length;
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new QuestEventResponseRef(span);
            var vanertGroup = packet[0];
            vanertGroup.Number = 1;

            var duprianGroup = packet[1];
            duprianGroup.Number = 1;
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}