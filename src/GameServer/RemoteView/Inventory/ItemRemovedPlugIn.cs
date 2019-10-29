// <copyright file="ItemRemovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemRemovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(ItemRemovedPlugIn), "The default implementation of the IItemRemovedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("06f1b02c-32b5-4d88-8d09-719246c8ebfe")]
    public class ItemRemovedPlugIn : IItemRemovedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRemovedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemRemovedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void RemoveItem(byte inventorySlot)
        {
            using var writer = this.player.Connection.StartSafeWrite(ItemRemoved.HeaderType, ItemRemoved.Length);
            _ = new ItemRemoved(writer.Span)
            {
                InventorySlot = inventorySlot,
            };

            writer.Commit();
        }
    }
}