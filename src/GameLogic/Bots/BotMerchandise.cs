// <copyright file="BotMerchandise.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.GameLogic.Views.PlayerShop;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Decides which loot of a bot is worth a stall on the market and what it costs, and puts it into the
/// personal store of the bot instead of selling it to an NPC (see <see cref="BotMerchantPlugIn"/>).
/// </summary>
/// <remarks>
/// The goods come from what the bot would otherwise sell to a merchant NPC: the loot it doesn't wear,
/// the gear it outgrew and the jewels above the stock it keeps for its own gear. A price class (<see cref="PriceClass"/>) follows from the excellent options,
/// luck and ancient set of the item, the price from the class and the settings of the
/// <see cref="BotMerchantConfiguration"/>.
/// </remarks>
internal static class BotMerchandise
{
    /// <summary>
    /// The item level up to which the price has no level surcharge: a bot upgrades items without
    /// luck up to +7, so anything above has been risked with luck.
    /// </summary>
    internal const int PriceBaseItemLevel = 7;

    private static readonly MoveItemAction MoveAction = new();

    private static readonly SetItemPriceAction PriceAction = new();

    /// <summary>The game's own action to close a personal store, which tells the players around.</summary>
    private static readonly CloseStoreAction StoreCloseAction = new();

    /// <summary>
    /// The excellent options which the players look for: on armor the damage decrease, defense success
    /// rate and damage reflection, on weapons the excellent damage rate, the damage increases and the
    /// attack speed. The others - more life, mana or zen - are cheap.
    /// </summary>
    private static readonly HashSet<AttributeDefinition> WantedExcellentAttributes =
    [
        Stats.ArmorDamageDecrease,
        Stats.DefenseRatePvm,
        Stats.DamageReflection,
        Stats.ExcellentDamageChance,
        Stats.PhysicalBaseDmgIncrease,
        Stats.WizardryBaseDmgIncrease,
        Stats.PhysicalBaseDmg,
        Stats.WizardryBaseDmg,
        Stats.AttackSpeedAny,
    ];

    /// <summary>
    /// The price class of a good.
    /// </summary>
    internal enum PriceClass
    {
        /// <summary>Not worth a stall: sold to an NPC.</summary>
        None,

        /// <summary>A normal item of a higher set with luck, or an excellent item with only a cheap option and luck.</summary>
        Cheap,

        /// <summary>An excellent item with one wanted option, without luck.</summary>
        Medium,

        /// <summary>An ancient item, an excellent item with two or more options, or with one wanted option and luck.</summary>
        Expensive,

        /// <summary>A jewel of the surplus which the bot doesn't keep for its own gear.</summary>
        Jewel,
    }

    /// <summary>
    /// Determines the price class of an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The price class; <see cref="PriceClass.None"/>, if the item isn't worth a stall.</returns>
    internal static PriceClass Classify(Item item, BotMerchantConfiguration configuration)
    {
        if (item.Definition is not { } definition
            || !definition.IsPersonalStoreSellable
            || HasOption(item, ItemOptionTypes.HarmonyOption))
        {
            // The game doesn't open a store with such an item.
            return PriceClass.None;
        }

        if (GetJewelPrice(item, configuration) is not null)
        {
            return PriceClass.Jewel;
        }

        if (item.IsAncient())
        {
            return PriceClass.Expensive;
        }

        var luck = HasOption(item, ItemOptionTypes.Luck);
        var excellentOptions = GetExcellentAttributes(item).ToList();
        if (excellentOptions.Count >= 2)
        {
            return PriceClass.Expensive;
        }

        if (excellentOptions.Count == 1)
        {
            return WantedExcellentAttributes.Contains(excellentOptions[0])
                ? luck ? PriceClass.Expensive : PriceClass.Medium
                : luck ? PriceClass.Cheap : PriceClass.None;
        }

        return luck
               && definition.DropLevel >= configuration.HighSetMinimumDropLevel
               && item.Level >= configuration.MinimumNormalItemLevel
            ? PriceClass.Cheap
            : PriceClass.None;
    }

    /// <summary>
    /// Draws a zen price within the spread around the calculated one and rounds it the way a player would:
    /// to 1,000 zen below 100,000, to 50,000 below a million, to 100,000 below ten million, and to whole
    /// millions above.
    /// </summary>
    /// <param name="amount">The calculated price.</param>
    /// <param name="spreadPercent">How far, in percent up or down, the price may differ; 0 keeps it exact.</param>
    /// <param name="random">A random number from 0 (the lowest price) to 1 (the highest).</param>
    /// <returns>The price for the store.</returns>
    internal static int SpreadZenPrice(int amount, int spreadPercent, double random)
    {
        if (spreadPercent <= 0)
        {
            return amount;
        }

        var drawn = amount * (1 + (spreadPercent / 100.0 * ((Math.Clamp(random, 0, 1) * 2) - 1)));
        double step = drawn switch
        {
            < 1_000 => 1,
            < 100_000 => 1_000,
            < 1_000_000 => 50_000,
            < 10_000_000 => 100_000,
            _ => 1_000_000,
        };

        var rounded = Math.Round(drawn / step, MidpointRounding.AwayFromZero) * step;
        return (int)Math.Clamp(rounded, 1, int.MaxValue);
    }

    /// <summary>
    /// Calculates the price of a good.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="priceClass">The price class of the item.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The currency and the amount; <c>null</c> for <see cref="PriceClass.None"/>.</returns>
    internal static (string Currency, int Amount)? CalculatePrice(Item item, PriceClass priceClass, BotMerchantConfiguration configuration)
    {
        var levelSteps = Math.Max(0, item.Level - PriceBaseItemLevel);
        var optionSteps = GetOptionLevel(item);
        switch (priceClass)
        {
            case PriceClass.Cheap:
                return (configuration.CheapCurrency, Sum(configuration.CheapPrice, configuration.CheapPricePerLevel * (long)levelSteps, configuration.CheapPricePerOption * optionSteps));
            case PriceClass.Medium:
                return (configuration.MediumCurrency, Sum(configuration.MediumPrice, configuration.MediumPricePerLevel * (long)levelSteps, configuration.MediumPricePerOption * optionSteps));
            case PriceClass.Expensive:
                var excellentOptions = GetExcellentAttributes(item).ToList();
                var isAncient = item.IsAncient();
                var luck = HasOption(item, ItemOptionTypes.Luck);
                long basePrice = excellentOptions.Count switch
                {
                    >= 3 => configuration.ExpensiveThreeOptionsPrice,
                    2 => configuration.ExpensivePrice,
                    _ => isAncient ? configuration.ExpensivePrice : configuration.ExpensiveOneOptionPrice,
                };

                if (isAncient && luck)
                {
                    basePrice += configuration.AncientLuckSurcharge;
                }

                var hasHighSetSurcharge = isAncient
                    || (luck && optionSteps > 0 && excellentOptions.Any(WantedExcellentAttributes.Contains));
                if (hasHighSetSurcharge && item.Definition is { } definition)
                {
                    var steps = Math.Max(0, definition.DropLevel - configuration.HighSetMinimumDropLevel) / Math.Max(1, configuration.HighSetSurchargeDropLevels);
                    basePrice += (long)steps * configuration.HighSetSurcharge;
                }

                return (configuration.ExpensiveCurrency, Sum(basePrice, configuration.ExpensivePricePerLevel * (long)levelSteps, configuration.ExpensivePricePerOption * optionSteps));
            case PriceClass.Jewel:
                return GetJewelPrice(item, configuration);
            default:
                return null;
        }
    }

    /// <summary>
    /// Puts the goods among the junk of the bot into its personal store, each with its price, instead of
    /// selling them to the merchant NPC.
    /// </summary>
    /// <param name="player">The bot.</param>
    /// <param name="junk">The items which the bot would sell to the merchant NPC.</param>
    /// <returns>The number of items which were put into the store.</returns>
    /// <remarks>
    /// Nothing is stocked when the plugin is off, when the store is open or full, or when the price
    /// isn't allowed, e.g. a price in jewels without the player shop currencies plugin - such goods are
    /// sold to the NPC as before.
    /// </remarks>
    internal static async ValueTask<int> StockStoreAsync(OfflinePlayer player, IReadOnlyCollection<Item> junk)
    {
        if (player.ShopStorage is not { StoreOpen: false } shopStorage)
        {
            return 0;
        }

        if (player.GameContext.FeaturePlugIns.GetPlugIn<BotMerchantPlugIn>() is not { Configuration: { } configuration } plugIn)
        {
            // Switched off: the goods of earlier shifts must not stay locked away in the store.
            await TakeOutOfStoreAsync(player, shopStorage.Items.ToList()).ConfigureAwait(false);
            return 0;
        }

        if (player.Level < Math.Max(configuration.MinimumLevel, BotMarketPlanner.MinimumStoreLevel))
        {
            return 0;
        }

        var stocked = 0;
        foreach (var item in junk)
        {
            var priceClass = Classify(item, configuration);
            if (plugIn.IsRetired(item, configuration)
                || CalculatePrice(item, priceClass, configuration) is not { } offer
                || !TryCreatePrice(player.GameContext, offer.Currency, offer.Amount, out var price)
                || (priceClass == PriceClass.Jewel && !MayStockJewel(shopStorage, item, price, configuration))
                || shopStorage.CheckInvSpace(item) is not { } storeSlot)
            {
                continue;
            }

            if (price.Kind == ShopPriceKind.Zen)
            {
                var spread = SpreadZenPrice(price.Amount, configuration.PriceSpreadPercent, Rand.NextDouble());
                price = price with { Amount = Math.Min(spread, ShopPrices.GetMaximumAmount(player.GameContext, ShopPriceKind.Zen)) };
            }

            var fromSlot = item.ItemSlot;
            if (await PriceAction.TrySetPriceAsync(player, fromSlot, price).ConfigureAwait(false) != ItemPriceResult.Success)
            {
                continue;
            }

            await MoveAction.MoveItemAsync(player, fromSlot, Storages.Inventory, storeSlot, Storages.PersonalStore).ConfigureAwait(false);
            if (shopStorage.GetItem(storeSlot) == item)
            {
                stocked++;
            }
            else
            {
                // The move was refused; the item stays junk and must not keep a price.
                item.SetStorePrice(null);
            }
        }

        if (stocked > 0)
        {
            player.Logger.LogDebug("Bot '{Name}' put {Count} item(s) into its store for the market.", player.Name, stocked);
        }

        return stocked;
    }

    /// <summary>
    /// Closes the store of the market shift, removes the last income and retires the goods which found
    /// no buyer for too long.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="shift">The market shift.</param>
    /// <remarks>
    /// Runs in the tick of the bot (see <see cref="OfflinePlayer.PendingBotActions"/>), or under its
    /// persistence lock when it logs out (see <see cref="EndShiftOnLogoutAsync"/>). The store is closed
    /// first: closing takes the store lock of the purchases, so no sale can add income afterward.
    /// </remarks>
    /// <returns>The task.</returns>
    internal static async ValueTask CloseShiftStoreAsync(OfflinePlayer bot, BotMarketShift shift)
    {
        if (bot.ShopStorage?.StoreOpen ?? false)
        {
            await StoreCloseAction.CloseStoreAsync(bot).ConfigureAwait(false);
        }

        if (shift.IsStoreOpened)
        {
            await RemoveIncomeAsync(bot, shift).ConfigureAwait(false);
            await RetireUnsoldGoodsAsync(bot).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Ends the market shift of a bot which logs out: closes its store and removes the income of the
    /// sales whose removal is still queued for a tick which won't come anymore.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <remarks>
    /// Runs before the logout saves the bot - otherwise the income of the last sales would be saved and
    /// stay with the bot. The shift lives in memory only, so after the next login nothing could tell
    /// the income from the own money anymore.
    /// </remarks>
    /// <returns>The task.</returns>
    internal static async ValueTask EndShiftOnLogoutAsync(OfflinePlayer bot)
    {
        if (bot.MarketShift is not { } shift)
        {
            return;
        }

        shift.IsEnding = true;
        await CloseShiftStoreAsync(bot, shift).ConfigureAwait(false);
        bot.MarketShift = null;
    }

    /// <summary>
    /// Removes the income of the shift: the zen above what the bot had when it opened its store, and the
    /// items which appeared in its inventory since then - the jewels paid for its goods. The bot buys
    /// nothing, and the jewels would fill its inventory until it can't take any payment anymore.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="shift">The market shift.</param>
    /// <remarks>Runs in the tick of the bot (see <see cref="OfflinePlayer.PendingBotActions"/>) or before its logout (see <see cref="EndShiftOnLogoutAsync"/>).</remarks>
    /// <returns>The task.</returns>
    internal static async ValueTask RemoveIncomeAsync(Player bot, BotMarketShift shift)
    {
        if (!shift.IsStoreOpened || bot.Inventory is not { } inventory)
        {
            return;
        }

        var money = Math.Max(0, bot.Money - shift.MoneyAtOpen);
        if (money > 0)
        {
            bot.Money = shift.MoneyAtOpen;
        }

        var income = inventory.Items.Where(item => !shift.ItemsAtOpen.Contains(item)).ToList();
        foreach (var item in income)
        {
            await bot.DestroyInventoryItemAsync(item).ConfigureAwait(false);
        }

        if (money > 0 || income.Count > 0)
        {
            bot.Logger.LogDebug("Bot '{Name}' removed its income of {Money} zen and {Count} item(s).", bot.Name, money, income.Count);
        }
    }

    /// <summary>
    /// After a market shift, counts one more shift without a buyer for each good left in the store,
    /// and takes the goods which reached <see cref="BotMerchantConfiguration.UnsoldShiftsUntilNpc"/> out
    /// of it - the next visit at a merchant NPC sells them, making room for fresh goods.
    /// </summary>
    /// <param name="bot">The bot, with its store closed.</param>
    /// <returns>The task.</returns>
    internal static async ValueTask RetireUnsoldGoodsAsync(OfflinePlayer bot)
    {
        if (bot.ShopStorage is not { StoreOpen: false } shopStorage
            || bot.GameContext.FeaturePlugIns.GetPlugIn<BotMerchantPlugIn>() is not { Configuration: { } configuration } plugIn)
        {
            return;
        }

        var retired = shopStorage.Items
            .Where(item => plugIn.CountUnsoldShift(item) >= configuration.UnsoldShiftsUntilNpc)
            .ToList();
        await TakeOutOfStoreAsync(bot, retired).ConfigureAwait(false);
    }

    /// <summary>
    /// Takes goods out of the store into the inventory, without their price: they become junk which the
    /// bot sells to a merchant NPC. A good which doesn't fit into the inventory stays in the store for now.
    /// </summary>
    private static async ValueTask TakeOutOfStoreAsync(OfflinePlayer bot, IReadOnlyCollection<Item> goods)
    {
        if (goods.Count == 0 || bot.Inventory is not { } inventory)
        {
            return;
        }

        var takenOut = 0;
        foreach (var item in goods)
        {
            if (inventory.CheckInvSpace(item) is not { } slot)
            {
                continue;
            }

            var fromSlot = item.ItemSlot;
            await MoveAction.MoveItemAsync(bot, fromSlot, Storages.PersonalStore, slot, Storages.Inventory).ConfigureAwait(false);
            if (inventory.GetItem(slot) == item)
            {
                item.SetStorePrice(null);
                takenOut++;
            }
        }

        if (takenOut > 0)
        {
            bot.Logger.LogDebug("Bot '{Name}' took {Count} unsold item(s) out of its store.", bot.Name, takenOut);
        }
    }

    /// <summary>
    /// Creates the price from the configured currency: zen or an item, e.g. a jewel, if the game allows it
    /// in player shops. Account coins are no currency of the bots - they are usually bought with real money.
    /// </summary>
    private static bool TryCreatePrice(IGameContext gameContext, string currency, int amount, out ShopPrice price)
    {
        if (!ShopPrices.TryParse(gameContext, currency, amount, out price)
            || price.Kind == ShopPriceKind.AccountCoin
            || !ShopPrices.IsAllowed(gameContext, price))
        {
            return false;
        }

        price = price with { Amount = Math.Min(price.Amount, ShopPrices.GetMaximumAmount(gameContext, price.Kind)) };
        return true;
    }

    /// <summary>
    /// Gets the configured price of a jewel; <c>null</c>, if the item is no jewel which the bots sell.
    /// </summary>
    private static (string Currency, int Amount)? GetJewelPrice(Item item, BotMerchantConfiguration configuration)
    {
        if (item.Definition is not { } definition)
        {
            return null;
        }

        var identifier = new ItemIdentifier(definition.Number, definition.Group);
        return identifier switch
        {
            _ when identifier == ItemConstants.JewelOfBless => (configuration.BlessCurrency, configuration.BlessPrice),
            _ when identifier == ItemConstants.JewelOfSoul => (configuration.SoulCurrency, configuration.SoulPrice),
            _ when identifier == ItemConstants.JewelOfChaos => (configuration.ChaosCurrency, configuration.ChaosPrice),
            _ when identifier == ItemConstants.JewelOfLife => (configuration.LifeCurrency, configuration.LifePrice),
            _ when identifier == ItemConstants.JewelOfCreation => (configuration.CreationCurrency, configuration.CreationPrice),
            _ => null,
        };
    }

    /// <summary>
    /// Determines whether the jewel may go into the store: the store doesn't hold the maximum of its kind
    /// yet, and its price isn't paid with the same jewel - a Bless for a Bless is no trade.
    /// </summary>
    private static bool MayStockJewel(IStorage shopStorage, Item jewel, ShopPrice price, BotMerchantConfiguration configuration)
    {
        if (ShopPrice.IsSameItem(price.PriceItem, jewel.Definition))
        {
            return false;
        }

        var stockedOfKind = shopStorage.Items.Count(i => ShopPrice.IsSameItem(i.Definition, jewel.Definition));
        return stockedOfKind < configuration.MaximumJewelsPerKind;
    }

    private static int Sum(long basePrice, long levelSurcharge, double optionSurcharge)
        => (int)Math.Clamp(basePrice + levelSurcharge + (long)Math.Floor(optionSurcharge), 1, int.MaxValue);

    private static bool HasOption(Item item, ItemOptionType optionType)
        => item.ItemOptions.Any(link => link.ItemOption?.OptionType == optionType);

    /// <summary>
    /// Gets the level of the additional option: 1 for +4, up to 4 for +16.
    /// </summary>
    private static int GetOptionLevel(Item item)
        => item.ItemOptions.Where(link => link.ItemOption?.OptionType == ItemOptionTypes.Option).Select(link => link.Level).DefaultIfEmpty(0).Max();

    private static IEnumerable<AttributeDefinition> GetExcellentAttributes(Item item)
        => item.ItemOptions
            .Where(link => link.ItemOption?.OptionType == ItemOptionTypes.Excellent)
            .Select(link => link.ItemOption!.PowerUpDefinition?.TargetAttribute)
            .OfType<AttributeDefinition>();
}
