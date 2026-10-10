// <copyright file="ShowDuelRequestResultPlugInSeason3.cs" company="MUnique">
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
/// The implementation of the <see cref="IShowDuelRequestResultPlugIn"/> for the clients before Season 4.
/// </summary>
/// <remarks>
/// These clients just know whether the duel started or not; the reason why it didn't start is
/// shown to the player as a message by the game logic instead.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowDuelRequestResultPlugInSeason3_Name), Description = nameof(PlugInResources.ShowDuelRequestResultPlugInSeason3_Description), ResourceType = typeof(PlugInResources))]
[Guid("8F773C3F-7AC2-49B4-97DF-A9A7D90C537B")]
[MaximumClient(3, 255, ClientLanguage.Invariant)]
public class ShowDuelRequestResultPlugInSeason3 : IShowDuelRequestResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDuelRequestResultPlugInSeason3"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDuelRequestResultPlugInSeason3(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowDuelRequestResultAsync(GameLogic.Views.Duel.DuelStartResult result, Player opponent)
    {
        var packetResult = result == GameLogic.Views.Duel.DuelStartResult.Success
            ? Network.Packets.ServerToClient.DuelStartResultSeason3.DuelStartResultSeason3Type.Started
            : Network.Packets.ServerToClient.DuelStartResultSeason3.DuelStartResultSeason3Type.NotStarted;
        await this._player.Connection.SendDuelStartResultSeason3Async(packetResult, opponent.GetId(this._player), opponent.Name).ConfigureAwait(false);
    }
}
