// <copyright file="ShowItemCraftingPreviewExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IShowItemCraftingPreviewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowItemCraftingPreviewExtendedPlugIn_Name), Description = nameof(PlugInResources.ShowItemCraftingPreviewExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("52076D2D-FBE5-441C-BFB8-C671ED1C1AF3")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ShowItemCraftingPreviewExtendedPlugIn : IShowItemCraftingPreviewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowItemCraftingPreviewExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowItemCraftingPreviewExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowCraftingPreviewAsync(ItemCrafting? crafting, byte successRate, int price)
    {
        await this._player.Connection.SendItemCraftingPreviewExtendedAsync(crafting?.Number ?? 0, successRate, (uint)Math.Max(price, 0)).ConfigureAwait(false);
    }
}
