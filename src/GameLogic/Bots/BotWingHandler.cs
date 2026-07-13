// <copyright file="BotWingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Grants a bot its wings at the classic level milestones, like a player who saves up for them:
/// at level <see cref="FirstTierLevel"/> the first pair (+0, luck, +12 option), at
/// <see cref="SecondTierLevel"/> the second pair (+9, luck, +16 option) and at
/// <see cref="ThirdTierLevel"/> the third pair (+15, luck, +16 option). Wings don't drop from
/// monsters, so they are created directly and put straight into the wing slot - the planner
/// guarantees the class qualification and level requirement a regular equip would check, and the
/// slot placement mounts the power-ups like one. The outgrown pair is destroyed - never dropped,
/// so no player can grab a made-up wing from the ground.
/// The wing model of the item data does the rest: which classes may wear which pair is defined by
/// <see cref="ItemDefinition.QualifiedCharacters"/>, so e.g. the third-tier wings (master classes
/// only) are simply not offered to a bot which did not evolve yet, and the Dark Lord/Rage Fighter
/// capes - their only pre-master wing - are re-granted as a fresh +9 cape at the second milestone.
/// </summary>
internal static class BotWingHandler
{
    /// <summary>The level at which a bot gets its first tier wings (+0, luck, +12 option).</summary>
    private const int FirstTierLevel = 180;

    /// <summary>The level at which a bot gets its second tier wings (+9, luck, +16 option).</summary>
    private const int SecondTierLevel = 280;

    /// <summary>The level at which a bot gets its third tier wings (+15, luck, +16 option).</summary>
    private const int ThirdTierLevel = 400;

    /// <summary>The item level of the second tier grant; also disambiguates the tier of a cape (see <see cref="TierOf"/>).</summary>
    private const byte SecondTierItemLevel = 9;

    /// <summary>First tier wing numbers (group 12): Wings of Elf, Heaven, Satan and Curse.</summary>
    private static readonly (byte Group, short Number)[] FirstTierIds = { (12, 0), (12, 1), (12, 2), (12, 41) };

    /// <summary>Second tier wing numbers (group 12): Wings of Spirits, Soul, Dragon, Darkness and Despair.</summary>
    private static readonly (byte Group, short Number)[] SecondTierIds = { (12, 3), (12, 4), (12, 5), (12, 6), (12, 42) };

    /// <summary>Third tier wing numbers (group 12): Wing of Storm, Eternal, Illusion, Ruin, Dimension and the Capes of Emperor/Overrule.</summary>
    private static readonly (byte Group, short Number)[] ThirdTierIds = { (12, 36), (12, 37), (12, 38), (12, 39), (12, 40), (12, 43), (12, 50) };

    /// <summary>
    /// The Cape of Lord (13, 30, Dark Lord) and Cape of Fighter (12, 49, Rage Fighter): the only
    /// pre-master wing of their classes, granted at the first milestone at +0 and again at the
    /// second as a fresh +9 cape. Unlike all other wings, the Cape of Lord lives in group 13 -
    /// group 12 number 30 is the Packed Jewel of Bless (which the wing-slot filter of
    /// <see cref="PlanNextGrant"/> would keep out of the candidates anyway).
    /// </summary>
    private static readonly (byte Group, short Number)[] CapeIds = { (13, 30), (12, 49) };

    /// <summary>
    /// Checks the bot's level milestones and puts on the earned wings; called from the bot's regular
    /// evaluation cadence, queued into the MuHelper tick because equipping mounts item power-ups.
    /// </summary>
    /// <param name="player">The bot player.</param>
    public static async ValueTask TryAdvanceWingsAsync(OfflinePlayer player)
    {
        if (PlanNextGrant(player) is not { } plan || player.Inventory is not { } inventory)
        {
            return;
        }

        // The outgrown pair is destroyed, not dropped - a conjured wing must never lie on the
        // ground for a player to pick up. Clearing the slot first also keeps the grant independent
        // of the backpack, which is usually too crammed with loot for a wing's 5x3 footprint.
        if (inventory.GetItem(InventoryConstants.WingsSlot) is { } outgrown)
        {
            await player.DestroyInventoryItemAsync(outgrown).ConfigureAwait(false);
            player.Logger.LogInformation("Bot '{Name}' discarded its outgrown wings '{Wings}'.", player.Name, outgrown);
        }

        // Directly into the wing slot: the planner already guarantees the class qualification and
        // the level requirement, and the slot placement mounts the power-ups and broadcasts the
        // changed appearance like a regular equip.
        var wings = CreateWings(player, plan);
        if (!await inventory.AddItemAsync(InventoryConstants.WingsSlot, wings).ConfigureAwait(false))
        {
            // Shouldn't happen - the slot was just cleared; don't leak the created item.
            await player.PersistenceContext.DeleteAsync(wings).ConfigureAwait(false);
            player.Logger.LogWarning("Bot '{Name}' could not equip its new wings '{Wings}'.", player.Name, wings);
            return;
        }

        player.Logger.LogInformation("Bot '{Name}' earned its tier {Tier} wings: '{Wings}'.", player.Name, plan.Tier, wings);

        try
        {
            // Persist right away like after using jewels - a milestone shouldn't be lost (and the
            // outgrown pair resurrected) by a crash before the next periodic save.
            await player.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Couldn't save bot '{Name}' right after granting wings; the periodic save will retry.", player.Name);
        }
    }

    /// <summary>
    /// Determines the wings the bot has earned but does not wear yet, or <c>null</c> when it already
    /// wears its best earned pair (or none is due). Pure decision logic - exposed for unit tests.
    /// </summary>
    /// <param name="player">The bot player.</param>
    internal static (ItemDefinition Definition, byte ItemLevel, int OptionLevel, int Tier)? PlanNextGrant(Player player)
    {
        if (player.Inventory is not { } inventory
            || player.SelectedCharacter?.CharacterClass is not { } characterClass
            || player.Attributes is not { } attributes)
        {
            return null;
        }

        var level = (int)attributes[Stats.Level];
        var earnedTier = level switch
        {
            >= ThirdTierLevel => 3,
            >= SecondTierLevel => 2,
            >= FirstTierLevel => 1,
            _ => 0,
        };

        // Walk down from the earned tier to the best one the class currently qualifies for: a bot
        // which did not evolve into its master class yet simply isn't qualified for the third tier
        // wings and keeps its second pair until the evolution.
        for (var tier = earnedTier; tier >= 1; tier--)
        {
            var candidates = player.GameContext.Configuration.Items
                .Where(d => TierIds(tier).Contains((d.Group, d.Number))
                            && d.ItemSlot?.ItemSlots.Contains(InventoryConstants.WingsSlot) == true
                            && d.QualifiedCharacters.Contains(characterClass))
                .ToList();
            if (candidates.Count == 0)
            {
                continue;
            }

            if (inventory.GetItem(InventoryConstants.WingsSlot) is { } equipped && TierOf(equipped) >= tier)
            {
                // Already wearing this tier (or a better one, e.g. right after a reset while
                // re-levelling through the lower milestones) - never downgrade.
                return null;
            }

            var isCaster = attributes[Stats.TotalEnergy] > attributes[Stats.TotalStrength];
            var definition = candidates.MaxBy(d => FindWingOption(d, isCaster).Score)!;
            var (itemLevel, optionLevel) = tier switch
            {
                3 => ((byte)15, 4),
                2 => (SecondTierItemLevel, 4),
                _ => ((byte)0, 3),
            };

            return (definition, itemLevel, optionLevel, tier);
        }

        return null;
    }

    private static IReadOnlyCollection<(byte Group, short Number)> TierIds(int tier)
    {
        return tier switch
        {
            3 => ThirdTierIds,
            2 => SecondTierIds.Concat(CapeIds).ToList(),
            _ => FirstTierIds.Concat(CapeIds).ToList(),
        };
    }

    /// <summary>
    /// The tier a worn wing counts as; the capes are the first-tier grant of their classes but count
    /// as the second one once re-granted at +9.
    /// </summary>
    private static int TierOf(Item wings)
    {
        if (wings.Definition is not { } definition)
        {
            return 0;
        }

        var id = (definition.Group, definition.Number);
        if (ThirdTierIds.Contains(id))
        {
            return 3;
        }

        if (CapeIds.Contains(id))
        {
            return wings.Level >= SecondTierItemLevel ? 2 : 1;
        }

        if (SecondTierIds.Contains(id))
        {
            return 2;
        }

        return FirstTierIds.Contains(id) ? 1 : 0;
    }

    /// <summary>
    /// Picks the wing's "additional" option (the +4-per-level one, <see cref="ItemOptionTypes.Option"/>)
    /// which fits the bot's fighting style best - wizardry damage for casters, physical damage
    /// otherwise - and scores it, so wings offering the matching damage option (the Magic Gladiator
    /// may wear both Wings of Heaven and Satan) win the candidate selection.
    /// </summary>
    private static (IncreasableItemOption? Option, int Score) FindWingOption(ItemDefinition definition, bool isCaster)
    {
        static int ScoreOf(IncreasableItemOption option, bool isCaster)
        {
            var target = option.PowerUpDefinition?.TargetAttribute;
            if (target == Stats.WizardryBaseDmg || target == Stats.CurseBaseDmg)
            {
                return isCaster ? 3 : 1;
            }

            if (target == Stats.PhysicalBaseDmg)
            {
                return isCaster ? 1 : 3;
            }

            return 0;
        }

        return definition.PossibleItemOptions
            .SelectMany(o => o.PossibleOptions)
            .Where(o => o.OptionType == ItemOptionTypes.Option)
            .Select(o => ((IncreasableItemOption?)o, ScoreOf(o, isCaster)))
            .OrderByDescending(pair => pair.Item2)
            .FirstOrDefault();
    }

    private static Item CreateWings(OfflinePlayer player, (ItemDefinition Definition, byte ItemLevel, int OptionLevel, int Tier) plan)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = plan.Definition;
        item.Level = plan.ItemLevel;
        item.Durability = plan.Definition.Durability;

        if (plan.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck) is { } luck)
        {
            var luckLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
            luckLink.ItemOption = luck;
            item.ItemOptions.Add(luckLink);
        }

        var isCaster = player.Attributes![Stats.TotalEnergy] > player.Attributes[Stats.TotalStrength];
        if (FindWingOption(plan.Definition, isCaster).Option is { } option)
        {
            var optionLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            optionLink.Level = plan.OptionLevel;
            item.ItemOptions.Add(optionLink);
        }

        return item;
    }
}
