// <copyright file="IShowTradeRequestAnswerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about a trade request answer.
    /// </summary>
    public interface IShowTradeRequestAnswerPlugIn : IViewPlugIn
    {
        /// <summary>
        /// The requested trade partner has answered the request. Information about the trading partner can be found at ITrader.TradingPartner.
        /// </summary>
        /// <param name="tradeAccepted">if set to <c>true</c> the trade has been accepted and will be opened.</param>
        void ShowTradeRequestAnswer(bool tradeAccepted);
    }
}