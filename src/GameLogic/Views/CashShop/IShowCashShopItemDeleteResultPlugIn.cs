// <copyright file="IShowCashShopItemDeleteResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation shows the result of deleting an item from cash shop storage.
/// </summary>
public interface IShowCashShopItemDeleteResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop item delete result.
    /// </summary>
    /// <param name="success">If set to <c>true</c>, the deletion was successful; otherwise, <c>false</c>.</param>
    /// <param name="itemSlot">The item slot that was deleted.</param>
    ValueTask ShowCashShopItemDeleteResultAsync(bool success, byte itemSlot);
}
