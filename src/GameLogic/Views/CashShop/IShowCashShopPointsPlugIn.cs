// <copyright file="IShowCashShopPointsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// Interface of a view whose implementation informs about the player's cash point balances.
/// </summary>
public interface IShowCashShopPointsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the cash shop points to the player.
    /// </summary>
    ValueTask ShowCashShopPointsAsync();
}
