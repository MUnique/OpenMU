// <copyright file="ItemAppearPlugIn.cs" company="MUnique">
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
/// The default implementation of the <see cref="IItemAppearPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ItemAppearPlugIn", "The default implementation of the IItemAppearPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("525105ee-c1bf-4800-b80b-bdcd6c8ce704")]
public class ItemAppearPlugIn : IItemAppearPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemAppearPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemAppearPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ItemAppearAsync(Item newItem)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = ItemAddedToInventoryRef.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemAddedToInventoryRef(span)
            {
                InventorySlot = newItem.ItemSlot,
            };
            var itemSize = itemSerializer.SerializeItem(packet.ItemData, newItem);

            var actualSize = ItemAddedToInventoryRef.GetRequiredSize(itemSize);
            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}