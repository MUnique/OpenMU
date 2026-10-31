// <copyright file="MapChangeExtendedPlugIn.cs" company="MUnique">
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
/// The extended implementation of the <see cref="IMapChangePlugIn"/> which sends the <see cref="MapChangedExtended"/>
/// packet, which also carries the discriminator of the map, e.g. the stage of Devil Square.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.MapChangeExtendedPlugIn_Name), Description = nameof(PlugInResources.MapChangeExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("4ECDF254-172C-4222-B66F-0DDCB1F731E8")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class MapChangeExtendedPlugIn : IMapChangePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapChangeExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MapChangeExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public ValueTask MapChangeAsync()
    {
        return this.SendMessageAsync(true);
    }

    /// <inheritdoc/>
    public ValueTask MapChangeFailedAsync()
    {
        return this.SendMessageAsync(false);
    }

    private async ValueTask SendMessageAsync(bool success)
    {
        if (this._player.SelectedCharacter?.CurrentMap is not { } map)
        {
            return;
        }

        var position = this._player.IsWalking ? this._player.WalkTarget : this._player.Position;

        await this._player.Connection.SendClearedMapChangedExtendedAsync(map.Number.ToUnsigned(), position.X, position.Y, this._player.Rotation.ToPacketByte(), map.GetDiscriminatorForClient(), success).ConfigureAwait(false);
    }
}
