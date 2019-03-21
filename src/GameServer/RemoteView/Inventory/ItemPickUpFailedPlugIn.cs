﻿// <copyright file="ItemPickUpFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Inventory
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IItemPickUpFailedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ItemPickUpFailedPlugIn", "The default implementation of the IItemPickUpFailedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("f73a2cee-14bb-4404-a321-767f848e3571")]
    public class ItemPickUpFailedPlugIn : IItemPickUpFailedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPickUpFailedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ItemPickUpFailedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ItemPickUpFailed(ItemPickFailReason reason)
        {
            byte reasonByte;
            switch (reason)
            {
                case ItemPickFailReason.General:
                    reasonByte = 0xFF;
                    break;
                case ItemPickFailReason.ItemStacked:
                    reasonByte = 0xFD;
                    break;
                case ItemPickFailReason.MaximumInventoryMoneyReached:
                    reasonByte = 0xFE;
                    break;
                default:
                    throw new ArgumentException($"Reason {reason} is unknown.");
            }

            using (var writer = this.player.Connection.StartSafeWrite(0xC3, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x22;
                packet[3] = reasonByte;
                writer.Commit();
            }
        }
    }
}