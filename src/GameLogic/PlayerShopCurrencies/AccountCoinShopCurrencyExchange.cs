// <copyright file="AccountCoinShopCurrencyExchange.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// Pays prices in account coins (WCoin C, WCoin P, Goblin Points), by transferring them from the account
/// of the buyer to the one of the seller.
/// </summary>
/// <remarks>
/// The pending coin grants of the buyer are applied first, so the balance is up to date. The account of the
/// buyer is saved right away, and the balances are restored when that fails. The account of the seller is
/// saved by its regular progress save, like the zen of a zen price: the persistence lock of the seller must
/// not be taken from the flow of the buyer (see the lock order invariant of <c>PlayerPersistence</c>).
/// </remarks>
public sealed class AccountCoinShopCurrencyExchange : IShopCurrencyExchange
{
    /// <inheritdoc />
    public ShopPaymentResult CheckFunds(Player buyer, ShopPrice price)
    {
        if (price.CoinType is not { } coinType || buyer.Account is not { } account)
        {
            return ShopPaymentResult.Failed;
        }

        return account.GetCashShopCoins(coinType) < price.Amount ? ShopPaymentResult.LackOfCurrency : ShopPaymentResult.Success;
    }

    /// <inheritdoc />
    public async ValueTask<ShopPaymentResult> TryPayAsync(ShopPurchase purchase)
    {
        if (purchase.Price.CoinType is not { } coinType
            || purchase.Buyer.Account is not { } buyerAccount
            || purchase.Seller.Account is not { } sellerAccount)
        {
            return ShopPaymentResult.Failed;
        }

        await purchase.Buyer.ApplyPendingCashShopCoinGrantsAsync().ConfigureAwait(false);

        var buyerBalance = buyerAccount.GetCashShopCoins(coinType);
        if (buyerBalance < purchase.Price.Amount)
        {
            return ShopPaymentResult.LackOfCurrency;
        }

        var sellerBalance = sellerAccount.GetCashShopCoins(coinType);
        if ((long)sellerBalance + purchase.Price.Amount > int.MaxValue)
        {
            return ShopPaymentResult.SellerOverflow;
        }

        buyerAccount.SetCashShopCoins(coinType, buyerBalance - purchase.Price.Amount);
        bool saved;
        try
        {
            saved = await purchase.Buyer.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            purchase.Buyer.Logger.LogError(ex, "Couldn't save the account of {buyer} after paying {price} {coinType} in the store of {seller}.", purchase.Buyer, purchase.Price.Amount, coinType, purchase.Seller);
            saved = false;
        }

        if (!saved)
        {
            buyerAccount.SetCashShopCoins(coinType, buyerBalance);
            return ShopPaymentResult.Failed;
        }

        // Read again, because the seller may have received coins in the meantime.
        sellerAccount.SetCashShopCoins(coinType, (int)Math.Min((long)sellerAccount.GetCashShopCoins(coinType) + purchase.Price.Amount, int.MaxValue));
        purchase.Buyer.Logger.LogInformation("{buyer} paid {price} {coinType} to {seller} for an item of the player store.", purchase.Buyer, purchase.Price.Amount, coinType, purchase.Seller);
        return ShopPaymentResult.Success;
    }

    /// <inheritdoc />
    public ValueTask NotifySellerAsync(ShopPurchase purchase)
    {
        return purchase.Seller.ShowLocalizedBlueMessageAsync(
            nameof(PlayerMessage.PlayerShopCoinsReceived),
            purchase.Price.Amount,
            purchase.Seller.GetCurrencyName(purchase.Price));
    }

    /// <inheritdoc />
    public ValueTask NotifyBuyerAsync(ShopPurchase purchase)
    {
        return purchase.Buyer.ShowLocalizedBlueMessageAsync(
            nameof(PlayerMessage.PlayerShopCoinsPaid),
            purchase.Price.Amount,
            purchase.Buyer.GetCurrencyName(purchase.Price));
    }
}
