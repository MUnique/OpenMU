// <copyright file="DropItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for drop item packets.
    /// </summary>
    [PlugIn("DropItemHandlerPlugIn", "Handler for drop item packets.")]
    [Guid("b79bc453-74a0-4eea-8bc3-014d737aaa88")]
    internal class DropItemHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly DropItemAction dropAction = new DropItemAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.DropItem;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // 0xC3, 0x06, 0x23, CoX, CoY, i
            var slot = packet[5];
            var x = packet[3];
            var y = packet[4];
            this.dropAction.DropItem(player, slot, new Point(x, y));
        }
    }
}
