// <copyright file="InventoryView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Linq;
    using System.Text;
    using GameLogic.Interfaces;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;
    using Network.Interfaces;

    /// <summary>
    /// The default implementation of the inventory view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class InventoryView : IInventoryView
    {
        private readonly IConnection connection;

        private readonly Player player;

        private readonly IItemSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
        /// <param name="itemSerializer">The item serializer.</param>
        public InventoryView(IConnection connection, Player player, IItemSerializer itemSerializer)
        {
            this.connection = connection;
            this.player = player;
            this.itemSerializer = itemSerializer;
        }

        /// <inheritdoc/>
        public void ItemMoved(Item item, byte toSlot, Storages storage)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x11))
            {
                var itemMoved = writer.Span;
                itemMoved[2] = 0x24;
                itemMoved[3] = storage == Storages.PersonalStore ? (byte)0 : (byte)storage;
                itemMoved[4] = toSlot;
                this.itemSerializer.SerializeItem(itemMoved.Slice(5), item);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemMoveFailed(Item item)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x11))
            {
                var itemMoveFailed = writer.Span;
                itemMoveFailed[2] = 0x24;
                itemMoveFailed[3] = 0xFF;
                itemMoveFailed[4] = 0;
                if (item != null)
                {
                    this.itemSerializer.SerializeItem(itemMoveFailed.Slice(5), item);
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateMoney()
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 0x08))
            {
                var message = writer.Span;
                message[2] = 0x22;
                message[3] = 0xFE;
                message.Slice(4, 4).SetIntegerSmallEndian((uint)this.player.Money);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemUpgraded(Item item)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x11))
            {
                var message = writer.Span;
                message[2] = 0xF3;
                message[3] = 0x14;
                message[4] = item.ItemSlot;
                this.itemSerializer.SerializeItem(message.Slice(5), item);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void UpdateInventoryList()
        {
            // C4 00 00 00 F3 10 ...
            const int slotNumberSize = sizeof(byte);
            var lengthPerItem = this.itemSerializer.NeededSpace + slotNumberSize;
            const int headerLength = 6;
            var itemCount = this.player.SelectedCharacter.Inventory.Items.Count();
            ushort length = (ushort)((itemCount * lengthPerItem) + headerLength);
            using (var writer = this.connection.StartSafeWrite(0xC4, length))
            {
                var packet = writer.Span;
                packet[3] = 0xF3;
                packet[4] = 0x10;
                packet[5] = (byte)itemCount;
                int i = 0;
                foreach (var item in this.player.SelectedCharacter.Inventory.Items)
                {
                    var offset = headerLength + (i * lengthPerItem);
                    packet[offset] = item.ItemSlot;
                    this.itemSerializer.SerializeItem(packet.Slice(offset + 1), item);
                    i++;
                }

                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemConsumed(byte inventorySlot, bool success)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x28;
                packet[3] = (byte)(success ? inventorySlot : 0xFF);
                packet[4] = 1;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemDurabilityChanged(Item item, bool afterConsumption)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 6))
            {
                var packet = writer.Span;
                packet[2] = 0x2A;
                packet[3] = item.ItemSlot;
                packet[4] = item.Durability;
                packet[5] = afterConsumption ? (byte)0x01 : (byte)0x00;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemAppear(Item newItem)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 5 + this.itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0x22;
                message[3] = newItem.ItemSlot;
                this.itemSerializer.SerializeItem(message.Slice(4), newItem);
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void ItemPickUpFailed(ItemPickFailReason reason)
        {
            byte reasonByte;
            switch (reason)
            {
                case ItemPickFailReason.General:
                    reasonByte = 0xFF;
                    break;
                case ItemPickFailReason.ItemStacked:
                    reasonByte = 0xFD;
                    break;
                case ItemPickFailReason.MaximumInventoryMoneyReached:
                    reasonByte = 0xFE;
                    break;
                default:
                    throw new ArgumentException($"Reason {reason} is unknown.");
            }

            using (var writer = this.connection.StartSafeWrite(0xC3, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x22;
                packet[3] = reasonByte;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void BuyNpcItemFailed()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4 + this.itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0x32;
                message[3] = 0xFF;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void NpcItemBought(Item newItem)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4 + this.itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0x32;
                message[3] = newItem.ItemSlot;
                this.itemSerializer.SerializeItem(message.Slice(4), newItem);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemSoldToNpc(bool success)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 8))
            {
                var message = writer.Span;
                message[2] = 0x33;
                message[3] = success ? (byte)1 : (byte)0;
                message.Slice(4).SetIntegerBigEndian((uint)this.player.Money);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemDropResult(byte slot, bool success)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var message = writer.Span;
                message[2] = 0x23;
                message[3] = success ? (byte)1 : (byte)0;
                message[4] = slot;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemSoldByPlayerShop(byte slot, Player buyer)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x0F))
            {
                var packet = writer.Span;
                packet[2] = 0x3F;
                packet[3] = 0x08;
                packet[4] = slot;
                packet.Slice(5).WriteString(buyer.SelectedCharacter.Name, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ItemBoughtFromPlayerShop(Item item)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4 + this.itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0x32;
                message[3] = item.ItemSlot;
                this.itemSerializer.SerializeItem(message.Slice(4), item);
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void ItemPriceSetResponse(byte itemSlot, ItemPriceResult result)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x3F;
                packet[3] = (byte)result;
                packet[4] = itemSlot;
                writer.Commit();
            }
        }
    }
}
