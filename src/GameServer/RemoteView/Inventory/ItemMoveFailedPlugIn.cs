// <copyright file="ItemMoveFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemMoveFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemMoveFailedPlugIn", "The default implementation of the IItemMoveFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("7fc3b870-a6a2-4751-bfa7-156ed97a1c87")]
    public class ItemMoveFailedPlugIn : IItemMoveFailedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMoveFailedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemMoveFailedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemMoveFailed(Item item)
        {
            var itemSerializer = this.player.ItemSerializer;
            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 5 + itemSerializer.NeededSpace))
            {
                var itemMoveFailed = writer.Span;
                itemMoveFailed[2] = 0x24;
                itemMoveFailed[3] = 0xFF;
                itemMoveFailed[4] = 0;
                if (item != null)
                {
                    this.player.ItemSerializer.SerializeItem(itemMoveFailed.Slice(5), item);
                }

                writer.Commit();
            }
        }
    }
}