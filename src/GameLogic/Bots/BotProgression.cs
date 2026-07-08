// <copyright file="BotProgression.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The shared progression rules of server-side bots: how a bot of a given class invests its stat
/// points, and which skills it may learn. Used by the <see cref="BotGenerator"/> when a bot is
/// created and by the <see cref="BotSkillProgressionPlugIn"/> when it levels up during play, so a
/// freshly generated bot and one that grew to the same level in-game end up with the same build.
/// </summary>
internal static class BotProgression
{
    /// <summary>
    /// The character level at which a bot changes into its second-generation class (e.g. Dark Knight
    /// to Blade Knight), the way a player completes the class-change quest. The quest itself boils down
    /// to exactly this assignment (see <c>QuestCompletionAction</c>), so bots take the direct route.
    /// </summary>
    public const int ClassEvolutionLevel = 200;

    /// <summary>
    /// The character class numbers from the game's data model (<c>CharacterClassNumber</c> lives in the
    /// initialization assembly which GameLogic does not reference, so the relevant values are mirrored here).
    /// </summary>
    private const byte FairyElfNumber = 8;
    private const byte MuseElfNumber = 10;
    private const byte DarkLordNumber = 16;
    private const byte LordEmperorNumber = 17;
    private const byte RageFighterNumber = 24;
    private const byte FistMasterNumber = 25;

    /// <summary>
    /// The base classes which evolve into a second-generation class at <see cref="ClassEvolutionLevel"/>:
    /// Dark Wizard, Dark Knight, Fairy Elf and Summoner. The Magic Gladiator, Dark Lord and Rage Fighter
    /// have no second generation - their next class is the level-400 master evolution, out of bot scope.
    /// </summary>
    private static readonly byte[] EvolvableClassNumbers = [0, 4, 8, 20];

    /// <summary>
    /// Skills of the buff type which must never enter a bot's auto-buff rotation: the summoner's
    /// enemy debuffs (Sleep/Weakness/Innovation - the offline buff handler casts buffs on SELF, so the
    /// bot would put itself to sleep), and Defense (18), which players get from equipping a shield
    /// rather than learning it.
    /// </summary>
    private static readonly short[] ExcludedBuffSkillNumbers = [18, 219, 221, 222];

    /// <summary>
    /// Gets the class the character evolves into at <see cref="ClassEvolutionLevel"/>, or null when the
    /// class has no (in-scope) evolution.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    public static CharacterClass? GetEvolutionTarget(CharacterClass characterClass)
    {
        return EvolvableClassNumbers.Contains(characterClass.Number)
            ? characterClass.NextGenerationClass
            : null;
    }

    /// <summary>
    /// How a bot invests its stat points, per class. Mirrors a sensible human build: everyone puts a
    /// large share into vitality (survival) and their damage stat; classes whose class skills have
    /// stat requirements additionally invest what those skills need - elves keep enough energy for
    /// their heal/defense/damage orbs, rage fighters for their buffs, dark lords raise leadership
    /// like a real player would. The requirement gate in <see cref="MeetsRequirements"/> then unlocks
    /// those skills exactly when the grown stats reach the game's own thresholds.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    public static IReadOnlyList<(AttributeDefinition Stat, int Weight)> GetStatWeights(CharacterClass characterClass)
    {
        return characterClass.Number switch
        {
            FairyElfNumber or MuseElfNumber => new[] { (Stats.BaseVitality, 40), (Stats.BaseAgility, 40), (Stats.BaseEnergy, 20) },
            DarkLordNumber or LordEmperorNumber => new[] { (Stats.BaseStrength, 35), (Stats.BaseVitality, 30), (Stats.BaseLeadership, 25), (Stats.BaseEnergy, 10) },
            RageFighterNumber or FistMasterNumber => new[] { (Stats.BaseStrength, 45), (Stats.BaseVitality, 35), (Stats.BaseEnergy, 20) },
            _ => new[] { (GetMainDamageStat(characterClass), 50), (Stats.BaseVitality, 50) },
        };
    }

    /// <summary>
    /// Splits the given points proportionally to the class's stat weights, returning whole-point
    /// amounts which sum up to <paramref name="points"/> (the remainder goes to the first stat).
    /// </summary>
    /// <param name="points">The number of points to split.</param>
    /// <param name="weights">The stat weights of the class.</param>
    public static IEnumerable<(AttributeDefinition Stat, int Amount)> SplitPoints(int points, IReadOnlyList<(AttributeDefinition Stat, int Weight)> weights)
    {
        var totalWeight = weights.Sum(w => w.Weight);
        if (points <= 0 || totalWeight <= 0)
        {
            yield break;
        }

        var assigned = 0;
        for (var i = 1; i < weights.Count; i++)
        {
            var amount = points * weights[i].Weight / totalWeight;
            assigned += amount;
            if (amount > 0)
            {
                yield return (weights[i].Stat, amount);
            }
        }

        yield return (weights[0].Stat, points - assigned);
    }

    /// <summary>
    /// Determines whether the skill is one a bot may learn: an actual attack skill, or a self/party
    /// buff or heal with a magic effect (which the offline buff/heal handlers know how to cast).
    /// Passive boosts, event skills, enemy debuffs and utility skills are left out.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    public static bool IsBotLearnableSkill(Skill skill)
    {
        if (skill.AttackDamage > 0
            && skill.SkillType is SkillType.DirectHit
                or SkillType.AreaSkillAutomaticHits
                or SkillType.AreaSkillExplicitHits
                or SkillType.AreaSkillExplicitTarget)
        {
            return true;
        }

        return skill.SkillType is SkillType.Buff or SkillType.Regeneration
               && skill.MagicEffectDef is not null
               && !ExcludedBuffSkillNumbers.Contains(skill.Number);
    }

    /// <summary>
    /// Determines whether the character meets the skill's learn requirements (the same ones the game
    /// enforces when casting, e.g. total energy for wizard spells or character level for knight skills).
    /// <paramref name="getAttributeValue"/> resolves an attribute's current value; returning null means
    /// the attribute is unknown in the caller's context, which conservatively fails the requirement.
    /// </summary>
    /// <param name="skill">The skill whose requirements are checked.</param>
    /// <param name="getAttributeValue">Resolves an attribute's current value; null means the attribute is unknown.</param>
    public static bool MeetsRequirements(Skill skill, Func<AttributeDefinition, float?> getAttributeValue)
    {
        foreach (var requirement in skill.Requirements)
        {
            if (requirement.Attribute is not { } attribute)
            {
                continue;
            }

            if (getAttributeValue(attribute) is not { } value || value < requirement.MinimumValue)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Maps a "total" attribute (used by skill requirements) to the base stat a generated character
    /// actually has, so requirements can be evaluated before the character was ever composed at runtime.
    /// Returns null for attributes that have no base-stat counterpart.
    /// </summary>
    /// <param name="attribute">The "total" attribute to map.</param>
    public static AttributeDefinition? TotalToBaseStat(AttributeDefinition attribute)
    {
        if (attribute == Stats.TotalEnergy)
        {
            return Stats.BaseEnergy;
        }

        if (attribute == Stats.TotalStrength)
        {
            return Stats.BaseStrength;
        }

        if (attribute == Stats.TotalAgility)
        {
            return Stats.BaseAgility;
        }

        if (attribute == Stats.TotalVitality)
        {
            return Stats.BaseVitality;
        }

        if (attribute == Stats.TotalLeadership)
        {
            return Stats.BaseLeadership;
        }

        if (attribute == Stats.Level)
        {
            return Stats.Level;
        }

        return null;
    }

    /// <summary>
    /// Determines whether a weapon of the given item group fits the class's fighting style: archers
    /// (agility-based classes) use bows, pure casters staves, everyone else melee weapons. Mirrors the
    /// starter-weapon choice of the <see cref="BotGenerator"/>, so an elf never swaps its bow for a
    /// random axe it happens to be qualified for (which would also displace its arrows).
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="itemGroup">The item group of the weapon.</param>
    public static bool IsPreferredWeaponGroup(CharacterClass characterClass, byte itemGroup)
    {
        const byte maxMeleeGroup = 3;
        const byte bowGroup = 4;
        const byte staffGroup = 5;

        float ClassStat(AttributeDefinition attribute)
            => characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == attribute)?.BaseValue ?? 0f;

        var strength = ClassStat(Stats.BaseStrength);
        var agility = ClassStat(Stats.BaseAgility);
        var energy = ClassStat(Stats.BaseEnergy);

        if (agility > strength && agility > energy)
        {
            return itemGroup == bowGroup;
        }

        if (energy > strength)
        {
            return itemGroup == staffGroup;
        }

        return itemGroup <= maxMeleeGroup;
    }

    private static AttributeDefinition GetMainDamageStat(CharacterClass characterClass)
    {
        return characterClass.StatAttributes
            .Where(a => a.Attribute == Stats.BaseStrength
                        || a.Attribute == Stats.BaseAgility
                        || a.Attribute == Stats.BaseEnergy
                        || a.Attribute == Stats.BaseLeadership)
            .OrderByDescending(a => a.BaseValue)
            .Select(a => a.Attribute!)
            .FirstOrDefault() ?? Stats.BaseStrength;
    }
}
