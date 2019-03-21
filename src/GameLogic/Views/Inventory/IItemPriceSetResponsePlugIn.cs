// <copyright file="IItemPriceSetResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;

    /// <summary>
    /// Interface of a view whose implementation informs about the result of setting the item price in the personal shop.
    /// </summary>
    public interface IItemPriceSetResponsePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client about the result of setting the item price in the personal shop.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="result">The result.</param>
        void ItemPriceSetResponse(byte itemSlot, ItemPriceResult result);
    }
}