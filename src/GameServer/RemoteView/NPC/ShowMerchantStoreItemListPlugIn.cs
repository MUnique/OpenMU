// <copyright file="ShowMerchantStoreItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowMerchantStoreItemListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowMerchantStoreItemListPlugIn", "The default implementation of the IShowMerchantStoreItemListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("53e00ae0-4d5b-4f63-88e0-7d526f8438af")]
    public class ShowMerchantStoreItemListPlugIn : IShowMerchantStoreItemListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMerchantStoreItemListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowMerchantStoreItemListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowMerchantStoreItemList(ICollection<Item> storeItems)
        {
            // C2 [len_high] [len_low] 31 [itemcount_high] [itemcount_low]
            // for each item: [slot number] [item data....]
            const int slotNumberSize = 1;
            const int headerSize = 6;
            var itemSerializer = this.player.ItemSerializer;
            int sizePerItem = itemSerializer.NeededSpace + slotNumberSize;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, headerSize + (storeItems.Count * sizePerItem)))
            {
                var packet = writer.Span;
                packet[3] = 0x31;
                packet.Slice(4).SetShortLittleEndian((ushort)storeItems.Count);

                int i = 0;
                foreach (var item in storeItems)
                {
                    var offset = headerSize + (i * sizePerItem);
                    var itemBlock = packet.Slice(offset, sizePerItem);
                    itemBlock[0] = item.ItemSlot;
                    itemSerializer.SerializeItem(itemBlock.Slice(slotNumberSize), item);
                    i++;
                }

                writer.Commit();
            }
        }
    }
}