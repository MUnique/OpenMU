// <copyright file="BotShoppingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Lets a bot trade with town merchants like a real player: when the backpack silts up or the potions
/// run low, the bot visits a merchant, sells its junk loot for Zen, repairs its gear and buys refills
/// with it - closing the economic loop (materializing supplies out of thin air stays only as an
/// emergency fallback, see the low threshold in <see cref="BotNavigator"/>). The trade uses the regular
/// player actions (<see cref="TalkNpcAction"/>, <see cref="SellItemToNpcAction"/>,
/// <see cref="BuyNpcItemAction"/>, <see cref="ItemRepairAction"/>), and while the dialog is open the
/// player state pauses the combat AI - the bot visibly "shops".
/// </summary>
internal static class BotShoppingHandler
{
    /// <summary>Start a shopping trip when fewer free backpack slots than this remain.</summary>
    private const int FreeSlotPressure = 20;

    /// <summary>Go restocking once a potion kind holds less than this share of the target stock.</summary>
    private const int PotionLowThresholdPercent = 66;

    /// <summary>Maximum purchases per potion kind per trip - a safety bound, the stock target is the real limit.</summary>
    private const int MaxPurchasesPerKind = 20;

    /// <summary>Maximum jewels bought per trip: a player buys one now and then, not a hoard at once.</summary>
    private const int MaxJewelPurchasesPerTrip = 3;

    /// <summary>Zen the bot keeps in reserve - it stops buying rather than spend its last coin.</summary>
    private const int MinZenReserve = 10000;

    /// <summary>
    /// Zen a bot keeps back before it spends anything on jewels. Jewels are a luxury next to potions and
    /// repairs, which keep the bot alive and fighting, so it only buys them out of real surplus.
    /// </summary>
    private const int JewelPurchaseReserve = 10_000_000;

    private static readonly TalkNpcAction TalkAction = new();
    private static readonly SellItemToNpcAction SellAction = new();
    private static readonly BuyNpcItemAction BuyAction = new();
    private static readonly ItemRepairAction RepairAction = new();
    private static readonly CloseNpcDialogAction CloseAction = new();
    private static readonly ItemPriceCalculator PriceCalculator = new();

    /// <summary>
    /// Determines whether the bot should go shopping: the backpack is filling up with sellable junk,
    /// or a potion stack is running low (and there is Zen to restock with).
    /// </summary>
    /// <param name="player">The bot player.</param>
    public static bool NeedsShopping(OfflinePlayer player)
    {
        if (player.Inventory is not { } inventory)
        {
            return false;
        }

        if (IsUnderSlotPressure(inventory) && GetSellableJunk(player, inventory).Count > 0)
        {
            return true;
        }

        if (player.Money > 5000 && GetLowPotionKinds(player).Any())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Finds the position of a merchant NPC on the map, preferring the one which sells what the bot
    /// needs right now. The search runs over the map's LIVE objects, not the spawn configuration:
    /// wandering merchants exist in the configuration but are only spawned now and then (their spawn
    /// trigger is not automatic) - a bot walking to a configured but unspawned merchant would wait at an
    /// empty spot and give up its trip, forever.
    /// </summary>
    /// <param name="player">The bot player, whose current needs rank the merchants.</param>
    /// <param name="map">The game map.</param>
    public static Point? FindMerchantPosition(OfflinePlayer player, GameMap map)
    {
        // Covers the whole 256x256 map from its center.
        var merchants = map.GetNpcsInRange(new Point(128, 128), 256)
            .Where(n => n.Definition is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore.Items.Count: > 0 })
            .ToList();

        var best = merchants
            .OrderByDescending(m => ScoreMerchant(player, m.Definition.MerchantStore!))
            .FirstOrDefault();
        return best?.Position;
    }

    /// <summary>
    /// Performs the actual trade with the merchant standing near the given position: opens the dialog,
    /// sells the junk loot, repairs the gear, buys refills and closes the dialog again.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="map">The game map.</param>
    /// <param name="merchantPosition">The position of the merchant.</param>
    /// <returns>True, if a merchant was found and the trade ran; false if no merchant is there.</returns>
    public static async ValueTask<bool> TryTradeAsync(OfflinePlayer player, GameMap map, Point merchantPosition)
    {
        var merchant = map.GetNpcsInRange(merchantPosition, 4)
            .FirstOrDefault(n => n.Definition is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore.Items.Count: > 0 });
        if (merchant is null || player.Inventory is not { } inventory)
        {
            // Not silent: exactly this case hid the wandering-merchant bug (a configured but
            // unspawned merchant) for a long time.
            player.Logger.LogInformation("Bot '{Name}' found no merchant near {Position} and gives up the trip.", player.Name, merchantPosition);
            return false;
        }

        await TalkAction.TalkToNpcAsync(player, merchant).ConfigureAwait(false);
        if (player.OpenedNpc is null)
        {
            player.Logger.LogInformation("Bot '{Name}' could not open the dialog of '{Merchant}'.", player.Name, merchant.Definition.Designation);
            return false;
        }

        try
        {
            var (sold, discarded) = await SellJunkAsync(player, inventory).ConfigureAwait(false);
            var repaired = await RepairGearAsync(player).ConfigureAwait(false);

            var store = player.OpenedNpc.Definition.MerchantStore;
            var boughtPotions = store is null ? 0 : await BuyPotionsAsync(player, store).ConfigureAwait(false);
            var boughtJewels = store is null ? 0 : await BuyJewelsAsync(player, store).ConfigureAwait(false);

            // Logged even for a 0/0 visit: an audit must be able to tell "went and had nothing to
            // do" from a silently failed trip. Every counter reports what REALLY happened - a sale
            // which the money limit refused is not a sale.
            player.Logger.LogInformation(
                "Bot '{Name}' traded with '{Merchant}': sold {Sold} item(s), discarded {Discarded}, repaired {Repaired}, bought {Potions} potion stack(s) and {Jewels} jewel(s), {Money} zen left.",
                player.Name,
                merchant.Definition.Designation,
                sold,
                discarded,
                repaired,
                boughtPotions,
                boughtJewels,
                player.Money);
        }
        finally
        {
            await CloseAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Collects the backpack items the bot has no use for. Not its potions, not the jewels within its
    /// working stock, and not a piece it would put on - but everything else goes, treasures included.
    /// Unlike a player, a bot cannot trade an excellent piece away, so hoarding one only silts up the
    /// backpack until the loot pickup stops entirely.
    /// </summary>
    private static List<Item> GetSellableJunk(OfflinePlayer player, IStorage inventory)
    {
        var stockLimit = BotJewelHandler.GetStockLimit(player);
        var keptJewels = new Dictionary<ItemIdentifier, int>();
        var junk = new List<Item>();

        foreach (var item in inventory.Items)
        {
            if (item.ItemSlot < InventoryConstants.EquippableSlotsCount
                || item.Definition is not { } definition
                || definition.IsAmmunition)
            {
                continue; // equipped, or an archer's arrows - selling those would disarm the bow.
            }

            var identifier = new ItemIdentifier(definition.Number, definition.Group);
            if (HealingHandler.HealthPotionPriority.Contains(identifier)
                || HealingHandler.ManaPotionPriority.Contains(identifier))
            {
                continue; // the survival kit the offline AI drinks from.
            }

            if (BotJewelHandler.UsableJewels.Contains(identifier))
            {
                // Keep the working stock and sell the surplus, counting per kind while walking the
                // backpack: asking "is the stock full?" for each jewel on its own would answer yes for
                // every one of fifteen Souls at a limit of ten, and the bot would end up with none.
                var kept = keptJewels.GetValueOrDefault(identifier);
                if (kept < stockLimit)
                {
                    keptJewels[identifier] = kept + 1;
                    continue;
                }

                junk.Add(item);
                continue;
            }

            // Whatever the bot would wear stays: selling a piece it picked up as an upgrade one tick
            // before it puts it on is pure loss.
            if (!BotEquipmentHandler.IsUpgradeFor(player, item))
            {
                junk.Add(item);
            }
        }

        return junk;
    }

    /// <summary>
    /// Sells the junk, most valuable piece first, so that a bot which is close to the money limit still
    /// captures as much of its loot as fits. What the limit refuses cannot be sold at all - and since
    /// there is no other way out of the backpack (a bot cannot trade, and dropping upgraded or excellent
    /// gear is something no player can do either), it is destroyed rather than left to wedge the bot.
    /// That only happens under slot pressure: a full wallet alone is no reason to burn loot.
    /// </summary>
    private static async ValueTask<(int Sold, int Discarded)> SellJunkAsync(OfflinePlayer player, IStorage inventory)
    {
        var maximumMoney = player.GameContext.Configuration.MaximumInventoryMoney;
        var underPressure = IsUnderSlotPressure(inventory);
        var junk = GetSellableJunk(player, inventory)
            .Select(i => (Item: i, Price: PriceCalculator.CalculateSellingPrice(i, i.Durability())))
            .OrderByDescending(x => x.Price)
            .ToList();

        var sold = 0;
        var discarded = 0;
        foreach (var (item, price) in junk)
        {
            if (await SellAction.SellItemAsync(player, item.ItemSlot).ConfigureAwait(false))
            {
                sold++;
                continue;
            }

            if (underPressure && (long)player.Money + price > maximumMoney)
            {
                await player.DestroyInventoryItemAsync(item).ConfigureAwait(false);
                discarded++;
            }
        }

        if (discarded > 0)
        {
            player.Logger.LogInformation(
                "Bot '{Name}' destroyed {Count} item(s) it could not sell: its money is at the maximum of {Maximum}.",
                player.Name,
                discarded,
                maximumMoney);
        }

        return (sold, discarded);
    }

    /// <summary>
    /// Repairs the equipped gear while the merchant dialog is open, which is what earns the NPC discount
    /// (see <see cref="ItemPriceCalculator.CalculateRepairPrice"/>). Repairing in the field without a
    /// dialog - what the MU Helper's own auto repair does, and why it stays off for bots - pays the full
    /// price instead. The cost scales with the missing durability, so repairing on every visit costs
    /// about the same as one big repair later, and never lets an item reach zero, where the price is
    /// multiplied by the destroyed-item penalty on top.
    /// </summary>
    /// <returns>The number of items which were repaired.</returns>
    private static async ValueTask<int> RepairGearAsync(OfflinePlayer player)
    {
        var damagedBefore = CountDamagedEquipment(player);
        if (damagedBefore == 0)
        {
            return 0;
        }

        await RepairAction.RepairAllItemsAsync(player).ConfigureAwait(false);
        return damagedBefore - CountDamagedEquipment(player);
    }

    /// <summary>
    /// Restocks the potions the offline AI actually drinks, buying the biggest stack the bot can afford
    /// of the best kind the merchant offers. Merchants often carry the same potion as a small and a large
    /// stack; taking the first match would buy the tiny one twenty times over.
    /// </summary>
    private static async ValueTask<int> BuyPotionsAsync(OfflinePlayer player, ItemStorage store)
    {
        var target = GetPotionStockTarget(player);
        var bought = 0;
        foreach (var priority in GetLowPotionKinds(player))
        {
            for (var i = 0; i < MaxPurchasesPerKind && GetCharges(player, priority) < target; i++)
            {
                if (FindBestOffer(player, store, priority) is not { } offer)
                {
                    break; // the merchant has none of this kind, or none the bot can afford.
                }

                var moneyBefore = player.Money;
                await BuyAction.BuyItemAsync(player, offer.ItemSlot).ConfigureAwait(false);
                if (player.Money >= moneyBefore)
                {
                    break; // purchase failed (no money / no space)
                }

                bought++;
            }
        }

        return bought;
    }

    /// <summary>
    /// Buys the jewels the bot can spend on its own gear, if the merchant happens to sell any. No stock
    /// merchant does, so on a default configuration this simply never fires - it is here for servers
    /// which put jewels into their shops, where it turns a bot's Zen into actual progress instead of
    /// letting it pile up against the money limit.
    /// </summary>
    private static async ValueTask<int> BuyJewelsAsync(OfflinePlayer player, ItemStorage store)
    {
        var bought = 0;
        var stockLimit = BotJewelHandler.GetStockLimit(player);
        foreach (var identifier in BotJewelHandler.UsableJewels)
        {
            while (bought < MaxJewelPurchasesPerTrip
                   && BotJewelHandler.CountInStock(player, identifier) < stockLimit
                   && FindAffordableOffer(player, store, identifier, JewelPurchaseReserve) is { } offer)
            {
                var moneyBefore = player.Money;
                await BuyAction.BuyItemAsync(player, offer.ItemSlot).ConfigureAwait(false);
                if (player.Money >= moneyBefore)
                {
                    break; // purchase failed (no money / no space)
                }

                bought++;
            }
        }

        return bought;
    }

    /// <summary>
    /// Gets the potion kinds whose stock is running low, as the priority lists the offline AI drinks by.
    /// The charges are counted over the whole list, not per item number: a bot with a full stack of
    /// medium healing potions is not out of healing just because it holds no large ones.
    /// </summary>
    private static IEnumerable<ItemIdentifier[]> GetLowPotionKinds(Player player)
    {
        var low = GetPotionStockTarget(player) * PotionLowThresholdPercent / 100;
        if (GetCharges(player, HealingHandler.HealthPotionPriority) < low)
        {
            yield return HealingHandler.HealthPotionPriority;
        }

        if (GetCharges(player, HealingHandler.ManaPotionPriority) < low)
        {
            yield return HealingHandler.ManaPotionPriority;
        }
    }

    private static int GetPotionStockTarget(Player player)
        => BotFeaturePlugIn.GetConfiguration(player.GameContext)?.GetEffectivePotionStockCharges() ?? 60;

    private static int GetCharges(Player player, ItemIdentifier[] kinds)
    {
        return (int)(player.Inventory?.Items
            .Where(i => i.Definition is { } definition && kinds.Contains(new ItemIdentifier(definition.Number, definition.Group)))
            .Sum(i => i.Durability) ?? 0);
    }

    /// <summary>
    /// Finds the best offer for a potion list: the highest priority kind the merchant has and the bot can
    /// afford, and of that kind the biggest stack - one purchase of a 255 charge stack beats twenty
    /// purchases of a stack of three, in Zen spent per charge as well as in backpack slots used.
    /// </summary>
    private static Item? FindBestOffer(Player player, ItemStorage store, ItemIdentifier[] priority)
    {
        foreach (var identifier in priority)
        {
            if (FindAffordableOffer(player, store, identifier, MinZenReserve) is { } offer)
            {
                return offer;
            }
        }

        return null;
    }

    private static Item? FindAffordableOffer(Player player, ItemStorage store, ItemIdentifier identifier, int reserve)
    {
        return store.Items
            .Where(i => Matches(i, identifier))
            .Where(i => PriceCalculator.CalculateFinalBuyingPrice(i) + reserve <= player.Money)
            .OrderByDescending(i => i.Durability)
            .FirstOrDefault();
    }

    /// <summary>
    /// Ranks a merchant by what the bot needs right now: the one which sells its potions wins while they
    /// run low, otherwise a jewel seller is worth the walk. Without this a bot with a full potion stock
    /// still always walked to the potion girl, past the merchant which had what it actually wanted.
    /// </summary>
    private static int ScoreMerchant(OfflinePlayer player, ItemStorage store)
    {
        var needsPotions = GetLowPotionKinds(player).Any();
        var sellsPotions = HealingHandler.HealthPotionPriority.Concat(HealingHandler.ManaPotionPriority)
            .Any(identifier => store.Items.Any(i => Matches(i, identifier)));
        var sellsJewels = BotJewelHandler.UsableJewels
            .Any(identifier => store.Items.Any(i => Matches(i, identifier)));

        var score = 0;
        if (sellsPotions)
        {
            score += needsPotions ? 2 : 1;
        }

        if (sellsJewels)
        {
            score += needsPotions ? 1 : 2;
        }

        return score;
    }

    private static bool Matches(Item item, ItemIdentifier identifier)
        => item.Definition is { } definition && identifier == new ItemIdentifier(definition.Number, definition.Group);

    private static bool IsUnderSlotPressure(IStorage inventory)
        => inventory.FreeSlots.Count() < FreeSlotPressure;

    private static int CountDamagedEquipment(Player player)
    {
        if (player.Inventory is not { } inventory)
        {
            return 0;
        }

        var damaged = 0;
        for (var slot = InventoryConstants.FirstEquippableItemSlotIndex; slot <= InventoryConstants.LastEquippableItemSlotIndex; slot++)
        {
            if (slot == InventoryConstants.PetSlot)
            {
                continue; // pets are repaired by the pet trainer, not here.
            }

            if (inventory.GetItem(slot) is { } item && item.Durability() < item.GetMaximumDurabilityOfOnePiece())
            {
                damaged++;
            }
        }

        return damaged;
    }
}
