// <copyright file="BuyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to buy an item from another player shop.
/// </summary>
public class BuyRequestAction
{
    private readonly CloseStoreAction _closeStoreAction = new();

    /// <summary>
    /// Buys the item from another player shop.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="requestedPlayer">The requested player.</param>
    /// <param name="slot">The slot.</param>
    public async ValueTask BuyItemAsync(Player player, Player requestedPlayer, byte slot)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (requestedPlayer.IsTemplatePlayer || player.IsTemplatePlayer)
        {
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ItemBlock, null)).ConfigureAwait(false);
            return;
        }

        if (!(requestedPlayer.ShopStorage?.StoreOpen ?? false))
        {
            player.Logger.LogDebug("Store not open, Character {0}", requestedPlayer.SelectedCharacter?.Name);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ShopNotOpened, null)).ConfigureAwait(false);
            return;
        }

        if (slot < InventoryConstants.FirstStoreItemSlotIndex)
        {
            player.Logger.LogWarning("Store Slot too low: {0}, possible hacker", slot);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.InvalidShopSlot, null)).ConfigureAwait(false);
            return;
        }

        var item = requestedPlayer.ShopStorage.GetItem(slot);
        if (item?.GetStorePrice() is not { } price)
        {
            player.Logger.LogDebug("Item unavailable, Slot {0}", slot);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.NameMismatchOrPriceMissing, null)).ConfigureAwait(false);
            return;
        }

        // Checked here, and not only when the item is moved into the store: the store may
        // contain it since before the item rules existed.
        if (item.Definition is { IsPersonalStoreSellable: false })
        {
            player.Logger.LogWarning(
                "Player {0} tried to buy {1} from the store of {2}, which its item definition doesn't allow to sell in a personal store.",
                player,
                item,
                requestedPlayer);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemCannotBeSoldInPersonalStore)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ItemBlock, null)).ConfigureAwait(false);
            return;
        }

        if (price.Kind != ShopPriceKind.Zen && !player.SupportsShopCurrencies())
        {
            // The client can't show such items, so it should never request them.
            player.Logger.LogWarning("Player {0} tried to buy {1} with a price in {2} from the store of {3}, which its client can't show.", player, item, price.Kind, requestedPlayer);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ItemBlock, null)).ConfigureAwait(false);
            return;
        }

        if (!ShopPrices.IsAllowed(player.GameContext, price))
        {
            player.Logger.LogDebug("The price of item {0} isn't allowed.", item);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopCurrencyNotAllowed), player.GetCurrencyName(price)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ItemBlock, null)).ConfigureAwait(false);
            return;
        }

        var exchange = ShopPrices.GetExchange(price.Kind);
        var fundsResult = exchange.CheckFunds(player, price);
        if (fundsResult != ShopPaymentResult.Success)
        {
            await this.ShowPaymentFailedAsync(player, requestedPlayer, price, fundsResult).ConfigureAwait(false);
            return;
        }

        // Check Inv Space
        var freeslot = player.Inventory?.CheckInvSpace(item);
        if (freeslot is null)
        {
            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.MoneyOverflowOrNotEnoughSpace, null)).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.InventoryNotEnoughSpace)).ConfigureAwait(false);
            return;
        }

        bool itemSold = false;
        using (await requestedPlayer.ShopStorage.StoreLock.LockAsync().ConfigureAwait(false))
        {
            if (!requestedPlayer.ShopStorage.StoreOpen)
            {
                player.Logger.LogDebug("Store not open anymore, Character {0}", requestedPlayer.SelectedCharacter?.Name);
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.ShopNotOpened, null)).ConfigureAwait(false);
                return;
            }

            item = requestedPlayer.ShopStorage.GetItem(slot);
            if (item is null)
            {
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.InvalidShopSlot, null)).ConfigureAwait(false);
                return;
            }

            if (item.GetStorePrice() is not { } currentPrice || currentPrice.Amount != price.Amount || !currentPrice.HasSameCurrency(price))
            {
                // The seller can't change prices while the store is open, but the slot may contain another item by now.
                player.Logger.LogDebug("The price of the item changed, Slot {0}", slot);
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.NameMismatchOrPriceMissing, null)).ConfigureAwait(false);
                return;
            }

            player.Logger.LogDebug("BuyRequest, Item Price: {0} {1}", price.Amount, price.PriceItem?.ToString() ?? price.CoinType?.ToString() ?? "zen");
            using var itemContext = requestedPlayer.GameContext.PersistenceContextProvider.CreateNewTradeContext();
            var purchase = new ShopPurchase(player, requestedPlayer, price, itemContext);
            var paymentResult = await exchange.TryPayAsync(purchase).ConfigureAwait(false);
            if (paymentResult != ShopPaymentResult.Success)
            {
                await this.ShowPaymentFailedAsync(player, requestedPlayer, price, paymentResult).ConfigureAwait(false);
                return;
            }

            itemContext.Attach(item);
            await requestedPlayer.ShopStorage.RemoveItemAsync(item).ConfigureAwait(false);
            await exchange.NotifySellerAsync(purchase).ConfigureAwait(false);
            await requestedPlayer.InvokeViewPlugInAsync<IItemSoldByPlayerShopPlugIn>(p => p.ItemSoldByPlayerShopAsync(slot, player)).ConfigureAwait(false);
            await requestedPlayer.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(slot)).ConfigureAwait(false);
            item.ItemSlot = (byte)freeslot;
            item.SetStorePrice(null);
            await player.Inventory!.AddItemAsync(item).ConfigureAwait(false);
            requestedPlayer.PersistenceContext.Detach(item);
            foreach (var (paymentItem, _) in purchase.PaymentItems)
            {
                player.PersistenceContext.Detach(paymentItem);
            }

            await itemContext.SaveChangesAsync().ConfigureAwait(false);
            player.PersistenceContext.Attach(item);
            foreach (var (paymentItem, _) in purchase.PaymentItems)
            {
                requestedPlayer.PersistenceContext.Attach(paymentItem);
            }

            await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(requestedPlayer, ItemBuyResult.Success, item)).ConfigureAwait(false);
            await exchange.NotifyBuyerAsync(purchase).ConfigureAwait(false);
            itemSold = true;

            player.GameContext.PlugInManager.GetPlugInPoint<IItemSoldToOtherPlayerPlugIn>()?.ItemSold(requestedPlayer, item, player);
        }

        if (itemSold)
        {
            if (requestedPlayer.ShopStorage.Items.Any())
            {
                // this update may be sent to other players as well which are currently looking at the store
                await player.InvokeViewPlugInAsync<Views.PlayerShop.IShowShopItemListPlugIn>(p => p.ShowShopItemListAsync(requestedPlayer, true)).ConfigureAwait(false);
            }
            else
            {
                await this._closeStoreAction.CloseStoreAsync(requestedPlayer).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask ShowPaymentFailedAsync(Player player, Player seller, ShopPrice price, ShopPaymentResult paymentResult)
    {
        var isZen = price.Kind == ShopPriceKind.Zen;
        switch (paymentResult)
        {
            case ShopPaymentResult.LackOfCurrency when isZen:
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(seller, ItemBuyResult.LackOfMoney, null)).ConfigureAwait(false);
                break;
            case ShopPaymentResult.LackOfCurrency:
                // The client shows a message about zen for the result, so we tell the actual price.
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(seller, ItemBuyResult.LackOfPriceCurrency, null)).ConfigureAwait(false);
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopNotEnoughCurrency), price.Amount, player.GetCurrencyName(price)).ConfigureAwait(false);
                break;
            case ShopPaymentResult.ExactAmountNotPayable:
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(seller, ItemBuyResult.LackOfPriceCurrency, null)).ConfigureAwait(false);
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerShopItemPriceNotPayableExactly), price.Amount, player.GetCurrencyName(price)).ConfigureAwait(false);
                break;
            case ShopPaymentResult.SellerOverflow:
            case ShopPaymentResult.SellerInventoryFull:
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(seller, ItemBuyResult.MoneyOverflowOrNotEnoughSpace, null)).ConfigureAwait(false);
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SellerInventoryFull)).ConfigureAwait(false);
                break;
            default:
                await player.InvokeViewPlugInAsync<IPlayerShopBuyRequestResultPlugIn>(p => p.ShowResultAsync(seller, ItemBuyResult.Undefined, null)).ConfigureAwait(false);
                break;
        }
    }
}