// <copyright file="ExperienceRateSplitTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

[TestFixture]
public class ExperienceRateSplitTest
{
    [Test]
    public async ValueTask SoloKillUsesMasterExperienceRateForMasterClassesAsync()
    {
        var masterContext = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 5.0f,
            maximumLevel: 10,
            maximumMasterLevel: 200);
        var normalContext = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 5.0f,
            maximumLevel: 11,
            maximumMasterLevel: 200);

        var masterPlayer = await this.CreatePlayerAsync(masterContext, level: 10, totalLevel: 10, isMasterClass: true).ConfigureAwait(false);
        var normalPlayer = await this.CreatePlayerAsync(normalContext, level: 10, totalLevel: 10, isMasterClass: false).ConfigureAwait(false);
        var killedObject = CreateKilledObject(level: 100);

        var masterGained = await masterPlayer.AddExpAfterKillAsync(killedObject.Object).ConfigureAwait(false);
        var normalGained = await normalPlayer.AddExpAfterKillAsync(killedObject.Object).ConfigureAwait(false);

        Assert.That(masterGained, Is.GreaterThan(0));
        Assert.That(normalGained, Is.GreaterThan(0));
        Assert.That(masterGained, Is.GreaterThan(normalGained * 3));
        Assert.That(masterPlayer.SelectedCharacter!.MasterExperience, Is.EqualTo(masterGained));
        Assert.That(normalPlayer.SelectedCharacter!.Experience, Is.EqualTo(normalGained));
    }

    [Test]
    public async ValueTask SoloKillAppliesServerExperienceRateToMasterExperienceAsync()
    {
        var highRateContext = this.CreateGameServerContext(
            normalExperienceRate: 3.0f,
            globalMasterExperienceRate: 2.0f,
            maximumLevel: 10,
            maximumMasterLevel: 200);
        var baseRateContext = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 2.0f,
            maximumLevel: 10,
            maximumMasterLevel: 200);

        var highRatePlayer = await this.CreatePlayerAsync(highRateContext, level: 10, totalLevel: 10, isMasterClass: true).ConfigureAwait(false);
        var baseRatePlayer = await this.CreatePlayerAsync(baseRateContext, level: 10, totalLevel: 10, isMasterClass: true).ConfigureAwait(false);
        var killedObject = CreateKilledObject(level: 100);

        var highRateGain = await highRatePlayer.AddExpAfterKillAsync(killedObject.Object).ConfigureAwait(false);
        var baseRateGain = await baseRatePlayer.AddExpAfterKillAsync(killedObject.Object).ConfigureAwait(false);

        Assert.That(highRateGain, Is.GreaterThan(0));
        Assert.That(baseRateGain, Is.GreaterThan(0));
        Assert.That(highRateGain, Is.GreaterThan(baseRateGain * 2));
    }

    [Test]
    public async ValueTask PartyDistributionUsesMasterExperienceRateForMasterMembersAsync()
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 4.0f,
            maximumLevel: 3,
            maximumMasterLevel: 200);

        var masterPlayer = await this.CreatePlayerAsync(context, level: 3, totalLevel: 2, isMasterClass: true).ConfigureAwait(false);
        var normalPlayer = await this.CreatePlayerAsync(context, level: 2, totalLevel: 2, isMasterClass: false).ConfigureAwait(false);

        var party = new Party(5, new NullLogger<Party>());
        await party.AddAsync(masterPlayer).ConfigureAwait(false);
        await party.AddAsync(normalPlayer).ConfigureAwait(false);
        await masterPlayer.AddObserverAsync(normalPlayer).ConfigureAwait(false);

        var killedObject = CreateKilledObject(level: 5);
        _ = await party.DistributeExperienceAfterKillAsync(killedObject.Object, masterPlayer).ConfigureAwait(false);

        var masterGained = masterPlayer.SelectedCharacter!.MasterExperience;
        var normalGained = normalPlayer.SelectedCharacter!.Experience;

        Assert.That(masterGained, Is.GreaterThan(0));
        Assert.That(normalGained, Is.GreaterThan(0));
        Assert.That(masterGained, Is.GreaterThan(normalGained * 3));
    }

    [Test]
    public async ValueTask ConcurrentNormalExperienceCantExceedMaximumLevelAsync()
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 1.0f,
            maximumLevel: 2,
            maximumMasterLevel: 200);

        var player = await this.CreatePlayerAsync(context, level: 1, totalLevel: 1, isMasterClass: false).ConfigureAwait(false);
        player.SelectedCharacter!.Experience = context.ExperienceTable[2] - 1;

        var initialLevelUpPoints = player.SelectedCharacter.LevelUpPoints;
        var pointsPerLevelUp = (int)player.Attributes![Stats.PointsPerLevelUp];

        await Task.WhenAll(
            player.AddExperienceAsync(10, null).AsTask(),
            player.AddExperienceAsync(10, null).AsTask()).ConfigureAwait(false);

        Assert.That((int)player.Attributes[Stats.Level], Is.EqualTo(2));
        Assert.That(player.SelectedCharacter.LevelUpPoints, Is.EqualTo(initialLevelUpPoints + pointsPerLevelUp));
    }

    [Test]
    public async ValueTask ConcurrentMasterExperienceStaysWithinConfiguredMaximumBoundsAsync()
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 1.0f,
            maximumLevel: 400,
            maximumMasterLevel: 1);

        var player = await this.CreatePlayerAsync(context, level: 400, totalLevel: 400, isMasterClass: true).ConfigureAwait(false);
        player.Attributes![Stats.MasterLevel] = 0;
        player.SelectedCharacter!.MasterExperience = context.MasterExperienceTable[1] - 1;
        var maxMasterExperience = context.MasterExperienceTable[context.Configuration.MaximumMasterLevel];

        await Task.WhenAll(
            player.AddMasterExperienceAsync(10, null).AsTask(),
            player.AddMasterExperienceAsync(10, null).AsTask()).ConfigureAwait(false);

        Assert.That((int)player.Attributes[Stats.MasterLevel], Is.LessThanOrEqualTo(context.Configuration.MaximumMasterLevel));
        Assert.That(player.SelectedCharacter.MasterExperience, Is.LessThanOrEqualTo(maxMasterExperience));
    }

    [Test]
    public async ValueTask OverflowIsAppliedBelowMaxWhenNotPreventedAsync()
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 1.0f,
            maximumLevel: 10,
            maximumMasterLevel: 200);

        var player = await this.CreatePlayerAsync(context, level: 1, totalLevel: 1, isMasterClass: false).ConfigureAwait(false);
        var requiredForLevel2 = context.ExperienceTable[2] - player.SelectedCharacter!.Experience;

        await player.AddExperienceAsync((int)requiredForLevel2 + 10, null).ConfigureAwait(false);

        Assert.That((int)player.Attributes![Stats.Level], Is.EqualTo(2));
        Assert.That(player.SelectedCharacter.Experience, Is.EqualTo(context.ExperienceTable[2] + 10));
    }

    [Test]
    public async ValueTask OverflowIsDiscardedBelowMaxWhenPreventedAsync()
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 1.0f,
            maximumLevel: 10,
            maximumMasterLevel: 200,
            preventExperienceOverflow: true);

        var player = await this.CreatePlayerAsync(context, level: 1, totalLevel: 1, isMasterClass: false).ConfigureAwait(false);
        var requiredForLevel2 = context.ExperienceTable[2] - player.SelectedCharacter!.Experience;

        await player.AddExperienceAsync((int)requiredForLevel2 + 10, null).ConfigureAwait(false);

        Assert.That((int)player.Attributes![Stats.Level], Is.EqualTo(2));
        Assert.That(player.SelectedCharacter.Experience, Is.EqualTo(context.ExperienceTable[2]));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask ExperienceAlwaysStopsAtMaximumLevelRegardlessOfOverflowSettingAsync(bool preventExperienceOverflow)
    {
        var context = this.CreateGameServerContext(
            normalExperienceRate: 1.0f,
            globalMasterExperienceRate: 1.0f,
            maximumLevel: 2,
            maximumMasterLevel: 200,
            preventExperienceOverflow);

        var player = await this.CreatePlayerAsync(context, level: 1, totalLevel: 1, isMasterClass: false).ConfigureAwait(false);

        await player.AddExperienceAsync(int.MaxValue, null).ConfigureAwait(false);
        await player.AddExperienceAsync(int.MaxValue, null).ConfigureAwait(false);

        Assert.That((int)player.Attributes![Stats.Level], Is.EqualTo(2));
    }

    private static Mock<IAttackable> CreateKilledObject(float level)
    {
        var attributes = new Mock<IAttributeSystem>();
        attributes.Setup(a => a[Stats.Level]).Returns(level);

        var result = new Mock<IAttackable>();
        result.SetupGet(a => a.Attributes).Returns(attributes.Object);
        result.SetupGet(a => a.CurrentMap).Returns((GameMap?)null);
        return result;
    }

    private async ValueTask<Player> CreatePlayerAsync(IGameContext context, short level, float totalLevel, bool isMasterClass)
    {
        var player = await TestHelper.CreatePlayerAsync(context).ConfigureAwait(false);
        player.SelectedCharacter!.CharacterClass!.IsMasterClass = isMasterClass;
        player.Attributes![Stats.Level] = level;
        player.Attributes[Stats.MasterLevel] = 0;
        player.Attributes[Stats.PointsPerLevelUp] = 1;
        player.Attributes[Stats.MasterPointsPerLevelUp] = 1;
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.ExperienceRate);
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.MasterExperienceRate);
        player.Attributes.AddElement(new SimpleElement(totalLevel, AggregateType.AddRaw), Stats.TotalLevel);
        player.SelectedCharacter.Experience = 0;
        player.SelectedCharacter.MasterExperience = 0;
        return player;
    }

    private IGameServerContext CreateGameServerContext(float normalExperienceRate, float globalMasterExperienceRate, short maximumLevel, short maximumMasterLevel, bool preventExperienceOverflow = false)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
        if (gameConfiguration.CharacterClasses is null)
        {
            typeof(GameConfiguration).GetProperty(nameof(GameConfiguration.CharacterClasses))?.SetValue(gameConfiguration, new List<CharacterClass>());
        }

        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumLevel = maximumLevel;
        gameConfiguration.MaximumMasterLevel = maximumMasterLevel;
        gameConfiguration.PreventExperienceOverflow = preventExperienceOverflow;
        gameConfiguration.MinimumMonsterLevelForMasterExperience = 0;
        gameConfiguration.ExperienceRate = 1.0f;
        gameConfiguration.MasterExperienceRate = globalMasterExperienceRate;
        var map = contextProvider.CreateNewContext().CreateNew<GameMapDefinition>();
        map.ExpMultiplier = 1.0f;
        gameConfiguration.Maps.Add(map);

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameServerContext = new GameServerContext(
            new GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new GameServerConfiguration(),
                ExperienceRate = normalExperienceRate,
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            contextProvider,
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;
        return gameServerContext;
    }
}
