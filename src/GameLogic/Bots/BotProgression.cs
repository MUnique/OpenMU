// <copyright file="BotProgression.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
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

    /// <summary>The item group of skill orbs.</summary>
    internal const byte SkillOrbItemGroup = 12;

    /// <summary>The item group of skill scrolls and parchments.</summary>
    internal const byte SkillScrollItemGroup = 15;

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
    /// The skills the game only activates on the castle siege map: the client refuses to cast them
    /// anywhere else, so a hunting bot using one does something no player can do. They are also the
    /// strongest numbers each class has - which is exactly why a "pick the strongest" rule walked into
    /// them - and the initialization marks them "active in castle siege" (see the character-created
    /// skill plugins) while handing them to every created character, so each player has them when a
    /// siege begins. The second group are the siege role skills - Stun, Cancel Stun, Swell Mana,
    /// Invisibility, Cancel Invisibility and Abolish Magic - which are not attacks at all; Stun in
    /// particular is an area skill with no damage and a single hit, so only its number sets it apart.
    /// </summary>
    private static readonly short[] CastleSiegeOnlySkillNumbers = [44, 45, 46, 57, 67, 68, 69, 70, 71, 72, 73, 74, 269];

    /// <summary>
    /// Skills a player only ever gets from an item - an orb or scroll consumed in the learnables
    /// handler (see <c>LearnablesConsumeHandlerPlugIn</c>), or a weapon or pet carrying the skill. The
    /// gate lives on that item (its level and stat requirements), not on the skill, so
    /// <see cref="MeetsRequirements"/> finds nothing to check and would hand them out for free. A bot
    /// never consumes orbs, and it only gets a weapon or pet skill by equipping the item - which the
    /// server handles on its own - so none of these is ever learned. <see cref="GetItemGrantedSkillNumbers"/>
    /// collects every such skill the configuration grants through an item; the numbers here are the ones
    /// whose granting item is not modeled in the configuration at all, so they stay excluded too.
    /// </summary>
    private static readonly short[] ItemOrWeaponBoundSkillNumbers = [270];

    /// <summary>
    /// Skills which the game only lets a character cast while riding a mount (see the skill numbers in
    /// the <c>SkillNumber</c> enum of the initialization assembly). Neither the mount nor, where it
    /// applies, the required weapon kind is modeled in the skill data, so nothing else tells a mounted
    /// cast from one on foot. Bots never use these skills - even if a looted pet ends up in the pet
    /// slot, they are never learned, never looted and never selected (see <see cref="RequiresMount"/>) -
    /// which keeps the logic free of any mount detection.
    /// </summary>
    private static readonly short[] MountRequiredSkillNumbers = [47, 49, 76];

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
    /// How a bot invests its stat points, per class and per bot, following the builds of community
    /// stat guides: the class decides the stats, and where a class has two established builds
    /// (knight agility/PK, gladiator warrior/mage, elf archer/supporter) each bot picks one
    /// deterministically from its character name, so the population is diverse but every bot keeps
    /// the same build across sessions and re-invests consistently after each reset. The first entry
    /// is always the build's primary stat - it absorbs rounding remainders and overflow from capped
    /// stats.
    /// The weights are the same on every server; only the vitality a bot actually receives differs -
    /// the callers cap it at the bot's personal <see cref="GetVitalityTarget"/> on reset servers
    /// (shield and agility-based defense tank there, so vitality is left near its base), while classic
    /// servers keep the guide's plain vitality percentage.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="characterName">The character name; decides the build variant for two-build classes.</param>
    public static IReadOnlyList<(AttributeDefinition Stat, int Weight)> GetStatWeights(CharacterClass characterClass, string characterName)
    {
        // Stable across processes (string.GetHashCode is randomized per run, which would re-spec
        // the bot on every server restart).
        var variant = characterName.Aggregate(0, (acc, c) => acc + c) % 2;
        var vit = Stats.BaseVitality;
        var str = Stats.BaseStrength;
        var agi = Stats.BaseAgility;
        var ene = Stats.BaseEnergy;
        var cmd = Stats.BaseLeadership;

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
    /// Collects the skills the configuration hands out through items - an orb or scroll consumed to
    /// learn it (see <c>LearnablesConsumeHandlerPlugIn</c>), or a weapon or pet carrying the skill.
    /// Such a skill is only ever free if it also carries requirements of its own (e.g. Rageful Blow,
    /// which is granted by an orb yet demands level 170), which <see cref="MeetsRequirements"/> then
    /// gates; a skill without requirements of its own is gated by the item alone and never learned.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration which defines the items.</param>
    /// <returns>The numbers of all skills which are granted by an item.</returns>
    public static IReadOnlySet<short> GetItemGrantedSkillNumbers(GameConfiguration gameConfiguration)
        => (gameConfiguration.Items ?? []).Where(item => item.Skill is not null).Select(item => item.Skill!.Number).ToHashSet();

    /// <summary>
    /// Determines whether the skill deals damage to a target, as opposed to buffing, summoning or the like.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public static bool IsAttackSkill(Skill skill)
        => skill.SkillType is SkillType.DirectHit
            or SkillType.AreaSkillAutomaticHits
            or SkillType.AreaSkillExplicitHits
            or SkillType.AreaSkillExplicitTarget;

    /// <summary>
    /// Determines whether the skill hits more than its primary target.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public static bool IsAreaSkill(Skill skill)
        => skill.SkillType is SkillType.AreaSkillAutomaticHits
            or SkillType.AreaSkillExplicitHits
            or SkillType.AreaSkillExplicitTarget;

    /// <summary>
    /// Determines whether the skill is one the game only activates during a castle siege, which a bot
    /// therefore never uses while hunting - not even when it already knows it, as a Dark Knight does:
    /// Crescent Moon Slash is handed to every one of them when the character is created.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public static bool IsCastleSiegeOnly(Skill skill) => CastleSiegeOnlySkillNumbers.Contains(skill.Number);

    /// <summary>
    /// Determines whether the skill belongs to a PET rather than to the character, and may therefore
    /// only be used while that pet is actually equipped. Plasma Storm is the Fenrir's, and nothing in
    /// the skill's own numbers gives the missing pet away: the attribute behind its damage
    /// (<see cref="Attributes.Stats.FenrirBaseDmg"/>) is derived from the character's own strength,
    /// agility, vitality and energy, so it is large for any high level character - with or without the
    /// pet. Scoring it by that attribute alone handed Plasma Storm, the longest ranged skill most
    /// classes own, to a whole population riding nothing.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public static bool RequiresPet(Skill skill) => skill.DamageType == DamageType.Fenrir;

    /// <summary>
    /// Determines whether the skill is bound to a mount and therefore never used by bots. Covers bots
    /// which had already learned such a skill before the gate existed, too -
    /// the combat handler skips it without any mount detection.
    /// </summary>
    /// <param name="skill">The skill.</param>
    public static bool RequiresMount(Skill skill) => MountRequiredSkillNumbers.Contains(skill.Number);

    /// <summary>
    /// Determines whether the skill is one a bot may own at all: an actual attack skill, or a
    /// self/party buff or heal with a magic effect (which the offline buff/heal handlers know how to
    /// cast) - but never a master skill (those go through the regular master action like for a human
    /// player, see <see cref="BotMasterHandler"/>), a castle-siege-only skill, or a mount-bound skill
    /// (never used by bots). Item-granted skills are welcome here: the orb or scroll in the bot's
    /// backpack is the gate, exactly as for a human consuming it. Passive boosts, event skills, enemy
    /// debuffs and utility skills are left out.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    public static bool MayBotOwnSkill(Skill skill)
    {
        if (skill.MasterDefinition is not null
            || CastleSiegeOnlySkillNumbers.Contains(skill.Number)
            || ItemOrWeaponBoundSkillNumbers.Contains(skill.Number)
            || MountRequiredSkillNumbers.Contains(skill.Number))
        {
            return false;
        }

        // Worth owning if it adds damage of its own, hits more than once, or hits more than one
        // target. Judging by AttackDamage alone would lock a Rage Fighter out of Chain Drive and
        // Dragon Roar, which carry a flat bonus of zero and four hits instead, because their damage
        // comes from the weapon - which is also how the server pays them out.
        if (IsAttackSkill(skill))
        {
            return skill.AttackDamage > 0
                   || skill.NumberOfHitsPerAttack > 1
                   || IsAreaSkill(skill);
        }

        return skill.SkillType is SkillType.Buff or SkillType.Regeneration
               && skill.MagicEffectDef is not null
               && !ExcludedBuffSkillNumbers.Contains(skill.Number);
    }

    /// <summary>
    /// Collects the items granting each skill, for one generation run. Built by the caller alongside
    /// <see cref="GetItemGrantedSkillNumbers"/> and handed into <see cref="MayBackfillSkill"/> - no
    /// static cache: generation runs rarely, and a cache keyed by the configuration would pin the whole
    /// object graph for the process lifetime (and go stale against in-place admin-panel edits).
    /// </summary>
    /// <param name="gameConfiguration">The game configuration which defines the items.</param>
    public static IReadOnlyDictionary<short, List<ItemDefinition>> GetGrantingItems(GameConfiguration gameConfiguration)
        => (gameConfiguration.Items ?? [])
            .Where(item => item.Skill is not null)
            .GroupBy(item => item.Skill!.Number)
            .ToDictionary(group => group.Key, group => group.ToList());

    /// <summary>
    /// Determines whether the skill may be granted when a bot is generated (the backfill of what it
    /// would have looted on its way up). A skill with no granting item at all passes; a skill granted
    /// only by worn equipment or a pet never passes - those are learned temporarily by equipping the
    /// item (which the server handles on its own), so writing them into the learned skills would make
    /// them permanent. A consumable (orb/scroll) grant passes only when the skill carries requirements
    /// of its own (otherwise the gate lives on the orb alone, and the loot path teaches it) and at
    /// least one granting orb or scroll is obtainable: class-qualified, drop level reached, and the
    /// item's own requirements met.
    /// </summary>
    /// <param name="skill">The skill to check.</param>
    /// <param name="grantingItems">The granting items by skill number, see <see cref="GetGrantingItems"/>.</param>
    /// <param name="characterClass">The bot's character class.</param>
    /// <param name="level">The bot's character level.</param>
    /// <param name="getAttributeValue">Resolves an attribute's current value; null means unknown and fails.</param>
    public static bool MayBackfillSkill(
        Skill skill,
        IReadOnlyDictionary<short, List<ItemDefinition>> grantingItems,
        CharacterClass characterClass,
        int level,
        Func<AttributeDefinition, float?> getAttributeValue)
    {
        if (!grantingItems.TryGetValue(skill.Number, out var granting) || granting.Count == 0)
        {
            return true;
        }

        var consumable = granting.Where(IsConsumableSkillGrant).ToList();
        if (consumable.Count == 0)
        {
            return false;
        }

        if (skill.Requirements is not { Count: > 0 })
        {
            return false;
        }

        return consumable.Any(item => IsObtainableGrantingItem(item, characterClass, level, getAttributeValue));
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
        if (attribute == Stats.TotalEnergy || attribute == Stats.TotalEnergyRequirementValue)
        {
            return Stats.BaseEnergy;
        }

        if (attribute == Stats.TotalStrength || attribute == Stats.TotalStrengthRequirementValue)
        {
            return Stats.BaseStrength;
        }

        if (attribute == Stats.TotalAgility || attribute == Stats.TotalAgilityRequirementValue)
        {
            return Stats.BaseAgility;
        }

        if (attribute == Stats.TotalVitality || attribute == Stats.TotalVitalityRequirementValue)
        {
            return Stats.BaseVitality;
        }

        if (attribute == Stats.TotalLeadership || attribute == Stats.TotalLeadershipRequirementValue)
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
    /// <param name="itemGroup">The item group of the weapon.</param>
    public static bool IsPreferredWeaponGroup(CharacterClass characterClass, string characterName, byte itemGroup)
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
        var primaryStat = GetStatWeights(characterClass, characterName)[0].Stat;
        if (primaryStat == Stats.BaseEnergy || energy > strength)
        {
            return itemGroup == staffGroup;
        }

        return itemGroup <= maxMeleeGroup;
    }

    private static bool IsConsumableSkillGrant(ItemDefinition item)
        => item.Group == SkillOrbItemGroup || item.Group == SkillScrollItemGroup;

    private static bool IsObtainableGrantingItem(
        ItemDefinition item,
        CharacterClass characterClass,
        int level,
        Func<AttributeDefinition, float?> getAttributeValue)
    {
        if (!item.QualifiedCharacters.Contains(characterClass))
        {
            return false;
        }

        if (level < item.DropLevel)
        {
            return false;
        }

        // The caller's getAttributeValue resolves TOTAL attributes (at generation time from base
        // stats via TotalToBaseStat) - exactly what MeetsRequirements expects. Item requirements use
        // the same totals, except scrolls which use the *RequirementValue variants, so those are
        // normalized first. Level is resolved from the passed level, which is also what the callers
        // map Stats.Level to.
        // Comparing the raw MinimumValue matches the real consume gate only because the caller
        // (MayBackfillSkill) restricts this to non-wearable consumables: there GetRequirement
        // returns the minimum unchanged, while wearable granting items scale it by item level and
        // options (see ItemExtensions.GetRequirement) - which is why those never reach this check.
        foreach (var requirement in item.Requirements)
        {
            if (requirement.Attribute is not { } attribute)
            {
                continue;
            }

            if (attribute == Stats.Level)
            {
                if (level < requirement.MinimumValue)
                {
                    return false;
                }

                continue;
            }

            var totalAttribute = NormalizeRequirementValue(attribute);
            if (getAttributeValue(totalAttribute) is not { } value || value < requirement.MinimumValue)
            {
                return false;
            }
        }

        return true;
    }

    private static AttributeDefinition NormalizeRequirementValue(AttributeDefinition attribute)
    {
        if (attribute == Stats.TotalEnergyRequirementValue)
        {
            return Stats.TotalEnergy;
        }

        if (attribute == Stats.TotalStrengthRequirementValue)
        {
            return Stats.TotalStrength;
        }

        if (attribute == Stats.TotalAgilityRequirementValue)
        {
            return Stats.TotalAgility;
        }

        if (attribute == Stats.TotalVitalityRequirementValue)
        {
            return Stats.TotalVitality;
        }

        if (attribute == Stats.TotalLeadershipRequirementValue)
        {
            return Stats.TotalLeadership;
        }

        return attribute;
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
