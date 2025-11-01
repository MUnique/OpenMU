// <copyright file="IShowCashShopOpenStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation handles the cash shop open/close state.
/// </summary>
public interface IShowCashShopOpenStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop open state response.
    /// </summary>
    /// <param name="isOpen">If set to <c>true</c>, the cash shop is open; otherwise, closed.</param>
    ValueTask ShowCashShopOpenStateAsync(bool isOpen);
}
