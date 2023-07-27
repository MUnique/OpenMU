// <copyright file="ChaosCastleStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IChaosCastleStateViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ChaosCastleStateViewPlugIn), "The default implementation of the IChaosCastleStateViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("E7E73888-8B8E-4D06-8D95-1C1CDEDDA9EC")]
public class ChaosCastleStateViewPlugIn : IChaosCastleStateViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChaosCastleStateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync(ChaosCastleStatus status, TimeSpan remainingTime, int maxMonster, int curMonster)
    {
        await this._player.Connection.SendBloodCastleStateAsync(
                Convert(status),
                (ushort)remainingTime.TotalSeconds,
                (ushort)maxMonster,
                (ushort)curMonster,
                0xFFFF,
                0xFF)
            .ConfigureAwait(false);
    }

    private static BloodCastleState.Status Convert(ChaosCastleStatus status)
    {
        return status switch
        {
            ChaosCastleStatus.Started => BloodCastleState.Status.ChaosCastleStarted,
            ChaosCastleStatus.Running => BloodCastleState.Status.ChaosCastleRunning,
            ChaosCastleStatus.RunningShrinkingStageOne => BloodCastleState.Status.ChaosCastleStageOne,
            ChaosCastleStatus.RunningShrinkingStageTwo => BloodCastleState.Status.ChaosCastleStageTwo,
            ChaosCastleStatus.RunningShrinkingStageThree => BloodCastleState.Status.ChaosCastleStageThree,
            ChaosCastleStatus.Ended => BloodCastleState.Status.ChaosCastleEnded,
            _ => throw new ArgumentException($"Unknown chaos castle status {status}"),
        };
    }
}