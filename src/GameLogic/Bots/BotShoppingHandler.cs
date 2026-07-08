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
    public static bool NeedsShopping(OfflinePlayer player)
    {
        if (player.Inventory is not { } inventory)
        {
            return false;
        }

        var freeSlots = inventory.FreeSlots.Count();
        if (freeSlots < FreeSlotPressure && inventory.Items.Any(IsSellableJunk))
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
    /// Finds the position of a merchant NPC on the map (from the map's spawn configuration; merchants
    /// are stationary), preferring one which sells potions.
    /// </summary>
    public static Point? FindMerchantPosition(GameMap map)
    {
        MonsterSpawnArea? best = null;
        foreach (var spawn in map.Definition.MonsterSpawns)
        {
            if (spawn.MonsterDefinition is not { ObjectKind: NpcObjectKind.PassiveNpc } definition
                || definition.MerchantStore is not { Items.Count: > 0 } store)
            {
                continue;
            }

            var sellsPotions = store.Items.Any(i => i.Definition?.Group == 14 && i.Definition.Number is 3 or 6);
            if (best is null || sellsPotions)
            {
                best = spawn;
                if (sellsPotions)
                {
                    break;
                }
            }
        }

        return best is null ? null : new Point(best.X1, best.Y1);
    }

    /// <summary>
    /// Performs the actual trade with the merchant standing near the given position: opens the dialog,
    /// sells the junk loot, buys potion refills and closes the dialog again.
    /// </summary>
    /// <returns>True, if a merchant was found and the trade ran; false if no merchant is there.</returns>
    public static async ValueTask<bool> TryTradeAsync(OfflinePlayer player, GameMap map, Point merchantPosition)
    {
        var merchant = map.GetNpcsInRange(merchantPosition, 4)
            .FirstOrDefault(n => n.Definition is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore.Items.Count: > 0 });
        if (merchant is null || player.Inventory is not { } inventory)
        {
            return false;
        }

        await TalkAction.TalkToNpcAsync(player, merchant).ConfigureAwait(false);
        if (player.OpenedNpc is null)
        {
            return false;
        }

        try
        {
            var soldCount = 0;
            foreach (var junk in inventory.Items.Where(IsSellableJunk).ToList())
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

                for (var i = 0; i < MaxPurchasesPerKind && GetPotionCharges(player, potionNumber) < PotionTargetCharges; i++)
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

            if (soldCount > 0 || boughtCount > 0)
            {
                player.Logger.LogInformation(
                    "Bot '{Name}' traded with '{Merchant}': sold {Sold} item(s), bought {Bought} potion stack(s), {Money} zen left.",
                    player.Name,
                    merchant.Definition.Designation,
                    soldCount,
                    boughtCount,
                    player.Money);
            }
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
    /// Sellable junk: plain unequipped gear in the backpack - not potions/ammunition, not jewels and
    /// not excellent/ancient pieces (those stay as the bot's "wealth", like a player would keep them).
    /// </summary>
    private static bool IsSellableJunk(Item item)
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

        var isExcellentOrAncient = item.ItemOptions.Any(o => o.ItemOption?.OptionType == DataModel.Configuration.Items.ItemOptionTypes.Excellent)
                                   || item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0);
        return !isExcellentOrAncient;
    }
}
