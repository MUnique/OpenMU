// <copyright file="TradeRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    /// <summary>
    /// Action to request a trade with another player.
    /// </summary>
    public class TradeRequestAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeRequestAction"/> class.
        /// </summary>
        public TradeRequestAction()
        {
        }

        /// <summary>
        /// Requests the trade from the other player.
        /// </summary>
        /// <param name="player">The player who requests the trade.</param>
        /// <param name="partner">The partner which will be requested.</param>
        /// <returns>The success of sending the request to the <paramref name="partner"/>.</returns>
        public bool RequestTrade(ITrader player, ITrader partner)
        {
            if (!partner.PlayerState.TryAdvanceTo(PlayerState.TradeRequested))
            {
                return false;
            }

            if (!player.PlayerState.TryAdvanceTo(PlayerState.TradeRequested))
            {
                partner.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
                return false;
            }

            player.TradingPartner = partner;
            partner.TradingPartner = player;
            partner.TradeView.ShowTradeRequest(player);
            return true;
        }
    }
}
