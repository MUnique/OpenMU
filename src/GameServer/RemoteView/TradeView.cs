// <copyright file="TradeView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using System.Text;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default implementation of the trade view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class TradeView : ITradeView
    {
        private readonly IConnection connection;
        private readonly ITrader trader;
        private readonly IItemSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="trader">The trader.</param>
        /// <param name="itemSerializer">The item serializer.</param>
        public TradeView(IConnection connection, ITrader trader, IItemSerializer itemSerializer)
        {
            this.connection = connection;
            this.trader = trader;
            this.itemSerializer = itemSerializer;
        }

        /// <inheritdoc/>
        public void TradeItemDisappear(byte slot, Item mi)
        {
            this.connection.Send(new byte[] { 0xC1, 0x04, 0x38, slot });
        }

        /// <inheritdoc/>
        public void TradeItemAppear(byte toSlot, Item item)
        {
            var packet = new byte[this.itemSerializer.NeededSpace + 5];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0x39;
            packet[3] = toSlot;
            this.itemSerializer.SerializeItem(packet, 4, item);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowTradeRequest(ITrader requester)
        {
            var packet = new byte[] { 0xC3, 13, 0x36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Encoding.ASCII.GetBytes(requester.Name, 0, requester.Name.Length, packet, 3);
            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public void ShowTradeRequestAnswer(bool tradeAccepted)
        {
            var packet = new byte[0x14];
            packet[0] = 0xC1;
            packet[1] = 0x14;
            packet[2] = 0x37;
            Encoding.UTF8.GetBytes(this.trader.TradingPartner.Name, 0, this.trader.TradingPartner.Name.Length, packet, 4);

            if (tradeAccepted)
            {
                packet[3] = 1; // accepted

                var tradePartnerLevel = (ushort)this.trader.TradingPartner.Level;
                packet[14] = tradePartnerLevel.GetHighByte();
                packet[15] = tradePartnerLevel.GetLowByte();

                var guildId = this.trader.TradingPartner.GuildStatus?.GuildId ?? 0;
                packet.SetIntegerBigEndian(guildId, 16);
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This message is sent when the trading partner presses or un-presses the trade accept button.
        /// Examples:
        /// C1 04 3C 00 Partner unpressed
        /// C1 04 3C 01 Partner pressed
        /// C1 04 3C 02 Button is red for x seconds
        /// </remarks>
        public void ChangeTradeButtonState(TradeButtonState state)
        {
            this.connection.Send(new byte[] { 0xC1, 0x04, 0x3C, (byte)state });
        }

        /// <inheritdoc/>
        public void SetTradeMoney(uint moneyAmount)
        {
            byte[] packet = { 0xC1, 8, 0x3B, 0x15, 0, 0, 0, 0 }; // todo: check if 0x15 correct
            packet.SetIntegerBigEndian(moneyAmount, 4);
            this.connection.Send(packet);
        }

        /// <inheritdoc />
        public void RequestedTradeMoneyHasBeenSet()
        {
            this.connection.Send(new byte[] { 0xC1, 4, 0x3A, 0x01 });
        }

        /// <inheritdoc />
        public void TradeFinished(TradeResult tradeResult)
        {
            var packet = new byte[] { 0xC1, 4, 0x3D, this.GetTradeResultByte(tradeResult) };
            this.connection.Send(packet);
            if (tradeResult != TradeResult.TimedOut && this.trader is Player player)
            {
                player.PlayerView.InventoryView.UpdateInventoryList();
                player.PlayerView.InventoryView.UpdateMoney();
            }
        }

        private byte GetTradeResultByte(TradeResult tradeResult)
        {
            switch (tradeResult)
            {
                case TradeResult.Cancelled: return 0;
                case TradeResult.Success: return 1;
                case TradeResult.FailedByFullInventory: return 2;
                case TradeResult.TimedOut: return 3;
                case TradeResult.FailedByItemsNotAllowedToTrade: return 4;
            }

            throw new ArgumentException($"TradeResult {tradeResult} not mapped to a byte value.");
        }
    }
}
