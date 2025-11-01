// <copyright file="IShowCashShopItemConsumeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation shows the result of consuming an item from cash shop storage.
/// </summary>
public interface IShowCashShopItemConsumeResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop item consume result.
    /// </summary>
    /// <param name="success">If set to <c>true</c>, the consumption was successful; otherwise, <c>false</c>.</param>
    /// <param name="itemSlot">The item slot that was consumed.</param>
    ValueTask ShowCashShopItemConsumeResultAsync(bool success, byte itemSlot);
}
