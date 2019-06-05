// <copyright file="UpdateInventoryListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdateInventoryListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdateInventoryListPlugIn", "The default implementation of the IUpdateInventoryListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ba8ca7c7-a497-497e-b2f7-9f9366ff6ac5")]
    public class UpdateInventoryListPlugIn : IUpdateInventoryListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateInventoryListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdateInventoryListPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void UpdateInventoryList()
        {
            // C4 00 00 00 F3 10 ...
            const int slotNumberSize = sizeof(byte);
            var itemSerializer = this.player.ItemSerializer;
            var lengthPerItem = Math.Max(itemSerializer.NeededSpace + slotNumberSize, 8);
            const int headerLength = 6;
            var itemCount = this.player.SelectedCharacter.Inventory.Items.Count();
            ushort length = (ushort)((itemCount * lengthPerItem) + headerLength);
            using (var writer = this.player.Connection.StartSafeWrite(0xC4, length))
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
                    itemSerializer.SerializeItem(packet.Slice(offset + 1), item);
                    i++;
                }

                writer.Commit();
            }
        }
    }
}