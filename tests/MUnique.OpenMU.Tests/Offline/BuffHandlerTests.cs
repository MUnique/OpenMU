// <copyright file="BuffHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using NUnit.Framework;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameServer.RemoteView.MuHelper;
using MUnique.OpenMU.Pathfinding;

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

    /// <summary>
    /// Tests that a buff isn't cast while the player is stunned or asleep,
    /// like it's the case when a human player casts it.
    /// </summary>
    /// <param name="stateAttributeName">The name of the attribute of the state.</param>
    [TestCase(nameof(Stats.IsStunned))]
    [TestCase(nameof(Stats.IsAsleep))]
    public async ValueTask BuffIsNotAppliedWhileIncapacitatedAsync(string stateAttributeName)
    {
        var stateAttribute = stateAttributeName == nameof(Stats.IsStunned) ? Stats.IsStunned : Stats.IsAsleep;
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 30;
        player.Attributes.AddElement(new AttributeSystem.ConstantElement(1), stateAttribute);
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5 });

        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        Assert.That(result, Is.True);
        Assert.That(player.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.False);
        Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(30));
    }

    /// <summary>
    /// Tests that a buff isn't cast when the current mini game doesn't allow the skill,
    /// like it's the case when a human player casts it.
    /// </summary>
    [Test]
    public async ValueTask BuffIsNotAppliedWhenMiniGameDisallowsItAsync()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 30;
        var miniGame = this.CreateMiniGameMock();
        miniGame.Setup(g => g.IsSkillAllowed(It.IsAny<Skill>(), It.IsAny<Player>(), It.IsAny<IAttackable>())).Returns(false);
        player.CurrentMiniGame = miniGame.Object;
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5 });

        try
        {
            var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

            Assert.That(result, Is.True);
            Assert.That(player.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.False);
            Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(30));
            miniGame.Verify(g => g.IsSkillAllowed(skill, player, player), Times.Once);
        }
        finally
        {
            player.CurrentMiniGame = null;
        }
    }

    /// <summary>
    /// Tests that an unaffordable buff which targets the party is reported as not applied,
    /// so that the helper continues with healing, picking up items and attacking.
    /// </summary>
    [Test]
    public async ValueTask UnaffordablePartyBuffLetsTheLoopContinueAsync()
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20, SkillTarget.ImplicitParty).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 10;
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5 });

        var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

        Assert.That(result, Is.True);
        Assert.That(player.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.False);
        Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that a party member gets buffed when it's on the same map, and not when it's on another map
    /// at the same coordinates.
    /// </summary>
    /// <param name="sameMap">Whether the party member is on the same map.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask PartyMemberIsOnlyBuffedOnTheSameMapAsync(bool sameMap)
    {
        var player = await this.CreateOfflinePlayerAsync().ConfigureAwait(false);
        var member = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var skill = await AddSelfBuffSkillAsync(player, manaCost: 20, SkillTarget.Explicit).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 100;
        player.Attributes[Stats.CurrentMana] = 50;
        var map = CreateMap(1);
        player.SetCurrentMapSilently(map);
        member.SetCurrentMapSilently(sameMap ? map : CreateMap(2));
        player.Position = new Point(100, 100);
        member.Position = new Point(100, 100);
        player.IsAlive = true;
        member.IsAlive = true;
        var party = this._gameContext.PartyManager.CreateParty();
        await party.AddAsync(player).ConfigureAwait(false);
        await party.AddAsync(member).ConfigureAwait(false);
        var handler = new BuffHandler(player, new MuHelperSettings { BuffSkill0Id = skill.Number, HuntingRange = 5, SupportParty = true });

        try
        {
            // The first cast buffs the player itself, the second one the party member.
            Assert.That(await handler.PerformBuffsAsync().ConfigureAwait(false), Is.False);
            var result = await handler.PerformBuffsAsync().ConfigureAwait(false);

            Assert.That(result, Is.EqualTo(!sameMap));
            Assert.That(member.MagicEffectList.HasEffect(skill.MagicEffectDef!), Is.EqualTo(sameMap));
            Assert.That(player.Attributes[Stats.CurrentMana], Is.EqualTo(sameMap ? 10 : 30));
        }
        finally
        {
            await player.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);
            await member.MagicEffectList.ClearAllEffectsAsync().ConfigureAwait(false);
        }
    }

    private static GameMap CreateMap(short number)
    {
        return new GameMap(new Persistence.BasicModel.GameMapDefinition { Number = number }, TimeSpan.FromMinutes(1), 8);
    }

    private static async ValueTask<Skill> AddSelfBuffSkillAsync(OfflinePlayer player, int manaCost, SkillTarget target = SkillTarget.ImplicitPlayer)
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
            Target = target,
            Range = 3,
            MagicEffectDef = magicEffect,
        };
        skill.ConsumeRequirements.Add(new Persistence.BasicModel.AttributeRequirement { Attribute = Stats.CurrentMana, MinimumValue = manaCost });
        await player.SkillList!.AddLearnedSkillAsync(skill).ConfigureAwait(false);
        return skill;
    }

    private Mock<MiniGameContext> CreateMiniGameMock()
    {
        var mapDefinition = new Persistence.BasicModel.GameMapDefinition { Number = 0 };
        var definition = new Persistence.BasicModel.MiniGameDefinition
        {
            Type = MiniGameType.ChaosCastle,
            Entrance = new Persistence.BasicModel.ExitGate { Map = mapDefinition },
        };
        var mapInitializer = new Mock<IMapInitializer>();
        mapInitializer.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));
        return new Mock<MiniGameContext>(
            new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, string.Empty),
            definition,
            this._gameContext,
            mapInitializer.Object);
    }

    private async ValueTask<OfflinePlayer> CreateOfflinePlayerAsync()
    {
        return await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
    }

}
