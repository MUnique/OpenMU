// <copyright file="BloodCastleStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IBloodCastleStateViewPlugin"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(BloodCastleStateViewPlugIn), "The default implementation of the IBloodCastleStateViewPlugin which is forwarding everything to the game client with specific data packets.")]
[Guid("75A44740-FEE8-447A-BBA4-081A2410E408")]
public class BloodCastleStateViewPlugIn : IBloodCastleStateViewPlugin
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleStateViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public BloodCastleStateViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync(BloodCastleStatus status, TimeSpan remainingTime, int maxMonster, int curMonster, IIdentifiable? questItemOwner, Item? questItem)
    {
        await this._player.Connection.SendBloodCastleStateAsync(
            Convert(status),
            (ushort)remainingTime.TotalSeconds,
            (ushort)maxMonster,
            (ushort)curMonster,
            questItemOwner?.GetId(this._player) ?? 0xFF,
            (byte)((questItem?.Level + 1) ?? 0xFF))
            .ConfigureAwait(false);
    }

    private static BloodCastleState.Status Convert(BloodCastleStatus status)
    {
        return status switch
        {
            BloodCastleStatus.Ended => BloodCastleState.Status.BloodCastleEnded,
            BloodCastleStatus.GateDestroyed => BloodCastleState.Status.BloodCastleGateDestroyed,
            BloodCastleStatus.GateNotDestroyed => BloodCastleState.Status.BloodCastleGateNotDestroyed,
            BloodCastleStatus.Started => BloodCastleState.Status.BloodCastleStarted,
            _ => throw new ArgumentException($"Unknown blood castle status {status}"),
        };
    }
}