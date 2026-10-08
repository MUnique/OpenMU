// <copyright file="SkillListTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests the skill list.
/// </summary>
[TestFixture]
public class SkillListTest
{
    private const ushort LearnedSkillId = 10;
    private const ushort NonLearnedSkillId = 999;
    private const ushort QualifiedItemSkillId = 1;
    private const ushort NonQualifiedItemSkillId = 9;
    private const ushort PassiveSkillId = 300;
    private const byte MaximumMasterSkillLevel = 20;

    /// <summary>
    /// Tests if the created skill list contains a skill that was learned by the character before.
    /// </summary>
    [Test]
    public async ValueTask LearnedSkillAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.LearnedSkills.Add(this.CreateSkillEntry(LearnedSkillId));
        var skillList = new SkillList(player);
        Assert.That(skillList.ContainsSkill(LearnedSkillId), Is.True);
    }

    /// <summary>
    /// Tests if the skill of an item is or isn't getting added to the skill list, depending if it's suitable to the character's class.
    /// </summary>
    [Test]
    public async ValueTask ItemSkillAddedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var skillList = player.SkillList as SkillList;
        await player.Inventory!.AddItemAsync(0, this.CreateItemWithSkill(QualifiedItemSkillId, player.SelectedCharacter!.CharacterClass)).ConfigureAwait(false);
        await player.Inventory!.AddItemAsync(1, this.CreateItemWithSkill(NonQualifiedItemSkillId)).ConfigureAwait(false);

        Assert.That(skillList!.ContainsSkill(QualifiedItemSkillId), Is.True);
        Assert.That(skillList!.ContainsSkill(NonQualifiedItemSkillId), Is.False);
    }

    /// <summary>
    /// Tests the removal of item skills.
    /// </summary>
    [Test]
    public async ValueTask ItemSkillRemovedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = this.CreateItemWithSkill(QualifiedItemSkillId, player.SelectedCharacter!.CharacterClass);
        item.Durability = 1;
        await player.Inventory!.AddItemAsync(0, item).ConfigureAwait(false);
        var skillList = new SkillList(player);
        Assert.That(await skillList.RemoveItemSkillAsync(item.Definition!.Skill!.Number.ToUnsigned()).ConfigureAwait(false), Is.True);
        Assert.That(skillList.ContainsSkill(QualifiedItemSkillId), Is.False);
    }

    /// <summary>
    /// Tests that a skill which is granted by more than one equipped item is only removed from the
    /// skill list when the last of these items is taken off.
    /// </summary>
    [Test]
    public async ValueTask ItemSkillRemovedWithLastItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass;
        await player.Inventory!.AddItemAsync(0, this.CreateItemWithSkill(QualifiedItemSkillId, characterClass)).ConfigureAwait(false);
        await player.Inventory!.AddItemAsync(1, this.CreateItemWithSkill(QualifiedItemSkillId, characterClass)).ConfigureAwait(false);
        var skillList = new SkillList(player);

        Assert.That(await skillList.RemoveItemSkillAsync(QualifiedItemSkillId).ConfigureAwait(false), Is.True);
        Assert.That(skillList.ContainsSkill(QualifiedItemSkillId), Is.True);

        Assert.That(await skillList.RemoveItemSkillAsync(QualifiedItemSkillId).ConfigureAwait(false), Is.True);
        Assert.That(skillList.ContainsSkill(QualifiedItemSkillId), Is.False);
    }

    /// <summary>
    /// Tests that a character which has the same skill twice in its learned skills can still
    /// create its skill list - the duplicate with the lower level is dropped.
    /// </summary>
    [Test]
    public async ValueTask DuplicateLearnedSkillIsRemovedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var learnedSkills = player.SelectedCharacter!.LearnedSkills;
        var lowLevelEntry = this.CreateSkillEntry(LearnedSkillId);
        var highLevelEntry = this.CreateSkillEntry(LearnedSkillId);
        highLevelEntry.Level = 5;
        learnedSkills.Add(lowLevelEntry);
        learnedSkills.Add(highLevelEntry);

        var skillList = new SkillList(player);

        Assert.That(skillList.ContainsSkill(LearnedSkillId), Is.True);
        Assert.That(learnedSkills, Is.EquivalentTo(new[] { highLevelEntry }));
        Assert.That(skillList.GetSkill(LearnedSkillId)!.Level, Is.EqualTo(5));
    }

    /// <summary>
    /// Tests that a learned skill isn't lost when an item which grants the same skill is unequipped.
    /// </summary>
    [Test]
    public async ValueTask LearnedSkillKeptWhenItemSkillRemovedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.LearnedSkills.Add(this.CreateSkillEntry(QualifiedItemSkillId));
        var item = this.CreateItemWithSkill(QualifiedItemSkillId, player.SelectedCharacter!.CharacterClass);
        item.Durability = 1;
        await player.Inventory!.AddItemAsync(0, item).ConfigureAwait(false);
        var skillList = new SkillList(player);

        Assert.That(await skillList.RemoveItemSkillAsync(QualifiedItemSkillId).ConfigureAwait(false), Is.True);
        Assert.That(skillList.ContainsSkill(QualifiedItemSkillId), Is.True);
    }

    /// <summary>
    /// Tests if the skill list does not contain non-learned skills.
    /// </summary>
    [Test]
    public async ValueTask NonLearnedSkillAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Assert.That(player.SkillList!.ContainsSkill(NonLearnedSkillId), Is.False);
    }

    /// <summary>
    /// Tests that a passive power-up of a learned master skill applies its value to the character,
    /// and follows the level of the skill.
    /// </summary>
    [Test]
    public async ValueTask PassivePowerUpFollowsMasterSkillValueAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var skillEntry = CreatePassiveSkillEntry("level * 2", AggregateType.AddRaw, Stats.MasteryMoveTargetChance, Stats.MasterSkillValue, 1);
        skillEntry.Level = 5;
        player.SelectedCharacter!.LearnedSkills.Add(skillEntry);

        using var skillList = new SkillList(player);
        Assert.That(player.Attributes![Stats.MasteryMoveTargetChance], Is.EqualTo(10));

        skillEntry.Level = 6;
        Assert.That(player.Attributes[Stats.MasteryMoveTargetChance], Is.EqualTo(12));

        skillList.Dispose();
        Assert.That(player.Attributes[Stats.MasteryMoveTargetChance], Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a multiplicative passive power-up applies the value of the master skill as factor.
    /// </summary>
    [Test]
    public async ValueTask MultiplicativePassivePowerUpAppliesFactorAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var skillEntry = CreatePassiveSkillEntry("1 + level / 10", AggregateType.Multiplicate, Stats.ShieldRecoveryMultiplier, Stats.MasterSkillValue, 1);
        skillEntry.Level = 5;
        player.SelectedCharacter!.LearnedSkills.Add(skillEntry);

        using var skillList = new SkillList(player);

        Assert.That(player.Attributes![Stats.ShieldRecoveryMultiplier], Is.EqualTo(1.5f).Within(0.0001f));
    }

    /// <summary>
    /// Tests that a passive power-up can depend on the level of the skill.
    /// </summary>
    [Test]
    public async ValueTask PassivePowerUpByLevelAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var valueBefore = player.Attributes![Stats.DurabilityReductionFactor];
        var skillEntry = CreatePassiveSkillEntry("level", AggregateType.AddRaw, Stats.DurabilityReductionFactor, Stats.SkillLevel, -0.002f);
        skillEntry.Level = 5;
        await player.SkillList!.AddLearnedSkillAsync(skillEntry.Skill!).ConfigureAwait(false);
        var learnedEntry = player.SkillList.GetSkill(PassiveSkillId)!;
        learnedEntry.Level = 5;

        Assert.That(player.Attributes[Stats.DurabilityReductionFactor], Is.EqualTo(valueBefore - 0.01f).Within(0.0001f));
    }

    private static SkillEntry CreatePassiveSkillEntry(string valueFormula, AggregateType aggregateType, AttributeDefinition targetAttribute, AttributeDefinition inputAttribute, float inputOperand)
    {
        var powerUp = new Persistence.BasicModel.PowerUpDefinition
        {
            TargetAttribute = targetAttribute,
            Boost = new Persistence.BasicModel.PowerUpDefinitionValue
            {
                ConstantValue = { Value = aggregateType == AggregateType.Multiplicate ? 1 : 0, AggregateType = aggregateType },
                RelatedValues =
                {
                    new Persistence.BasicModel.AttributeRelationship { InputAttribute = inputAttribute, InputOperand = inputOperand, InputOperator = InputOperator.Multiply },
                },
            },
        };
        var masterDefinition = new Persistence.BasicModel.MasterSkillDefinition
        {
            ValueFormula = valueFormula,
            MaximumLevel = MaximumMasterSkillLevel,
            TargetAttribute = targetAttribute,
            Aggregation = aggregateType,
        };
        masterDefinition.PassivePowerUps.Add(powerUp);
        var skill = new Persistence.BasicModel.Skill { Number = PassiveSkillId.ToSigned(), SkillType = SkillType.PassiveBoost, MasterDefinition = masterDefinition };
        return new Persistence.BasicModel.SkillEntry { Skill = skill };
    }

    private Item CreateItemWithSkill(ushort skillId, CharacterClass? qualifiedClass = null)
    {
        var itemDefinition = new Mock<ItemDefinition>();
        itemDefinition.SetupAllProperties();

        var skillDefinition = new Mock<Skill>();
        skillDefinition.Object.Number = skillId.ToSigned();
        skillDefinition.Setup(sd => sd.QualifiedCharacters).Returns(new List<CharacterClass>());
        if (qualifiedClass is not null)
        {
            skillDefinition.Object.QualifiedCharacters.Add(qualifiedClass);
        }

        itemDefinition.Object.Skill = skillDefinition.Object;
        itemDefinition.Object.Height = 1;
        itemDefinition.Object.Width = 1;
        itemDefinition.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());

        var item = new Item
        {
            HasSkill = true,
            Definition = itemDefinition.Object,
        };
        return item;
    }

    private SkillEntry CreateSkillEntry(ushort skillId)
    {
        var skillEntry = new SkillEntry { Skill = new OpenMU.DataModel.Configuration.Skill { Number = skillId.ToSigned() } };
        return skillEntry;
    }
}