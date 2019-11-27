// <copyright file="RequestedTradeMoneyHasBeenSetPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IRequestedTradeMoneyHasBeenSetPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("RequestedTradeMoneyHasBeenSetPlugIn", "The default implementation of the IRequestedTradeMoneyHasBeenSetPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d07bc23a-e4ec-462b-96d6-b2da739664d0")]
    public class RequestedTradeMoneyHasBeenSetPlugIn : IRequestedTradeMoneyHasBeenSetPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestedTradeMoneyHasBeenSetPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public RequestedTradeMoneyHasBeenSetPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void RequestedTradeMoneyHasBeenSet()
        {
            using var writer = this.player.Connection.StartSafeWrite(TradeMoneySetResponse.HeaderType, TradeMoneySetResponse.Length);
            _ = new TradeMoneySetResponse(writer.Span);
            writer.Commit();
        }
    }
}