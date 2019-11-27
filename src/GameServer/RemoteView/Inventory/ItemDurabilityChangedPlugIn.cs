// <copyright file="ItemDurabilityChangedPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IItemDurabilityChangedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemDurabilityChangedPlugIn", "The default implementation of the IItemDurabilityChangedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f121286f-2e43-4a66-8f34-5dbe69304e1e")]
    public class ItemDurabilityChangedPlugIn : IItemDurabilityChangedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDurabilityChangedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemDurabilityChangedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemDurabilityChanged(Item item, bool afterConsumption)
        {
            using var writer = this.player.Connection.StartSafeWrite(
                Network.Packets.ServerToClient.ItemDurabilityChanged.HeaderType,
                Network.Packets.ServerToClient.ItemDurabilityChanged.Length);
            _ = new ItemDurabilityChanged(writer.Span)
            {
                InventorySlot = item.ItemSlot,
                Durability = item.Durability,
                ByConsumption = afterConsumption,
            };

            writer.Commit();
        }
    }
}