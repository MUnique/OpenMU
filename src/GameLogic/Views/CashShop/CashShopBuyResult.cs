// <copyright file="CashShopBuyResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a request to buy a package in the cash shop.
/// </summary>
public enum CashShopBuyResult
{
    /// <summary>
    /// The package was bought and its products were added to the storage.
    /// </summary>
    Success,

    /// <summary>
    /// The account doesn't have enough coins of the type of the package.
    /// </summary>
    NotEnoughCoins,

    /// <summary>
    /// The package is currently not for sale.
    /// </summary>
    NotForSale,

    /// <summary>
    /// The package doesn't exist.
    /// </summary>
    PackageNotFound,

    /// <summary>
    /// The requested price option doesn't exist in the package.
    /// </summary>
    InvalidPriceOption,

    /// <summary>
    /// The requested coin type isn't the one of the package.
    /// </summary>
    WrongCoinType,

    /// <summary>
    /// The storage of the account doesn't have space for the products.
    /// </summary>
    StorageFull,

    /// <summary>
    /// A <see cref="PlugIns.ICashShopPackageBuyingPlugIn"/> refused the purchase.
    /// </summary>
    RefusedByPlugIn,

    /// <summary>
    /// The purchase couldn't be saved.
    /// </summary>
    SaveFailed,
}
