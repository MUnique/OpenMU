// <copyright file="ShopPriceKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

/// <summary>
/// The kind of a <see cref="ShopPrice"/>, which decides how it's paid.
/// </summary>
public enum ShopPriceKind
{
    /// <summary>
    /// The price is paid with the zen of the inventory.
    /// </summary>
    Zen,

    /// <summary>
    /// The price is paid with items of the inventory, e.g. jewels.
    /// </summary>
    Item,

    /// <summary>
    /// The price is paid with coins of the account.
    /// </summary>
    AccountCoin,
}
