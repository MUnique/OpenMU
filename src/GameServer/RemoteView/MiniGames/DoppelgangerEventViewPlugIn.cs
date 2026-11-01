// <copyright file="DoppelgangerEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using DoppelgangerResult = MUnique.OpenMU.GameLogic.MiniGames.Doppelganger.DoppelgangerResult;

/// <summary>
/// The default implementation of the <see cref="IDoppelgangerEventViewPlugIn"/> which
/// sends the doppelganger event packets (0xBF 0x0F - 0x14) to the client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DoppelgangerEventViewPlugIn_Name), Description = nameof(PlugInResources.DoppelgangerEventViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5C0B7E2A-9D41-4F6B-A8E3-2F7C1D93B460")]
public sealed class DoppelgangerEventViewPlugIn : IDoppelgangerEventViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public DoppelgangerEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowStateAsync(DoppelgangerState state)
    {
        // The state of the packet has the same values as the state of the game logic.
        await this._player.Connection.SendDoppelgangerStateUpdateAsync((DoppelgangerStateUpdate.DoppelgangerState)(byte)state).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMonsterPositionAsync(int position)
    {
        await this._player.Connection.SendDoppelgangerMonsterPositionAsync(ToPositionByte(position)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowIceWalkerAsync(int position)
    {
        await this._player.Connection.SendDoppelgangerIceWalkerStateAsync(DoppelgangerIceWalkerState.IceWalkerState.Appeared, ToPositionByte(position)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask HideIceWalkerAsync()
    {
        await this._player.Connection.SendDoppelgangerIceWalkerStateAsync(DoppelgangerIceWalkerState.IceWalkerState.Disappeared, 0).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowIceWalkerCountdownAsync()
    {
        await this._player.Connection.SendUpdateMiniGameStateAsync(UpdateMiniGameState.MiniGameTypeState.DoppelgangerIceWalkerCountdown).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowPlayInfoAsync(TimeSpan remainingTime, IReadOnlyCollection<(Player Player, int Position)> playerPositions)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        int WritePacket()
        {
            var count = Math.Min(playerPositions.Count, byte.MaxValue);
            var length = DoppelgangerPlayInfoRef.GetRequiredSize(count);
            var packet = new DoppelgangerPlayInfoRef(connection.Output.GetSpan(length)[..length]);
            packet.RemainingSeconds = (ushort)Math.Clamp(remainingTime.TotalSeconds, 0, ushort.MaxValue);
            packet.PlayerCount = (byte)count;

            var i = 0;
            foreach (var (player, position) in playerPositions.Take(count))
            {
                var playerPosition = packet[i];
                playerPosition.PlayerId = player.GetId(this._player);
                playerPosition.MapNumber = (byte)(player.CurrentMap?.MapId ?? 0);
                playerPosition.Position = ToPositionByte(position);
                i++;
            }

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMonsterGoalAsync(int goalCount, int maximumGoalCount)
    {
        await this._player.Connection.SendDoppelgangerMonsterGoalAsync(
            (byte)Math.Clamp(maximumGoalCount, 0, byte.MaxValue),
            (byte)Math.Clamp(goalCount, 0, byte.MaxValue))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowResultAsync(DoppelgangerResult result)
    {
        // The client doesn't show the reward experience, so it's always sent as 0.
        await this._player.Connection.SendDoppelgangerResultAsync((Network.Packets.ServerToClient.DoppelgangerResult.ResultType)(byte)result, 0).ConfigureAwait(false);
    }

    private static byte ToPositionByte(int position) => (byte)Math.Clamp(position, 0, IDoppelgangerEventViewPlugIn.MaximumPathPosition);
}
