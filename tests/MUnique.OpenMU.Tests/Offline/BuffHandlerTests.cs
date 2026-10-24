// <copyright file="BuffHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using NUnit.Framework;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;

/// <summary>
/// Tests for <see cref="BuffHandler"/>.
/// </summary>
[TestFixture]
public class BuffHandlerTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true immediately
    /// when no buff skills are configured.
    /// </summary>
    [Test]
    public async ValueTask ReturnsTrueWhenNoBuffsConfiguredAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var config = new MuHelperSettings
        {
            BuffSkill0Id = 0,
            BuffSkill1Id = 0,
            BuffSkill2Id = 0,
        };

        var handler = new BuffHandler(player, config);

        // Act
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="BuffHandler.PerformBuffsAsync"/> returns true when config is null.
    /// </summary>
    [Test]
    public async ValueTask ReturnsTrueWhenConfigNullAsync()
    {
        // Arrange
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var handler = new BuffHandler(player, null);

        // Act
        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that a self buff isn't applied when the player can't pay its mana costs,
    /// like it's the case when a human player casts it.
    /// </summary>
    [Test]
    public async ValueTask SelfBuffIsNotAppliedWithoutEnoughManaAsync()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 10;
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5 });

        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        Assert.That(result, Is.True);
        Assert.That(player.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.False);
        Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that a self buff consumes its mana costs when it's applied.
    /// </summary>
    [Test]
    public async ValueTask SelfBuffConsumesManaAsync()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 30;
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5 });

        try
        {
            var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

            Assert.That(result, Is.False);
            Assert.That(player.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.True);
            Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(10));
        }
        finally
        {
            await player.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);
        }
    }

    private static async ValueTask<Skill> AddSelfBuffSkillAsync(OfflinePlayer player, int manaCost)
    {
        var magicEffect = new Persistence.BasicModel.MagicEffectDefinition
        {
            Number = 200,
            Duration = new Persistence.BasicModel.PowerUpDefinitionValue { ConstantValue = { Value = 60 } },
        };
        var skill = new Persistence.BasicModel.Skill
        {
            Number = 200,
            SkillType = SkillType.Buff,
            Target = SkillTarget.ImplicitPlayer,
            MagicEffectDef = magicEffect,
        };
        skill.ConsumeRequirements.Add(new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.CurrentMana, MinimumValue = manaCost });
        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        return skill;
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
