// <copyright file="IllusionTempleEventStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IIllusionTempleEventStateViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("0A68AF5F-2982-4748-BB82-AB493D80E8D2")]
public class IllusionTempleEventStateViewPlugIn : IIllusionTempleEventStateViewPlugIn
{
    private const string PlugInName = "Illusion Temple Event State";

    private const string PlugInDescription = "View plugin which tells the client about the state of an illusion temple event, so that it opens the event interface and removes the barriers of the arena.";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleEventStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public IllusionTempleEventStateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ChangeEventStateAsync(byte templeNumber, IllusionTempleEventStatus state)
    {
        await this._player.Connection.SendIllusionTempleEventStateAsync(templeNumber, Convert(state)).ConfigureAwait(false);
    }

    private static IllusionTempleEventState.EventState Convert(IllusionTempleEventStatus state)
    {
        return state switch
        {
            IllusionTempleEventStatus.WaitingRoom => IllusionTempleEventState.EventState.WaitingRoom,
            IllusionTempleEventStatus.Preparation => IllusionTempleEventState.EventState.Preparation,
            IllusionTempleEventStatus.BattleStarted => IllusionTempleEventState.EventState.BattleStarted,
            IllusionTempleEventStatus.Ended => IllusionTempleEventState.EventState.Ended,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }
}
