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
    private const byte DarkWizardNumber = 0;
    private const byte SoulMasterNumber = 2;
    private const byte GrandMasterNumber = 3;
    private const byte DarkKnightNumber = 4;
    private const byte BladeKnightNumber = 6;
    private const byte BladeMasterNumber = 7;
    private const byte FairyElfNumber = 8;
    private const byte MuseElfNumber = 10;
    private const byte HighElfNumber = 11;
    private const byte MagicGladiatorNumber = 12;
    private const byte DuelMasterNumber = 13;
    private const byte DarkLordNumber = 16;
    private const byte LordEmperorNumber = 17;
    private const byte SummonerNumber = 20;
    private const byte BloodySummonerNumber = 22;
    private const byte DimensionMasterNumber = 23;
    private const byte RageFighterNumber = 24;
    private const byte FistMasterNumber = 25;

    /// <summary>
    /// The share of each invested batch that goes into vitality on reset-meta servers, until the bot's
    /// personal <see cref="GetVitalityTarget"/> is reached (out of a nominal weight total of ~100).
    /// </summary>
    private const int ResetMetaVitalityWeight = 5;

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
    /// Gets the master class the character evolves into at the game's maximum level (the
    /// third-generation evolution the level-400 master quests perform), or null when the current class
    /// has none. Unlike <see cref="GetEvolutionTarget"/> this applies to all classes: the
    /// second-generation classes evolve into their masters (Blade Knight -> Blade Master, ...), and
    /// Magic Gladiator, Dark Lord and Rage Fighter - which have no second generation - evolve directly
    /// (-> Duel Master, Lord Emperor, Fist Master). When and whether a bot takes this step is decided
    /// by <see cref="BotMasterHandler.IsMasterEvolutionDue"/>.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    public static CharacterClass? GetMasterEvolutionTarget(CharacterClass characterClass)
    {
        return characterClass is { IsMasterClass: false, NextGenerationClass: { IsMasterClass: true } masterClass }
            ? masterClass
            : null;
    }

    /// <summary>
    /// How a bot invests its stat points, per class and per bot, in one of two meta profiles chosen
    /// by the server type (see <see cref="BotResetHandler.GetResetConfiguration"/> at the call sites):
    /// <list type="bullet">
    /// <item><b>Reset meta</b> (reset feature enabled) - modeled on the actual endgame characters of a
    /// reset server: everything goes into the class's combat stats (shield and agility-based defense do
    /// the tanking there), vitality only receives a token share until the bot's personal
    /// <see cref="GetVitalityTarget"/> is reached (enforce via <see cref="SplitPoints"/> capacities).</item>
    /// <item><b>Classic meta</b> (no reset feature) - the builds of community stat guides for classic
    /// servers, where point pools are small and vitality is a plain percentage of the build.</item>
    /// </list>
    /// Where a class has two established builds (knight agility/PK, gladiator warrior/mage, elf
    /// archer/supporter), each bot picks one deterministically from its character name, so the
    /// population is diverse but every bot keeps the same build across sessions and re-invests
    /// consistently after each reset. The first entry is always the build's primary stat - it absorbs
    /// rounding remainders and overflow from capped stats.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="characterName">The character name; decides the build variant for two-build classes.</param>
    /// <param name="resetMeta">Whether the reset-server meta profile applies.</param>
    public static IReadOnlyList<(AttributeDefinition Stat, int Weight)> GetStatWeights(CharacterClass characterClass, string characterName, bool resetMeta)
    {
        // Stable across processes (string.GetHashCode is randomized per run, which would re-spec
        // the bot on every server restart).
        var variant = characterName.Aggregate(0, (acc, c) => acc + c) % 2;
        var vit = Stats.BaseVitality;
        var str = Stats.BaseStrength;
        var agi = Stats.BaseAgility;
        var ene = Stats.BaseEnergy;
        var cmd = Stats.BaseLeadership;

        if (resetMeta)
        {
            const int v = ResetMetaVitalityWeight;
            return characterClass.Number switch
            {
                DarkKnightNumber or BladeKnightNumber or BladeMasterNumber => variant == 0
                    ? new[] { (str, 59), (agi, 39), (ene, 2), (vit, v) }
                    : new[] { (str, 52), (agi, 35), (ene, 13), (vit, v) },
                DarkWizardNumber or SoulMasterNumber or GrandMasterNumber =>
                    new[] { (ene, 49), (agi, 47), (str, 4), (vit, v) },
                FairyElfNumber or MuseElfNumber or HighElfNumber =>
                    new[] { (agi, 67), (ene, 27), (str, 6), (vit, v) },
                MagicGladiatorNumber or DuelMasterNumber => variant == 0
                    ? new[] { (str, 57), (agi, 26), (ene, 17), (vit, v) }
                    : new[] { (ene, 66), (agi, 18), (str, 16), (vit, v) },
                DarkLordNumber or LordEmperorNumber =>
                    new[] { (str, 54), (cmd, 23), (agi, 18), (ene, 5), (vit, v) },
                SummonerNumber or BloodySummonerNumber or DimensionMasterNumber =>
                    new[] { (ene, 70), (agi, 18), (str, 12), (vit, v) },
                RageFighterNumber or FistMasterNumber =>
                    new[] { (str, 64), (agi, 18), (ene, 18), (vit, v) },
                _ when GetMainDamageStat(characterClass) == str =>
                    new[] { (str, 60), (agi, 35), (vit, v) },
                _ => new[] { (GetMainDamageStat(characterClass), 65), (agi, 30), (vit, v) },
            };
        }

        return characterClass.Number switch
        {
            DarkKnightNumber or BladeKnightNumber or BladeMasterNumber => variant == 0
                ? new[] { (str, 62), (agi, 26), (vit, 8), (ene, 4) }
                : new[] { (str, 50), (vit, 28), (agi, 18), (ene, 4) },
            DarkWizardNumber or SoulMasterNumber or GrandMasterNumber =>
                new[] { (ene, 66), (vit, 22), (agi, 8), (str, 4) },
            FairyElfNumber or MuseElfNumber or HighElfNumber => variant == 0
                ? new[] { (agi, 62), (vit, 23), (ene, 10), (str, 5) }
                : new[] { (ene, 65), (vit, 22), (agi, 8), (str, 5) },
            MagicGladiatorNumber or DuelMasterNumber => variant == 0
                ? new[] { (str, 57), (agi, 22), (vit, 15), (ene, 6) }
                : new[] { (ene, 58), (vit, 26), (agi, 11), (str, 5) },
            DarkLordNumber or LordEmperorNumber =>
                new[] { (str, 38), (cmd, 30), (vit, 22), (agi, 8), (ene, 2) },
            SummonerNumber or BloodySummonerNumber or DimensionMasterNumber =>
                new[] { (ene, 64), (vit, 24), (agi, 8), (str, 4) },
            RageFighterNumber or FistMasterNumber =>
                new[] { (str, 45), (vit, 35), (ene, 20) },
            _ => new[] { (GetMainDamageStat(characterClass), 50), (vit, 50) },
        };
    }

    /// <summary>
    /// The bot's personal vitality target on reset-meta servers: how many points it invests into
    /// vitality over its whole career (100..500, rolled deterministically from the character name,
    /// so the population gets a natural spread from glassy to sturdy and every bot keeps its roll
    /// across restarts and resets). The endgame players such servers breed leave vitality almost
    /// untouched - shield and agility-based defense tank instead - so the target is intentionally low.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    public static int GetVitalityTarget(string characterName)
    {
        var sum = characterName.Aggregate(0, (acc, c) => acc + c);
        return 100 + ((sum * 7919) % 401);
    }

    /// <summary>
    /// Splits the given points proportionally to the class's stat weights, returning whole-point
    /// amounts which sum up to <paramref name="points"/> - unless capacities cut it short. The
    /// optional <paramref name="capacityOf"/> callback limits how many points a stat may still take
    /// (its <see cref="AttributeDefinition.MaximumValue"/> on fun servers, the vitality target on
    /// reset-meta servers); a filled stat drops out of the split and its share flows to the remaining
    /// stats over subsequent rounds. When every stat is full, the rest of the points stay unassigned,
    /// like for a maxed-out human character.
    /// </summary>
    /// <param name="points">The number of points to split.</param>
    /// <param name="weights">The stat weights of the class.</param>
    /// <param name="capacityOf">Optionally resolves how many more points a stat can take; null means unlimited.</param>
    public static IEnumerable<(AttributeDefinition Stat, int Amount)> SplitPoints(
        int points,
        IReadOnlyList<(AttributeDefinition Stat, int Weight)> weights,
        Func<AttributeDefinition, long>? capacityOf = null)
    {
        if (points <= 0 || weights.Count == 0)
        {
            yield break;
        }

        var allocated = new int[weights.Count];
        var capacity = new long[weights.Count];
        for (var i = 0; i < weights.Count; i++)
        {
            capacity[i] = Math.Max(0, capacityOf?.Invoke(weights[i].Stat) ?? long.MaxValue);
        }

        var remaining = points;
        while (remaining > 0)
        {
            var activeTotalWeight = 0;
            var firstActive = -1;
            for (var i = 0; i < weights.Count; i++)
            {
                if (weights[i].Weight > 0 && allocated[i] < capacity[i])
                {
                    activeTotalWeight += weights[i].Weight;
                    if (firstActive < 0)
                    {
                        firstActive = i;
                    }
                }
            }

            if (activeTotalWeight <= 0)
            {
                break; // every stat is at its capacity - the rest stays unspent.
            }

            var assignedThisRound = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                if (weights[i].Weight <= 0 || allocated[i] >= capacity[i])
                {
                    continue;
                }

                var share = (int)Math.Min((long)remaining * weights[i].Weight / activeTotalWeight, capacity[i] - allocated[i]);
                allocated[i] += share;
                assignedThisRound += share;
            }

            if (assignedThisRound == 0)
            {
                // Rounding tail (fewer points left than active stats): the primary stat takes it.
                var tail = (int)Math.Min(remaining, capacity[firstActive] - allocated[firstActive]);
                allocated[firstActive] += tail;
                assignedThisRound = tail;
                if (assignedThisRound == 0)
                {
                    break;
                }
            }

            remaining -= assignedThisRound;
        }

        for (var i = 0; i < weights.Count; i++)
        {
            if (allocated[i] > 0)
            {
                yield return (weights[i].Stat, allocated[i]);
            }
        }
    }

    /// <summary>
    /// Determines whether the skill is one a bot may learn: an actual attack skill, or a self/party
    /// buff or heal with a magic effect (which the offline buff/heal handlers know how to cast).
    /// Passive boosts, event skills, enemy debuffs and utility skills are left out.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    public static bool IsBotLearnableSkill(Skill skill)
    {
        if (skill.MasterDefinition is not null)
        {
            // Master skills are never learned for free - they cost the master points earned per master
            // level and go through the regular action (see BotMasterHandler), like for a human player.
            return false;
        }

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
    /// Determines whether a weapon of the given item group fits the fighting style of this bot's BUILD:
    /// archers use bows, casters staves, everyone else melee weapons. The build decides, not just the
    /// class - a Magic Gladiator specced into energy (see the variants in <see cref="GetStatWeights"/>)
    /// is a caster and must get a staff, while its strength-specced sibling wants a blade; deciding by
    /// the class's base attributes alone handed both of them swords. Classes whose base attributes make
    /// them archers (the elves) keep their bow in every build - it is the only weapon they can wield.
    /// Used both for the starter gear (<see cref="BotGenerator"/>) and for later upgrades
    /// (<see cref="BotEquipmentHandler"/>), so an elf never swaps its bow for a random axe it happens to
    /// be qualified for (which would also displace its arrows).
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="characterName">The character name; decides the build variant, see <see cref="GetStatWeights"/>.</param>
    /// <param name="resetMeta">Whether the reset-server meta profile applies.</param>
    /// <param name="itemGroup">The item group of the weapon.</param>
    public static bool IsPreferredWeaponGroup(CharacterClass characterClass, string characterName, bool resetMeta, byte itemGroup)
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

        // The build's primary stat (the first weight, see GetStatWeights) tells a caster from a fighter;
        // the class fallback covers classes without an energy build of their own.
        var primaryStat = GetStatWeights(characterClass, characterName, resetMeta)[0].Stat;
        if (primaryStat == Stats.BaseEnergy || energy > strength)
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
