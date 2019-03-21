// <copyright file="ItemUpgradedPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IItemUpgradedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemUpgradedPlugIn", "The default implementation of the IItemUpgradedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ce4ed0a2-ec4e-4cbe-aabe-5573df86a659")]
    public class ItemUpgradedPlugIn : IItemUpgradedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUpgradedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemUpgradedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemUpgraded(Item item)
        {
            var itemSerializer = this.player.ItemSerializer;
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5 + itemSerializer.NeededSpace))
            {
                var message = writer.Span;
                message[2] = 0xF3;
                message[3] = 0x14;
                message[4] = item.ItemSlot;
                this.player.ItemSerializer.SerializeItem(message.Slice(5), item);
                writer.Commit();
            }
        }
    }
}