// <copyright file="IShowCashShopEventItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation shows the cash shop event/featured item list.
/// </summary>
public interface IShowCashShopEventItemListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop event item list.
    /// </summary>
    ValueTask ShowCashShopEventItemListAsync();
}
