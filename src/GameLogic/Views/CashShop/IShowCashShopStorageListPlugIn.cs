// <copyright file="IShowCashShopStorageListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation shows the cash shop storage list.
/// </summary>
public interface IShowCashShopStorageListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop storage list.
    /// </summary>
    ValueTask ShowCashShopStorageListAsync();
}
