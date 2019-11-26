// <copyright file="ShowDroppedItemsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowDroppedItemsPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowDroppedItemsPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops)
        {
            var itemSerializer = this.player.ItemSerializer;
            var droppedItemLength = ItemsDropped.DroppedItem.GetRequiredSize(itemSerializer.NeededSpace);
            int itemCount = droppedItems.Count();
            using var writer = this.player.Connection.StartSafeWrite(ItemsDropped.HeaderType, ItemsDropped.GetRequiredSize(itemCount, droppedItemLength));
            var packet = new ItemsDropped(writer.Span)
            {
                ItemCount = (byte)itemCount,
            };

            int i = 0;
            foreach (var item in droppedItems)
            {
                var itemBlock = packet[i, droppedItemLength];
                itemBlock.Id = item.Id;
                if (freshDrops)
                {
                    itemBlock.IsFreshDrop = true;
                }

                itemBlock.PositionX = item.Position.X;
                itemBlock.PositionY = item.Position.Y;
                itemSerializer.SerializeItem(itemBlock.ItemData, item.Item);

                i++;
            }

            writer.Commit();
        }
    }
}