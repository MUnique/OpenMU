// <copyright file="ShowDroppedItemsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowDroppedItemsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowDroppedItemsPlugIn", "The default implementation of the IShowDroppedItemsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("f89308c3-5fe7-46e2-adfc-85a56ba23233")]
public class ShowDroppedItemsPlugIn : IShowDroppedItemsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowDroppedItemsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowDroppedItemsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowDroppedItemsAsync(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        int itemCount = droppedItems.Count();
        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var droppedItemLength = ItemsDroppedRef.DroppedItemRef.GetRequiredSize(itemSerializer.NeededSpace);
            var size = ItemsDroppedRef.GetRequiredSize(itemCount, droppedItemLength);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new ItemsDroppedRef(span)
            {
                ItemCount = (byte)itemCount,
            };

            int headerSize = ItemsDroppedRef.GetRequiredSize(0, 0);
            int actualSize = headerSize;
            int i = 0;
            foreach (var item in droppedItems)
            {
                var itemBlock = new ItemsDroppedRef.DroppedItemRef(span[actualSize..]);
                itemBlock.Id = item.Id;
                if (freshDrops)
                {
                    itemBlock.IsFreshDrop = true;
                }

                itemBlock.PositionX = item.Position.X;
                itemBlock.PositionY = item.Position.Y;
                var itemSize = itemSerializer.SerializeItem(itemBlock.ItemData, item.Item);
                actualSize += ItemsDroppedRef.DroppedItemRef.GetRequiredSize(itemSize);
                i++;
            }

            span.Slice(0, actualSize).SetPacketSize();
            return actualSize;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}