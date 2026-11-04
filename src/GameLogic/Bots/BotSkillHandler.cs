// <copyright file="BotSkillHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Lets a bot learn skills like a real player: by looting the orb or scroll from the ground and
/// consuming it through the regular <see cref="ItemConsumeAction"/> - the same validations a human
/// faces (class qualification, the item's level and stat requirements, unknown skill only). Skills
/// are therefore learned when their orb or scroll actually drops where the bot hunts, instead of
/// appearing the moment the bot's stats allow it - a low-level bot can no longer fight with a skill
/// whose orb or scroll does not drop yet where it hunts.
/// </summary>
internal static class BotSkillHandler
{
    private static readonly ItemConsumeAction ConsumeAction = new();

    /// <summary>
    /// Determines whether the dropped item teaches the bot a skill it wants: an orb or scroll for a
    /// skill the bot does not know yet, is qualified for (skill and item alike), may currently consume
    /// (the item's own requirements), and may actually fight with (no siege-only, mount-bound, master
    /// or non-combat skills). Like for gear upgrades, the pickup handler asks
    /// this before collecting anything from the ground.
    /// </summary>
    /// <param name="player">The bot player which would learn the skill.</param>
    /// <param name="item">The dropped item to evaluate.</param>
    public static bool WantsSkillItem(Player player, Item item)
    {
        if (player.SelectedCharacter?.CharacterClass is not { } characterClass
            || item.Definition is not { } definition
            || definition.Skill is not { } skill)
        {
            return false;
        }

        if (definition.Group != BotProgression.SkillOrbItemGroup && definition.Group != BotProgression.SkillScrollItemGroup)
        {
            return false;
        }

        if (player.SkillList?.ContainsSkill(skill.Number.ToUnsigned()) == true)
        {
            return false;
        }

        if (!skill.QualifiedCharacters.Contains(characterClass)
            || !definition.QualifiedCharacters.Contains(characterClass)
            || !BotProgression.MayBotOwnSkill(skill))
        {
            return false;
        }

        // The same gate a human faces when consuming the orb or scroll.
        return player.CompliesRequirements(item);
    }

    /// <summary>
    /// Consumes every looted orb and scroll in the bot's backpack which still teaches something new.
    /// Runs on the equipment cadence (see <c>BotNavigator</c>) and queued into the AI tick like every
    /// other self-mutation, so it never races the combat handler enumerating the skill list. One pass
    /// consumes whatever is consumable right now; what the bot cannot consume yet (a requirement it
    /// does not meet) was never picked up in the first place.
    /// </summary>
    /// <param name="player">The bot player whose backpack is scanned.</param>
    public static async ValueTask TryLearnSkillsAsync(OfflinePlayer player)
    {
        if (player.Inventory is not { } inventory)
        {
            return;
        }

        // Snapshot, because consuming mutates the item collection while we iterate.
        var backpackItems = inventory.Items
            .Where(i => i.ItemSlot >= InventoryConstants.EquippableSlotsCount)
            .ToList();

        foreach (var item in backpackItems)
        {
            if (!WantsSkillItem(player, item))
            {
                continue;
            }

            var skillName = item.Definition?.Skill?.Name;
            var slot = item.ItemSlot;

            // No target item: skill orbs and scrolls are consumed on their own. Byte.MaxValue addresses
            // no slot, so the action resolves a null target instead of some unrelated equipped piece.
            await ConsumeAction.HandleConsumeRequestAsync(player, slot, byte.MaxValue, FruitUsage.Undefined).ConfigureAwait(false);

            if (inventory.GetItem(slot) != item)
            {
                player.Logger.LogDebug("Bot '{Name}' learned '{Skill}' from a looted orb or scroll.", player.Name, skillName);
            }
        }
    }
}
