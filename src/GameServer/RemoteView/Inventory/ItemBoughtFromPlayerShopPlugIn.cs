// <copyright file="ItemBoughtFromPlayerShopPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IItemBoughtFromPlayerShopPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ItemBoughtFromPlayerShopPlugIn", "The default implementation of the IItemBoughtFromPlayerShopPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("3b2498f2-3ae8-4700-8a61-1ffe49822caf")]
public class ItemBoughtFromPlayerShopPlugIn : IItemBoughtFromPlayerShopPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemBoughtFromPlayerShopPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemBoughtFromPlayerShopPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ItemBoughtFromPlayerShopAsync(Item item)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = ItemBoughtRef.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemBoughtRef(span)
            {
                InventorySlot = item.ItemSlot,
            };
            itemSerializer.SerializeItem(packet.ItemData, item);

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}