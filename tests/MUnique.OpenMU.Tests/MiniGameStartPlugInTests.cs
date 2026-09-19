// <copyright file="MiniGameStartPlugInTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the mini game start plug-in behaviors: forced starts bypassing the
/// timetable, blocking while a previous event is still running, and the
/// tick-derived entrance announcements.
/// </summary>
[TestFixture]
public class MiniGameStartPlugInTests
{
    private TestStartPlugIn _plugIn = null!;
    private TestStartConfiguration _configuration = null!;
    private List<MiniGameContext> _gamesToDispose = new();

    /// <summary>
    /// Sets up the plug-in with an empty timetable, so that only forced starts proceed.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._configuration = new TestStartConfiguration
        {
            Timetable = new List<TimeOnly>(),
            TaskDuration = TimeSpan.FromHours(1),
            PreStartMessageDelay = TimeSpan.FromSeconds(3),
        };
        this._plugIn = new TestStartPlugIn(Mock.Of<IGameContext>());
        this._plugIn.Configuration = this._configuration;
        this._gamesToDispose = new();
    }

    /// <summary>
    /// Disposes the canned games, so that their background loops don't leak into other tests.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        foreach (var game in this._gamesToDispose)
        {
            await game.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that a forced start bypasses a future next-run timestamp.
    /// </summary>
    [Test]
    public async Task ForcedStartBypassesFutureNextRunUtcAsync()
    {
        var contextMock = this.CreateGameContextMock();
        try
        {
            var state = this._plugIn.GetStateForTest(contextMock.Object);
            state.NextRunUtc = DateTime.UtcNow.AddHours(1);
            state.LastRunUtc = DateTime.MinValue;

            this._plugIn.ForceStart();
            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.Prepared));
        }
        finally
        {
            await contextMock.Object.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that a forced start also finishes a previous run stuck in Started state
    /// (its NextRunUtc still lies in the future), so the new run starts right away
    /// instead of waiting for the task duration to elapse.
    /// </summary>
    [Test]
    public async Task ForcedStartWhileStuckInStartedFinishesThenStartsAsync()
    {
        // The plug-in must be backed by the fake context: CreateState ignores the
        // passed context and uses this one, and the guard reads MiniGames off it.
        var (fakeContext, _) = this.CreateFakeContext();
        this._plugIn = new TestStartPlugIn(fakeContext.Object);
        this._plugIn.Configuration = this._configuration;
        var contextMock = this.CreateGameContextMock();
        try
        {
            var state = this._plugIn.GetStateForTest(contextMock.Object);
            state.State = PeriodicTaskState.Started;
            state.NextRunUtc = DateTime.UtcNow.AddHours(1);
            state.LastRunUtc = DateTime.UtcNow;

            this._plugIn.ForceStart();
            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);
            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.NotStarted));

            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);
            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.Prepared));
        }
        finally
        {
            await contextMock.Object.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that without a forced start, a future next-run timestamp stops the tick.
    /// </summary>
    [Test]
    public async Task WithoutForcedStartFutureNextRunUtcDoesNothingAsync()
    {
        var contextMock = this.CreateGameContextMock();
        try
        {
            var state = this._plugIn.GetStateForTest(contextMock.Object);
            state.NextRunUtc = DateTime.UtcNow.AddHours(1);
            state.LastRunUtc = DateTime.MinValue;

            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.NotStarted));
        }
        finally
        {
            await contextMock.Object.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that a previous, still active event blocks a new start and consumes the forced start.
    /// </summary>
    [Test]
    public async Task PreviousActiveEventBlocksStartAsync()
    {
        var (fakeContext, manager) = this.CreateFakeContext();
        this._plugIn = new TestStartPlugIn(fakeContext.Object);
        this._plugIn.Configuration = this._configuration;
        var game = this.CreateGame(fakeContext, manager, this.CreateDefinition(TimeSpan.FromMinutes(2)));
        manager.Games.Add(game);
        var contextMock = this.CreateGameContextMock();
        try
        {
            var state = this._plugIn.GetStateForTest(contextMock.Object);
            state.NextRunUtc = DateTime.UtcNow.AddHours(1);
            state.LastRunUtc = DateTime.UtcNow;

            this._plugIn.ForceStart();
            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);
            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.NotStarted));

            // The forced start was consumed by the blocked attempt.
            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);
            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.NotStarted));
        }
        finally
        {
            await contextMock.Object.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests that a finished previous event doesn't block a new start.
    /// </summary>
    [Test]
    public async Task FinishedPreviousEventDoesNotBlockStartAsync()
    {
        var (fakeContext, _) = this.CreateFakeContext();
        this._plugIn = new TestStartPlugIn(fakeContext.Object);
        this._plugIn.Configuration = this._configuration;
        var contextMock = this.CreateGameContextMock();
        try
        {
            var state = this._plugIn.GetStateForTest(contextMock.Object);
            state.NextRunUtc = DateTime.UtcNow.AddHours(1);
            state.LastRunUtc = DateTime.UtcNow;

            this._plugIn.ForceStart();
            await this._plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

            Assert.That(state.State, Is.EqualTo(PeriodicTaskState.Prepared));
        }
        finally
        {
            await contextMock.Object.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tests the event-active check: no games, an open game, and a disposed game.
    /// </summary>
    [Test]
    public async Task IsEventActiveReflectsLiveGamesAsync()
    {
        var (fakeContext, manager) = this.CreateFakeContext();

        Assert.That(this._plugIn.IsEventActive(fakeContext.Object), Is.False);

        var game = this.CreateGame(fakeContext, manager, this.CreateDefinition(TimeSpan.FromMinutes(2)));
        manager.Games.Add(game);
        Assert.That(this._plugIn.IsEventActive(fakeContext.Object), Is.True);

        await game.DisposeAsync().ConfigureAwait(false);
        Assert.That(this._plugIn.IsEventActive(fakeContext.Object), Is.False);
    }

    /// <summary>
    /// Tests the full announcement cycle: the opening message on start, silence on
    /// following ticks, the closing message exactly once when the entrance ends,
    /// and silence afterwards.
    /// </summary>
    [Test]
    public async Task OpenAnnouncementThenCloseAnnouncementAsync()
    {
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        var (fakeContext, manager, notifications) = this.CreateAnnouncingContext(definition);
        var game = this.CreateGame(fakeContext, manager, definition);
        manager.GameToCreate = game;
        var state = new PeriodicTaskGameServerState(fakeContext.Object);

        await this._plugIn.StartForTestAsync(state).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2" }));

        // The entrance is still open with two minutes left: no follow-up message.
        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2" }));

        // The entrance ends: the closing message goes out exactly once.
        manager.Games.Remove(game);
        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2", "Closed" }));

        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2", "Closed" }));
    }

    /// <summary>
    /// Tests that a short entrance without an opening message still gets its
    /// closing message when the entrance ends.
    /// </summary>
    [Test]
    public async Task ShortEntranceStillAnnouncesClosingAsync()
    {
        var definition = this.CreateDefinition(TimeSpan.FromSeconds(30));
        var (fakeContext, manager, notifications) = this.CreateAnnouncingContext(definition);
        var game = this.CreateGame(fakeContext, manager, definition);
        manager.GameToCreate = game;
        var state = new PeriodicTaskGameServerState(fakeContext.Object);

        await this._plugIn.StartForTestAsync(state).ConfigureAwait(false);
        Assert.That(notifications, Is.Empty);

        manager.Games.Remove(game);
        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Closed" }));

        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Closed" }));
    }

    /// <summary>
    /// Tests the originally reported bug: skipping the entering phase closes the
    /// entrance, and the next tick announces the closing instead of further countdowns.
    /// </summary>
    [Test]
    public async Task SkipDuringEnterAnnouncesClosedOnNextTickAsync()
    {
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        var (fakeContext, manager, notifications) = this.CreateAnnouncingContext(definition);
        var game = this.CreateGame(fakeContext, manager, definition);
        manager.GameToCreate = game;
        var state = new PeriodicTaskGameServerState(fakeContext.Object);

        await this._plugIn.StartForTestAsync(state).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2" }));

        Assert.That(game.SkipCurrentWait(), Is.True);
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (game.State == MiniGameState.Open && DateTime.UtcNow < deadline)
        {
            await Task.Delay(50).ConfigureAwait(false);
        }

        Assert.That(game.State, Is.Not.EqualTo(MiniGameState.Open));

        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.EqualTo(new[] { "Open 2", "Closed" }));
    }

    /// <summary>
    /// Tests that the entrance queries never create games: no announcement state may
    /// reference a game that was never started.
    /// </summary>
    [Test]
    public async Task EntranceQueriesCreateNoGamesAsync()
    {
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        var (fakeContext, manager) = this.CreateFakeContext();

        // The fake manager throws when asked to create a game.
        var duration = await this._plugIn.GetDurationUntilNextStartAsync(fakeContext.Object, definition).ConfigureAwait(false);
        Assert.That(duration, Is.Not.EqualTo(TimeSpan.Zero));

        var context = await this._plugIn.GetMiniGameContextAsync(fakeContext.Object, definition).ConfigureAwait(false);
        Assert.That(context, Is.Null);
        Assert.That(manager.Games, Is.Empty);
    }

    /// <summary>
    /// Tests that games with a non-shared map creation policy never drive the
    /// global entrance countdown.
    /// </summary>
    [Test]
    public async Task NonSharedGamesDoNotDriveGlobalCountdownAsync()
    {
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerPlayer;
        var (fakeContext, manager, notifications) = this.CreateAnnouncingContext(definition);
        var game = this.CreateGame(fakeContext, manager, definition);
        manager.Games.Add(game);

        await this._plugIn.AnnounceEntranceAsync(fakeContext.Object).ConfigureAwait(false);
        Assert.That(notifications, Is.Empty);
    }
    private (Mock<IGameContext> Context, TestMiniGameManager Manager) CreateFakeContext()
    {
        var contextMock = new Mock<IGameContext>();
        var manager = new TestMiniGameManager();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(new GameConfiguration());
        contextMock.SetupGet(c => c.DropGenerator).Returns(NullDropGenerator.Instance);
        contextMock.SetupGet(c => c.MiniGames).Returns(manager);
        return (contextMock, manager);
    }

    private (Mock<IGameContext> Context, TestMiniGameManager Manager, List<string> Notifications) CreateAnnouncingContext(MiniGameDefinition definition)
    {
        var (contextMock, manager) = this.CreateFakeContext();
        var notifications = new List<string>();
        contextMock.Setup(c => c.SendGlobalNotificationAsync(It.IsAny<string>()))
            .Callback<string>(notifications.Add)
            .Returns(ValueTask.CompletedTask);
        var configurationMock = new Mock<GameConfiguration>();
        configurationMock.SetupGet(c => c.MiniGameDefinitions).Returns(new List<MiniGameDefinition> { definition });
        contextMock.SetupGet(c => c.Configuration).Returns(configurationMock.Object);
        this._configuration.EntranceOpenedMessage = "Open {0}";
        this._configuration.EntranceClosedMessage = "Closed";
        return (contextMock, manager, notifications);
    }

    private MiniGameDefinition CreateDefinition(TimeSpan enterDuration)
    {
        var mapDefinition = new GameMapDefinition { Number = 0 };
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = mapDefinition });
        var definition = definitionMock.Object;
        definition.Type = MiniGameType.BloodCastle;
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        definition.EnterDuration = enterDuration;
        definition.GameDuration = TimeSpan.FromMinutes(5);
        definition.ExitDuration = TimeSpan.FromMinutes(1);
        definition.MaximumPlayerCount = 10;
        return definition;
    }

    private MiniGameContext CreateGame(
        Mock<IGameContext> gameContextMock,
        TestMiniGameManager manager,
        MiniGameDefinition definition)
    {
        var mapInitializerMock = new Mock<IMapInitializer>();
        mapInitializerMock.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(mapDefinition => new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 16));
        var mapDefinition = definition.Entrance?.Map ?? throw new InvalidOperationException("Test definition has no entrance map.");
        var game = new MiniGameContext(
            new MiniGameMapKey(mapDefinition.Number, definition.GameLevel, string.Empty),
            definition,
            gameContextMock.Object,
            mapInitializerMock.Object);
        this._gamesToDispose.Add(game);
        return game;
    }

    /// <summary>
    /// Tests that removing a stale instance never unregisters its replacement: after a
    /// forced restart recreates the game, the late remove of the killed run must keep
    /// the new instance registered.
    /// </summary>
    [Test]
    public async Task RemoveOfStaleInstanceKeepsReplacementAsync()
    {
        var (_, manager) = this.CreateRealManager();
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        var first = await manager.GetOrCreateAsync(definition, null!).ConfigureAwait(false);
        this._gamesToDispose.Add(first);
        await manager.RemoveAsync(first).ConfigureAwait(false);

        var second = await manager.GetOrCreateAsync(definition, null!).ConfigureAwait(false);
        this._gamesToDispose.Add(second);
        Assert.That(second, Is.Not.SameAs(first));

        await manager.RemoveAsync(first).ConfigureAwait(false);
        Assert.That(manager.TryGetRunningMiniGame(definition, null), Is.SameAs(second));
    }

    /// <summary>
    /// Tests that looking up a non-shared game without a requester returns null
    /// instead of throwing.
    /// </summary>
    [Test]
    public void TryGetRunningWithNullRequesterAndNonSharedPolicyReturnsNull()
    {
        var manager = new MiniGameManager(Mock.Of<IGameContext>(), Mock.Of<IMapInitializer>());
        var definition = this.CreateDefinition(TimeSpan.FromMinutes(2));
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerPlayer;

        Assert.That(manager.TryGetRunningMiniGame(definition, null), Is.Null);
    }

    private (Mock<IGameContext> Context, MiniGameManager Manager) CreateRealManager()
    {
        var contextMock = new Mock<IGameContext>();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.DropGenerator).Returns(NullDropGenerator.Instance);
        var mapInitializerMock = new Mock<IMapInitializer>();
        mapInitializerMock.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(mapDefinition => new GameMap(mapDefinition, TimeSpan.FromMinutes(1), 16));
        var manager = new MiniGameManager(contextMock.Object, mapInitializerMock.Object);
        contextMock.SetupGet(c => c.MiniGames).Returns(manager);
        return (contextMock, manager);
    }

    private Mock<GameContext> CreateGameContextMock()
    {
        var duelConfigurationMock = new Mock<DuelConfiguration>();
        duelConfigurationMock.SetupGet(c => c.DuelAreas).Returns(new List<DuelArea>());
        var configuration = new GameConfiguration
        {
            DuelConfiguration = duelConfigurationMock.Object,
            MaximumPartySize = 5,
            MaximumLevel = 400,
        };
        var contextMock = new Mock<GameContext>(
            configuration,
            Mock.Of<IPersistenceContextProvider>(),
            Mock.Of<IMapInitializer>(),
            NullLoggerFactory.Instance,
            new PlugInManager(null, NullLoggerFactory.Instance, null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());

        // No plug-in discovery (null configurations): the background timers must not
        // run real periodic plug-ins against this barely-configured context.
        // Stopping the tasks as well, so that nothing fires during the tests at all.
        contextMock.Object.StopPeriodicTasks();
        return contextMock;
    }

    /// <summary>
    /// A test double of the start plug-in which exposes the protected seams.
    /// </summary>
    private sealed class TestStartPlugIn : MiniGameStartBasePlugIn<TestStartConfiguration, PeriodicTaskGameServerState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStartPlugIn"/> class.
        /// </summary>
        /// <param name="stateContext">The context which started states belong to.</param>
        public TestStartPlugIn(IGameContext stateContext)
        {
            this.StateContext = stateContext;
        }

        /// <summary>
        /// Gets the context which started states belong to.
        /// </summary>
        public IGameContext StateContext { get; }

        /// <inheritdoc />
        public override MiniGameType Key => MiniGameType.BloodCastle;

        /// <inheritdoc />
        public override object CreateDefaultConfig() => new TestStartConfiguration();

        /// <summary>
        /// Gets the state for the given context.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <returns>The state for the given context.</returns>
        public PeriodicTaskGameServerState GetStateForTest(IGameContext gameContext) => this.GetStateByGameContext(gameContext);

        /// <summary>
        /// Runs the start logic for the given state.
        /// </summary>
        /// <param name="state">The state.</param>
        public Task StartForTestAsync(PeriodicTaskGameServerState state) => this.OnStartedAsync(state).AsTask();

        /// <inheritdoc />
        protected override PeriodicTaskGameServerState CreateState(IGameContext gameContext) => new(this.StateContext);
    }

    /// <summary>
    /// A concrete start configuration for tests.
    /// </summary>
    private sealed class TestStartConfiguration : MiniGameStartConfiguration
    {
    }

    /// <summary>
    /// An in-memory mini game manager for tests.
    /// </summary>
    private sealed class TestMiniGameManager : IMiniGameManager
    {
        /// <summary>
        /// Gets the currently tracked games.
        /// </summary>
        public List<MiniGameContext> Games { get; } = new();

        /// <summary>
        /// Gets or sets the canned game which <see cref="GetOrCreateAsync"/> returns.
        /// </summary>
        public MiniGameContext? GameToCreate { get; set; }

        /// <inheritdoc />
        public event EventHandler<GameMap>? GameMapCreated;

        /// <inheritdoc />
        public event EventHandler<GameMap>? GameMapRemoved;

        /// <inheritdoc />
        public IReadOnlyList<GameMap> Maps => this.Games.Select(game => game.Map).ToList();

        /// <inheritdoc />
        public ValueTask<MiniGameContext> GetOrCreateAsync(MiniGameDefinition miniGameDefinition, Player requester)
        {
            if (this.GameToCreate is { } game)
            {
                if (!this.Games.Contains(game))
                {
                    this.Games.Add(game);
                }

                return ValueTask.FromResult(game);
            }

            throw new InvalidOperationException("No game prepared for creation.");
        }

        /// <inheritdoc />
        public MiniGameContext? TryGetRunningMiniGame(MiniGameDefinition miniGameDefinition, Player? requester)
        {
            return this.Games.FirstOrDefault(game => game.Definition == miniGameDefinition);
        }

        /// <inheritdoc />
        public IReadOnlyList<MiniGameContext> GetRunningMiniGames(MiniGameType miniGameType)
        {
            return this.Games.Where(game => game.Definition.Type == miniGameType).ToList();
        }

        /// <inheritdoc />
        public ValueTask RemoveAsync(MiniGameContext miniGameContext)
        {
            this.Games.Remove(miniGameContext);
            return ValueTask.CompletedTask;
        }
    }
}
