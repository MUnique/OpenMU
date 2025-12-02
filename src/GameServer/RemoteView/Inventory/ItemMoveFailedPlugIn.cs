// <copyright file="ItemMoveFailedPlugIn.cs" company="MUnique">
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
/// The default implementation of the <see cref="IItemMoveFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ItemMoveFailedPlugIn", "The default implementation of the IItemMoveFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("7fc3b870-a6a2-4751-bfa7-156ed97a1c87")]
public class ItemMoveFailedPlugIn : IItemMoveFailedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemMoveFailedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ItemMoveFailedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ItemMoveFailedAsync(Item? item)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = ItemMoveRequestFailedRef.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemMoveRequestFailedRef(span);
            if (item != null)
            {
                var itemSize = itemSerializer.SerializeItem(packet.ItemData, item);
                var actualSize = ItemMoveRequestFailedRef.GetRequiredSize(itemSize);
                span.Slice(0, actualSize).SetPacketSize();
                return actualSize;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}