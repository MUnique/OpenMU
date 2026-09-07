// <copyright file="BotSkillHandlerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Tests the loot predicate of <see cref="BotSkillHandler"/> - which dropped orbs and scrolls a bot
/// wants. The actual consumption goes through the regular consume handlers and is covered by
/// <see cref="ItemConsumptionTest"/>.
/// </summary>
[TestFixture]
public class BotSkillHandlerTest
{
    /// <summary>
    /// Tests that an orb teaching an unknown, class-qualified, lootable skill with met requirements
    /// is wanted.
    /// </summary>
    [Test]
    public async ValueTask WantsSkillItem_UnknownLootableOrb_ReturnsTrue()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var (_, orb) = CreateOrb(characterClass, 9, "Evil Spirit", 12);
        var item = new Item { Definition = orb, Durability = 1, ItemSlot = 12 };

        Assert.That(BotSkillHandler.WantsSkillItem(player, item), Is.True);
    }

    /// <summary>
    /// Tests that an orb for an already known skill is left alone.
    /// </summary>
    [Test]
    public async ValueTask WantsSkillItem_AlreadyKnownSkill_ReturnsFalse()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var (skill, orb) = CreateOrb(characterClass, 9, "Evil Spirit", 12);
        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        var item = new Item { Definition = orb, Durability = 1, ItemSlot = 12 };

        Assert.That(BotSkillHandler.WantsSkillItem(player, item), Is.False);
    }

    /// <summary>
    /// Tests that an orb for a mount-bound skill is left alone: bots never use those, even mounted.
    /// </summary>
    [Test]
    public async ValueTask WantsSkillItem_MountBoundSkill_ReturnsFalse()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var (_, orb) = CreateOrb(characterClass, 47, "Impale", 12, SkillType.DirectHit, attackDamage: 15);
        var item = new Item { Definition = orb, Durability = 1, ItemSlot = 12 };

        Assert.That(BotSkillHandler.WantsSkillItem(player, item), Is.False);
    }

    /// <summary>
    /// Tests that an orb whose requirements the bot does not meet is left alone.
    /// </summary>
    [Test]
    public async ValueTask WantsSkillItem_RequirementsUnmet_ReturnsFalse()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var (_, orb) = CreateOrb(characterClass, 9, "Evil Spirit", 12);
        orb.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 500 });
        var item = new Item { Definition = orb, Durability = 1, ItemSlot = 12 };

        Assert.That(BotSkillHandler.WantsSkillItem(player, item), Is.False);
    }

    /// <summary>
    /// Tests that only orbs and scrolls are wanted: a weapon carrying a skill teaches it temporarily
    /// on equip instead, never by looting.
    /// </summary>
    [Test]
    public async ValueTask WantsSkillItem_NonOrbScrollGroup_ReturnsFalse()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var (_, orb) = CreateOrb(characterClass, 9, "Evil Spirit", 0);
        var item = new Item { Definition = orb, Durability = 1, ItemSlot = 12 };

        Assert.That(BotSkillHandler.WantsSkillItem(player, item), Is.False);
    }

    private static (Skill Skill, TestOrbDefinition Orb) CreateOrb(CharacterClass characterClass, short skillNumber, string skillName, byte group, SkillType skillType = SkillType.AreaSkillAutomaticHits, int attackDamage = 45)
    {
        var skill = new TestSkill
        {
            Number = skillNumber,
            Name = skillName,
            SkillType = skillType,
            AttackDamage = attackDamage,
            NumberOfHitsPerAttack = 1,
        };
        skill.QualifiedCharacters.Add(characterClass);
        var orb = new TestOrbDefinition
        {
            Group = group,
            Number = 1,
            Skill = skill,
        };
        orb.QualifiedCharacters.Add(characterClass);
        return (skill, orb);
    }

    private sealed class TestSkill : Skill
    {
        public TestSkill()
        {
            this.QualifiedCharacters = new List<CharacterClass>();
            this.Requirements = new List<AttributeRequirement>();
        }
    }

    private sealed class TestOrbDefinition : ItemDefinition
    {
        public TestOrbDefinition()
        {
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }
}
