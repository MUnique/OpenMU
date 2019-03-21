// <copyright file="IShowTradeRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about an incoming trade request.
    /// </summary>
    public interface IShowTradeRequestPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the trade request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowTradeRequest(ITrader requester);
    }
}