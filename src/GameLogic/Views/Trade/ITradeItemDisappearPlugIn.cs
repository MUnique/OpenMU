// <copyright file="ITradeItemDisappearPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an removed item in the item box of the trading partner.
    /// </summary>
    public interface ITradeItemDisappearPlugIn : IViewPlugIn
    {
        /// <summary>
        /// An item has been removed from the trade window by the other player.
        /// </summary>
        /// <remarks>
        /// It will be also sent when an item is moved within the trade window.
        /// </remarks>
        /// <param name="fromSlot">The slot from which the item has been removed.</param>
        /// <param name="item">The item which has been removed.</param>
        void TradeItemDisappear(byte fromSlot, Item item);
    }
}