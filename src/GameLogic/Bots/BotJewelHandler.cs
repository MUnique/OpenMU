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
    /// <summary>Upper bound of jewel consumptions per shopping trip - a player doesn't burn the whole hoard at once.</summary>
    private const int MaxUsesPerTrip = 2;

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
        var lifeUsed = false;
        while (uses < MaxUsesPerTrip && PlanNextUse(player, lifeUsed) is { } plan)
        {
            if (!await ApplyJewelAsync(player, plan.Jewel, plan.Target).ConfigureAwait(false))
            {
                break;
            }

            uses++;
            lifeUsed |= plan.IsLife;
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
    /// <param name="lifeUsed">Whether a Jewel of Life was already used this trip (at most one).</param>
    internal static (Item Jewel, Item Target, bool IsLife)? PlanNextUse(Player player, bool lifeUsed)
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
        if (blessStock.Count > 0
            && equipped.Where(i => i.CanLevelBeUpgraded() && i.Level <= BlessMaxTargetLevel)
                .OrderBy(i => i.Level)
                .FirstOrDefault() is { } blessTarget)
        {
            return (blessStock[0], blessTarget, false);
        }

        // 2. Soul - risky: only with a spare in stock, and only where the possible loss is bearable -
        // items without luck stop at +6 -> +7 (see SoulMaxTargetLevelPlain), lucky ones may go for +9.
        if (soulStock.Count >= MinSoulStock
            && equipped.Where(i => i.CanLevelBeUpgraded()
                                   && i.Level >= SoulMinTargetLevel
                                   && i.Level <= (HasLuck(i) ? SoulMaxTargetLevelLucky : SoulMaxTargetLevelPlain))
                .OrderByDescending(HasLuck)
                .ThenBy(i => i.Level)
                .FirstOrDefault() is { } soulTarget)
        {
            return (soulStock[0], soulTarget, false);
        }

        // 3. Life - sparingly: at most one per trip, only on gear that is already +6 or better. Whether
        // the item can actually carry the option is the consume handler's call; a rejected consume
        // keeps the jewel.
        if (!lifeUsed
            && lifeStock.Count >= MinLifeStock
            && equipped.Where(i => i.IsWearable() && i.Level >= LifeMinTargetLevel)
                .OrderByDescending(i => i.Level)
                .FirstOrDefault() is { } lifeTarget)
        {
            return (lifeStock[0], lifeTarget, true);
        }

        return null;
    }

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
