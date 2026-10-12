// <copyright file="MiniGameForceStartCommandTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that game master start commands honor the event server scope.
/// </summary>
[TestFixture]
public class MiniGameForceStartCommandTests
{
    private readonly List<IAsyncDisposable> _teardowns = new();

    /// <summary>
    /// Disposes the game contexts created by the tests.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        foreach (var disposable in this._teardowns)
        {
            await disposable.DisposeAsync().ConfigureAwait(false);
        }

        this._teardowns.Clear();
    }

    /// <summary>
    /// An event which is disabled on the game master's server must not be force-started.
    /// </summary>
    [Test]
    public async Task ForceStartIsSkippedOnExcludedServerAsync()
    {
        var (player, strategyMock, _) = await this.CreatePlayerOnServerAsync(serverId: 0, allowedServerIds: new List<byte> { 1 }).ConfigureAwait(false);

        await new StartBloodCastleEventChatCommandPlugIn().HandleCommandAsync(player, "/startbc").ConfigureAwait(false);

        strategyMock.Verify(s => s.ForceStart(), Times.Never);
    }

    /// <summary>
    /// An event which runs on the game master's server is force-started (proves the wiring above).
    /// </summary>
    [Test]
    public async Task ForceStartRunsOnAllowedServerAsync()
    {
        var (player, strategyMock, _) = await this.CreatePlayerOnServerAsync(serverId: 1, allowedServerIds: new List<byte> { 1 }).ConfigureAwait(false);

        await new StartBloodCastleEventChatCommandPlugIn().HandleCommandAsync(player, "/startbc").ConfigureAwait(false);

        strategyMock.Verify(s => s.ForceStart(), Times.Once);
    }

    /// <summary>
    /// The scope decision is disabled for excluded servers.
    /// </summary>
    [Test]
    public void DisabledOnExcludedServer()
    {
        var contextMock = this.CreateServerContextMock(serverId: 0);
        var strategyMock = this.CreateStrategyMock(new MiniGameStartConfigurationStub { AllowedServerIds = new List<byte> { 1 } });

        Assert.That(StartMiniGameEventChatCommandPlugInBase.IsDisabledOnServer(contextMock.Object, strategyMock.Object), Is.True);
    }

    /// <summary>
    /// The scope decision is enabled for listed servers.
    /// </summary>
    [Test]
    public void EnabledOnListedServer()
    {
        var contextMock = this.CreateServerContextMock(serverId: 1);
        var strategyMock = this.CreateStrategyMock(new MiniGameStartConfigurationStub { AllowedServerIds = new List<byte> { 1 } });

        Assert.That(StartMiniGameEventChatCommandPlugInBase.IsDisabledOnServer(contextMock.Object, strategyMock.Object), Is.False);
    }

    /// <summary>
    /// Contexts without a game server always allow the command.
    /// </summary>
    [Test]
    public void EnabledWithoutGameServer()
    {
        var contextMock = new Mock<IGameContext>();
        var strategyMock = this.CreateStrategyMock(new MiniGameStartConfigurationStub { AllowedServerIds = new List<byte> { 1 } });

        Assert.That(StartMiniGameEventChatCommandPlugInBase.IsDisabledOnServer(contextMock.Object, strategyMock.Object), Is.False);
    }

    /// <summary>
    /// A missing configuration allows the command, keeping previous behavior.
    /// </summary>
    [Test]
    public void EnabledWithoutConfiguration()
    {
        var contextMock = this.CreateServerContextMock(serverId: 0);
        var strategyMock = this.CreateStrategyMock(null);

        Assert.That(StartMiniGameEventChatCommandPlugInBase.IsDisabledOnServer(contextMock.Object, strategyMock.Object), Is.False);
    }

    private Mock<IGameContext> CreateServerContextMock(byte serverId)
    {
        var contextMock = new Mock<IGameContext>();
        contextMock.As<IGameServerContext>().SetupGet(c => c.Id).Returns(serverId);
        return contextMock;
    }

    private Mock<IPeriodicMiniGameStartPlugIn> CreateStrategyMock(MiniGameStartConfiguration? configuration)
    {
        var strategyMock = new Mock<IPeriodicMiniGameStartPlugIn>();
        strategyMock.SetupGet(s => s.Key).Returns(MiniGameType.BloodCastle);
        strategyMock.As<ISupportCustomConfiguration<MiniGameStartConfiguration>>().SetupGet(s => s.Configuration).Returns(configuration!);
        return strategyMock;
    }

    private async ValueTask<(Player Player, Mock<IPeriodicMiniGameStartPlugIn> Strategy, GameServerContext Context)> CreatePlayerOnServerAsync(byte serverId, IList<byte> allowedServerIds)
    {
        var gameConfigurationMock = new Mock<GameConfiguration>();
        var mapMock = new Mock<GameMapDefinition>();
        mapMock.SetupAllProperties();
        mapMock.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        mapMock.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        mapMock.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        mapMock.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfigurationMock.SetupGet(c => c.Maps).Returns(new List<GameMapDefinition> { mapMock.Object });
        gameConfigurationMock.SetupGet(c => c.MiniGameDefinitions).Returns(new List<MiniGameDefinition>());
        gameConfigurationMock.SetupGet(c => c.Skills).Returns(new List<Skill>());
        gameConfigurationMock.SetupGet(c => c.CharacterClasses).Returns(new List<CharacterClass>());
        gameConfigurationMock.SetupGet(c => c.GlobalAttributeCombinations).Returns(new List<AttributeRelationship>());
        gameConfigurationMock.SetupGet(c => c.GlobalBaseAttributeValues).Returns(new List<ConstValueAttribute>());

        var serverConfigurationMock = new Mock<GameServerConfiguration>();
        serverConfigurationMock.SetupGet(c => c.Maps).Returns(gameConfigurationMock.Object.Maps);
        var definition = new GameServerDefinition
        {
            ServerID = serverId,
            GameConfiguration = gameConfigurationMock.Object,
            ServerConfiguration = serverConfigurationMock.Object,
        };

        var strategyMock = new Mock<IPeriodicMiniGameStartPlugIn>();
        strategyMock.SetupGet(s => s.Key).Returns(MiniGameType.BloodCastle);
        strategyMock.Setup(s => s.IsEventActive(It.IsAny<IGameContext>())).Returns(false);
        strategyMock.As<ISupportCustomConfiguration<MiniGameStartConfiguration>>()
            .SetupGet(s => s.Configuration)
            .Returns(new MiniGameStartConfigurationStub { AllowedServerIds = allowedServerIds });

        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint<IPeriodicMiniGameStartPlugIn>(strategyMock.Object);

        var contextProvider = new InMemoryPersistenceContextProvider();
        var changeMediator = new ConfigurationChangeMediator();
        var mapInitializer = new GameServerMapInitializer(definition, new NullLogger<GameServerMapInitializer>(), NullDropGenerator.Instance, changeMediator);
        var context = new GameServerContext(
            definition,
            Mock.Of<IGuildServer>(),
            Mock.Of<IEventPublisher>(),
            Mock.Of<ILoginServer>(),
            Mock.Of<IFriendServer>(),
            contextProvider,
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            changeMediator,
            null);
        mapInitializer.PlugInManager = context.PlugInManager;
        mapInitializer.PathFinderPool = context.PathFinderPool;
        context.StopPeriodicTasks();
        this._teardowns.Add(context);

        var player = await PlayerTestHelper.CreatePlayerAsync(context).ConfigureAwait(false);
        return (player, strategyMock, context);
    }

    /// <summary>
    /// A minimal mini game start configuration for the command tests.
    /// </summary>
    private sealed class MiniGameStartConfigurationStub : MiniGameStartConfiguration
    {
    }
}
