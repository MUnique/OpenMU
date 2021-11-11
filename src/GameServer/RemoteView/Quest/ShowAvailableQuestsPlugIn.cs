// <copyright file="ShowAvailableQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowAvailableQuestsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - Show available quests", "The default implementation of the IShowAvailableQuestsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("63C7B3E2-EF66-49BB-A9F8-EFBD2389588F")]
public class ShowAvailableQuestsPlugIn : IShowAvailableQuestsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAvailableQuestsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAvailableQuestsPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public void ShowAvailableQuests()
    {
        var connection = this._player.Connection;
        if (connection is null || this._player.OpenedNpc is null)
        {
            return;
        }

        var totalQuests = this._player.GetAvailableQuestsOfOpenedNpc().ToList();
        var questCount = totalQuests.Count;

        using var writer = connection.StartSafeWrite(AvailableQuests.HeaderType, AvailableQuests.GetRequiredSize(questCount));
        var message = new AvailableQuests(writer.Span)
        {
            QuestCount = (ushort)questCount,
            QuestNpcNumber = (ushort)this._player.OpenedNpc.Definition.Number,
        };

        for (int i = 0; i < questCount; i++)
        {
            var block = message[i];
            var quest = totalQuests[i];
            block.Number = (ushort)quest.StartingNumber;
            block.Group = (ushort)quest.Group;
        }

        writer.Commit();
    }
}