// <copyright file="ItemRepairHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for item repair packets.
    /// </summary>
    internal class ItemRepairHandler : IPacketHandler
    {
        private readonly ItemRepairAction repairAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepairHandler"/> class.
        /// </summary>
        public ItemRepairHandler()
        {
            this.repairAction = new ItemRepairAction();
        }

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
