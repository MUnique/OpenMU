// <copyright file="ItemUpgradedPlugIn.cs" company="MUnique">
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
/// The default implementation of the <see cref="IItemUpgradedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ItemUpgradedPlugIn", "The default implementation of the IItemUpgradedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ce4ed0a2-ec4e-4cbe-aabe-5573df86a659")]
public class ItemUpgradedPlugIn : IItemUpgradedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemUpgradedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemUpgradedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ItemUpgradedAsync(Item item)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = InventoryItemUpgradedRef.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new InventoryItemUpgradedRef(span)
            {
                InventorySlot = item.ItemSlot,
            };
            var itemSize = itemSerializer.SerializeItem(packet.ItemData, item);
            var actualSize = InventoryItemUpgradedRef.GetRequiredSize(itemSize);
            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}