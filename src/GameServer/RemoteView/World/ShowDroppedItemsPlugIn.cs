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
            const int itemHeaderSize = 4; ////Id + Coordinates
            const byte freshDropFlag = 0x80;
            var itemSerializer = this.player.ItemSerializer;

            int itemCount = droppedItems.Count();
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, 9 + (itemSerializer.NeededSpace * itemCount)))
            {
                var data = writer.Span;
                data[3] = 0x20;
                data[4] = (byte)itemCount;

                int i = 0;
                int startOffset = 5;
                foreach (var item in droppedItems)
                {
                    var startIndex = startOffset + ((itemSerializer.NeededSpace + itemHeaderSize) * i);
                    var itemBlock = data.Slice(startIndex, itemSerializer.NeededSpace + itemHeaderSize);
                    itemBlock[0] = item.Id.GetHighByte();
                    if (freshDrops)
                    {
                        data[0] |= freshDropFlag;
                    }

                    itemBlock[1] = item.Id.GetLowByte();
                    itemBlock[2] = item.Position.X;
                    itemBlock[3] = item.Position.Y;
                    itemSerializer.SerializeItem(data.Slice(startIndex + 4), item.Item);

                    i++;
                }

                writer.Commit();
            }
        }
    }
}