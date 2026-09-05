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
    private static readonly IReadOnlySet<short> NoItemGrantedSkills = new HashSet<short>();
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
    /// Tests that skills only ever obtained by consuming an orb/scroll or equipping a weapon or pet
    /// carrying the skill are never learned for free: the gate lives on that item, not on the skill, so
    /// <see cref="MeetsRequirements"/> would find nothing to fail. The set of item-granted skills comes
    /// from the game configuration (<see cref="BotProgression.GetItemGrantedSkillNumbers"/>), which
    /// covers orbs, scrolls, weapons and pets alike.
    /// </summary>
    /// <param name="skillNumber">The number of an item-granted skill.</param>
    [TestCase((short)41, "Twisting Slash")]
    [TestCase((short)51, "Ice Arrow")]
    [TestCase((short)55, "Fire Slash")]
    [TestCase((short)56, "Power Slash")]
    [TestCase((short)62, "Earthshake")]
    [TestCase((short)66, "Force Wave")]
    [TestCase((short)260, "Killing Blow")]
    [TestCase((short)261, "Beast Uppercut")]
    [TestCase((short)270, "Phoenix Shot")]
    public void IsBotLearnableSkill_ItemGrantedSkillWithoutOwnRequirements_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, new HashSet<short> { skillNumber }), Is.False);
    }

    /// <summary>
    /// Tests that a skill whose granting item is not modeled in the configuration stays excluded even
    /// when the configuration yields no item for it. Its number sits in the explicit list, because the
    /// missing item would otherwise give it away for free.
    /// </summary>
    [Test]
    public void IsBotLearnableSkill_ExplicitlyExcludedWithoutGrantingItem_ReturnsFalse()
    {
        var skill = new Skill
        {
            Number = 270,
            Name = "Phoenix Shot",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, NoItemGrantedSkills), Is.False);
    }

    /// <summary>
    /// Tests that an item-granted skill which also carries requirements of its own stays learnable: the
    /// item is not the gate, the requirements are. Rageful Blow is granted by an orb yet demands level
    /// 170 (see the initialization), and a bot meeting that requirement may use it like any player.
    /// </summary>
    [Test]
    public void IsBotLearnableSkill_ItemGrantedSkillWithOwnRequirements_ReturnsTrue()
    {
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 170 })
        {
            Number = 42,
            Name = "Rageful Blow",
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 60,
            NumberOfHitsPerAttack = 1,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, new HashSet<short> { 42 }), Is.True);
    }

    /// <summary>
    /// Tests that the skills the initialization marks "active in castle siege" are never learned or cast
    /// by a hunting bot - the client refuses them outside a siege, so a bot using one does something no
    /// player can do.
    /// </summary>
    /// <param name="skillNumber">The number of a siege-marked attack skill.</param>
    [TestCase((short)44, "Crescent Moon Slash")]
    [TestCase((short)45, "Lance")]
    [TestCase((short)46, "Starfall")]
    [TestCase((short)57, "Spiral Slash")]
    [TestCase((short)73, "Mana Rays")]
    [TestCase((short)74, "Fire Blast")]
    [TestCase((short)269, "Charge")]
    public void IsBotLearnableSkill_SiegeMarkedSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 4,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, NoItemGrantedSkills), Is.False);
    }

    /// <summary>
    /// Tests that the siege role skills stay out as well - they are handed out for the siege and are not
    /// attacks at all.
    /// </summary>
    /// <param name="skillNumber">The number of a siege guild-role skill.</param>
    [TestCase((short)67, "Stun")]
    [TestCase((short)68, "Cancel Stun")]
    [TestCase((short)69, "Swell Mana")]
    [TestCase((short)70, "Invisibility")]
    [TestCase((short)71, "Cancel Invisibility")]
    [TestCase((short)72, "Abolish Magic")]
    public void IsBotLearnableSkill_CastleSiegeRoleSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.AreaSkillAutomaticHits,
            AttackDamage = 0,
            NumberOfHitsPerAttack = 1,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, NoItemGrantedSkills), Is.False);
    }

    /// <summary>
    /// Tests that mount-bound skills are never learned by a bot - a bot fighting with one on foot
    /// does something no player can do.
    /// </summary>
    [TestCase((short)47, "Impale", 28)]
    [TestCase((short)49, "Fire Breath", 110)]
    [TestCase((short)76, "Plasma Storm", 110)]
    public void IsBotLearnableSkill_MountRequiredSkill_ReturnsFalse(short skillNumber, string name, int levelRequirement)
    {
        var skill = new SkillWithRequirements(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = levelRequirement })
        {
            Number = skillNumber,
            Name = name,
            SkillType = SkillType.DirectHit,
            AttackDamage = 15,
            NumberOfHitsPerAttack = 1,
        };

        Assert.That(BotProgression.IsBotLearnableSkill(skill, NoItemGrantedSkills), Is.False);
        Assert.That(BotProgression.RequiresMount(skill), Is.True);
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
    /// Tests that a skill with no granting item is not item-gated.
    /// </summary>
    [Test]
    public void IsGrantingItemObtainable_NoGrantingItem_ReturnsTrue()
    {
        var config = new GameConfiguration();
        var characterClass = new CharacterClass();
        var skill = new Skill { Number = 9, Name = "Evil Spirit" };

        Assert.That(BotProgression.IsGrantingItemObtainable(skill, config, characterClass, 1, _ => 0f), Is.True);
    }

    /// <summary>
    /// Tests that a bot below the granting item's drop level cannot have the skill yet: with the item
    /// dropping from level 50 on, a low-level bot with enough energy still must wait.
    /// </summary>
    [Test]
    public void IsGrantingItemObtainable_BelowDropLevel_ReturnsFalse()
    {
        var (config, characterClass, skill) = CreateEvilSpiritSetup(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.IsGrantingItemObtainable(skill, config, characterClass, 30, GetValue), Is.False);
    }

    /// <summary>
    /// Tests that the same bot may learn the skill once it reaches the drop level with the required energy.
    /// </summary>
    [Test]
    public void IsGrantingItemObtainable_AtDropLevelWithRequirements_ReturnsTrue()
    {
        var (config, characterClass, skill) = CreateEvilSpiritSetup(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.IsGrantingItemObtainable(skill, config, characterClass, 50, GetValue), Is.True);
    }

    /// <summary>
    /// Tests that the granting item must accept the bot's class.
    /// </summary>
    [Test]
    public void IsGrantingItemObtainable_WrongClass_ReturnsFalse()
    {
        var (config, _, skill) = CreateEvilSpiritSetup(dropLevel: 50);
        var otherClass = new CharacterClass { Number = 4 };
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 300f : null;

        Assert.That(BotProgression.IsGrantingItemObtainable(skill, config, otherClass, 50, GetValue), Is.False);
    }

    /// <summary>
    /// Tests that the item's own requirements gate the skill: without the required energy the scroll
    /// could not have been consumed, even at the drop level.
    /// </summary>
    [Test]
    public void IsGrantingItemObtainable_RequirementsNotMet_ReturnsFalse()
    {
        var (config, characterClass, skill) = CreateEvilSpiritSetup(dropLevel: 50);
        float? GetValue(AttributeDefinition attribute) => attribute == Stats.TotalEnergy ? 100f : null;

        Assert.That(BotProgression.IsGrantingItemObtainable(skill, config, characterClass, 50, GetValue), Is.False);
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

    private static (GameConfiguration Config, CharacterClass CharacterClass, Skill Skill) CreateEvilSpiritSetup(byte dropLevel)
    {
        var config = new TestGameConfiguration();
        var characterClass = new CharacterClass { Number = 0 };
        var skill = new Skill { Number = 9, Name = "Evil Spirit" };
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
        config.Items.Add(scroll);
        return (config, characterClass, skill);
    }

    private sealed class TestGameConfiguration : GameConfiguration
    {
        public TestGameConfiguration()
        {
            this.Items = new List<ItemDefinition>();
        }
    }

    private sealed class TestItemDefinition : ItemDefinition
    {
        public TestItemDefinition()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
            this.BasePowerUpAttributes = new List<ItemBasePowerUpDefinition>();
        }
    }

    /// <summary>
    /// Tests that mount-bound skills are never lootable: bots never use them, so the orb must stay
    /// on the ground.
    /// </summary>
    [TestCase((short)47, "Impale")]
    [TestCase((short)49, "Fire Breath")]
    [TestCase((short)76, "Plasma Storm")]
    public void IsBotLootableSkill_MountRequiredSkill_ReturnsFalse(short skillNumber, string name)
    {
        var skill = new Skill { Number = skillNumber, Name = name, SkillType = SkillType.DirectHit, AttackDamage = 15, NumberOfHitsPerAttack = 1 };

        Assert.That(BotProgression.IsBotLootableSkill(skill), Is.False);
        Assert.That(BotProgression.RequiresMount(skill), Is.True);
    }

    /// <summary>
    /// Tests that an ordinary orb-gated attack skill is lootable - including one the level-up
    /// progression would never grant for free because the gate lives on its orb alone (it carries
    /// no skill requirements of its own).
    /// </summary>
    [TestCase((short)9, "Evil Spirit", SkillType.AreaSkillAutomaticHits, 45)]
    [TestCase((short)41, "Twisting Slash", SkillType.AreaSkillAutomaticHits, 0)]
    public void IsBotLootableSkill_OrbGatedAttackSkill_ReturnsTrue(short skillNumber, string name, SkillType skillType, int attackDamage)
    {
        var skill = new Skill { Number = skillNumber, Name = name, SkillType = skillType, AttackDamage = attackDamage, NumberOfHitsPerAttack = 1 };

        Assert.That(BotProgression.IsBotLootableSkill(skill), Is.True);
    }

    /// <summary>
    /// Tests that non-combat skills stay out of the loot rotation: summons, excluded buffs and
    /// siege-only attacks alike.
    /// </summary>
    [Test]
    public void IsBotLootableSkill_NonCombatSkill_ReturnsFalse()
    {
        var summonGoblin = new Skill { Number = 30, Name = "Summon Goblin", SkillType = SkillType.SummonMonster, AttackDamage = 0 };
        var defense = new Skill { Number = 18, Name = "Defense", SkillType = SkillType.Buff, AttackDamage = 0, MagicEffectDef = new MagicEffectDefinition() };
        var crescentMoon = new Skill { Number = 44, Name = "Crescent Moon Slash", SkillType = SkillType.DirectHit, AttackDamage = 90 };

        Assert.That(BotProgression.IsBotLootableSkill(summonGoblin), Is.False);
        Assert.That(BotProgression.IsBotLootableSkill(defense), Is.False);
        Assert.That(BotProgression.IsBotLootableSkill(crescentMoon), Is.False);
    }

    /// <summary>
    /// Tests that a castable class buff with a magic effect is lootable from its orb.
    /// </summary>
    [Test]
    public void IsBotLootableSkill_CastableBuff_ReturnsTrue()
    {
        var greaterDefense = new Skill { Number = 27, Name = "Greater Defense", SkillType = SkillType.Buff, AttackDamage = 0, MagicEffectDef = new MagicEffectDefinition() };

        Assert.That(BotProgression.IsBotLootableSkill(greaterDefense), Is.True);
    }

    private sealed class SkillWithRequirements : Skill
    {
        public SkillWithRequirements(AttributeRequirement requirement)
        {
            this.Requirements = new List<AttributeRequirement> { requirement };
        }
    }
}
