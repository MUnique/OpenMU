// <copyright file="ITradeFinishedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about a finished trade.
    /// </summary>
    public interface ITradeFinishedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// The trade process has finished with the specified result.
        /// </summary>
        /// <param name="tradeResult">The trade result.</param>
        void TradeFinished(TradeResult tradeResult);
    }
}