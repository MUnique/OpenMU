// <copyright file="MiniGameMapReleaseTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that the map of a finished mini game is released. Every run of a mini game creates its own map instance,
/// so a map which is kept alive by something which lives as long as the server leaks with every run.
/// </summary>
[TestFixture]
public class MiniGameMapReleaseTest
{
    private static readonly TimeSpan CollectionTimeout = TimeSpan.FromSeconds(10);

    private static readonly Point SpawnPoint = new(10, 10);

    /// <summary>
    /// Tests that the map of a finished mini game, with an NPC which is registered for configuration changes,
    /// isn't kept alive by the configuration change mediator or the weather plug-in.
    /// </summary>
    [Test]
    public async Task FinishedMiniGameMapIsCollectedAsync()
    {
        var (gameContext, mapInitializer, mediator) = CreateGameContext(withConfigurationChangeMediator: true);
        await using var _ = gameContext.ConfigureAwait(false);
        var weatherPlugIn = new WeatherUpdatePlugIn();

        var mapReference = await RunAndFinishMiniGameAsync(gameContext, weatherPlugIn).ConfigureAwait(false);

        await CollectibleAssert.IsCollectedAsync(mapReference, "map of the finished mini game", CollectionTimeout).ConfigureAwait(false);
        GC.KeepAlive(mapInitializer);
        GC.KeepAlive(mediator);
        GC.KeepAlive(weatherPlugIn);
    }

    /// <summary>
    /// Tests that the weather plug-in alone doesn't keep the map of a finished mini game alive.
    /// </summary>
    [Test]
    public async Task WeatherPlugInDoesNotKeepFinishedMiniGameMapAliveAsync()
    {
        var (gameContext, mapInitializer, _) = CreateGameContext(withConfigurationChangeMediator: false);
        await using var _ = gameContext.ConfigureAwait(false);
        var weatherPlugIn = new WeatherUpdatePlugIn();

        var mapReference = await RunAndFinishMiniGameAsync(gameContext, weatherPlugIn).ConfigureAwait(false);

        await CollectibleAssert.IsCollectedAsync(mapReference, "map of the finished mini game", CollectionTimeout).ConfigureAwait(false);
        GC.KeepAlive(mapInitializer);
        GC.KeepAlive(weatherPlugIn);
    }

    /// <summary>
    /// Tests that tearing down a map disposes its NPCs and releases their configuration change registrations,
    /// so that a later change of the shared spawn configuration doesn't spawn them on the torn down map again.
    /// </summary>
    [Test]
    public async Task TearDownDisposesNpcsAndReleasesTheirRegistrationsAsync()
    {
        var (gameContext, mapInitializer, mediator) = CreateGameContext(withConfigurationChangeMediator: true);
        await using var _ = gameContext.ConfigureAwait(false);
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var spawnArea = mapDefinition.MonsterSpawns.Single();
        var map = mapInitializer.CreateGameMap(mapDefinition);
        await mapInitializer.InitializeStateAsync(map).ConfigureAwait(false);
        Assert.That(map.GetNpcsInRange(SpawnPoint, 1), Has.Count.EqualTo(1));

        await map.TearDownAsync().ConfigureAwait(false);
        var changeMediator = mediator ?? throw new AssertionException("The mediator wasn't created.");
        await changeMediator.HandleConfigurationChangedAsync(typeof(MonsterSpawnArea), spawnArea.GetId(), spawnArea).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(map.IsTornDown, Is.True);
            Assert.That(map.GetNpcsInRange(SpawnPoint, 1), Is.Empty);
        });
    }

    /// <summary>
    /// Tests that a spawn on a torn down map, e.g. of a spawn wave which raced the end of its mini game, is skipped.
    /// </summary>
    [Test]
    public async Task TornDownMapAcceptsNoSpawnsAsync()
    {
        var (gameContext, mapInitializer, _) = CreateGameContext(withConfigurationChangeMediator: true);
        await using var _ = gameContext.ConfigureAwait(false);
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var map = mapInitializer.CreateGameMap(mapDefinition);
        await map.TearDownAsync().ConfigureAwait(false);

        var npc = await mapInitializer.InitializeSpawnAsync(0, map, mapDefinition.MonsterSpawns.Single()).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(npc, Is.Null);
            Assert.That(map.GetNpcsInRange(SpawnPoint, 1), Is.Empty);
        });
    }

    /// <summary>
    /// Tests that a registration which is added to an already disposed NPC, e.g. by a spawn which raced the
    /// teardown of its map, is released right away instead of being kept by the NPC.
    /// </summary>
    [Test]
    public async Task RegistrationOfDisposedNpcIsReleasedImmediatelyAsync()
    {
        var (gameContext, mapInitializer, _) = CreateGameContext(withConfigurationChangeMediator: true);
        await using var _ = gameContext.ConfigureAwait(false);
        var mapDefinition = gameContext.Configuration.Maps.Single();
        var map = mapInitializer.CreateGameMap(mapDefinition);
        var npc = await mapInitializer.InitializeSpawnAsync(0, map, mapDefinition.MonsterSpawns.Single()).ConfigureAwait(false)
            ?? throw new AssertionException("The NPC wasn't spawned.");
        await npc.DisposeAsync().ConfigureAwait(false);
        var registration = new Mock<IDisposable>();

        npc.RegisterDisposable(registration.Object);

        registration.Verify(r => r.Dispose(), Times.Once);
    }

    /// <summary>
    /// Creates, runs and finishes a mini game, and returns a weak reference to its map. It's a separate method,
    /// so that no local variable of the test keeps the map alive (see <see cref="CollectibleAssert"/>).
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task<WeakReference> RunAndFinishMiniGameAsync(GameContext gameContext, WeatherUpdatePlugIn weatherPlugIn)
    {
        var requester = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var game = await gameContext.MiniGames.GetOrCreateAsync(CreateMiniGameDefinition(gameContext), requester).ConfigureAwait(false);
        Assert.That(game.Map.GetNpcsInRange(SpawnPoint, 1), Has.Count.EqualTo(1));
        await weatherPlugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        var mapReference = new WeakReference(game.Map);

        await game.DisposeAsync().ConfigureAwait(false);

        // The next weather run happens after the game is gone.
        weatherPlugIn.ForceStart();
        await weatherPlugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        return mapReference;
    }

    private static (GameContext GameContext, MapInitializer MapInitializer, ConfigurationChangeMediator? Mediator) CreateGameContext(bool withConfigurationChangeMediator)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var mapDefinition = configuration.Maps.Single();
        mapDefinition.MonsterSpawns.Add(new Persistence.BasicModel.MonsterSpawnArea
        {
            Id = Guid.NewGuid(),
            GameMap = mapDefinition,
            MonsterDefinition = new Persistence.BasicModel.MonsterDefinition
            {
                Id = Guid.NewGuid(),
                Number = 1,
                Designation = "Merchant",
                ObjectKind = NpcObjectKind.PassiveNpc,
                MerchantStore = new Persistence.BasicModel.ItemStorage { Id = Guid.NewGuid() },
            },
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.Automatic,
            X1 = SpawnPoint.X,
            Y1 = SpawnPoint.Y,
            X2 = SpawnPoint.X,
            Y2 = SpawnPoint.Y,
        });

        var mediator = withConfigurationChangeMediator ? new ConfigurationChangeMediator() : null;
        var mapInitializer = new MapInitializer(configuration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, mediator);
        var gameContext = new GameContext(configuration, new InMemoryPersistenceContextProvider(), mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, mediator ?? new ConfigurationChangeMediator());
        gameContext.StopPeriodicTasks();
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        return (gameContext, mapInitializer, mediator);
    }

    private static MiniGameDefinition CreateMiniGameDefinition(IGameContext gameContext)
    {
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.ChangeEvents).Returns(new List<MiniGameChangeEvent>());
        definitionMock.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = gameContext.Configuration.Maps.Single() });
        var definition = definitionMock.Object;
        definition.Type = MiniGameType.DevilSquare;
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        definition.EnterDuration = TimeSpan.FromMinutes(1);
        definition.GameDuration = TimeSpan.FromMinutes(5);
        definition.ExitDuration = TimeSpan.FromMinutes(1);
        definition.MaximumPlayerCount = 10;
        return definition;
    }
}
