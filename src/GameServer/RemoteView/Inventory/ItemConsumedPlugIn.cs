// <copyright file="ItemConsumedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemConsumedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemConsumedPlugIn", "The default implementation of the IItemConsumedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("06f1b02c-32b5-4d88-8d09-719246c8ebfe")]
    public class ItemConsumedPlugIn : IItemConsumedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConsumedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemConsumedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ItemConsumed(byte inventorySlot, bool success)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x28;
                packet[3] = (byte)(success ? inventorySlot : 0xFF);
                packet[4] = 1;
                writer.Commit();
            }
        }
    }
}