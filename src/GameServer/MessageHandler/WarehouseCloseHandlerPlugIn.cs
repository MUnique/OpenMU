﻿// <copyright file="WarehouseCloseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for warehouse close packets.
    /// </summary>
    [PlugIn("WarehouseCloseHandlerPlugIn", "Handler for warehouse close packets.")]
    [Guid("7859931f-3341-4bd7-91ad-1b0b03f11198")]
    internal class WarehouseCloseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly CloseNpcDialogAction closeDialogAction = new CloseNpcDialogAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.VaultClose;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.closeDialogAction.CloseNpcDialog(player);
        }
    }
}
