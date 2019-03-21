// <copyright file="ISetTradeMoneyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about a changed money value by the trading partner.
    /// </summary>
    public interface ISetTradeMoneyPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Sets the amount of money which should be traded to the other player.
        /// </summary>
        /// <remarks>
        /// This message is sent when the trading partner put a certain amount of zen (also 0) into the trade.
        /// It overrides all previous sent zen values.
        /// </remarks>
        /// <param name="moneyAmount">The money amount.</param>
        void SetTradeMoney(uint moneyAmount);
    }
}