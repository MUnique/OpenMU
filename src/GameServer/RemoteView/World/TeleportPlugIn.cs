// <copyright file="TeleportPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ITeleportPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(TeleportPlugIn), "The default implementation of the ITeleportPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("9506F77B-CA72-4150-87E3-57C889C91F02")]
[MinimumClient(1, 0, ClientLanguage.Invariant)]
public class TeleportPlugIn : ITeleportPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeleportPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public TeleportPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowTeleportedAsync()
    {
        if (this._player.SelectedCharacter?.CurrentMap is null)
        {
            return;
        }

        var mapNumber = this._player.SelectedCharacter.CurrentMap.Number.ToUnsigned();
        var position = this._player.Position;
        await this._player.Connection.SendMapChangedAsync(mapNumber, position.X, position.Y, this._player.Rotation.ToPacketByte(), false).ConfigureAwait(false);
    }
}