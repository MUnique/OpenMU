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
/// upgrades are periodically equipped, with the replaced piece dropped to the ground so the backpack
/// does not silt up with junk. All moves go through the regular <see cref="MoveItemAction"/>, which
/// enforces the same class- and stat-requirements as for a human player.
/// </summary>
internal static class BotEquipmentHandler
{
    /// <summary>A candidate must beat the equipped piece by at least this score margin to be worth swapping.</summary>
    private const int UpgradeScoreMargin = 1;

    private static readonly MoveItemAction MoveAction = new();
    private static readonly DropItemAction DropAction = new();

    /// <summary>
    /// Determines whether the dropped item would be an upgrade over the bot's currently equipped gear,
    /// so the pickup handler only collects items worth carrying.
    /// </summary>
    /// <param name="player">The bot player which would wear the item.</param>
    /// <param name="item">The dropped item to evaluate.</param>
    public static bool IsUpgradeFor(Player player, Item item)
    {
        if (item.Definition is not { } definition
            || player.SelectedCharacter?.CharacterClass is not { } characterClass
            || player.Inventory is not { } inventory)
        {
            return false;
        }

        if (!IsWearableCandidate(definition, characterClass)
            || !player.CompliesRequirements(item))
        {
            return false;
        }

        var candidateScore = Score(item);
        foreach (var slot in definition.ItemSlot!.ItemSlots)
        {
            var equipped = inventory.GetItem((byte)slot);
            if (equipped?.Definition?.IsAmmunition == true)
            {
                // Never displace the ammunition (an archer's arrows) - the bow would stop working.
                continue;
            }

            if (equipped is null || Score(equipped) + UpgradeScoreMargin <= candidateScore)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Scans the bot's backpack for equippable upgrades and puts the best one on; the replaced piece is
    /// dropped to the ground (a real player would leave the outgrown junk behind too), unless it is
    /// something valuable (excellent/ancient), which stays in the backpack.
    /// </summary>
    /// <param name="player">The bot player whose backpack is scanned.</param>
    public static async ValueTask TryEquipUpgradesAsync(OfflinePlayer player)
    {
        if (player.Inventory is not { } inventory
            || player.SelectedCharacter?.CharacterClass is not { } characterClass)
        {
            return;
        }

        // Snapshot, because equipping mutates the item collection while we iterate.
        var backpackItems = inventory.Items
            .Where(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount)
            .ToList();

        foreach (var item in backpackItems)
        {
            if (item.Definition is not { } definition
                || !IsWearableCandidate(definition, characterClass)
                || !player.CompliesRequirements(item))
            {
                continue;
            }

            var candidateScore = Score(item);

            // Prefer an empty qualified slot; otherwise replace the weakest equipped piece it beats.
            byte? targetSlot = null;
            Item? replaced = null;
            foreach (var slot in definition.ItemSlot!.ItemSlots)
            {
                var equipped = inventory.GetItem((byte)slot);
                if (equipped?.Definition?.IsAmmunition == true)
                {
                    // Never displace the ammunition (an archer's arrows) - the bow would stop working.
                    continue;
                }

                if (equipped is null)
                {
                    targetSlot = (byte)slot;
                    replaced = null;
                    break;
                }

                if (Score(equipped) + UpgradeScoreMargin <= candidateScore
                    && (replaced is null || Score(equipped) < Score(replaced)))
                {
                    targetSlot = (byte)slot;
                    replaced = equipped;
                }
            }

            if (targetSlot is not { } equipSlot)
            {
                continue;
            }

            if (replaced is not null)
            {
                // Make room: move the old piece to a free backpack slot first.
                if (FindFreeBackpackSlot(inventory) is not { } freeSlot)
                {
                    continue;
                }

                await MoveAction.MoveItemAsync(player, equipSlot, Storages.Inventory, freeSlot, Storages.Inventory).ConfigureAwait(false);
                if (inventory.GetItem(equipSlot) is not null)
                {
                    // The unequip was rejected - don't try to force the swap.
                    continue;
                }
            }

            var fromSlot = item.ItemSlot;
            await MoveAction.MoveItemAsync(player, fromSlot, Storages.Inventory, equipSlot, Storages.Inventory).ConfigureAwait(false);
            if (inventory.GetItem(equipSlot) != item)
            {
                // Equip rejected (e.g. requirements after all) - leave everything as is.
                continue;
            }

            player.Logger.LogInformation(
                "Bot '{Name}' equipped '{New}'{Replaced}.",
                player.Name,
                item,
                replaced is null ? string.Empty : $" (replacing '{replaced}')");

            if (replaced is not null && !IsWorthKeeping(replaced))
            {
                try
                {
                    await DropAction.DropItemAsync(player, replaced.ItemSlot, player.Position).ConfigureAwait(false);
                }
                catch (InvalidOperationException ex)
                {
                    // E.g. the map ran out of object ids for another drop - not worth failing the swap
                    // over; the outgrown piece simply stays in the backpack (plenty of room with the
                    // extended inventory) until a later pass or forever.
                    player.Logger.LogDebug(ex, "Bot '{Name}' could not drop replaced item '{Item}'.", player.Name, replaced);
                }
            }

            // One swap per pass keeps the work per tick small; the next pass picks up the rest.
            return;
        }
    }

    /// <summary>
    /// Whether the item is gear this bot would wear at all: an equippable, class-qualified piece which -
    /// if it is a weapon - matches the class's fighting style (an elf only considers bows, a wizard only
    /// staves), so bots don't fill their off-hand with random qualified junk like a Small Axe.
    /// </summary>
    private static bool IsWearableCandidate(ItemDefinition definition, CharacterClass characterClass)
    {
        const byte lastWeaponGroup = 5;
        return definition.ItemSlot is { ItemSlots.Count: > 0 }
               && !definition.IsAmmunition
               && definition.QualifiedCharacters.Contains(characterClass)
               && (definition.Group > lastWeaponGroup || BotProgression.IsPreferredWeaponGroup(characterClass, (byte)definition.Group));
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

    private static bool IsWorthKeeping(Item item)
    {
        return item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent)
               || item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0);
    }

    private static byte? FindFreeBackpackSlot(IStorage inventory)
    {
        return inventory.FreeSlots
            .Where(s => s >= InventoryConstants.EquippableSlotsCount)
            .Cast<byte?>()
            .FirstOrDefault();
    }
}
