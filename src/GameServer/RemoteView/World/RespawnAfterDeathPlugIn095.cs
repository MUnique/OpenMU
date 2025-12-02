// <copyright file="RespawnAfterDeathPlugIn095.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IRespawnAfterDeathPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(RespawnAfterDeathPlugIn095), "The default implementation of the IRespawnAfterDeathPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("02CBC7FF-73F1-4240-9859-AA1F6656E02C")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class RespawnAfterDeathPlugIn095 : IRespawnAfterDeathPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="RespawnAfterDeathPlugIn095"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public RespawnAfterDeathPlugIn095(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask RespawnAsync()
    {
        if (this._player.SelectedCharacter?.CurrentMap is null || this._player.Attributes is null)
        {
            return;
        }

        var mapNumber = (byte)this._player.SelectedCharacter.CurrentMap.Number;
        var position = this._player.IsWalking ? this._player.WalkTarget : this._player.Position;

        await this._player.Connection.SendRespawnAfterDeath095Async(
                position.X,
                position.Y,
                mapNumber,
                this._player.Rotation.ToPacketByte(),
                (ushort)this._player.Attributes[Stats.CurrentHealth],
                (ushort)this._player.Attributes[Stats.CurrentMana],
                (ushort)this._player.Attributes[Stats.CurrentAbility],
                (uint)this._player.SelectedCharacter.Experience,
                (uint)this._player.Money)
            .ConfigureAwait(false);
    }
}