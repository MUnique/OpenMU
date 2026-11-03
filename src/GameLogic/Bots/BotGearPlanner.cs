// <copyright file="BotGearPlanner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Plans how many stat points a bot keeps for the requirements of its gear, so the weighted split of
/// <see cref="BotProgression.SplitPoints"/> can't leave it unable to wear its armor, shield or weapon.
/// </summary>
/// <remarks>
/// Two kinds of gear count, both by the same requirement calculation the game applies to players
/// (<see cref="ItemExtensions.GetRequirements"/>, which grows with the item level and the options):
/// <list type="bullet">
///   <item><description>
///     The pieces the bot wears, one item level above their current one. The bot takes a piece off to
///     apply a jewel and has to be able to put it back on, also after the upgrade succeeded; a piece it
///     can't wear anymore ends up sold as junk. These are always kept.
///   </description></item>
///   <item><description>
///     The best weapon (of the build's fighting style) and armor pieces the bot can find at its level:
///     monster drops whose drop level and level requirement don't exceed the character level, counted as
///     dropped (+0, no options) - event rewards like the Archangel weapons are not found while hunting.
///     A candidate which would raise the cost of all planned gear beyond
///     <see cref="CandidateBudgetPercent"/> % of the bot's points (counting what the requirements take
///     beyond the build's own investment) is skipped - otherwise the top set of a class (e.g. the Titan
///     Armor's 899 strength at level 150) would swallow the whole build of a character which can't reach
///     it yet anyway. Of the affordable ones, the bot plans for the cheapest for its build among those
///     close to the best drop level (see <see cref="DropLevelSlack"/>).
///   </description></item>
/// </list>
/// Shields only count while worn: a bow or a two-handed staff leaves no hand for one, so planning for the
/// best shield would put points aside which many builds never use.
/// Wings, pets and jewelry are not part of it: they require only a character level, never a stat (a
/// Dark Lord's pets need leadership, which its build invests anyway).
/// </remarks>
internal static class BotGearPlanner
{
    /// <summary>
    /// The highest share of all a bot's points a candidate's requirements may take beyond what the build
    /// invests into the stats anyway. Half keeps the build recognizable while still letting e.g. a
    /// supporter elf put the agility of a good armor set aside.
    /// </summary>
    private const int CandidateBudgetPercent = 50;

    /// <summary>
    /// How many drop levels below the best affordable candidate a cheaper one still counts as just as
    /// good. The sets of a class lie close together in drop level but are cut for different builds (a
    /// Magic Gladiator's strength set next to its energy set), and picking by drop level alone planned
    /// the strength set for the energy build.
    /// </summary>
    private const int DropLevelSlack = 20;

    /// <summary>The highest item group which is a weapon (0 sword, 1 axe, 2 mace, 3 spear, 4 bow, 5 staff).</summary>
    private const byte LastWeaponGroup = 5;

    /// <summary>The item group of the shields.</summary>
    private const byte ShieldGroup = 6;

    /// <summary>The item groups of the armor pieces: 7 helm, 8 armor, 9 pants, 10 gloves, 11 boots.</summary>
    private static readonly byte[] ArmorGroups = [7, 8, 9, 10, 11];

    /// <summary>
    /// The candidates of each class and build variant, computed once per game configuration: what a piece
    /// requires as dropped doesn't depend on the bot's level, only which pieces it reaches does - so a
    /// level-up only filters this list instead of scanning every item definition again. Weakly keyed by
    /// configuration, so a thrown-away configuration takes its cache with it.
    /// </summary>
    private static readonly ConditionalWeakTable<GameConfiguration, ConcurrentDictionary<(CharacterClass Class, int Variant), Candidate[]>> CandidatesByConfiguration = new();

    /// <summary>
    /// Plans the points the bot keeps per stat for its gear: the invested points (above the class base)
    /// each stat should at least have.
    /// </summary>
    /// <param name="configuration">The game configuration which defines the items.</param>
    /// <param name="characterClass">The bot's character class.</param>
    /// <param name="characterName">The character name; decides the build and its weapon style.</param>
    /// <param name="characterLevel">The character level (not the reset-aware effective level - the level requirement of an item checks the plain one).</param>
    /// <param name="equippedItems">The items the bot wears.</param>
    /// <param name="totalPoints">All the points of the bot: those already invested and those to split now.</param>
    /// <returns>The minimum invested points per base stat; stats without a requirement are missing.</returns>
    public static Dictionary<AttributeDefinition, long> GetStatMinimums(
        GameConfiguration configuration,
        CharacterClass characterClass,
        string characterName,
        int characterLevel,
        IEnumerable<Item> equippedItems,
        long totalPoints)
    {
        var minimums = GetWornMinimums(configuration, characterClass, equippedItems);
        foreach (var candidate in ChooseCandidates(configuration, characterClass, characterName, characterLevel, totalPoints, minimums))
        {
            Merge(minimums, candidate.Requirements);
        }

        return minimums;
    }

    /// <summary>
    /// Chooses the gear the bot plans for at its level: per kind - the weapon of the build's style first,
    /// then the helm, the armor, the pants, the gloves and the boots - a monster drop whose level
    /// requirement the character meets and whose stat requirements, together with what is planned
    /// already, still fit the budget (see the remarks of the class) - and the bot's points at all: the
    /// minimums of all planned gear together must not need more points than the bot has. Of those, the
    /// cheapest one for the build within <see cref="DropLevelSlack"/> drop levels of the best. The budget is
    /// shared because the requirements add up over the stats: an energy elf's Faith Armor fits on its
    /// own, as does an agility bow, but not both with the rest of its build.
    /// </summary>
    /// <param name="configuration">The game configuration which defines the items.</param>
    /// <param name="characterClass">The bot's character class.</param>
    /// <param name="characterName">The character name; decides the build and its weapon style.</param>
    /// <param name="characterLevel">The character level.</param>
    /// <param name="totalPoints">All the points of the bot: those already invested and those to split now.</param>
    /// <param name="planned">The minimums planned already (the worn gear); null means none.</param>
    /// <returns>The chosen candidates with their requirements as invested points of the base stats.</returns>
    internal static IReadOnlyList<Candidate> ChooseCandidates(
        GameConfiguration configuration,
        CharacterClass characterClass,
        string characterName,
        int characterLevel,
        long totalPoints,
        IReadOnlyDictionary<AttributeDefinition, long>? planned = null)
    {
        var weights = BotProgression.GetStatWeights(characterClass, characterName);
        var totalWeight = weights.Sum(w => w.Weight);
        var budget = totalPoints * CandidateBudgetPercent / 100;
        long BuildShare(AttributeDefinition stat)
            => totalWeight <= 0 ? 0 : totalPoints * weights.FirstOrDefault(w => w.Stat == stat).Weight / totalWeight;

        // What the planned minimums cost beyond the build's own investment.
        long CostOf(IReadOnlyDictionary<AttributeDefinition, long> minimums)
            => minimums.Sum(m => Math.Max(0, m.Value - BuildShare(m.Key)));

        var chosenMinimums = new Dictionary<AttributeDefinition, long>(planned ?? new Dictionary<AttributeDefinition, long>());
        var baseCost = CostOf(chosenMinimums);
        var chosen = new List<Candidate>();
        var kinds = GetCandidates(configuration, characterClass, characterName)
            .Where(c => c.Definition.DropLevel <= characterLevel && c.LevelRequirement <= characterLevel)
            .GroupBy(c => c.Definition.Group <= LastWeaponGroup ? -1 : c.Definition.Group)
            .OrderBy(kind => kind.Key);

        foreach (var kind in kinds)
        {
            // The worn gear is kept whatever it costs; only what the candidates add counts against the budget.
            var affordable = kind
                .Select(c => (Candidate: c, Combined: Combine(chosenMinimums, c.Requirements)))
                .Select(c => (c.Candidate, c.Combined, Cost: CostOf(c.Combined) - baseCost))
                .Where(c => c.Cost <= budget && c.Combined.Values.Sum() <= totalPoints)
                .ToList();
            if (affordable.Count == 0)
            {
                continue;
            }

            var bestDropLevel = affordable.Max(c => c.Candidate.Definition.DropLevel);
            var pick = affordable
                .Where(c => c.Candidate.Definition.DropLevel >= bestDropLevel - DropLevelSlack)
                .OrderBy(c => c.Cost)
                .ThenByDescending(c => c.Candidate.Definition.DropLevel)
                .First();
            chosen.Add(pick.Candidate);
            chosenMinimums = pick.Combined;
        }

        return chosen;
    }

    /// <summary>
    /// Whether the item is gear whose stat requirements the planner looks at: weapons, armor pieces and -
    /// if asked for - shields; never ammunition.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <param name="includeShields">Whether shields count as well.</param>
    /// <returns><c>True</c>, if the item is planned gear.</returns>
    internal static bool IsPlannedGear(ItemDefinition definition, bool includeShields)
    {
        return definition.ItemSlot is { ItemSlots.Count: > 0 }
               && !definition.IsAmmunition
               && (definition.Group <= LastWeaponGroup
                   || ArmorGroups.Contains((byte)definition.Group)
                   || (includeShields && definition.Group == ShieldGroup));
    }

    /// <summary>
    /// Whether the player meets the item's stat and level requirements on its base stats alone. The
    /// bonuses of worn gear don't count - the item's own among them, which come off with it when the bot
    /// takes it off for a jewel or a mix - so the answer holds whatever the bot takes off.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if the requirements are met without any bonus.</returns>
    internal static bool MeetsRequirementsOnBaseStats(Player player, Item item)
    {
        if (player.Attributes is not { } attributes)
        {
            return false;
        }

        foreach (var (attribute, value) in item.GetRequirements(player.GameContext.Configuration))
        {
            if (attributes[BotProgression.TotalToBaseStat(attribute) ?? attribute] < value)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>A copy of the worn item, one item level higher (as far as the item allows).</summary>
    /// <param name="item">The worn item.</param>
    /// <returns>The copy.</returns>
    internal static Item UpgradedCopy(Item item)
    {
        var copy = new TemporaryItem();
        copy.AssignValues(item);
        copy.Level = (byte)Math.Max(item.Level, Math.Min(item.Level + 1, item.Definition!.MaximumItemLevel));
        return copy;
    }

    /// <summary>
    /// All the gear a bot of the class and build could ever plan for - monster drops of its weapon style
    /// and the armor pieces it is qualified for - with their requirements as dropped (cached, see
    /// <see cref="CandidatesByConfiguration"/>).
    /// </summary>
    private static Candidate[] GetCandidates(GameConfiguration configuration, CharacterClass characterClass, string characterName)
    {
        var byBuild = CandidatesByConfiguration.GetValue(configuration, _ => new ConcurrentDictionary<(CharacterClass Class, int Variant), Candidate[]>());
        return byBuild.GetOrAdd(
            (characterClass, BotBuild.GetVariant(characterName)),
            _ => configuration.Items
                .Where(d => d.DropsFromMonsters
                            && d.QualifiedCharacters.Contains(characterClass)
                            && IsPlannedGear(d, includeShields: false)
                            && (d.Group > LastWeaponGroup || BotProgression.IsPreferredWeaponGroup(characterClass, characterName, (byte)d.Group)))
                .Select(d => ToCandidate(d, configuration, characterClass))
                .ToArray());
    }

    /// <summary>The candidate as it would drop at its lowest: +0, without options.</summary>
    private static Candidate ToCandidate(ItemDefinition definition, GameConfiguration configuration, CharacterClass characterClass)
    {
        var requirements = GetBaseStatRequirements(new TemporaryItem { Definition = definition }, configuration, characterClass, out var levelRequirement);
        return new Candidate(definition, requirements, levelRequirement);
    }

    /// <summary>The minimums of the worn gear: its requirements one item level up.</summary>
    private static Dictionary<AttributeDefinition, long> GetWornMinimums(GameConfiguration configuration, CharacterClass characterClass, IEnumerable<Item> equippedItems)
    {
        var minimums = new Dictionary<AttributeDefinition, long>();
        foreach (var item in equippedItems)
        {
            if (item.Definition is { } definition && IsPlannedGear(definition, includeShields: true))
            {
                Merge(minimums, GetBaseStatRequirements(UpgradedCopy(item), configuration, characterClass, out _));
            }
        }

        return minimums;
    }

    /// <summary>A copy of the minimums, raised by the requirements.</summary>
    private static Dictionary<AttributeDefinition, long> Combine(IReadOnlyDictionary<AttributeDefinition, long> minimums, IReadOnlyDictionary<AttributeDefinition, long> requirements)
    {
        var combined = new Dictionary<AttributeDefinition, long>(minimums);
        Merge(combined, requirements);
        return combined;
    }

    /// <summary>Raises each minimum to the requirement, where the requirement is higher.</summary>
    private static void Merge(Dictionary<AttributeDefinition, long> minimums, IReadOnlyDictionary<AttributeDefinition, long> requirements)
    {
        foreach (var (stat, value) in requirements)
        {
            minimums[stat] = Math.Max(minimums.GetValueOrDefault(stat), value);
        }
    }

    /// <summary>
    /// The item's stat requirements as invested points of the base stats: the requirement minus the class
    /// base. The requirement is checked against the total stat, which item and buff bonuses raise - not
    /// counting them keeps the planned value on the safe side.
    /// </summary>
    private static Dictionary<AttributeDefinition, long> GetBaseStatRequirements(Item item, GameConfiguration configuration, CharacterClass characterClass, out int levelRequirement)
    {
        levelRequirement = 0;
        var result = new Dictionary<AttributeDefinition, long>();
        foreach (var (attribute, value) in item.GetRequirements(configuration))
        {
            if (attribute == Stats.Level)
            {
                levelRequirement = Math.Max(levelRequirement, value);
                continue;
            }

            if (value <= 0 || BotProgression.TotalToBaseStat(attribute) is not { } baseStat)
            {
                continue;
            }

            var classBase = (long)(characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == baseStat)?.BaseValue ?? 0f);
            if (value - classBase > 0)
            {
                result[baseStat] = Math.Max(result.GetValueOrDefault(baseStat), value - classBase);
            }
        }

        return result;
    }

    /// <summary>
    /// A piece of gear the bot could find at its level, with what it requires as dropped.
    /// </summary>
    /// <param name="Definition">The item definition.</param>
    /// <param name="Requirements">The stat requirements as invested points of the base stats.</param>
    /// <param name="LevelRequirement">The character level the item requires.</param>
    internal sealed record Candidate(ItemDefinition Definition, Dictionary<AttributeDefinition, long> Requirements, int LevelRequirement);
}
