// <copyright file="AppearanceChangedExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IAppearanceChangedPlugIn"/> which is forwarding appearance changes of other players to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AppearanceChangedExtendedPlugIn_Name), Description = nameof(PlugInResources.AppearanceChangedExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A2F298E4-9F48-402A-B30D-9BC2BA8DEB2E")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class AppearanceChangedExtendedPlugIn : IAppearanceChangedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppearanceChangedExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AppearanceChangedExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AppearanceChangedAsync(Player changedPlayer, Item item, bool isEquipped)
    {
        var connection = this._player.Connection;
        if (connection is null || changedPlayer.Inventory is null)
        {
            return;
        }

        await connection.SendAppearanceChangedExtendedAsync(
            changedPlayer.GetId(this._player),
            item.ItemSlot,
            (byte)((isEquipped ? item.Definition?.Group : 0xFF) ?? 0xFF),
            (ushort)(item.Definition?.Number ?? 0xFFFF),
            item.Level,
            (byte)(ItemSerializerHelper.GetExcellentByte(item) | ItemSerializerHelper.GetFenrirByte(item)),
            (byte)(item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator != 0)?.AncientSetDiscriminator ?? 0),
            changedPlayer.SelectedCharacter?.HasFullAncientSetEquipped() is true)
            .ConfigureAwait(false);
    }
}
