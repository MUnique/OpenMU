﻿// <copyright file="PlayerShopSetItemPricePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler which sets prices for an item in the player shop (3F 01).
    /// </summary>
    [PlugIn("Player Shop - Set Price", "Packet handler which sets prices for an item in the player shop (3F 01).")]
    [Guid("0E78ADBD-4B3D-4D3E-B5B1-34FA66BFC854")]
    [BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
    internal class PlayerShopSetItemPricePacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StoreHandlerGroupPlugIn));

        private readonly SetItemPriceAction setPriceAction = new SetItemPriceAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => 0x01;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var itemSlot = packet[4];
            var price = (int)packet.MakeDwordBigEndian(5);
            Logger.DebugFormat("Player [{0}] sets price of slot {1} to {2}", player.SelectedCharacter.Name, itemSlot, price);
            this.setPriceAction.SetPrice(player, itemSlot, price);
        }
    }
}