// <copyright file="CashShopActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.CashShop;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.GameLogic.CashShop;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The actions of a player in the cash shop.
/// </summary>
public class CashShopActions
{
    /// <summary>
    /// The storage items which a player used, by player. Their removal may not be saved yet, so they
    /// would still be loaded from the database.
    /// </summary>
    private static readonly ConditionalWeakTable<Player, HashSet<Guid>> UsedStorageItems = new();

    /// <summary>
    /// The result of a purchase, which is either a buy or a gift.
    /// </summary>
    private enum PurchaseResult
    {
        Success,
        NotEnoughCoins,
        NotForSale,
        NotGiftable,
        PackageNotFound,
        InvalidPriceOption,
        WrongCoinType,
        StorageFull,
        RefusedByPlugIn,
        SaveFailed,
    }

    /// <summary>
    /// Opens the cash shop, if the player is allowed to.
    /// </summary>
    /// <remarks>
    /// Like the client, which only allows to open it when standing in a safezone,
    /// the cash shop can't be opened outside of a safezone, while trading or while
    /// talking to a npc.
    /// </remarks>
    /// <param name="player">The player.</param>
    public async ValueTask OpenAsync(Player player)
    {
        var isAllowed = IsAllowedToUse(player);
        player.IsCashShopOpen = isAllowed;
        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowOpenResultAsync(isAllowed)).ConfigureAwait(false);
    }

    /// <summary>
    /// Closes the cash shop.
    /// </summary>
    /// <param name="player">The player.</param>
    public void Close(Player player)
    {
        player.IsCashShopOpen = false;
    }

    /// <summary>
    /// Shows the available cash shop points of the account.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowPointsAsync(Player player)
    {
        if (!IsOpenAndAllowed(player) || player.Account is not { } account)
        {
            return;
        }

        // The client requests the points whenever the cash shop is opened, so new grants show up then.
        await player.ApplyPendingCashShopCoinGrantsAsync().ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowPointsAsync(account)).ConfigureAwait(false);
    }

    /// <summary>
    /// Buys a package of the cash shop and adds its products to the storage of the account.
    /// </summary>
    /// <remarks>
    /// Only packages whose products <see cref="CashShopProductDelivery.CanDeliver">can be delivered</see> can be bought.
    /// </remarks>
    /// <param name="player">The player.</param>
    /// <param name="packageSequence">The package sequence number of the script.</param>
    /// <param name="priceSequence">The price sequence number of the chosen price option; 0, if the package has only one.</param>
    /// <param name="coinType">The coin type with which the player wants to pay; <c>null</c>, if the client sent an unknown one.</param>
    public async ValueTask BuyAsync(Player player, int packageSequence, int priceSequence, CashShopCoinType? coinType)
    {
        if (!IsOpenAndAllowed(player)
            || player.Account is not { } account
            || player.GameContext.Configuration.CashShopConfiguration is not { } configuration)
        {
            return;
        }

        var result = await PurchaseAsync(player, account, configuration, packageSequence, priceSequence, coinType, account.GetId(), null, null).ConfigureAwait(false) switch
        {
            PurchaseResult.Success => CashShopBuyResult.Success,
            PurchaseResult.NotEnoughCoins => CashShopBuyResult.NotEnoughCoins,
            PurchaseResult.NotForSale => CashShopBuyResult.NotForSale,
            PurchaseResult.PackageNotFound => CashShopBuyResult.PackageNotFound,
            PurchaseResult.InvalidPriceOption => CashShopBuyResult.InvalidPriceOption,
            PurchaseResult.WrongCoinType => CashShopBuyResult.WrongCoinType,
            PurchaseResult.StorageFull => CashShopBuyResult.StorageFull,
            PurchaseResult.RefusedByPlugIn => CashShopBuyResult.RefusedByPlugIn,
            _ => CashShopBuyResult.SaveFailed,
        };

        if (result == CashShopBuyResult.Success)
        {
            player.Logger.LogInformation("Player {player} bought the cash shop package {packageSequence} with the price option {priceSequence}.", player, packageSequence, priceSequence);
        }

        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowBuyResultAsync(result)).ConfigureAwait(false);
    }

    /// <summary>
    /// Buys a package of the cash shop as gift and adds its products to the gift storage of the account
    /// of the recipient.
    /// </summary>
    /// <remarks>
    /// The recipient may be offline or online on any game server, because the storage is loaded
    /// from the database whenever it's shown.
    /// </remarks>
    /// <param name="player">The player.</param>
    /// <param name="packageSequence">The package sequence number of the script.</param>
    /// <param name="priceSequence">The price sequence number of the chosen price option; 0, if the package has only one.</param>
    /// <param name="coinType">The coin type with which the player wants to pay; <c>null</c>, if the client sent an unknown one.</param>
    /// <param name="recipientName">The name of the character which receives the gift.</param>
    /// <param name="message">The message of the gift.</param>
    public async ValueTask GiftAsync(Player player, int packageSequence, int priceSequence, CashShopCoinType? coinType, string recipientName, string message)
    {
        if (!IsOpenAndAllowed(player)
            || player.Account is not { } account
            || player.SelectedCharacter is not { } character
            || player.GameContext.Configuration.CashShopConfiguration is not { } configuration
            || CashShopFeaturePlugIn.GetSettings(player.GameContext) is not { } settings)
        {
            return;
        }

        var recipientAccountId = string.IsNullOrWhiteSpace(recipientName)
            ? null
            : await player.RunPersistenceExclusiveAsync(
                    () => player.PersistenceContext.GetAccountIdByCharacterNameAsync(recipientName))
                .ConfigureAwait(false);
        CashShopGiftResult result;
        if (!settings.IsGiftingEnabled)
        {
            result = CashShopGiftResult.NotGiftable;
        }
        else if (recipientAccountId is not { } accountId)
        {
            result = CashShopGiftResult.RecipientNotFound;
        }
        else if (!settings.CanGiftToOwnAccount && accountId == account.GetId())
        {
            result = CashShopGiftResult.RecipientIsOwnAccount;
        }
        else
        {
            result = await PurchaseAsync(player, account, configuration, packageSequence, priceSequence, coinType, accountId, recipientName, item =>
            {
                item.IsGift = true;
                item.GiftSenderName = character.Name;
                item.GiftMessage = message;
            }).ConfigureAwait(false) switch
            {
                PurchaseResult.Success => CashShopGiftResult.Success,
                PurchaseResult.NotEnoughCoins => CashShopGiftResult.NotEnoughCoins,
                PurchaseResult.NotForSale => CashShopGiftResult.NotForSale,
                PurchaseResult.NotGiftable => CashShopGiftResult.NotGiftable,
                PurchaseResult.PackageNotFound => CashShopGiftResult.PackageNotFound,
                PurchaseResult.InvalidPriceOption => CashShopGiftResult.InvalidPriceOption,
                PurchaseResult.WrongCoinType => CashShopGiftResult.WrongCoinType,
                PurchaseResult.StorageFull => CashShopGiftResult.RecipientStorageFull,
                PurchaseResult.RefusedByPlugIn => CashShopGiftResult.RefusedByPlugIn,
                _ => CashShopGiftResult.SaveFailed,
            };
        }

        if (result == CashShopGiftResult.Success)
        {
            player.Logger.LogInformation("Player {player} sent the cash shop package {packageSequence} with the price option {priceSequence} as gift to {recipient}.", player, packageSequence, priceSequence, recipientName);
        }

        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowGiftResultAsync(result)).ConfigureAwait(false);
    }

    /// <summary>
    /// Uses an item of the cash shop storage, which delivers its product, see <see cref="CashShopProductDelivery"/>.
    /// Most products are items which are added to the inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="storageIndex">The index of the item in the storage of the account, see <see cref="ICashShopViewPlugIn.ShowStorageAsync"/>.</param>
    /// <param name="priceSequence">The price sequence number of the item, to verify that the index still refers to it.</param>
    public async ValueTask UseStorageItemAsync(Player player, int storageIndex, int priceSequence)
    {
        if (!IsOpenAndAllowed(player)
            || player.Account is not { } account
            || player.GameContext.Configuration.CashShopConfiguration is not { } configuration)
        {
            return;
        }

        ICashShopProductDeliveryPlugIn? delivery = null;
        CashShopProduct? deliveredProduct = null;
        var result = await player.RunPersistenceExclusiveAsync(async () =>
        {
            var storageItems = await GetStorageItemsAsync(player, account).ConfigureAwait(false);
            if (storageIndex < 0
                || storageIndex >= storageItems.Count
                || storageItems[storageIndex] is not { } storageItem
                || storageItem.PriceSequence != priceSequence)
            {
                return CashShopUseResult.ItemNotFound;
            }

            var product = configuration.Packages
                .SelectMany(package => package.Products)
                .FirstOrDefault(p => p.PriceSequence == storageItem.PriceSequence && p.ProductSequence == storageItem.ProductSequence);
            delivery = product is null ? null : CashShopProductDelivery.GetDelivery(player.GameContext, product);
            if (delivery is null)
            {
                return CashShopUseResult.CannotUse;
            }

            var deliveryResult = await delivery.DeliverAsync(player, product!).ConfigureAwait(false);
            if (deliveryResult != CashShopUseResult.Success)
            {
                return deliveryResult;
            }

            deliveredProduct = product;
            await player.PersistenceContext.DeleteAsync(storageItem).ConfigureAwait(false);
            UsedStorageItems.GetOrCreateValue(player).Add(storageItem.GetId());

            // The delivered product and the removal of the storage item are committed together. If this save fails,
            // they stay in the context of the player and are committed by the next save. Until then, the storage
            // item is still loaded from the database, so it's skipped as used.
            try
            {
                await player.SaveProgressAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogError(ex, "Couldn't save after player {player} used a cash shop storage item; it's saved with the next save.", player);
            }

            return CashShopUseResult.Success;
        }).ConfigureAwait(false);

        if (delivery is not null && deliveredProduct is not null)
        {
            await delivery.DeliveredAsync(player, deliveredProduct).ConfigureAwait(false);
            player.Logger.LogInformation("Player {player} used the cash shop storage item with the price option {priceSequence}.", player, priceSequence);
        }

        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowUseResultAsync(result)).ConfigureAwait(false);
    }

    /// <summary>
    /// Shows a page of the cash shop storage.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="pageNumber">The one-based number of the requested page.</param>
    /// <param name="isGiftStorage">If set to <c>true</c>, the gift storage is requested.</param>
    public async ValueTask ShowStorageAsync(Player player, int pageNumber, bool isGiftStorage)
    {
        if (!IsOpenAndAllowed(player) || player.Account is not { } account)
        {
            return;
        }

        var items = await player.RunPersistenceExclusiveAsync(
                () => GetStorageItemsAsync(player, account))
            .ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICashShopViewPlugIn>(p => p.ShowStorageAsync(items, pageNumber, isGiftStorage)).ConfigureAwait(false);
    }

    /// <summary>
    /// Buys a package: takes its price from the coins of the account and creates the storage items of its products.
    /// </summary>
    private static async ValueTask<PurchaseResult> PurchaseAsync(Player player, Account account, CashShopConfiguration configuration, int packageSequence, int priceSequence, CashShopCoinType? coinType, Guid storageAccountId, string? giftRecipientName, Action<CashShopStorageItem>? initializeGift)
    {
        CashShopPackage? boughtPackage = null;
        IReadOnlyCollection<CashShopProduct> boughtProducts = [];
        var paidPrice = 0;
        var result = await player.RunPersistenceExclusiveAsync(async () =>
        {
            var package = configuration.Packages.FirstOrDefault(p => p.PackageSequence == packageSequence);
            if (package is null)
            {
                return PurchaseResult.PackageNotFound;
            }

            if (coinType != package.CoinType)
            {
                return PurchaseResult.WrongCoinType;
            }

            if (!TryGetProducts(package, priceSequence, out var products, out var price) || price < 0)
            {
                return PurchaseResult.InvalidPriceOption;
            }

            if (!package.IsForSale || products.Any(product => !CashShopProductDelivery.CanDeliver(player.GameContext, product)))
            {
                return PurchaseResult.NotForSale;
            }

            if (initializeGift is not null && !package.IsGiftable)
            {
                return PurchaseResult.NotGiftable;
            }

            if (CashShopFeaturePlugIn.GetSettings(player.GameContext) is { MaximumStorageItems: > 0 } settings)
            {
                var storedItems = await player.PersistenceContext.GetCashShopStorageItemsAsync(storageAccountId).ConfigureAwait(false);
                if (storedItems.Count + products.Count > settings.MaximumStorageItems)
                {
                    return PurchaseResult.StorageFull;
                }
            }

            if (player.GameContext.PlugInManager.GetPlugInPoint<ICashShopPackageBuyingPlugIn>() is { } buyingPlugIns)
            {
                var eventArgs = new CancelEventArgs();
                await buyingPlugIns.PackageBuyingAsync(player, package, products, giftRecipientName, eventArgs).ConfigureAwait(false);
                if (eventArgs.Cancel)
                {
                    return PurchaseResult.RefusedByPlugIn;
                }
            }

            await player.ApplyPendingCashShopCoinGrantsAsync().ConfigureAwait(false);
            var coins = account.GetCashShopCoins(package.CoinType);
            if (coins < price)
            {
                return PurchaseResult.NotEnoughCoins;
            }

            account.SetCashShopCoins(package.CoinType, coins - price);
            var storageItems = new List<CashShopStorageItem>(products.Count);
            foreach (var product in products)
            {
                var storageItem = player.PersistenceContext.CreateNew<CashShopStorageItem>();
                storageItem.AccountId = storageAccountId;
                initializeGift?.Invoke(storageItem);
                storageItem.ProductSequence = product.ProductSequence;
                storageItem.PriceSequence = product.PriceSequence;
                storageItem.AddedAt = DateTime.UtcNow;
                storageItems.Add(storageItem);
            }

            if (await TrySaveAsync(player).ConfigureAwait(false))
            {
                boughtPackage = package;
                boughtProducts = products;
                paidPrice = price;
                return PurchaseResult.Success;
            }

            account.SetCashShopCoins(package.CoinType, coins);
            foreach (var storageItem in storageItems)
            {
                await player.PersistenceContext.DeleteAsync(storageItem).ConfigureAwait(false);
            }

            return PurchaseResult.SaveFailed;
        }).ConfigureAwait(false);

        if (boughtPackage is not null
            && player.GameContext.PlugInManager.GetPlugInPoint<ICashShopPackageBoughtPlugIn>() is { } boughtPlugIns)
        {
            await boughtPlugIns.PackageBoughtAsync(player, boughtPackage, boughtProducts, paidPrice, giftRecipientName).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Determines whether the player is allowed to open and use the cash shop: like the client, which only
    /// allows to open it when standing in a safezone, not while trading, talking to an NPC or being dead.
    /// </summary>
    private static bool IsAllowedToUse(Player player)
    {
        return player.GameContext.Configuration.CashShopConfiguration is not null
               && CashShopFeaturePlugIn.GetSettings(player.GameContext) is not null
               && player.PlayerState.CurrentState == PlayerState.EnteredWorld
               && player.IsAlive
               && player.IsAtSafezone();
    }

    /// <summary>
    /// Determines whether the player opened the cash shop and is still allowed to use it. The conditions are
    /// checked again for every action, because the player may e.g. start trading while the shop is open, or the
    /// feature may be deactivated.
    /// </summary>
    private static bool IsOpenAndAllowed(Player player) => player.IsCashShopOpen && IsAllowedToUse(player);

    /// <summary>
    /// Gets the storage items of the account, without the ones which were used but whose removal isn't saved yet.
    /// </summary>
    private static async ValueTask<IReadOnlyList<CashShopStorageItem>> GetStorageItemsAsync(Player player, Account account)
    {
        var items = await player.PersistenceContext.GetCashShopStorageItemsAsync(account.GetId()).ConfigureAwait(false);
        return UsedStorageItems.TryGetValue(player, out var used) && used.Count > 0
            ? items.Where(item => !used.Contains(item.GetId())).ToList()
            : items;
    }

    /// <summary>
    /// Saves the progress of the player. A failed save throws, which is logged here.
    /// </summary>
    private static async ValueTask<bool> TrySaveAsync(Player player)
    {
        try
        {
            return await player.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Couldn't save the cash shop purchase of player {player}.", player);
            return false;
        }
    }

    private static bool TryGetProducts(CashShopPackage package, int priceSequence, out IReadOnlyCollection<CashShopProduct> products, out int price)
    {
        if (package.IsBundle)
        {
            products = package.Products.ToList();
            price = package.Price;
            return products.Count > 0;
        }

        // The client sends 0, when the package has only one price option.
        var product = priceSequence == 0 && package.Products.Count == 1
            ? package.Products.First()
            : package.Products.FirstOrDefault(p => p.PriceSequence == priceSequence);
        products = product is null ? [] : [product];
        price = product?.Price ?? 0;
        return product is not null;
    }
}
