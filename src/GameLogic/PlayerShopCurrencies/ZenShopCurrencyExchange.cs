// <copyright file="ZenShopCurrencyExchange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Pays prices in zen, by transferring the zen of the inventory of the buyer to the one of the seller.
/// </summary>
public sealed class ZenShopCurrencyExchange : IShopCurrencyExchange
{
    /// <inheritdoc />
    public ShopPaymentResult CheckFunds(Player buyer, ShopPrice price)
    {
        return buyer.Money < price.Amount ? ShopPaymentResult.LackOfCurrency : ShopPaymentResult.Success;
    }

    /// <inheritdoc />
    public ValueTask<ShopPaymentResult> TryPayAsync(ShopPurchase purchase)
    {
        if (!purchase.Buyer.TryRemoveMoney(purchase.Price.Amount))
        {
            return ValueTask.FromResult(ShopPaymentResult.LackOfCurrency);
        }

        if (!purchase.Seller.TryAddMoney(purchase.Price.Amount))
        {
            purchase.Buyer.TryAddMoney(purchase.Price.Amount);
            return ValueTask.FromResult(ShopPaymentResult.SellerOverflow);
        }

        return ValueTask.FromResult(ShopPaymentResult.Success);
    }

    /// <inheritdoc />
    public ValueTask NotifySellerAsync(ShopPurchase purchase)
    {
        return purchase.Seller.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync());
    }

    /// <inheritdoc />
    public ValueTask NotifyBuyerAsync(ShopPurchase purchase)
    {
        return purchase.Buyer.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync());
    }
}
