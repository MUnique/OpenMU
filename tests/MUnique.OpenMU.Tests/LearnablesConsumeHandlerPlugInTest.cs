// <copyright file="LearnablesConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests that a skill can only be learned with an item when the player could also use it.
/// </summary>
[TestFixture]
public class LearnablesConsumeHandlerPlugInTest
{
    private const byte ItemSlot = 12;

    /// <summary>
    /// Tests that a skill is only learned with an item if the player complies with the requirements of the skill,
    /// even when the item itself has no requirements.
    /// </summary>
    /// <param name="skillEnergyRequirement">The energy requirement of the skill. The test player has 10 energy.</param>
    /// <param name="expectedLearned">If set to <c>true</c>, the skill is expected to be learned.</param>
    [TestCase(10, true)]
    [TestCase(20, false)]
    public async ValueTask SkillIsOnlyLearnedWhenItCanBeUsedAsync(int skillEnergyRequirement, bool expectedLearned)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var skill = CreateSkill(1, skillEnergyRequirement);
        var scroll = await AddItemAsync(player, CreateItemDefinition(15, 0, skill, player), 0).ConfigureAwait(false);

        var consumed = await new LearnablesConsumeHandlerPlugIn().ConsumeItemAsync(player, scroll, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(consumed, Is.EqualTo(expectedLearned));
            Assert.That(player.SkillList!.ContainsSkill((ushort)skill.Number), Is.EqualTo(expectedLearned));
            Assert.That(scroll.Durability, Is.EqualTo(expectedLearned ? 0 : 1), "the item is only consumed when the skill is learned");
        });
    }

    /// <summary>
    /// Tests that the Orb of Summoning requires the requirements of the skill of its item level.
    /// </summary>
    /// <param name="itemLevel">The level of the orb.</param>
    /// <param name="expectedLearned">If set to <c>true</c>, the skill is expected to be learned.</param>
    [TestCase(0, true)]
    [TestCase(1, false)]
    public async ValueTask SummoningOrbRequiresTheRequirementsOfTheSkillOfItsLevelAsync(byte itemLevel, bool expectedLearned)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var skills = new[] { CreateSkill(30, 10), CreateSkill(31, 20) };
        foreach (var skill in skills)
        {
            player.GameContext.Configuration.Skills.Add(skill);
        }

        var definition = CreateItemDefinition(ItemConstants.SummonOrb.Group, ItemConstants.SummonOrb.Number!.Value, skills[0], player);
        var orb = await AddItemAsync(player, definition, itemLevel).ConfigureAwait(false);

        var consumed = await new SummoningOrbConsumeHandlerPlugIn().ConsumeItemAsync(player, orb, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(consumed, Is.EqualTo(expectedLearned));
            Assert.That(player.SkillList!.ContainsSkill((ushort)skills[itemLevel].Number), Is.EqualTo(expectedLearned));
        });
    }

    private static Skill CreateSkill(short number, int energyRequirement)
    {
        var skill = new TestSkill(number);
        skill.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergy, MinimumValue = energyRequirement });
        return skill;
    }

    private static ItemDefinition CreateItemDefinition(byte group, short number, Skill skill, Player player)
    {
        var definition = new TestItemDefinition(group, number, skill);
        definition.QualifiedCharacters.Add(player.SelectedCharacter!.CharacterClass!);
        return definition;
    }

    private static async ValueTask<Item> AddItemAsync(Player player, ItemDefinition definition, byte level)
    {
        var item = new Item { Definition = definition, Durability = 1, Level = level };
        await player.Inventory!.AddItemAsync(ItemSlot, item).ConfigureAwait(false);
        return item;
    }

    private sealed class TestSkill : Skill
    {
        public TestSkill(short number)
        {
            this.Number = number;
            this.Name = $"Skill {number}";
            this.Requirements = new List<AttributeRequirement>();
            this.ConsumeRequirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }

    private sealed class TestItemDefinition : ItemDefinition
    {
        public TestItemDefinition(byte group, short number, Skill skill)
        {
            this.Group = group;
            this.Number = number;
            this.Skill = skill;
            this.Width = 1;
            this.Height = 1;
            this.MaximumItemLevel = 6;
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }
}
