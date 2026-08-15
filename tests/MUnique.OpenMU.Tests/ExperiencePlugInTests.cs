// <copyright file="ExperiencePlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the plugin points around the experience gain of a player.
/// </summary>
[TestFixture]
public class ExperiencePlugInTests
{
    /// <summary>
    /// Tests that a plugin can modify the experience which is gained for a kill.
    /// </summary>
    [Test]
    public async ValueTask CalculationPlugInModifiesGainedExperienceAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        var killedObject = CreateKilledObject(level: 100);

        var withoutPlugIn = await player.CalculateExpAfterKillAsync(killedObject).ConfigureAwait(false);
        Assert.That(withoutPlugIn, Is.GreaterThan(0));

        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IExperienceCalculationPlugIn>(new DoubleExperiencePlugIn());
        var withPlugIn = await player.CalculateExpAfterKillAsync(killedObject).ConfigureAwait(false);

        Assert.That(withPlugIn, Is.EqualTo(withoutPlugIn * 2));
    }

    /// <summary>
    /// Tests that the calculation plugin also affects the experience which is actually granted.
    /// </summary>
    [Test]
    public async ValueTask CalculationPlugInAffectsGrantedExperienceAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        var killedObject = CreateKilledObject(level: 100);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IExperienceCalculationPlugIn>(new DoubleExperiencePlugIn());

        var gained = await player.AddExpAfterKillAsync(killedObject).ConfigureAwait(false);

        Assert.That(gained, Is.GreaterThan(0));
        Assert.That(player.SelectedCharacter!.Experience, Is.EqualTo(gained));
    }

    /// <summary>
    /// Tests that a plugin is informed about the gained experience, which is how the pets get
    /// their share of it.
    /// </summary>
    [Test]
    public async ValueTask GainedExperienceIsReportedToPlugInAsync()
    {
        var player = await this.CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = new ExperienceRecordingPlugIn();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerGainedExperiencePlugIn>(plugIn);
        var killedObject = CreateKilledObject(level: 100);

        var gained = await player.AddExpAfterKillAsync(killedObject).ConfigureAwait(false);

        Assert.That(plugIn.Gains, Has.Count.EqualTo(1));
        Assert.That(plugIn.Gains[0].Experience, Is.EqualTo(gained));
        Assert.That(plugIn.Gains[0].IsMasterExperience, Is.False);
        Assert.That(plugIn.Gains[0].KilledObject, Is.SameAs(killedObject));
    }

    /// <summary>
    /// Tests that no plugin is called when the player can't gain any experience.
    /// </summary>
    [Test]
    public async ValueTask GainedExperienceIsNotReportedWithoutExperienceAsync()
    {
        var player = await this.CreatePlayerAsync(maximumLevel: 1).ConfigureAwait(false);
        var plugIn = new ExperienceRecordingPlugIn();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerGainedExperiencePlugIn>(plugIn);

        await player.AddExpAfterKillAsync(CreateKilledObject(level: 0)).ConfigureAwait(false);

        Assert.That(plugIn.Gains, Is.Empty);
    }

    /// <summary>
    /// Tests that a master level up is reported to the corresponding plugin point.
    /// </summary>
    [Test]
    public async ValueTask MasterLevelUpIsReportedToPlugInAsync()
    {
        var player = await this.CreatePlayerAsync(maximumLevel: 10, isMasterClass: true, level: 10).ConfigureAwait(false);
        var plugIn = new MasterLevelUpRecordingPlugIn();
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<ICharacterMasterLevelUpPlugIn>(plugIn);
        // The level up happens when the required experience is exceeded, so one more point is needed.
        var requiredExperience = player.GameContext.MasterExperienceTable[1] + 1;

        await player.AddMasterExperienceAsync((int)requiredExperience, null).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.MasterExperience, Is.EqualTo(player.GameContext.MasterExperienceTable[1]));
        Assert.That(plugIn.LevelUpCount, Is.EqualTo(1));
    }

    private static Mock<IAttackable> CreateKilledObjectMock(float level)
    {
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[Stats.Level]).Returns(level);

        var result = new Mock<IAttackable>();
        result.SetupGet(a => a.Attributes).Returns(attributes.Object);
        result.SetupGet(a => a.CurrentMap).Returns((GameMap?)null);
        return result;
    }

    private static IAttackable CreateKilledObject(float level) => CreateKilledObjectMock(level).Object;

    private async ValueTask<Player> CreatePlayerAsync(short maximumLevel = 100, bool isMasterClass = false, short level = 1)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumLevel = maximumLevel;
        gameConfiguration.MaximumMasterLevel = 200;
        gameConfiguration.MinimumMonsterLevelForMasterExperience = 0;
        gameConfiguration.ExperienceRate = 1.0f;
        gameConfiguration.MasterExperienceRate = 1.0f;
        var map = contextProvider.CreateNewContext().CreateNew<GameMapDefinition>();
        map.ExpMultiplier = 1.0f;
        gameConfiguration.Maps.Add(map);

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(
            gameConfiguration,
            contextProvider,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.CharacterClass!.IsMasterClass = isMasterClass;
        player.Attributes![Stats.Level] = level;
        player.Attributes[Stats.MasterLevel] = 0;
        player.Attributes[Stats.PointsPerLevelUp] = 1;
        player.Attributes[Stats.MasterPointsPerLevelUp] = 1;
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.ExperienceRate);
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.MasterExperienceRate);
        player.Attributes.AddElement(new SimpleElement(level, AggregateType.AddRaw), Stats.TotalLevel);
        player.SelectedCharacter.Experience = 0;
        player.SelectedCharacter.MasterExperience = 0;
        return player;
    }

    [Guid("A0C4C8E1-2D2E-4F1A-9C51-0B8E1E2A5D71")]
    private sealed class DoubleExperiencePlugIn : IExperienceCalculationPlugIn
    {
        public ValueTask CalculateExperienceAsync(Player player, ExperienceCalculationArgs args)
        {
            args.Experience *= 2;
            return ValueTask.CompletedTask;
        }
    }

    [Guid("B31E7E52-6F3B-4E0C-9C8A-8F0A2C4D6E13")]
    private sealed class ExperienceRecordingPlugIn : IPlayerGainedExperiencePlugIn
    {
        public List<(int Experience, IAttackable KilledObject, bool IsMasterExperience)> Gains { get; } = new();

        public ValueTask PlayerGainedExperienceAsync(Player player, int experience, IAttackable killedObject, bool isMasterExperience)
        {
            this.Gains.Add((experience, killedObject, isMasterExperience));
            return ValueTask.CompletedTask;
        }
    }

    [Guid("C2F5A9D3-8B1E-4A7C-B0D2-5E6F1A3C8B24")]
    private sealed class MasterLevelUpRecordingPlugIn : ICharacterMasterLevelUpPlugIn
    {
        public int LevelUpCount { get; private set; }

        public ValueTask CharacterMasterLeveledUpAsync(Player player)
        {
            this.LevelUpCount++;
            return ValueTask.CompletedTask;
        }
    }
}
