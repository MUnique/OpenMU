// <copyright file="BotShoppingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Lets a bot trade with town merchants like a real player: when the backpack silts up or the potions
/// run low, the bot visits a merchant, sells its junk loot for Zen and buys potion refills with it -
/// closing the economic loop (materializing supplies out of thin air stays only as an emergency
/// fallback, see the low threshold in <see cref="BotNavigator"/>). The trade uses the regular player
/// actions (<see cref="TalkNpcAction"/>, <see cref="SellItemToNpcAction"/>, <see cref="BuyNpcItemAction"/>),
/// and while the dialog is open the player state pauses the combat AI - the bot visibly "shops".
/// </summary>
internal static class BotShoppingHandler
{
    /// <summary>Start a shopping trip when fewer free backpack slots than this remain.</summary>
    private const int FreeSlotPressure = 20;

    /// <summary>Start a shopping trip when a potion kind has fewer charges than this.</summary>
    private const int PotionLowThreshold = 40;

    /// <summary>
    /// Stop buying refills once this many charges are stocked. Merchants sell potions by the piece,
    /// so this target keeps a trip affordable (roughly a bot's income between two trips) while lasting
    /// a good while of hunting.
    /// </summary>
    private const int PotionTargetCharges = 60;

    /// <summary>Maximum purchases per potion kind per trip.</summary>
    private const int MaxPurchasesPerKind = 20;

    /// <summary>Zen the bot keeps in reserve - it stops buying rather than spend its last coin.</summary>
    private const int MinZenReserve = 10000;

    private static readonly TalkNpcAction TalkAction = new();
    private static readonly SellItemToNpcAction SellAction = new();
    private static readonly BuyNpcItemAction BuyAction = new();
    private static readonly CloseNpcDialogAction CloseAction = new();

    private static readonly ItemIdentifier[] Valuables =
    [
        ItemConstants.JewelOfChaos,
        ItemConstants.JewelOfBless,
        ItemConstants.JewelOfSoul,
        ItemConstants.JewelOfLife,
        ItemConstants.JewelOfCreation,
        ItemConstants.JewelOfGuardian,
        ItemConstants.Gemstone,
        ItemConstants.JewelOfHarmony,
        ItemConstants.LowerRefineStone,
        ItemConstants.HigherRefineStone,
    ];

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

        var freeSlots = inventory.FreeSlots.Count();
        if (freeSlots < FreeSlotPressure && inventory.Items.Any(i => IsSellableJunk(player, i)))
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
    /// Finds the position of a merchant NPC on the map, preferring one which sells potions. The
    /// search runs over the map's LIVE objects, not the spawn configuration: wandering merchants
    /// exist in the configuration but are only spawned now and then (their spawn trigger is not
    /// automatic) - a bot walking to a configured but unspawned merchant would wait at an empty
    /// spot and give up its trip, forever.
    /// </summary>
    /// <param name="map">The game map.</param>
    public static Point? FindMerchantPosition(GameMap map)
    {
        // Covers the whole 256x256 map from its center.
        var merchants = map.GetNpcsInRange(new Point(128, 128), 256)
            .Where(n => n.Definition is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore.Items.Count: > 0 })
            .ToList();
        var best = merchants.FirstOrDefault(m => SellsPotions(m.Definition.MerchantStore!)) ?? merchants.FirstOrDefault();
        return best?.Position;
    }

    /// <summary>
    /// Performs the actual trade with the merchant standing near the given position: opens the dialog,
    /// sells the junk loot, buys potion refills and closes the dialog again.
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
            var soldCount = 0;
            foreach (var junk in inventory.Items.Where(i => IsSellableJunk(player, i)).ToList())
            {
                await SellAction.SellItemAsync(player, junk.ItemSlot).ConfigureAwait(false);
                soldCount++;
            }

            var boughtCount = 0;
            var store = player.OpenedNpc.Definition.MerchantStore;
            foreach (var potionNumber in GetLowPotionKinds(player))
            {
                var storeItem = store?.Items.FirstOrDefault(i => i.Definition?.Group == 14 && i.Definition.Number == potionNumber);
                if (storeItem is null)
                {
                    continue;
                }

                for (var i = 0; i < MaxPurchasesPerKind
                     && GetPotionCharges(player, potionNumber) < PotionTargetCharges
                     && player.Money > MinZenReserve; i++)
                {
                    var moneyBefore = player.Money;
                    await BuyAction.BuyItemAsync(player, storeItem.ItemSlot).ConfigureAwait(false);
                    if (player.Money >= moneyBefore)
                    {
                        break; // purchase failed (no money / no space)
                    }

                    boughtCount++;
                }
            }

            // Logged even for a 0/0 visit: an audit must be able to tell "went and had nothing to
            // do" from a silently failed trip.
            player.Logger.LogInformation(
                "Bot '{Name}' traded with '{Merchant}': sold {Sold} item(s), bought {Bought} potion stack(s), {Money} zen left.",
                player.Name,
                merchant.Definition.Designation,
                soldCount,
                boughtCount,
                player.Money);
        }
        finally
        {
            await CloseAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Gets the potion kinds (item numbers in group 14) whose stack is running low.
    /// </summary>
    private static IEnumerable<byte> GetLowPotionKinds(Player player)
    {
        if (GetPotionCharges(player, 3) < PotionLowThreshold)
        {
            yield return 3; // Large Healing Potion
        }

        if (GetPotionCharges(player, 6) < PotionLowThreshold)
        {
            yield return 6; // Large Mana Potion
        }
    }

    private static int GetPotionCharges(Player player, byte potionNumber)
    {
        return (int)(player.Inventory?.Items
            .Where(i => i.Definition?.Group == 14 && i.Definition.Number == potionNumber)
            .Sum(i => i.Durability) ?? 0);
    }

    /// <summary>
    /// Sellable junk: unequipped gear in the backpack - not potions/ammunition and not jewels. An
    /// excellent or ancient piece is junk too unless it is an upgrade the bot is about to wear:
    /// unlike a player, a bot cannot trade its treasures away, so hoarding them only silts up the
    /// backpack until the loot pickup stops entirely.
    /// </summary>
    private static bool IsSellableJunk(OfflinePlayer player, Item item)
    {
        if (item.ItemSlot < InventoryConstants.EquippableSlotsCount
            || item.Definition is not { } definition)
        {
            return false;
        }

        if (definition.Group >= 12 || definition.IsAmmunition)
        {
            return false; // potions, jewels, scrolls, event items etc.
        }

        if (Valuables.Contains(new ItemIdentifier(definition.Number, definition.Group)))
        {
            return false;
        }

        var isTreasure = item.ItemOptions.Any(o => o.ItemOption?.OptionType == DataModel.Configuration.Items.ItemOptionTypes.Excellent)
                         || item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0);
        if (isTreasure)
        {
            return !BotEquipmentHandler.IsUpgradeFor(player, item);
        }

        return true;
    }

    private static bool SellsPotions(DataModel.Entities.ItemStorage store)
    {
        return store.Items.Any(i => i.Definition?.Group == 14 && i.Definition.Number is 3 or 6);
    }
}
