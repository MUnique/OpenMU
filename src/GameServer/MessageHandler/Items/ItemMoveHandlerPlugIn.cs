﻿// <copyright file="ItemMoveHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for item move packets.
    /// </summary>
    [PlugIn("ItemMoveHandlerPlugIn", "Handler for item move packets.")]
    [Guid("c499c596-7711-4971-bc83-7abd9e6b5553")]
    internal class ItemMoveHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly MoveItemAction moveAction = new MoveItemAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.InventoryMove;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            byte fromID = packet[3];
            byte fromSlot = packet[4];
            byte toID = packet[17];
            byte toSlot = packet[18];

            this.moveAction.MoveItem(player, fromSlot, (Storages)fromID, toSlot, (Storages)toID);
        }
    }
}
