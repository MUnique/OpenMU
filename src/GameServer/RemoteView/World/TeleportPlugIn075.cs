// <copyright file="TeleportPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ITeleportPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(TeleportPlugIn075), "The default implementation of the ITeleportPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("490DB5E5-9DB6-4068-9708-E7D69F82BF3B")]
public class TeleportPlugIn075 : ITeleportPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeleportPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public TeleportPlugIn075(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowTeleportedAsync()
    {
        if (this._player.SelectedCharacter?.CurrentMap is null)
        {
            return;
        }

        var mapNumber = (byte)this._player.SelectedCharacter.CurrentMap.Number;
        var position = this._player.Position;
        await this._player.Connection.SendMapChanged075Async(mapNumber, position.X, position.Y, this._player.Rotation.ToPacketByte(), isMapChange: false).ConfigureAwait(false);
    }
}