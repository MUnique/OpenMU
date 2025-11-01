// <copyright file="IShowCashShopItemBuyResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation shows the result of a cash shop item purchase.
/// </summary>
public interface IShowCashShopItemBuyResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop item buy result.
    /// </summary>
    /// <param name="result">The result of the purchase attempt.</param>
    /// <param name="productId">The product identifier.</param>
    ValueTask ShowCashShopItemBuyResultAsync(CashShopBuyResult result, int productId);
}
