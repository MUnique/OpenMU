﻿// <copyright file="ItemRepairHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for item repair packets.
    /// </summary>
    [PlugIn("ItemRepairHandlerPlugIn", "Handler for item repair packets.")]
    [Guid("85b4a195-c90c-47f8-bae2-833b5d2ef398")]
    internal class ItemRepairHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ItemRepairAction repairAction = new ItemRepairAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.ItemRepair;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var slot = packet[3];
            if (slot == 0xFF)
            {
                this.repairAction.RepairAllItems(player);
            }
            else
            {
                this.repairAction.RepairItem(player, slot);
            }
        }
    }
}
