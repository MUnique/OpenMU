// <copyright file="IShowShopCurrenciesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Interface of a view whose implementation informs about the items and account coins which can be used for the
/// prices of items in player shops, in addition to zen.
/// </summary>
/// <remarks>
/// Only clients which can show and buy items with other prices than zen have an implementation of it.
/// </remarks>
public interface IShowShopCurrenciesPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the items and account coins which can be used for prices in player shops, in addition to zen.
    /// </summary>
    /// <param name="priceItems">The items with which prices can be paid.</param>
    /// <param name="coinTypes">The account coins with which prices can be paid.</param>
    ValueTask ShowShopCurrenciesAsync(IReadOnlyCollection<ItemDefinition> priceItems, IReadOnlyCollection<CashShopCoinType> coinTypes);
}
