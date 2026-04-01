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
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.ExperienceRate);
        player.Attributes.AddElement(new SimpleElement(1.0f, AggregateType.AddRaw), Stats.MasterExperienceRate);
        player.Attributes.AddElement(new SimpleElement(totalLevel, AggregateType.AddRaw), Stats.TotalLevel);
        player.SelectedCharacter.Experience = 0;
        player.SelectedCharacter.MasterExperience = 0;
        return player;
    }

    private IGameServerContext CreateGameServerContext(float normalExperienceRate, float globalMasterExperienceRate, short maximumLevel, short maximumMasterLevel)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumLevel = maximumLevel;
        gameConfiguration.MaximumMasterLevel = maximumMasterLevel;
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
