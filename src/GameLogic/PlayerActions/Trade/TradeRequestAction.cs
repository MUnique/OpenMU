// <copyright file="TradeRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using MUnique.OpenMU.GameLogic.Views.Trade;

    /// <summary>
    /// Action to request a trade with another player.
    /// </summary>
    public class TradeRequestAction
    {
        /// <summary>
        /// Requests the trade from the other player.
        /// </summary>
        /// <param name="player">The player who requests the trade.</param>
        /// <param name="partner">The partner which will be requested.</param>
        /// <returns>The success of sending the request to the <paramref name="partner"/>.</returns>
        public bool RequestTrade(ITrader player, ITrader partner)
        {
            if (player.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>() == null || partner.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>() == null)
            {
                return false;
            }

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
            partner.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>()?.ShowTradeRequest(player);
            return true;
        }
    }
}
