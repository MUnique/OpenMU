// <copyright file="DuelFinishedPlugInSeason3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IDuelFinishedPlugIn"/> for the clients before Season 4.
/// </summary>
/// <remarks>
/// These clients don't know a packet which names the winner of the duel, so they just get the end
/// of the duel. The score, which the client got during the duel, tells who won.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.DuelFinishedPlugInSeason3_Name), Description = nameof(PlugInResources.DuelFinishedPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("A48CCCEF-C7CD-4D05-98CC-DC7C9696778B")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
public class DuelFinishedPlugInSeason3 : IDuelFinishedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelFinishedPlugInSeason3"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DuelFinishedPlugInSeason3(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask DuelFinishedAsync(Player winner, Player loser)
    {
        await this._player.Connection.SendDuelEndSeason3Async(this._player.GetId(this._player), this._player.Name).ConfigureAwait(false);
    }
}
