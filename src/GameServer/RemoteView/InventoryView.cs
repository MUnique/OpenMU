// <copyright file="InventoryView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Linq;
    using System.Text;
    using GameLogic.Views;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Network;

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
            var itemMoved = new byte[0x11];
            itemMoved[0] = 0xC3;
            itemMoved[1] = 0x11;
            itemMoved[2] = 0x24;
            itemMoved[3] = storage == Storages.PersonalStore ? (byte)0 : (byte)storage;
            itemMoved[4] = toSlot;
            this.itemSerializer.SerializeItem(itemMoved, 5, item);

            this.connection.Send(itemMoved);
        }

        /// <inheritdoc/>
        public void UpdateMoney()
        {
            var message = new byte[] { 0xC3, 0x08, 0x22, 0xFE, 0, 0, 0, 0 };
            message.SetIntegerSmallEndian((uint)this.player.Money, 4);
            this.connection.Send(message);
        }

        /// <inheritdoc/>
        public void ItemUpgraded(Item item)
        {
            var message = new byte[0x11];
            message[0] = 0xC1;
            message[1] = 0x11;
            message[2] = 0xF3;
            message[3] = 0x14;
            message[4] = item.ItemSlot;
            this.itemSerializer.SerializeItem(message, 5, item);
            this.connection.Send(message);
        }

        /// <inheritdoc/>
        public void UpdateInventoryList()
        {
            // C4 00 00 00 F3 10 ...
            const int slotNumberSize = sizeof(byte);
            var lengthPerItem = this.itemSerializer.NeededSpace + slotNumberSize;
            const int headerLength = 6;
            var itemCount = this.player.SelectedCharacter.Inventory.Items.Count();
            ushort len = (ushort)((itemCount * lengthPerItem) + headerLength);
            byte[] packet = new byte[len];
            packet[0] = 0xC4;
            packet[1] = len.GetHighByte();
            packet[2] = len.GetLowByte();
            packet[3] = 0xF3;
            packet[4] = 0x10;
            packet[5] = (byte)itemCount;
            int i = 0;
            foreach (var item in this.player.SelectedCharacter.Inventory.Items)
            {
                var offset = headerLength + (i * lengthPerItem);
                packet[offset] = item.ItemSlot;
                this.itemSerializer.SerializeItem(packet, offset + 1, item);
                i++;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ItemConsumed(byte inventorySlot, bool success)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0x28, (byte)(success ? 1 : 0), inventorySlot });
        }

        /// <inheritdoc/>
        public void ItemDurabilityChanged(Item item)
        {
            this.connection.Send(new byte[] { 0xC1, 0x06, 0x2A, item.ItemSlot, item.Durability, 0x00 });
        }

        /// <inheritdoc/>
        public void ItemAppear(Item newItem)
        {
            var message = new byte[5 + this.itemSerializer.NeededSpace];
            message[0] = 0xC3;
            message[1] = (byte)message.Length;
            message[2] = 0x22;
            message[3] = newItem.ItemSlot;
            this.itemSerializer.SerializeItem(message, 4, newItem);
            this.connection.Send(message);
        }

        /// <inheritdoc/>
        public void BuyNpcItemFailed()
        {
            this.connection.Send(new byte[] { 0xC1, 0x10, 0x32, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        /// <inheritdoc/>
        public void NpcItemBought(Item newItem)
        {
            byte[] newPacket = new byte[] { 0xC1, 0x10, 0x32, newItem.ItemSlot, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            this.itemSerializer.SerializeItem(newPacket, 4, newItem);
            this.connection.Send(newPacket);
        }

        /// <inheritdoc/>
        public void ItemSoldToNpc(bool success)
        {
            var message = new byte[] { 0xC3, 0x08, 0x33, success ? (byte)1 : (byte)0, 0, 0, 0, 0 };
            message.SetIntegerSmallEndian((uint)this.player.Money, 4);
            this.connection.Send(message);
        }

        /// <inheritdoc/>
        public void ItemDropResult(byte slot, bool success)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0x23, success ? (byte)1 : (byte)0, slot });
        }

        /// <inheritdoc/>
        public void ItemSoldByPlayerShop(byte slot, Player buyer)
        {
            var packet = new byte[] { 0xC1, 0x0F, 0x3F, 8, slot, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Encoding.UTF8.GetBytes(buyer.SelectedCharacter.Name, 0, buyer.SelectedCharacter.Name.Length, packet, 5);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ItemBoughtFromPlayerShop(Item item)
        {
            var message = new byte[0x10];
            message[0] = 0xC1;
            message[1] = 0x10;
            message[2] = 0x32;
            message[3] = item.ItemSlot;
            this.itemSerializer.SerializeItem(message, 4, item);
            this.connection.Send(message);
        }
    }
}
