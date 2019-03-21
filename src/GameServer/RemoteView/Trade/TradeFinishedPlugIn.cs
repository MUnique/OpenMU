// <copyright file="TradeFinishedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ITradeFinishedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("TradeFinishedPlugIn", "The default implementation of the ITradeFinishedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ec11fe95-31c6-4a2e-942d-4d10a84830c1")]
    public class TradeFinishedPlugIn : ITradeFinishedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeFinishedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public TradeFinishedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void TradeFinished(TradeResult tradeResult)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x3D;
                packet[3] = this.GetTradeResultByte(tradeResult);
                writer.Commit();
            }

            if (tradeResult != TradeResult.TimedOut)
            {
                this.player.ViewPlugIns.GetPlugIn<IUpdateInventoryListPlugIn>()?.UpdateInventoryList();
                this.player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
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