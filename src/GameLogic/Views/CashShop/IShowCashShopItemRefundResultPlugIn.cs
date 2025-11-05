// <copyright file="IShowCashShopItemRefundResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation informs about the result of a cash shop item refund.
/// </summary>
public interface IShowCashShopItemRefundResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of the cash shop item refund.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowCashShopItemRefundResultAsync(CashShopRefundResult result);
}