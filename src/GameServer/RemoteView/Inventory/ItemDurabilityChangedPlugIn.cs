// <copyright file="ItemDurabilityChangedPlugIn.cs" company="MUnique">
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
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 6))
            {
                var packet = writer.Span;
                packet[2] = 0x2A;
                packet[3] = item.ItemSlot;
                packet[4] = item.Durability;
                packet[5] = afterConsumption ? (byte)0x01 : (byte)0x00;
                writer.Commit();
            }
        }
    }
}