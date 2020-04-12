// <copyright file="ShowMerchantStoreItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.NPC;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
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
        public void ShowMerchantStoreItemList(ICollection<Item> storeItems, StoreKind storeKind)
        {
            var itemSerializer = this.player.ItemSerializer;
            int sizePerItem = StoredItem.GetRequiredSize(itemSerializer.NeededSpace);
            using var writer = this.player.Connection.StartSafeWrite(StoreItemList.HeaderType, StoreItemList.GetRequiredSize(storeItems.Count, sizePerItem));
            var packet = new StoreItemList(writer.Span)
            {
                ItemCount = (byte)storeItems.Count,
                Type = Convert(storeKind),
            };

            int i = 0;
            foreach (var item in storeItems)
            {
                var storedItem = packet[i, sizePerItem];
                storedItem.ItemSlot = item.ItemSlot;
                itemSerializer.SerializeItem(storedItem.ItemData, item);
                i++;
            }

            writer.Commit();
        }

        private static StoreItemList.ItemWindow Convert(StoreKind storeKind)
        {
            return storeKind switch
            {
                StoreKind.Normal => StoreItemList.ItemWindow.Normal,
                StoreKind.ChaosMachine => StoreItemList.ItemWindow.ChaosMachine,
                StoreKind.ResurrectionFailed => StoreItemList.ItemWindow.ResurrectionFailed,
                _ => throw new ArgumentException($"Unknown value {storeKind}", nameof(storeKind)),
            };
        }
    }
}