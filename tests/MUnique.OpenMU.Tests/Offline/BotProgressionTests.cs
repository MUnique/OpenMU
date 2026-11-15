// <copyright file="BotProgressionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests for <see cref="BotProgression"/>: the point split with capacities and the per-bot rolls.
/// </summary>
[TestFixture]
public class BotProgressionTests
{
    /// <summary>The Dark Wizard build of <see cref="BotProgression.GetStatWeights"/>.</summary>
    private static readonly (AttributeDefinition Stat, int Weight)[] DarkWizardWeights =
        [(Stats.BaseEnergy, 66), (Stats.BaseVitality, 22), (Stats.BaseAgility, 8), (Stats.BaseStrength, 4)];

    /// <summary>The Dark Lord build of <see cref="BotProgression.GetStatWeights"/>.</summary>
    private static readonly (AttributeDefinition Stat, int Weight)[] DarkLordWeights =
        [(Stats.BaseStrength, 38), (Stats.BaseLeadership, 30), (Stats.BaseVitality, 22), (Stats.BaseAgility, 8), (Stats.BaseEnergy, 2)];

    /// <summary>
    /// Tests that the split assigns all points proportionally when nothing is capped.
    /// </summary>
    [Test]
    public void SplitPoints_AssignsAllPointsProportionally()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 35), (Stats.BaseVitality, 5) };

        var result = BotProgression.SplitPoints(1000, weights).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(1000));
        Assert.That(result[Stats.BaseAgility], Is.EqualTo(350));
        Assert.That(result[Stats.BaseVitality], Is.EqualTo(50));
        Assert.That(result[Stats.BaseStrength], Is.EqualTo(600));
    }

    /// <summary>
    /// Tests that a capped stat drops out of the split and its share flows to the remaining stats.
    /// </summary>
    [Test]
    public void SplitPoints_CappedStatOverflowsToOthers()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 35), (Stats.BaseVitality, 5) };
        long CapacityOf(AttributeDefinition stat) => stat == Stats.BaseVitality ? 10 : long.MaxValue;

        var result = BotProgression.SplitPoints(1000, weights, CapacityOf).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result[Stats.BaseVitality], Is.EqualTo(10));
        Assert.That(result.Values.Sum(), Is.EqualTo(1000));
        Assert.That(result[Stats.BaseStrength] + result[Stats.BaseAgility], Is.EqualTo(990));
    }

    /// <summary>
    /// Tests that points stay unassigned when every stat is at its capacity, like for a maxed character.
    /// </summary>
    [Test]
    public void SplitPoints_AllCapped_LeavesPointsUnassigned()
    {
        var weights = new[] { (Stats.BaseStrength, 60), (Stats.BaseAgility, 40) };

        // Every stat is capped at the same value, so which one is asked for does not matter.
        Func<AttributeDefinition, long> capacityOf = _ => 25;

        var result = BotProgression.SplitPoints(1000, weights, capacityOf).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(50));
        Assert.That(result.Values, Is.All.EqualTo(25));
    }

    /// <summary>
    /// Tests that a bot which gets its points level-up by level-up (5 or 7 at a time) ends up with the
    /// same build as one which gets them all at once: the small-weight stats used to round down to 0
    /// on every level-up and stayed at the class base forever.
    /// </summary>
    /// <param name="pointsPerLevel">The points granted per level-up.</param>
    /// <param name="useDarkLordBuild">True for the five-stat Dark Lord build, false for the Dark Wizard build.</param>
    [TestCase(5, false)]
    [TestCase(7, true)]
    public void SplitPoints_LevelUpsOneByOne_MatchSplitAtOnce(int pointsPerLevel, bool useDarkLordBuild)
    {
        var weights = useDarkLordBuild ? DarkLordWeights : DarkWizardWeights;
        const int levelUps = 399;

        var oneByOne = LevelUp(weights, levelUps, pointsPerLevel);
        var atOnce = BotProgression.SplitPoints(levelUps * pointsPerLevel, weights).ToDictionary(r => r.Stat, r => (long)r.Amount);

        Assert.That(oneByOne.Values.Sum(), Is.EqualTo(levelUps * pointsPerLevel));
        foreach (var (stat, _) in weights)
        {
            Assert.That(oneByOne[stat], Is.EqualTo(atOnce[stat]).Within(1), stat.Designation);
            Assert.That(oneByOne[stat], Is.GreaterThan(0), stat.Designation);
        }
    }

    /// <summary>
    /// Tests that already invested points above a stat's share make it skip the split, so a bot whose
    /// stats are off the build (e.g. a caster with no strength from the old rounding) is evened out.
    /// </summary>
    [Test]
    public void SplitPoints_SkewedStats_AreEvenedOut()
    {
        var invested = new Dictionary<AttributeDefinition, long>
        {
            { Stats.BaseEnergy, 1000 },
            { Stats.BaseVitality, 300 },
        };

        var result = BotProgression.SplitPoints(50, DarkWizardWeights, investedOf: s => invested.GetValueOrDefault(s))
            .ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(50));
        Assert.That(result.ContainsKey(Stats.BaseEnergy), Is.False);
        Assert.That(result.ContainsKey(Stats.BaseVitality), Is.False);
        Assert.That(result[Stats.BaseAgility], Is.GreaterThan(result[Stats.BaseStrength]));
    }

    /// <summary>
    /// Tests that the pool of a reset which keeps the stats brings a skewed character to the build's
    /// proportions of its whole investment, instead of adding a proportional slice on top of the skew.
    /// </summary>
    [Test]
    public void SplitPoints_ResetKeepingStats_ReachesBuildProportions()
    {
        var invested = new Dictionary<AttributeDefinition, long>
        {
            { Stats.BaseEnergy, 1600 },
            { Stats.BaseVitality, 400 },
        };
        const int resetPoints = 2000;

        foreach (var (stat, amount) in BotProgression.SplitPoints(resetPoints, DarkWizardWeights, investedOf: s => invested.GetValueOrDefault(s)))
        {
            invested[stat] = invested.GetValueOrDefault(stat) + amount;
        }

        var total = invested.Values.Sum();
        Assert.That(total, Is.EqualTo(1600 + 400 + resetPoints));
        foreach (var (stat, weight) in DarkWizardWeights)
        {
            Assert.That(invested[stat], Is.EqualTo(total * weight / 100).Within(1), stat.Designation);
        }
    }

    /// <summary>
    /// Tests that the vitality target of a reset-meta server still holds when it is reached over
    /// many level-ups: vitality stops at the target and its share flows into the rest of the build.
    /// </summary>
    [Test]
    public void SplitPoints_LevelUpsUpToVitalityTarget_OverflowToOthers()
    {
        const long vitalityTarget = 100;

        var result = LevelUp(DarkWizardWeights, 399, 5, (stat, invested) => stat == Stats.BaseVitality ? vitalityTarget - invested : long.MaxValue);

        Assert.That(result[Stats.BaseVitality], Is.EqualTo(vitalityTarget));
        Assert.That(result.Values.Sum(), Is.EqualTo(399 * 5));
        Assert.That(result[Stats.BaseStrength], Is.GreaterThan(0));
    }

    /// <summary>
    /// Tests that a leftover point which no share covers goes to the primary stat on a tie.
    /// </summary>
    [Test]
    public void SplitPoints_TieGoesToPrimaryStat()
    {
        var weights = new[] { (Stats.BaseStrength, 50), (Stats.BaseAgility, 50) };

        var result = BotProgression.SplitPoints(1, weights).Single();

        Assert.That(result, Is.EqualTo((Stats.BaseStrength, 1)));
    }

    /// <summary>
    /// Tests that the minimums (gear requirements) are served before the weighted split.
    /// </summary>
    [Test]
    public void SplitPoints_MinimumsAreServedFirst()
    {
        var minimums = new Dictionary<AttributeDefinition, long> { { Stats.BaseStrength, 100 } };

        var result = BotProgression.SplitPoints(200, DarkWizardWeights, minimums: minimums).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(200));
        Assert.That(result[Stats.BaseStrength], Is.EqualTo(100));
        Assert.That(result[Stats.BaseEnergy], Is.GreaterThan(result.Where(r => r.Key != Stats.BaseEnergy && r.Key != Stats.BaseStrength).Sum(r => r.Value)));
    }

    /// <summary>
    /// Tests that the points are shared out by what is missing when they don't cover all minimums.
    /// </summary>
    [Test]
    public void SplitPoints_MinimumsBeyondThePoints_AreSharedByWhatIsMissing()
    {
        var minimums = new Dictionary<AttributeDefinition, long> { { Stats.BaseStrength, 300 }, { Stats.BaseAgility, 100 } };

        var result = BotProgression.SplitPoints(40, DarkWizardWeights, minimums: minimums).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[Stats.BaseStrength], Is.EqualTo(30));
        Assert.That(result[Stats.BaseAgility], Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that already invested points count towards a minimum.
    /// </summary>
    [Test]
    public void SplitPoints_InvestedPointsCountTowardsTheMinimum()
    {
        var minimums = new Dictionary<AttributeDefinition, long> { { Stats.BaseStrength, 100 } };
        var invested = new Dictionary<AttributeDefinition, long> { { Stats.BaseStrength, 90 }, { Stats.BaseEnergy, 2000 } };

        var result = BotProgression.SplitPoints(5, DarkWizardWeights, investedOf: s => invested.GetValueOrDefault(s), minimums: minimums)
            .ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(5));
        Assert.That(result[Stats.BaseStrength], Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that a stat outside the build gets its minimum and nothing beyond it: a Rage Fighter
    /// invests no agility, yet a Brass Armor requires some.
    /// </summary>
    [Test]
    public void SplitPoints_MinimumOfStatOutsideTheBuild_GetsOnlyTheMinimum()
    {
        var weights = new[] { (Stats.BaseStrength, 45), (Stats.BaseVitality, 35), (Stats.BaseEnergy, 20) };
        var minimums = new Dictionary<AttributeDefinition, long> { { Stats.BaseAgility, 50 } };

        var result = BotProgression.SplitPoints(1000, weights, minimums: minimums).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result.Values.Sum(), Is.EqualTo(1000));
        Assert.That(result[Stats.BaseAgility], Is.EqualTo(50));
    }

    /// <summary>
    /// Tests that a minimum stays within the stat's capacity (a fun server's stat maximum).
    /// </summary>
    [Test]
    public void SplitPoints_MinimumRespectsCapacity()
    {
        var minimums = new Dictionary<AttributeDefinition, long> { { Stats.BaseStrength, 100 } };
        long CapacityOf(AttributeDefinition stat) => stat == Stats.BaseStrength ? 30 : long.MaxValue;

        var result = BotProgression.SplitPoints(200, DarkWizardWeights, CapacityOf, minimums: minimums).ToDictionary(r => r.Stat, r => r.Amount);

        Assert.That(result[Stats.BaseStrength], Is.EqualTo(30));
        Assert.That(result.Values.Sum(), Is.EqualTo(200));
    }

    /// <summary>
    /// Tests that the vitality target roll stays within 100..500 and is stable for the same name.
    /// </summary>
    [Test]
    public void GetVitalityTarget_IsStableAndWithinRange()
    {
        foreach (var name in new[] { "Kaeoris", "Milynara", "Hallin", "Oriwen", "X" })
        {
            var target = BotProgression.GetVitalityTarget(name);
            Assert.That(target, Is.InRange(100, 500), name);
            Assert.That(BotProgression.GetVitalityTarget(name), Is.EqualTo(target), name);
        }
    }

    /// <summary>
    /// Tests that orb/scroll skills carrying no requirements of their own are never backfilled at
    /// generation: the gate lives on the orb or scroll alone, so the loot path teaches them instead.
    /// </summary>
    /// <param name="skillNumber">The number of an orb-gated skill.</param>
    /// <param name="name">The name of the skill.</param>
    [TestCase((short)41, "Twisting Slash")]
    [TestCase((short)51, "Ice Arrow")]
    [TestCase((short)55, "Fire Slash")]
    [TestCase((short)56, "Power Slash")]
    [TestCase((short)62, "Earthshake")]
    [TestCase((short)66, "Force Wave")]
    [TestCase((short)260, "Killing Blow")]
    [TestCase((short)261, "Beast Uppercut")]
    [TestCase((short)270, "Phoenix Shot")]
    public void MayBackfillSkill_ConsumableGrantWithoutSkillRequirements_ReturnsFalse(short skillNumber, string name)
    {
        var characterClass = new CharacterClass();
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };
        var orb = new TestItemDefinition
        {
            Group = 12,
            Number = 1,
            DropLevel = 1,
            Skill = skill,
        };
        orb.QualifiedCharacters.Add(characterClass);
        var grants = new Dictionary<short, List<ItemDefinition>> { [skillNumber] = new List<ItemDefinition> { orb } };

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 1, _ => 0f), Is.False);
    }

    /// <summary>
    /// Tests that a skill whose granting item is not modeled in the configuration stays excluded even
    /// when the configuration yields no item for it. Its number sits in the explicit list, because the
    /// missing item would otherwise give it away for free.
    /// </summary>
    [Test]
    public void MayBotOwnSkill_ExplicitlyExcludedWithoutGrantingItem_ReturnsFalse()
    {
        var skill = new Skill
        {
            Number = 270,
            Name = "Phoenix Shot",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };

        Assert.That(BotProgression.MayBotOwnSkill(skill), Is.False);
    }

    /// <summary>
    /// Tests that an orb/scroll skill which also carries requirements of its own is backfilled once its
    /// granting orb is obtainable: the item is not the only gate, the requirements are. Rageful Blow is
    /// granted by an orb yet demands level 170 (see the initialization), and a bot meeting that
    /// requirement may use it like any player.
    /// </summary>
    [Test]
    public void MayBackfillSkill_ObtainableOrbWithSkillRequirements_ReturnsTrue()
    {
        var characterClass = new CharacterClass();
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 170 })
        {
            Number = 42,
            Name = "Rageful Blow",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 60,
            NumberOfHitsPerAttack = 1,
        };
        var orb = new TestItemDefinition
        {
            Group = 12,
            Number = 12,
            DropLevel = 78,
            Skill = skill,
        };
        orb.QualifiedCharacters.Add(characterClass);
        orb.Requirements.Add(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 170 });
        var grants = new Dictionary<short, List<ItemDefinition>> { [42] = new List<ItemDefinition> { orb } };

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 170, _ => 0f), Is.True);
    }

    /// <summary>
    /// Tests that the skills the initialization marks "active in castle siege" are never learned or cast
    /// by a hunting bot - the client refuses them outside a siege, so a bot using one does something no
    /// player can do.
    /// </summary>
    /// <param name="skillNumber">The number of a siege-marked attack skill.</param>
    /// <param name="name">The name of the skill.</param>
    [TestCase((short)44, "Crescent Moon Slash")]
    [TestCase((short)45, "Lance")]
    [TestCase((short)46, "Starfall")]
    [TestCase((short)57, "Spiral Slash")]
    [TestCase((short)73, "Mana Rays")]
    [TestCase((short)74, "Fire Blast")]
    [TestCase((short)269, "Charge")]
    public void MayBotOwnSkill_SiegeMarkedSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };

        Assert.That(BotProgression.MayBotOwnSkill(skill), Is.False);
    }

    /// <summary>
    /// Tests that the siege role skills stay out as well - they are handed out for the siege and are not
    /// attacks at all.
    /// </summary>
    /// <param name="skillNumber">The number of a siege guild-role skill.</param>
    /// <param name="name">The name of the skill.</param>
    [TestCase((short)67, "Stun")]
    [TestCase((short)68, "Cancel Stun")]
    [TestCase((short)69, "Swell Mana")]
    [TestCase((short)70, "Invisibility")]
    [TestCase((short)71, "Cancel Invisibility")]
    [TestCase((short)72, "Abolish Magic")]
    public void MayBotOwnSkill_CastleSiegeRoleSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 1,
        };

        Assert.That(BotProgression.MayBotOwnSkill(skill), Is.False);
    }

    /// <summary>
    /// Tests that ordinary skills carry no mount requirement.
    /// </summary>
    [Test]
    public void RequiresMount_OrdinarySkill_ReturnsFalse()
    {
        var evilSpirit = new Skill
        {
            Number = 9,
            Name = "Evil Spirit",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 45,
            NumberOfHitsPerAttack = 1,
        };

        Assert.That(BotProgression.RequiresMount(evilSpirit), Is.False);
    }

    /// <summary>
    /// Tests that a skill with no granting item at all is backfilled freely.
    /// </summary>
    [Test]
    public void MayBackfillSkill_NoGrantingItem_ReturnsTrue()
    {
        var grants = new Dictionary<short, List<ItemDefinition>>();
        var skill = new Skill { Number = 9, Name = "Evil Spirit" };

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, new CharacterClass(), 1, _ => 0f), Is.True);
    }

    /// <summary>
    /// Tests that a bot below the granting orb/scroll's drop level is not backfilled: with the item
    /// dropping from level 50 on, a low-level bot with enough energy still must wait for the loot path.
    /// </summary>
    [Test]
    public void MayBackfillSkill_BelowDropLevel_ReturnsFalse()
    {
        var (grants, characterClass, skill) = CreateScrollGrant(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 30, GetValue), Is.False);
    }

    /// <summary>
    /// Tests that the same bot is backfilled once it reaches the drop level with the required energy.
    /// </summary>
    [Test]
    public void MayBackfillSkill_AtDropLevelWithRequirements_ReturnsTrue()
    {
        var (grants, characterClass, skill) = CreateScrollGrant(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 50, GetValue), Is.True);
    }

    /// <summary>
    /// Tests that the granting orb/scroll must accept the bot's class.
    /// </summary>
    [Test]
    public void MayBackfillSkill_WrongClass_ReturnsFalse()
    {
        var (grants, _, skill) = CreateScrollGrant(dropLevel: 50);
        var otherClass = new CharacterClass { Number = 4 };
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, otherClass, 50, GetValue), Is.False);
    }

    /// <summary>
    /// Tests that the orb/scroll's own requirements gate the backfill: without the required energy the
    /// scroll could not have been consumed, even at the drop level.
    /// </summary>
    [Test]
    public void MayBackfillSkill_RequirementsNotMet_ReturnsFalse()
    {
        var (grants, characterClass, skill) = CreateScrollGrant(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 100f : null;

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 50, GetValue), Is.False);
    }

    /// <summary>
    /// Tests that a skill granted only by worn equipment or a pet is never backfilled, even with stat
    /// requirements of its own: those are learned temporarily by equipping the item, so writing them
    /// into the learned skills would make them permanent.
    /// </summary>
    [Test]
    public void MayBackfillSkill_EquipmentGrantedOnly_ReturnsFalse()
    {
        var characterClass = new CharacterClass { Number = 4 };
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 110 })
        {
            Number = 62,
            Name = "Earthshake",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 150,
            NumberOfHitsPerAttack = 1,
        };
        var pet = new TestItemDefinition
        {
            Group = 13,
            Number = 4,
            Name = "Dark Horse",
            DropLevel = 110,
            Skill = skill,
        };
        pet.QualifiedCharacters.Add(characterClass);
        var grants = new Dictionary<short, List<ItemDefinition>> { [skill.Number] = new List<ItemDefinition> { pet } };

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 110, _ => 0f), Is.False);
    }

    /// <summary>
    /// Tests that a worn-equipment grant does not block the backfill when an obtainable orb exists
    /// alongside it: the permanent skill comes from the consumable, the equipment grant is temporary.
    /// </summary>
    [Test]
    public void MayBackfillSkill_MixedGrantsWithObtainableOrb_ReturnsTrue()
    {
        var characterClass = new CharacterClass { Number = 4 };
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 170 })
        {
            Number = 42,
            Name = "Rageful Blow",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 60,
            NumberOfHitsPerAttack = 1,
        };
        var weapon = new TestItemDefinition
        {
            Group = 0,
            Number = 1,
            Name = "Blade of Rageful Blow",
            DropLevel = 78,
            Skill = skill,
        };
        weapon.QualifiedCharacters.Add(characterClass);
        var orb = new TestItemDefinition
        {
            Group = 12,
            Number = 12,
            Name = "Orb of Rageful Blow",
            DropLevel = 78,
            Skill = skill,
        };
        orb.QualifiedCharacters.Add(characterClass);
        orb.Requirements.Add(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 170 });
        var grants = new Dictionary<short, List<ItemDefinition>> { [skill.Number] = new List<ItemDefinition> { weapon, orb } };
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.Level ? 170f : null;

        Assert.That(BotProgression.MayBackfillSkill(skill, grants, characterClass, 170, GetValue), Is.True);
    }

    /// <summary>
    /// Tests that the scrolls' *RequirementValue attributes resolve to the same base stats as the
    /// skills' totals, so the generation-time lookup finds the bot's stats.
    /// </summary>
    [Test]
    public void TotalToBaseStat_RequirementValues_MapToBaseStats()
    {
        Assert.That(BotProgression.TotalToBaseStat(Stats.TotalEnergyRequirementValue), Is.EqualTo(Stats.BaseEnergy));
        Assert.That(BotProgression.TotalToBaseStat(Stats.TotalStrengthRequirementValue), Is.EqualTo(Stats.BaseStrength));
        Assert.That(BotProgression.TotalToBaseStat(Stats.TotalAgilityRequirementValue), Is.EqualTo(Stats.BaseAgility));
        Assert.That(BotProgression.TotalToBaseStat(Stats.TotalVitalityRequirementValue), Is.EqualTo(Stats.BaseVitality));
        Assert.That(BotProgression.TotalToBaseStat(Stats.TotalLeadershipRequirementValue), Is.EqualTo(Stats.BaseLeadership));
    }

    /// <summary>
    /// Tests that mount-bound skills are never lootable: bots never use them, so the orb must stay
    /// on the ground.
    /// </summary>
    [TestCase((short)47, "Impale")]
    [TestCase((short)49, "Fire Breath")]
    [TestCase((short)76, "Plasma Storm")]
    public void MayBotOwnSkill_MountRequiredSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill { Number = skillNumber, Name = name, SkillType = SkillType.DirectHit, AttackDamage = 15, NumberOfHitsPerAttack = 1 };

        Assert.That(BotProgression.MayBotOwnSkill(skill), Is.False);
        Assert.That(BotProgression.RequiresMount(skill), Is.True);
    }

    /// <summary>
    /// Tests that an ordinary orb-gated attack skill is lootable - including one the level-up
    /// progression would never grant for free because the gate lives on its orb alone (it carries
    /// no skill requirements of its own).
    /// </summary>
    [TestCase((short)9, "Evil Spirit", SkillType.AreaSkillAutomaticHits, 45)]
    [TestCase((short)41, "Twisting Slash", SkillType.AreaSkillAutomaticHits, 0)]
    public void MayBotOwnSkill_OrbGatedAttackSkill_ReturnsTrue(short skillNumber, string name, SkillType skillType, int attackDamage)
    {
        var skill = new Skill { Number = skillNumber, Name = name, SkillType = skillType, AttackDamage = attackDamage, NumberOfHitsPerAttack = 1 };

        Assert.That(BotProgression.MayBotOwnSkill(skill), Is.True);
    }

    /// <summary>
    /// Tests that non-combat skills stay out of the loot rotation: summons, excluded buffs and
    /// siege-only attacks alike.
    /// </summary>
    [Test]
    public void MayBotOwnSkill_NonCombatSkill_ReturnsFalse()
    {
        var summonGoblin = new Skill { Number = 30, Name = "Summon Goblin", SkillType = SkillType.SummonMonster, AttackDamage = 0 };
        var defense = new Skill { Number = 18, Name = "Defense", SkillType = SkillType.Buff, AttackDamage = 0, MagicEffectDef = new MagicEffectDefinition() };
        var crescentMoon = new Skill { Number = 44, Name = "Crescent Moon Slash", SkillType = SkillType.DirectHit, AttackDamage = 90 };

        Assert.That(BotProgression.MayBotOwnSkill(summonGoblin), Is.False);
        Assert.That(BotProgression.MayBotOwnSkill(defense), Is.False);
        Assert.That(BotProgression.MayBotOwnSkill(crescentMoon), Is.False);
    }

    /// <summary>
    /// Tests that a castable class buff with a magic effect is lootable from its orb.
    /// </summary>
    [Test]
    public void MayBotOwnSkill_CastableBuff_ReturnsTrue()
    {
        var greaterDefense = new Skill { Number = 27, Name = "Greater Defense", SkillType = SkillType.Buff, AttackDamage = 0, MagicEffectDef = new MagicEffectDefinition() };

        Assert.That(BotProgression.MayBotOwnSkill(greaterDefense), Is.True);
    }

    private static (Dictionary<short, List<ItemDefinition>> Grants, CharacterClass CharacterClass, Skill Skill) CreateScrollGrant(byte dropLevel)
    {
        var characterClass = new CharacterClass { Number = 0 };
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.TotalEnergy, MinimumValue = 220 })
        {
            Number = 9,
            Name = "Evil Spirit",
        };
        var scroll = new TestItemDefinition
        {
            Group = 15,
            Number = 8,
            Name = "Scroll of Evil Spirit",
            DropLevel = dropLevel,
            Skill = skill,
        };
        scroll.QualifiedCharacters.Add(characterClass);
        scroll.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 220 });
        var grants = new Dictionary<short, List<ItemDefinition>> { [skill.Number] = new List<ItemDefinition> { scroll } };
        return (grants, characterClass, skill);
    }

    private sealed class TestItemDefinition : ItemDefinition
    {
        public TestItemDefinition()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }

    private sealed class SkillWithRequirements : Skill
    {
        public SkillWithRequirements(AttributeRequirement requirement)
        {
            this.Requirements = new List<AttributeRequirement> { requirement };
        }
    }

    /// <summary>
    /// Plays the given number of level-ups the way the bot does at runtime: each one splits only its
    /// own points, knowing what was invested before.
    /// </summary>
    private static Dictionary<AttributeDefinition, long> LevelUp(
        IReadOnlyList<(AttributeDefinition Stat, int Weight)> weights,
        int levelUps,
        int pointsPerLevel,
        Func<AttributeDefinition, long, long>? capacityOf = null)
    {
        var invested = weights.ToDictionary(w => w.Stat, _ => 0L);
        for (var level = 0; level < levelUps; level++)
        {
            Func<AttributeDefinition, long>? capacity = capacityOf is null ? null : s => capacityOf(s, invested[s]);
            foreach (var (stat, amount) in BotProgression.SplitPoints(pointsPerLevel, weights, capacity, s => invested[s]))
            {
                invested[stat] += amount;
            }
        }

        return invested;
    }
}
