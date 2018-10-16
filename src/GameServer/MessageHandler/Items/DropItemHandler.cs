// <copyright file="DropItemHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for drop item packets.
    /// </summary>
    internal class DropItemHandler : IPacketHandler
    {
        private readonly DropItemAction dropAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropItemHandler"/> class.
        /// </summary>
        public DropItemHandler()
        {
            this.dropAction = new DropItemAction();
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // 0xC3, 0x06, 0x23, CoX, CoY, i
            var slot = packet[5];
            var x = packet[3];
            var y = packet[4];
            this.dropAction.DropItem(player, slot, x, y);
        }
    }
}
