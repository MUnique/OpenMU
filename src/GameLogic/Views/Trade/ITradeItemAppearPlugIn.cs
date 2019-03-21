// <copyright file="ITradeItemAppearPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Trade
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an added item in the item box of the trading partner.
    /// </summary>
    public interface ITradeItemAppearPlugIn : IViewPlugIn
    {
        /// <summary>
        /// An item has been added to the trade window by the other player.
        /// </summary>
        /// <remarks>
        /// It is also sent when an item gets moved inside the trade window. In this case a TradeItemDisappear message was sent before for the old slot.
        /// </remarks>
        /// <param name="toSlot">The slot at which the item has been added.</param>
        /// <param name="item">The item which has been added.</param>
        void TradeItemAppear(byte toSlot, Item item);
    }
}