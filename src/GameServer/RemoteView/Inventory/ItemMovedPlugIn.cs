// <copyright file="ItemMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemMovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemMovedPlugIn", "The default implementation of the IItemMovedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("5c4c20fe-763d-42b6-bdfa-2ec943b191bc")]
    public class ItemMovedPlugIn : IItemMovedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemMovedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemMoved(Item item, byte toSlot, Storages storage)
        {
            var itemSerializer = this.player.ItemSerializer;
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 5 + itemSerializer.NeededSpace))
            {
                var itemMoved = writer.Span;
                itemMoved[2] = 0x24;
                itemMoved[3] = storage == Storages.PersonalStore ? (byte)0 : (byte)storage;
                itemMoved[4] = toSlot;
                itemSerializer.SerializeItem(itemMoved.Slice(5), item);
                writer.Commit();
            }
        }
    }
}
