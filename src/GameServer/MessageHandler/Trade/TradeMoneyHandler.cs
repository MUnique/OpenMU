// <copyright file="TradeMoneyHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Trade
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handles the trade money packets.
    /// </summary>
    internal class TradeMoneyHandler : IPacketHandler
    {
        private readonly TradeMoneyAction tradeAction = new TradeMoneyAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
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
