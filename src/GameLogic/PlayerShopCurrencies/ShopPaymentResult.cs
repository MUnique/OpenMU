// <copyright file="ShopPaymentResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

/// <summary>
/// The result of paying the price of an item of a player shop.
/// </summary>
public enum ShopPaymentResult
{
    /// <summary>
    /// The price has been paid.
    /// </summary>
    Success,

    /// <summary>
    /// The buyer doesn't have enough of the currency.
    /// </summary>
    LackOfCurrency,

    /// <summary>
    /// The buyer has enough jewels, but they can't pay the exact price, because bundles are not split
    /// and there is no change.
    /// </summary>
    ExactAmountNotPayable,

    /// <summary>
    /// The seller can't receive the payment, because it would exceed the maximum amount of the currency.
    /// </summary>
    SellerOverflow,

    /// <summary>
    /// The seller can't receive the payment, because the inventory has not enough space for the jewels.
    /// </summary>
    SellerInventoryFull,

    /// <summary>
    /// The payment failed for another reason, e.g. because the progress of the buyer couldn't be saved.
    /// </summary>
    Failed,
}
