// <copyright file="IShopCurrencyExchange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

/// <summary>
/// Pays the price of an item of a player shop of a specific <see cref="ShopPriceKind"/>.
/// </summary>
public interface IShopCurrencyExchange
{
    /// <summary>
    /// Checks if the buyer has enough of the currency to pay the price, without changing anything.
    /// </summary>
    /// <param name="buyer">The buyer.</param>
    /// <param name="price">The price.</param>
    /// <returns>The result; <see cref="ShopPaymentResult.Success"/>, if the buyer can probably pay it.</returns>
    ShopPaymentResult CheckFunds(Player buyer, ShopPrice price);

    /// <summary>
    /// Tries to pay the price, i.e. to transfer it from the buyer to the seller.
    /// If it fails, nothing is changed.
    /// </summary>
    /// <remarks>
    /// It's called while the store lock of the seller is held, before the sold item is moved.
    /// </remarks>
    /// <param name="purchase">The purchase.</param>
    /// <returns>The result.</returns>
    ValueTask<ShopPaymentResult> TryPayAsync(ShopPurchase purchase);

    /// <summary>
    /// Notifies the seller about the received payment, after it was successful.
    /// </summary>
    /// <param name="purchase">The purchase.</param>
    /// <returns>The value task.</returns>
    ValueTask NotifySellerAsync(ShopPurchase purchase);

    /// <summary>
    /// Notifies the buyer about the payment, after it was successful.
    /// </summary>
    /// <param name="purchase">The purchase.</param>
    /// <returns>The value task.</returns>
    ValueTask NotifyBuyerAsync(ShopPurchase purchase);
}
