// <copyright file="CashShopGiftResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

/// <summary>
/// The result of a request to send a package of the cash shop as gift.
/// </summary>
public enum CashShopGiftResult
{
    /// <summary>
    /// The package was bought and its products were added to the gift storage of the recipient.
    /// </summary>
    Success,

    /// <summary>
    /// The account doesn't have enough coins of the type of the package.
    /// </summary>
    NotEnoughCoins,

    /// <summary>
    /// The recipient character doesn't exist.
    /// </summary>
    RecipientNotFound,

    /// <summary>
    /// The package is currently not for sale.
    /// </summary>
    NotForSale,

    /// <summary>
    /// The package can't be sent as gift.
    /// </summary>
    NotGiftable,

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
    /// The storage of the recipient's account doesn't have space for the products.
    /// </summary>
    RecipientStorageFull,

    /// <summary>
    /// The recipient is a character of the sender's account, which isn't allowed.
    /// </summary>
    RecipientIsOwnAccount,

    /// <summary>
    /// A <see cref="PlugIns.ICashShopPackageBuyingPlugIn"/> refused the purchase.
    /// </summary>
    RefusedByPlugIn,

    /// <summary>
    /// The gift couldn't be saved.
    /// </summary>
    SaveFailed,
}
