// <copyright file="SetTradeMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Trade
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ISetTradeMoneyPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("SetTradeMoneyPlugIn", "The default implementation of the ISetTradeMoneyPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("ca421b33-4ac2-4496-a8a3-11b740cded8c")]
    public class SetTradeMoneyPlugIn : ISetTradeMoneyPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTradeMoneyPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public SetTradeMoneyPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void SetTradeMoney(uint moneyAmount)
        {
            using var writer = this.player.Connection.StartSafeWrite(TradeMoneyUpdate.HeaderType, TradeMoneyUpdate.Length);
            _ = new TradeMoneyUpdate(writer.Span)
            {
                MoneyAmount = moneyAmount,
            };
            writer.Commit();
        }
    }
}