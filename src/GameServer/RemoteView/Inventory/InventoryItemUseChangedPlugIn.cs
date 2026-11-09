// <copyright file="InventoryItemUseChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IInventoryItemUseChangedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.InventoryItemUseChangedPlugIn_Name), Description = nameof(PlugInResources.InventoryItemUseChangedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("77EB94A6-09DB-424F-B58A-F540D10DD7A6")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class InventoryItemUseChangedPlugIn : IInventoryItemUseChangedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryItemUseChangedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public InventoryItemUseChangedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask InventoryItemUseChangedAsync(Item item)
    {
        await this._player.Connection.SendInventoryItemEquipResultAsync(item.ItemSlot, (byte)item.Durability).ConfigureAwait(false);
    }
}
