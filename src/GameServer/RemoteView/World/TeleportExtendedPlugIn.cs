// <copyright file="TeleportExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="ITeleportPlugIn"/> which sends the <see cref="MapChangedExtended"/>
/// packet, which also carries the discriminator of the map, e.g. the stage of Devil Square.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.TeleportExtendedPlugIn_Name), Description = nameof(PlugInResources.TeleportExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("C7589970-B7A3-4C1C-83B0-40981697E794")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class TeleportExtendedPlugIn : ITeleportPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeleportExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public TeleportExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowTeleportedAsync()
    {
        if (this._player.SelectedCharacter?.CurrentMap is not { } map)
        {
            return;
        }

        var position = this._player.Position;
        await this._player.Connection.SendClearedMapChangedExtendedAsync(map.Number.ToUnsigned(), position.X, position.Y, this._player.Rotation.ToPacketByte(), map.GetDiscriminatorForClient(), false).ConfigureAwait(false);
    }
}
