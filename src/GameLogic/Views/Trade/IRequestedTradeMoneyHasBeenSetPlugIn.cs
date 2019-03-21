// <copyright file="IRequestedTradeMoneyHasBeenSetPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    /// <summary>
    /// Interface of a view whose implementation informs about that the trade money value of the own player has been changed.
    /// </summary>
    public interface IRequestedTradeMoneyHasBeenSetPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the money amount which has been previously requested to be set,
        /// has been successfully placed into the trade.
        /// </summary>
        void RequestedTradeMoneyHasBeenSet();
    }
}