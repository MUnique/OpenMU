// <copyright file="LearnableSkillRequirementsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="LearnableSkillRequirements"/> and that a skill can only be learned
/// with an item when the player could also use it.
/// </summary>
[TestFixture]
public class LearnableSkillRequirementsTest
{
    private const byte ItemSlot = 12;

    /// <summary>
    /// Tests that learning takes the highest of the requirements of the item and of the skill,
    /// and that the requirement values of the item are compared with the total attribute.
    /// </summary>
    [Test]
    public void RequirementsAreTheHighestOfItemAndSkill()
    {
        var skill = new TestSkill(30);
        skill.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergy, MinimumValue = 90 });
        skill.Requirements.Add(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 10 });
        var item = new TestItemDefinition(15, 0, skill);
        item.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergyRequirementValue, MinimumValue = 30 });
        item.Requirements.Add(new AttributeRequirement { Attribute = Stats.Level, MinimumValue = 50 });

        var requirements = LearnableSkillRequirements.GetRequirements(item, skill);

        Assert.That(requirements[Stats.TotalEnergy], Is.EqualTo(90));
        Assert.That(requirements[Stats.Level], Is.EqualTo(50));
        Assert.That(requirements.ContainsKey(Stats.TotalEnergyRequirementValue), Is.False);
    }

    /// <summary>
    /// Tests that the Orb of Summoning teaches the skill of its item level, and other items always their own skill.
    /// </summary>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="expectedSkillNumber">The expected skill number, or 0 if no skill is expected.</param>
    [TestCase(0, 30)]
    [TestCase(2, 32)]
    [TestCase(3, 0)]
    public void SummoningOrbTeachesTheSkillOfItsLevel(byte itemLevel, short expectedSkillNumber)
    {
        var skills = new List<Skill> { new TestSkill(30), new TestSkill(31), new TestSkill(32) };
        var configuration = new Mock<GameConfiguration>();
        configuration.Setup(c => c.Skills).Returns(skills);
        var orb = new TestItemDefinition(ItemConstants.SummonOrb.Group, ItemConstants.SummonOrb.Number!.Value, skills[0]);
        var scroll = new TestItemDefinition(15, 0, skills[1]);

        var orbSkill = LearnableSkillRequirements.GetLearnableSkill(orb, itemLevel, configuration.Object);

        Assert.That(orbSkill?.Number ?? 0, Is.EqualTo(expectedSkillNumber));
        Assert.That(LearnableSkillRequirements.GetLearnableSkill(scroll, itemLevel, configuration.Object), Is.SameAs(skills[1]));
    }

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
        var skill = new TestSkill(1);
        skill.Requirements.Add(new AttributeRequirement { Attribute = Stats.TotalEnergy, MinimumValue = skillEnergyRequirement });
        var definition = new TestItemDefinition(15, 0, skill);
        definition.QualifiedCharacters.Add(player.SelectedCharacter!.CharacterClass!);
        var scroll = new Item { Definition = definition, Durability = 1 };
        await player.Inventory!.AddItemAsync(ItemSlot, scroll).ConfigureAwait(false);

        var consumed = await new LearnablesConsumeHandlerPlugIn().ConsumeItemAsync(player, scroll, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.EqualTo(expectedLearned));
        Assert.That(player.SkillList!.ContainsSkill((ushort)skill.Number), Is.EqualTo(expectedLearned));
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
            this.Requirements = new List<AttributeRequirement>();
            this.QualifiedCharacters = new List<CharacterClass>();
        }
    }
}
