// <copyright file="BotJewelHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Lets a bot invest its looted jewels into its own gear like a real player would: after a shopping
/// trip (a rare, safe moment in town), it takes a piece of equipment off, applies a Jewel of Bless,
/// Soul or Life through the regular <see cref="ItemConsumeAction"/> - the same validations, success
/// rates and failure penalties as for a human - and puts the piece back on.
/// The policy mirrors common player behavior: Bless (always succeeds) pushes the weakest equipped
/// piece towards +6, whether it has luck or not; Soul (50%, +25% with luck; a failure at +6 drops
/// the item to +5, from +7 on it resets it to +0) is only risked with a spare in stock, on plain
/// items only at +6 and up to +9 only on lucky ones; Life (50%, removes the option on failure) is
/// used sparingly on already upgraded gear. Only a couple of jewels are spent per trip, so the
/// upgrades trickle in over many visits like for a player instead of the whole hoard being burned
/// at once.
/// </summary>
internal static class BotJewelHandler
{
    /// <summary>
    /// The jewels a bot has a use for: it upgrades its own gear with them (see the policy above).
    /// Everything else - Chaos, Creation, Guardian, Gemstone, Harmony, the refine stones - is only
    /// spendable by trading or crafting, which a bot does neither of.
    /// </summary>
    internal static readonly ItemIdentifier[] UsableJewels =
    [
        ItemConstants.JewelOfBless,
        ItemConstants.JewelOfSoul,
        ItemConstants.JewelOfLife,
    ];

    /// <summary>
    /// Jewels a bot may still be carrying from before but can never spend: it neither trades nor crafts.
    /// They are not picked up anymore, so this is about clearing out what is already in the backpack.
    /// </summary>
    internal static readonly ItemIdentifier[] UnusableJewels =
    [
        ItemConstants.JewelOfChaos,
        ItemConstants.JewelOfCreation,
        ItemConstants.JewelOfGuardian,
        ItemConstants.Gemstone,
        ItemConstants.JewelOfHarmony,
        ItemConstants.LowerRefineStone,
        ItemConstants.HigherRefineStone,
    ];

    /// <summary>
    /// Jewels of Bless per shopping trip. Each kind has its OWN budget on purpose: with one shared
    /// budget the Bless rule - which has a target whenever any equipped piece is below +6, and a swapped
    /// in piece arrives at the level it dropped with - consumed every use, every trip, and the Soul and
    /// Life rules below were never reached at all.
    /// </summary>
    private const int MaxBlessPerTrip = 2;

    /// <summary>Jewels of Soul per shopping trip.</summary>
    private const int MaxSoulPerTrip = 1;

    /// <summary>Jewels of Life per shopping trip.</summary>
    private const int MaxLifePerTrip = 1;

    /// <summary>Safety net for the planning loop; the per-kind budgets are the real limit.</summary>
    private const int MaxUsesPerTrip = MaxBlessPerTrip + MaxSoulPerTrip + MaxLifePerTrip;

    /// <summary>The Jewel of Bless upgrades item levels 0..5 (see <c>BlessJewelConsumeHandlerPlugIn</c>).</summary>
    private const byte BlessMaxTargetLevel = 5;

    /// <summary>Souls are never spent below +6 - that range is Bless territory (safe and cheap).</summary>
    private const byte SoulMinTargetLevel = 6;

    /// <summary>
    /// Without luck a Soul is only risked at +6, where a failure merely drops the item to +5 (a Bless
    /// restores that): from +7 on, a failed Soul resets the item to +0
    /// (<c>ResetToLevel0WhenFailMinLevel</c> in <c>SoulJewelConsumeHandlerPlugIn</c>), and at the base
    /// 50% success rate that gamble wipes gear more often than not.
    /// </summary>
    private const byte SoulMaxTargetLevelPlain = 6;

    /// <summary>
    /// With luck (+25% success) the Soul may be risked up to +8, so lucky items can reach the jewel
    /// ceiling of +9 (<c>MaximumLevel</c> in <c>SoulJewelConsumeHandlerPlugIn</c>).
    /// </summary>
    private const byte SoulMaxTargetLevelLucky = 8;

    /// <summary>Only risk a Soul with at least this many in stock - one failure must not wipe out the reserve.</summary>
    private const int MinSoulStock = 2;

    /// <summary>Life is only worth risking on gear which already proved worth upgrading.</summary>
    private const byte LifeMinTargetLevel = 6;

    /// <summary>Only risk a Life with at least this many in stock.</summary>
    private const int MinLifeStock = 2;

    /// <summary>Fallback stock per kind when the bot feature has no configuration at hand.</summary>
    private const int DefaultJewelStockPerKind = 10;

    private static readonly ItemConsumeAction ConsumeAction = new();
    private static readonly MoveItemAction MoveAction = new();

    /// <summary>
    /// Spends up to <see cref="MaxUsesPerTrip"/> looted jewels on the bot's own equipment. Call this
    /// right after a finished merchant trade: the bot stands in the safezone, the NPC dialog is closed
    /// (player state is back at <c>EnteredWorld</c>, which the consume handlers require), and the
    /// navigator's shopping cooldown provides the rare, player-like cadence.
    /// </summary>
    /// <param name="player">The bot player.</param>
    public static async ValueTask TryUpgradeGearAsync(OfflinePlayer player)
    {
        if (player.Inventory is null)
        {
            return;
        }

        var uses = 0;
        var blessLeft = MaxBlessPerTrip;
        var soulLeft = MaxSoulPerTrip;
        var lifeLeft = MaxLifePerTrip;
        while (uses < MaxUsesPerTrip && PlanNextUse(player, blessLeft, soulLeft, lifeLeft) is { } plan)
        {
            if (!await ApplyJewelAsync(player, plan.Jewel, plan.Target).ConfigureAwait(false))
            {
                break;
            }

            uses++;
            if (IsJewel(plan.Jewel, ItemConstants.JewelOfBless))
            {
                blessLeft--;
            }
            else if (IsJewel(plan.Jewel, ItemConstants.JewelOfSoul))
            {
                soulLeft--;
            }
            else
            {
                lifeLeft--;
            }
        }

        if (uses > 0)
        {
            try
            {
                // Persist right away like after a bot reset - a rolled Soul/Life outcome shouldn't be
                // replayable by losing it to a crash before the next periodic save.
                await player.SaveProgressAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogWarning(ex, "Couldn't save bot '{Name}' right after using jewels; the periodic save will retry.", player.Name);
            }
        }
    }

    /// <summary>
    /// Picks the next (jewel, equipped target item) pair according to the player-like policy, or
    /// <c>null</c> when nothing sensible is left to do. Pure decision logic - exposed for unit tests.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="blessLeft">How many Jewels of Bless may still be used this trip.</param>
    /// <param name="soulLeft">How many Jewels of Soul may still be used this trip.</param>
    /// <param name="lifeLeft">How many Jewels of Life may still be used this trip.</param>
    internal static (Item Jewel, Item Target)? PlanNextUse(Player player, int blessLeft, int soulLeft, int lifeLeft)
    {
        if (player.Inventory is not { } inventory)
        {
            return null;
        }

        var backpack = inventory.Items.Where(i => i.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex).ToList();
        var equipped = inventory.Items
            .Where(i => i.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex
                        && i.Definition?.IsAmmunition == false)
            .ToList();

        var blessStock = backpack.Where(i => IsJewel(i, ItemConstants.JewelOfBless)).ToList();
        var soulStock = backpack.Where(i => IsJewel(i, ItemConstants.JewelOfSoul)).ToList();
        var lifeStock = backpack.Where(i => IsJewel(i, ItemConstants.JewelOfLife)).ToList();

        // 1. Bless - free progress: push the weakest equipped piece towards +6.
        if (blessLeft > 0
            && blessStock.Count > 0
            && equipped.Where(i => i.CanLevelBeUpgraded() && i.Level <= BlessMaxTargetLevel)
                .OrderBy(i => i.Level)
                .FirstOrDefault() is { } blessTarget)
        {
            return (blessStock[0], blessTarget);
        }

        // 2. Soul - risky: only with a spare in stock, and only where the possible loss is bearable -
        // items without luck stop at +6 -> +7 (see SoulMaxTargetLevelPlain), lucky ones may go for +9.
        if (soulLeft > 0
            && soulStock.Count >= MinSoulStock
            && equipped.Where(i => i.CanLevelBeUpgraded()
                                   && i.Level >= SoulMinTargetLevel
                                   && i.Level <= (HasLuck(i) ? SoulMaxTargetLevelLucky : SoulMaxTargetLevelPlain))
                .OrderByDescending(HasLuck)
                .ThenBy(i => i.Level)
                .FirstOrDefault() is { } soulTarget)
        {
            return (soulStock[0], soulTarget);
        }

        // 3. Life - sparingly: at most one per trip, only on gear that is already +6 or better. Whether
        // the item can actually carry the option is the consume handler's call; a rejected consume
        // keeps the jewel.
        if (lifeLeft > 0
            && lifeStock.Count >= MinLifeStock
            && equipped.Where(i => i.IsWearable() && i.Level >= LifeMinTargetLevel)
                .OrderByDescending(i => i.Level)
                .FirstOrDefault() is { } lifeTarget)
        {
            return (lifeStock[0], lifeTarget);
        }

        return null;
    }

    /// <summary>
    /// Whether the bot carries jewels it should get rid of at a merchant: any it cannot use at all, or
    /// more of a usable kind than its stock limit. Deliberately cheap - the trip planner asks this on
    /// every check, so unlike the full junk scan it must not plan equipment swaps.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <returns><c>True</c>, if there is jewel surplus to sell.</returns>
    internal static bool HasSurplus(Player player)
    {
        if (player.Inventory is not { } inventory)
        {
            return false;
        }

        var limit = GetStockLimit(player);
        var counts = new Dictionary<ItemIdentifier, int>();
        foreach (var item in inventory.Items)
        {
            if (item.ItemSlot < InventoryConstants.EquippableSlotsCount
                || item.Definition is not { } definition)
            {
                continue;
            }

            var identifier = new ItemIdentifier(definition.Number, definition.Group);
            if (UnusableJewels.Contains(identifier))
            {
                return true;
            }

            if (!UsableJewels.Contains(identifier))
            {
                continue;
            }

            var count = counts.GetValueOrDefault(identifier) + 1;
            if (count > limit)
            {
                return true;
            }

            counts[identifier] = count;
        }

        return false;
    }

    /// <summary>
    /// Whether the bot has a jewel it could spend on its gear right now. Jewels are only used at the
    /// end of a merchant trip - the rare, safe, player-like moment for it - so the trip planner asks
    /// this to decide whether a visit is worth making at all.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <returns><c>True</c>, if an upgrade is pending.</returns>
    internal static bool HasPendingUpgrade(Player player)
        => PlanNextUse(player, MaxBlessPerTrip, MaxSoulPerTrip, MaxLifePerTrip) is not null;

    /// <summary>
    /// Whether the bot still has room in its stock for this jewel - the pickup handler asks before
    /// taking one from the ground, and the merchant trade asks to tell a working stock from surplus.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="item">The jewel to judge.</param>
    /// <returns><c>True</c>, if it is a usable jewel and the stock is not full yet.</returns>
    internal static bool WantsMoreOf(Player player, Item item)
    {
        if (item.Definition is not { } definition)
        {
            return false;
        }

        var identifier = new ItemIdentifier(definition.Number, definition.Group);
        return UsableJewels.Contains(identifier)
               && CountInStock(player, identifier) < GetStockLimit(player);
    }

    /// <summary>
    /// Gets the configured number of jewels a bot keeps of each usable kind.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <returns>The stock limit per kind.</returns>
    internal static int GetStockLimit(Player player)
        => BotFeaturePlugIn.GetConfiguration(player.GameContext)?.GetEffectiveJewelStockPerKind()
           ?? DefaultJewelStockPerKind;

    /// <summary>
    /// Counts how many jewels of the given kind the bot carries.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="identifier">The jewel kind.</param>
    /// <returns>The number of jewels of that kind in the inventory.</returns>
    internal static int CountInStock(Player player, ItemIdentifier identifier)
        => player.Inventory?.Items.Count(i => IsJewel(i, identifier)) ?? 0;

    /// <summary>
    /// Applies one jewel to one equipped item the way a player does it: take the piece off into a free
    /// backpack slot (the consume handlers refuse to modify equipped items), consume the jewel on it
    /// through the regular action, and wear the piece again - whatever the outcome, even a Soul
    /// failure's downgraded item goes back on.
    /// </summary>
    /// <returns><c>true</c>, if the jewel was actually consumed.</returns>
    private static async ValueTask<bool> ApplyJewelAsync(OfflinePlayer player, Item jewel, Item target)
    {
        var inventory = player.Inventory!;
        var equipSlot = target.ItemSlot;

        // A free slot is not enough - the piece needs a hole of its SIZE (a 2x3 armor does not fit into
        // the 1x1 gap a jewel left behind), which is exactly what CheckInvSpace answers.
        if (inventory.CheckInvSpace(target) is not { } freeSlot
            || freeSlot <= InventoryConstants.LastEquippableItemSlotIndex)
        {
            return false;
        }

        await MoveAction.MoveItemAsync(player, equipSlot, Storages.Inventory, freeSlot, Storages.Inventory).ConfigureAwait(false);
        if (inventory.GetItem(freeSlot) != target)
        {
            // The unequip was rejected - don't force it.
            return false;
        }

        var jewelSlot = jewel.ItemSlot;
        var levelBefore = target.Level;
        try
        {
            await ConsumeAction.HandleConsumeRequestAsync(player, jewelSlot, freeSlot, FruitUsage.Undefined).ConfigureAwait(false);
        }
        finally
        {
            // Wear the piece again in any case; the move action re-checks the requirements itself.
            await MoveAction.MoveItemAsync(player, freeSlot, Storages.Inventory, equipSlot, Storages.Inventory).ConfigureAwait(false);
        }

        var consumed = inventory.GetItem(jewelSlot) != jewel;
        if (consumed)
        {
            player.Logger.LogInformation(
                "Bot '{Name}' used '{Jewel}' on '{Item}': level {Before} -> {After}.",
                player.Name,
                jewel.Definition?.Name,
                target,
                levelBefore,
                target.Level);
        }

        return consumed;
    }

    private static bool IsJewel(Item item, ItemIdentifier identifier)
    {
        return item.Definition is { } definition
               && identifier == new ItemIdentifier(definition.Number, definition.Group);
    }

    private static bool HasLuck(Item item)
    {
        return item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck);
    }
}
