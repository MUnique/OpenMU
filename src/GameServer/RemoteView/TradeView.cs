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
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x38;
                packet[3] = slot;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void TradeItemAppear(byte toSlot, Item item)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, this.itemSerializer.NeededSpace + 5))
            {
                var packet = writer.Span;
                packet[2] = 0x39;
                packet[3] = toSlot;
                this.itemSerializer.SerializeItem(packet.Slice(4), item);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowTradeRequest(ITrader requester)
        {
            using (var writer = this.connection.StartSafeWrite(0xC3, 13))
            {
                var packet = writer.Span;
                packet[2] = 0x36;
                packet.Slice(3).WriteString(requester.Name, Encoding.UTF8);
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void ShowTradeRequestAnswer(bool tradeAccepted)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 0x14))
            {
                var packet = writer.Span;
                packet[2] = 0x37;
                packet.Slice(4).WriteString(this.trader.TradingPartner.Name, Encoding.UTF8);

                if (tradeAccepted)
                {
                    packet[3] = 1; // accepted

                    var tradePartnerLevel = (ushort)this.trader.TradingPartner.Level;
                    packet[14] = tradePartnerLevel.GetHighByte();
                    packet[15] = tradePartnerLevel.GetLowByte();

                    var guildId = this.trader.TradingPartner.GuildStatus?.GuildId ?? 0;
                    packet.Slice(16).SetIntegerBigEndian(guildId);
                }

                writer.Commit();
            }
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
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x3C;
                packet[3] = (byte)state;
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void SetTradeMoney(uint moneyAmount)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 8))
            {
                var packet = writer.Span;
                packet[2] = 0x3B;
                packet[3] = 0x15;
                packet.Slice(4).SetIntegerBigEndian(moneyAmount);
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void RequestedTradeMoneyHasBeenSet()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x3A;
                packet[3] = 0x01;
                writer.Commit();
            }
        }

        /// <inheritdoc />
        public void TradeFinished(TradeResult tradeResult)
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x3D;
                packet[3] = this.GetTradeResultByte(tradeResult);
                writer.Commit();
            }

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
                default: throw new ArgumentException($"TradeResult {tradeResult} not mapped to a byte value.");
            }
        }
    }
}
