// <copyright file="ItemAppearPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
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
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemAppearPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemAppearPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemAppear(Item newItem)
        {
            var itemSerializer = this.player.ItemSerializer;
            using var writer = this.player.Connection.StartSafeWrite(ItemAddedToInventory.HeaderType, ItemAddedToInventory.GetRequiredSize(itemSerializer.NeededSpace));
            var message = new ItemAddedToInventory(writer.Span)
            {
                InventorySlot = newItem.ItemSlot,
            };
            itemSerializer.SerializeItem(message.ItemData, newItem);
            writer.Commit();
        }
    }
}