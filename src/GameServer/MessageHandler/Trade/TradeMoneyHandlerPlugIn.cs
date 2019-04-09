// <copyright file="TradeMoneyHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handles the trade money packets.
    /// </summary>
    [PlugIn("TradeMoneyHandlerPlugIn", "Handles the trade money packets.")]
    [Guid("3c18f0ca-4ad8-4e07-a111-0acbe81256ca")]
    internal class TradeMoneyHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly TradeMoneyAction tradeAction = new TradeMoneyAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.TradeMoney;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 8)
            {
                return;
            }

            uint moneyAmount = packet.MakeDwordBigEndian(4);
            this.tradeAction.TradeMoney(player, moneyAmount);
        }
    }
}
