// <copyright file="BotEquipmentHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Lets a bot progress its equipment like a real player: dropped gear is evaluated before pickup
/// (see <see cref="IsUpgradeFor"/>, used by the offline <see cref="ItemPickupHandler"/>), and looted
/// upgrades are periodically equipped; the replaced piece goes into the backpack and is sold on the next
/// shopping trip, so the backpack does not silt up with junk. All moves go through the regular
/// <see cref="MoveItemAction"/>, which enforces the same class- and stat-requirements as for a human
/// player - including which hand a weapon needs (see <see cref="ItemExtensions.ConflictsWithEquippedHands"/>).
/// </summary>
internal static class BotEquipmentHandler
{
    /// <summary>A candidate must beat the equipped piece by at least this score margin to be worth swapping.</summary>
    private const int UpgradeScoreMargin = 1;

    /// <summary>The highest item group which is a weapon (0 sword, 1 axe, 2 mace, 3 spear, 4 bow, 5 staff).</summary>
    private const byte LastWeaponGroup = 5;

    /// <summary>The item group of the shields.</summary>
    private const byte ShieldGroup = 6;

    private static readonly MoveItemAction MoveAction = new();

    /// <summary>
    /// Determines whether the dropped item would be an upgrade over the bot's currently equipped gear,
    /// so the pickup handler only collects items worth carrying.
    /// </summary>
    /// <param name="player">The bot player which would wear the item.</param>
    /// <param name="item">The dropped item to evaluate.</param>
    public static bool IsUpgradeFor(Player player, Item item)
    {
        return TryPlanSwap(player, item) is not null;
    }

    /// <summary>
    /// Scans the bot's backpack for equippable upgrades and puts the best one on; the replaced piece
    /// stays in the backpack, where the next shopping trip sells it.
    /// </summary>
    /// <param name="player">The bot player whose backpack is scanned.</param>
    public static async ValueTask TryEquipUpgradesAsync(OfflinePlayer player)
    {
        if (player.Inventory is not { } inventory)
        {
            return;
        }

        // Snapshot, because equipping mutates the item collection while we iterate.
        var backpackItems = inventory.Items
            .Where(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount)
            .ToList();

        foreach (var item in backpackItems)
        {
            if (TryPlanSwap(player, item) is not { } plan)
            {
                continue;
            }

            if (await TryApplySwapAsync(player, inventory, item, plan).ConfigureAwait(false))
            {
                // One swap per pass keeps the work per tick small; the next pass picks up the rest.
                return;
            }
        }
    }

    /// <summary>
    /// Puts the planned piece on: the gear it replaces goes to the backpack first (an equip only works
    /// into a free slot), then the candidate is equipped through the regular <see cref="MoveItemAction"/>.
    /// If the engine refuses the equip after all - the requirements are checked against the TOTAL stats,
    /// which drop as soon as the old piece with its bonuses comes off - everything moved so far is put
    /// back on. Without that rollback the bot ended up with an empty slot, re-equipped the old piece on
    /// its next pass and started over: hundreds of swaps per hour, fighting without a weapon half of the
    /// time.
    /// </summary>
    /// <returns><c>true</c> if the candidate is now equipped.</returns>
    private static async ValueTask<bool> TryApplySwapAsync(OfflinePlayer player, IStorage inventory, Item item, EquipSwap plan)
    {
        var undo = new List<(Item Item, byte EquipSlot)>(2);
        foreach (var removed in plan.Removed)
        {
            if (inventory.CheckInvSpace(removed) is not { } freeSlot)
            {
                // No room of the item's SIZE in the backpack (a 2x3 armor needs a 2x3 hole).
                await RollbackAsync(player, undo).ConfigureAwait(false);
                return false;
            }

            var equipSlot = removed.ItemSlot;
            await MoveAction.MoveItemAsync(player, equipSlot, Storages.Inventory, freeSlot, Storages.Inventory).ConfigureAwait(false);
            if (inventory.GetItem(equipSlot) is not null)
            {
                player.Logger.LogDebug("Bot '{Name}' could not take off '{Item}' for a swap.", player.Name, removed);
                await RollbackAsync(player, undo).ConfigureAwait(false);
                return false;
            }

            undo.Add((removed, equipSlot));
        }

        await MoveAction.MoveItemAsync(player, item.ItemSlot, Storages.Inventory, plan.Slot, Storages.Inventory).ConfigureAwait(false);
        if (inventory.GetItem(plan.Slot) != item)
        {
            player.Logger.LogDebug("Bot '{Name}' could not equip '{Item}' - putting its old gear back on.", player.Name, item);
            await RollbackAsync(player, undo).ConfigureAwait(false);
            return false;
        }

        player.Logger.LogInformation(
            "Bot '{Name}' equipped '{New}'{Replaced}.",
            player.Name,
            item,
            plan.Removed.Count == 0 ? string.Empty : $" (replacing {string.Join(", ", plan.Removed.Select(r => $"'{r}'"))})");

        // The outgrown gear stays in the backpack and is SOLD on the next shopping trip (see
        // BotShoppingHandler.IsSellableJunk): dropping it on the ground littered the hunting grounds
        // with the bots' hand-me-downs, which the bots then picked up again - including the bot which
        // had just dropped the piece, whose persistence context still tracked it (an entity conflict on
        // the pickup). Selling it also feeds the Zen the bot restocks its potions with.
        return true;
    }

    private static async ValueTask RollbackAsync(OfflinePlayer player, List<(Item Item, byte EquipSlot)> undo)
    {
        foreach (var (item, equipSlot) in undo)
        {
            await MoveAction.MoveItemAsync(player, item.ItemSlot, Storages.Inventory, equipSlot, Storages.Inventory).ConfigureAwait(false);
            if (player.Inventory?.GetItem(equipSlot) != item)
            {
                player.Logger.LogWarning("Bot '{Name}' could not put '{Item}' back on into slot {Slot}.", player.Name, item, equipSlot);
            }
        }
    }

    /// <summary>
    /// Plans how the item could be worn: into which slot, and which equipped pieces it would replace.
    /// Returns <c>null</c> when the bot would not (or could not) wear it - which is exactly what makes an
    /// item worth picking up from the ground, so the pickup handler asks the same question through
    /// <see cref="IsUpgradeFor"/>. Beside the piece in the target slot, a two-handed weapon also replaces
    /// whatever blocks the other hand: the engine refuses to equip it otherwise (see
    /// <see cref="ItemExtensions.ConflictsWithEquippedHands"/>), and a bot which does not plan for that
    /// keeps trying (and failing) to put it on forever.
    /// </summary>
    private static EquipSwap? TryPlanSwap(Player player, Item item)
    {
        if (item.Definition is not { } definition
            || player.SelectedCharacter?.CharacterClass is not { } characterClass
            || player.Inventory is not { } inventory
            || !IsWearableCandidate(player, definition, characterClass)
            || !player.CompliesRequirements(item))
        {
            return null;
        }

        var candidateScore = Score(item);
        EquipSwap? best = null;
        var bestReplacedScore = 0;
        foreach (var slot in GetTargetSlots(definition))
        {
            var equipped = inventory.GetItem(slot);
            if (equipped?.Definition?.IsAmmunition == true)
            {
                // Never displace the ammunition (an archer's arrows) - the bow would stop working.
                continue;
            }

            var removed = new List<Item>(2);
            if (equipped is not null)
            {
                removed.Add(equipped);
            }

            if (definition.ConflictsWithEquippedHands(inventory, slot))
            {
                // Only the main hand resolves such a conflict: taking the two-handed weapon off to fit a
                // shield into the other hand would disarm the bot.
                if (slot != InventoryConstants.LeftHandSlot
                    || inventory.GetItem(InventoryConstants.RightHandSlot) is not { } blocking)
                {
                    continue;
                }

                removed.Add(blocking);
            }

            var replacedScore = removed.Sum(Score);
            if (removed.Count > 0 && replacedScore + UpgradeScoreMargin > candidateScore)
            {
                continue;
            }

            // Prefer the cheapest swap: an empty slot beats replacing gear, and among occupied slots the
            // weakest gear goes first (this is what spreads rings over both ring slots).
            if (best is null || replacedScore < bestReplacedScore)
            {
                best = new EquipSwap(slot, removed);
                bestReplacedScore = replacedScore;
            }
        }

        return best;
    }

    /// <summary>
    /// The slots the bot considers for this piece: a weapon only goes into the main hand and a shield
    /// only into the off-hand - a bot filling its off-hand with a second (junk) weapon it happens to be
    /// qualified for is neither useful nor a sight any real character offers. Everything else may go into
    /// any of its qualified slots (rings have two).
    /// </summary>
    private static IEnumerable<byte> GetTargetSlots(ItemDefinition definition)
    {
        var slots = definition.ItemSlot!.ItemSlots.Select(s => (byte)s).ToList();
        if (definition.Group <= LastWeaponGroup && slots.Contains(InventoryConstants.LeftHandSlot))
        {
            return [InventoryConstants.LeftHandSlot];
        }

        if (definition.Group == ShieldGroup && slots.Contains(InventoryConstants.RightHandSlot))
        {
            return [InventoryConstants.RightHandSlot];
        }

        return slots;
    }

    /// <summary>
    /// Whether the item is gear this bot would wear at all: an equippable, class-qualified piece which -
    /// if it is a weapon - matches the fighting style of the bot's build (an elf only considers bows, a
    /// caster only staves), so bots don't fill their hands with random qualified junk like a Small Axe.
    /// </summary>
    private static bool IsWearableCandidate(Player player, ItemDefinition definition, CharacterClass characterClass)
    {
        if (definition.ItemSlot is not { ItemSlots.Count: > 0 }
            || definition.IsAmmunition
            || !definition.QualifiedCharacters.Contains(characterClass))
        {
            return false;
        }

        if (definition.Group > LastWeaponGroup)
        {
            return true;
        }

        var resetMeta = BotResetHandler.GetResetConfiguration(player.GameContext) is not null;
        return BotProgression.IsPreferredWeaponGroup(characterClass, player.SelectedCharacter!.Name, resetMeta, (byte)definition.Group);
    }

    /// <summary>
    /// A rough, monotonic quality score of an equippable item: the definition's drop level tracks the
    /// gear tier, the item level its upgrades, and excellent/ancient options add their extra worth.
    /// </summary>
    private static int Score(Item item)
    {
        if (item.Definition is not { } definition)
        {
            return 0;
        }

        var score = definition.DropLevel + (item.Level * 3);
        score += 12 * item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent);
        if (item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0))
        {
            score += 15;
        }

        return score;
    }

    /// <summary>
    /// A planned equip: the slot the candidate goes into, and the equipped pieces which have to come off
    /// for it (the gear in the target slot, plus the other hand's item when a two-handed weapon needs it).
    /// </summary>
    private sealed record EquipSwap(byte Slot, IReadOnlyList<Item> Removed);
}
