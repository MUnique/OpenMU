// <copyright file="PeriodicTaskServerScopeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the server scope of periodic tasks (<see cref="PeriodicTaskConfiguration.AllowedServerIds"/>):
/// an empty list runs everywhere, a non-empty list restricts the event to the listed game servers,
/// so open-world events can't be farmed again by switching servers.
/// </summary>
[TestFixture]
public class PeriodicTaskServerScopeTests
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
    /// An empty allow-list runs on any server.
    /// </summary>
    [Test]
    public void EmptyListIsEnabledOnAnyServer()
    {
        var configuration = new PeriodicTaskConfiguration { AllowedServerIds = new List<byte>() };

        Assert.That(configuration.IsEnabledOnServer(3), Is.True);
    }

    /// <summary>
    /// A missing list (e.g. a stored configuration from before the filter existed) runs everywhere.
    /// </summary>
    [Test]
    public void NullListIsEnabledOnAnyServer()
    {
        var configuration = new PeriodicTaskConfiguration { AllowedServerIds = null! };

        Assert.That(configuration.IsEnabledOnServer(3), Is.True);
    }

    /// <summary>
    /// A listed server runs the event.
    /// </summary>
    [Test]
    public void ListedServerIsEnabled()
    {
        var configuration = new PeriodicTaskConfiguration { AllowedServerIds = new List<byte> { 1, 2 } };

        Assert.That(configuration.IsEnabledOnServer(1), Is.True);
    }

    /// <summary>
    /// An unlisted server skips the event.
    /// </summary>
    [Test]
    public void UnlistedServerIsDisabled()
    {
        var configuration = new PeriodicTaskConfiguration { AllowedServerIds = new List<byte> { 1, 2 } };

        Assert.That(configuration.IsEnabledOnServer(0), Is.False);
    }

    /// <summary>
    /// A start on an unlisted server never happens: neither forced nor on schedule.
    /// </summary>
    [Test]
    public async Task StartOnUnlistedServerDoesNothingAsync()
    {
        var plugIn = this.CreatePlugIn(
            allowedServerIds: new List<byte> { 1 },
            timetable: new List<TimeOnly> { TimeOnly.FromDateTime(DateTime.UtcNow) });
        var contextMock = this.CreateServerContextMock(serverId: 0);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(contextMock.Object).State, Is.EqualTo(PeriodicTaskState.NotStarted));
    }

    /// <summary>
    /// A forced start on a listed server prepares the event.
    /// </summary>
    [Test]
    public async Task ForcedStartOnListedServerPreparesTheEventAsync()
    {
        var plugIn = this.CreatePlugIn(allowedServerIds: new List<byte> { 1 });
        var contextMock = this.CreateServerContextMock(serverId: 1);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(contextMock.Object).State, Is.EqualTo(PeriodicTaskState.Prepared));
    }

    /// <summary>
    /// A forced start reaches every allowed server: each context consumes it independently
    /// instead of racing for a single shared flag.
    /// </summary>
    [Test]
    public async Task ForcedStartReachesEveryAllowedServerAsync()
    {
        var plugIn = this.CreatePlugIn(allowedServerIds: new List<byte> { 0, 1 });
        var firstMock = this.CreateServerContextMock(serverId: 0);
        var secondMock = this.CreateServerContextMock(serverId: 1);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(firstMock.Object).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(secondMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(secondMock.Object).State, Is.EqualTo(PeriodicTaskState.Prepared));
    }

    /// <summary>
    /// A skipped server observes the forced start for itself, but the force still fires on allowed servers.
    /// </summary>
    [Test]
    public async Task SkippedServerPreservesForcedStartForOthersAsync()
    {
        var plugIn = this.CreatePlugIn(allowedServerIds: new List<byte> { 1 });
        var skippedMock = this.CreateServerContextMock(serverId: 0);
        var allowedMock = this.CreateServerContextMock(serverId: 1);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(skippedMock.Object).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(allowedMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(allowedMock.Object).State, Is.EqualTo(PeriodicTaskState.Prepared));
    }

    /// <summary>
    /// The server list survives a save/load round-trip through the plug-in configuration storage.
    /// </summary>
    [Test]
    public void AllowedServerIdsSurviveConfigurationRoundTrip()
    {
        var configuration = new PlugInConfiguration();
        configuration.SetConfiguration(new PeriodicTaskConfiguration { AllowedServerIds = new List<byte> { 0, 2 } }, null);

        Assert.That(configuration.GetConfiguration<PeriodicTaskConfiguration>(null)?.AllowedServerIds, Is.EqualTo(new byte[] { 0, 2 }));
    }

    /// <summary>
    /// A stored configuration from before the filter existed deserializes to an empty list (all servers).
    /// </summary>
    [Test]
    public void MissingServerListDeserializesToAllServers()
    {
        var configuration = new PlugInConfiguration { CustomConfiguration = "{\"Timetable\":[]}" };

        Assert.That(configuration.GetConfiguration<PeriodicTaskConfiguration>(null)?.AllowedServerIds, Is.Empty);
    }

    /// <summary>
    /// Contexts without a game server (e.g. plain test contexts) always run the task.
    /// </summary>
    [Test]
    public async Task ContextWithoutGameServerAlwaysRunsAsync()
    {
        var plugIn = this.CreatePlugIn(allowedServerIds: new List<byte> { 1 });
        var contextMock = this.CreateServerContextMock(serverId: null);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(contextMock.Object).State, Is.EqualTo(PeriodicTaskState.Prepared));
    }

    /// <summary>
    /// A run which started while its server was listed still finishes after the server
    /// is removed from the list, so no monsters are left behind.
    /// </summary>
    [Test]
    public async Task RunFinishesAfterServerIsRemovedMidRunAsync()
    {
        // Built inline (instead of CreatePlugIn) to keep a reference to the configuration for the mid-run change.
        var configuration = new PeriodicTaskConfiguration
        {
            AllowedServerIds = new List<byte> { 1 },
            Timetable = new List<TimeOnly>(),
            TaskDuration = TimeSpan.FromHours(1),
            PreStartMessageDelay = TimeSpan.FromSeconds(3),
        };
        var plugIn = new TestScopePlugIn { Configuration = configuration };
        var contextMock = this.CreateServerContextMock(serverId: 1);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        var state = plugIn.GetState(contextMock.Object);
        state.NextRunUtc = DateTime.UtcNow.AddSeconds(-1);
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        configuration.AllowedServerIds = new List<byte> { 2 };
        state.NextRunUtc = DateTime.UtcNow.AddSeconds(-1);
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.FinishedCount, Is.EqualTo(1));
    }

    /// <summary>
    /// The entrance countdown is hidden on servers without the event, so no opening is announced there.
    /// </summary>
    [Test]
    public async Task CountdownIsNullOnUnlistedServerAsync()
    {
        var plugIn = new TestScopeMiniGamePlugIn
        {
            Configuration = new TestMiniGameStartConfiguration
            {
                AllowedServerIds = new List<byte> { 1 },
                Timetable = new List<TimeOnly>(),
            },
        };
        var contextMock = new Mock<IGameContext>();
        contextMock.As<IGameServerContext>().SetupGet(c => c.Id).Returns((byte)0);
        contextMock.SetupGet(c => c.MiniGames).Returns(Mock.Of<IMiniGameManager>());

        var remaining = await plugIn.GetDurationUntilNextStartAsync(contextMock.Object, new MiniGameDefinition()).ConfigureAwait(false);

        Assert.That(remaining, Is.Null);
    }

    /// <summary>
    /// The entrance countdown is still reported on servers with the event.
    /// </summary>
    [Test]
    public async Task CountdownIsReportedOnListedServerAsync()
    {
        var plugIn = new TestScopeMiniGamePlugIn
        {
            Configuration = new TestMiniGameStartConfiguration
            {
                AllowedServerIds = new List<byte> { 1 },
                Timetable = new List<TimeOnly> { TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(1)) },
            },
        };
        var contextMock = new Mock<IGameContext>();
        contextMock.As<IGameServerContext>().SetupGet(c => c.Id).Returns((byte)1);
        contextMock.SetupGet(c => c.MiniGames).Returns(Mock.Of<IMiniGameManager>());

        var remaining = await plugIn.GetDurationUntilNextStartAsync(contextMock.Object, new MiniGameDefinition()).ConfigureAwait(false);

        Assert.That(remaining, Is.GreaterThan(TimeSpan.Zero));
    }

    /// <summary>
    /// The Happy Hour bonus applies only on allowed servers: each server owns its multiplier element.
    /// </summary>
    [Test]
    public async Task HappyHourBonusStaysOnAllowedServerAsync()
    {
        var plugIn = new HappyHourPlugIn
        {
            Configuration = new HappyHourConfiguration
            {
                AllowedServerIds = new List<byte> { 0 },
                Timetable = new List<TimeOnly>(),
                TaskDuration = TimeSpan.FromHours(1),
                PreStartMessageDelay = TimeSpan.Zero,
                ExperienceMultiplier = 2.0f,
            },
        };
        var allowedMock = this.CreateServerContextMock(serverId: 0);
        var excludedMock = this.CreateServerContextMock(serverId: 1);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(allowedMock.Object).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(allowedMock.Object).ConfigureAwait(false);
        await plugIn.ExecuteTaskAsync(excludedMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetMultiplierElement(allowedMock.Object), Is.Not.SameAs(plugIn.GetMultiplierElement(excludedMock.Object)));
        Assert.That(plugIn.GetMultiplierElement(allowedMock.Object).Value, Is.EqualTo(2.0f));
        Assert.That(plugIn.GetMultiplierElement(excludedMock.Object).Value, Is.EqualTo(1.0f));
    }

    /// <summary>
    /// A server added back to the list doesn't start a stale forced event: while excluded,
    /// its ticks already observed (and consumed) the force.
    /// </summary>
    [Test]
    public async Task ReAddedServerDoesNotStartStaleForcedEventAsync()
    {
        var configuration = new PeriodicTaskConfiguration
        {
            AllowedServerIds = new List<byte> { 1 },
            Timetable = new List<TimeOnly>(),
            TaskDuration = TimeSpan.FromHours(1),
            PreStartMessageDelay = TimeSpan.FromSeconds(3),
        };
        var plugIn = new TestScopePlugIn { Configuration = configuration };
        var contextMock = this.CreateServerContextMock(serverId: 0);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        configuration.AllowedServerIds = new List<byte> { 0 };
        await plugIn.ExecuteTaskAsync(contextMock.Object).ConfigureAwait(false);

        Assert.That(plugIn.GetState(contextMock.Object).State, Is.EqualTo(PeriodicTaskState.NotStarted));
    }

    /// <summary>
    /// Open games are still announced on servers without the event, so in-flight runs finish gracefully.
    /// </summary>
    [Test]
    public async Task OpenGamesAreAnnouncedOnUnlistedServerAsync()
    {
        var (plugIn, contextMock, server, manager, definition, notifications) = await this.CreateOpenGameScenarioAsync(serverId: 1).ConfigureAwait(false);
        try
        {
            server.SetupGet(c => c.Id).Returns((byte)0);
            await plugIn.AnnounceEntranceAsync(contextMock.Object).ConfigureAwait(false);
            await DisposeRunningGameAsync(manager, definition).ConfigureAwait(false);
            await plugIn.AnnounceEntranceAsync(contextMock.Object).ConfigureAwait(false);

            Assert.That(notifications, Is.EqualTo(new[] { "Open 5", "Closed" }));
        }
        finally
        {
            await DisposeRunningGameAsync(manager, definition).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// An open lobby stays joinable on servers without the event instead of answering "not open".
    /// </summary>
    [Test]
    public async Task OpenLobbyIsJoinableOnUnlistedServerAsync()
    {
        var (plugIn, contextMock, server, manager, definition, _) = await this.CreateOpenGameScenarioAsync(serverId: 1).ConfigureAwait(false);
        try
        {
            server.SetupGet(c => c.Id).Returns((byte)0);
            var remaining = await plugIn.GetDurationUntilNextStartAsync(contextMock.Object, definition).ConfigureAwait(false);

            Assert.That(remaining, Is.EqualTo(TimeSpan.Zero));
        }
        finally
        {
            await DisposeRunningGameAsync(manager, definition).ConfigureAwait(false);
        }
    }

    private async ValueTask<(TestScopeMiniGamePlugIn PlugIn, Mock<IGameContext> Context, Mock<IGameServerContext> Server, MiniGameManager Manager, MiniGameDefinition Definition, List<string> Notifications)> CreateOpenGameScenarioAsync(byte serverId)
    {
        var plugIn = new TestScopeMiniGamePlugIn
        {
            Configuration = new TestMiniGameStartConfiguration
            {
                AllowedServerIds = new List<byte> { 1 },
                Timetable = new List<TimeOnly>(),
                EntranceOpenedMessage = "Open {0}",
                EntranceClosedMessage = "Closed",
            },
        };
        var mapDefinition = new GameMapDefinition { Number = 0 };
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = mapDefinition });
        var definition = definitionMock.Object;
        definition.Type = MiniGameType.BloodCastle;
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        definition.EnterDuration = TimeSpan.FromMinutes(5);
        definition.GameDuration = TimeSpan.FromMinutes(5);
        definition.ExitDuration = TimeSpan.FromMinutes(1);
        definition.MaximumPlayerCount = 10;

        var configurationMock = new Mock<GameConfiguration>();
        configurationMock.SetupGet(c => c.MiniGameDefinitions).Returns(new List<MiniGameDefinition> { definition });

        var contextMock = new Mock<IGameContext>();
        var server = contextMock.As<IGameServerContext>();
        server.SetupGet(c => c.Id).Returns(serverId);
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.DropGenerator).Returns(NullDropGenerator.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(configurationMock.Object);
        var notifications = new List<string>();
        contextMock.Setup(c => c.SendGlobalNotificationAsync(It.IsAny<string>()))
            .Callback<string>(notifications.Add)
            .Returns(ValueTask.CompletedTask);
        contextMock.Setup(c => c.SendGlobalNotificationAsync(It.IsAny<LocalizedString>(), It.IsAny<object?[]>()))
            .Callback<LocalizedString, object?[]>((message, arguments) => notifications.Add(string.Format(CultureInfo.InvariantCulture, message.ValueInNeutralLanguage, arguments)))
            .Returns(ValueTask.CompletedTask);
        var mapInitializerMock = new Mock<IMapInitializer>();
        mapInitializerMock.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));
        var manager = new MiniGameManager(contextMock.Object, mapInitializerMock.Object);
        contextMock.SetupGet(c => c.MiniGames).Returns(manager);

        var state = new PeriodicTaskGameServerState(contextMock.Object);
        await plugIn.StartForTestAsync(state).ConfigureAwait(false);

        return (plugIn, contextMock, server, manager, definition, notifications);
    }

    private static async ValueTask DisposeRunningGameAsync(MiniGameManager manager, MiniGameDefinition definition)
    {
        if (manager.TryGetRunningMiniGame(definition, null) is { } game)
        {
            await game.DisposeAsync().ConfigureAwait(false);
        }
    }

    private TestScopePlugIn CreatePlugIn(IList<byte> allowedServerIds, IList<TimeOnly>? timetable = null)
    {
        var plugIn = new TestScopePlugIn();
        plugIn.Configuration = new PeriodicTaskConfiguration
        {
            AllowedServerIds = allowedServerIds,
            Timetable = timetable ?? new List<TimeOnly>(),
            TaskDuration = TimeSpan.FromHours(1),
            PreStartMessageDelay = TimeSpan.FromSeconds(3),
        };

        return plugIn;
    }

    private Mock<GameContext> CreateServerContextMock(byte? serverId)
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
        // The interface setup must precede the first Object access (see Moq docs).
        if (serverId.HasValue)
        {
            contextMock.As<IGameServerContext>().SetupGet(c => c.Id).Returns(serverId.Value);
        }

        contextMock.Object.StopPeriodicTasks();
        this._teardowns.Add(contextMock.Object);

        return contextMock;
    }

    /// <summary>
    /// A minimal mini game start plug-in for the countdown scope tests.
    /// </summary>
    private sealed class TestScopeMiniGamePlugIn : MiniGameStartBasePlugIn<TestMiniGameStartConfiguration, PeriodicTaskGameServerState>
    {
        /// <inheritdoc />
        public override MiniGameType Key => MiniGameType.BloodCastle;

        /// <inheritdoc />
        public override object CreateDefaultConfig() => new TestMiniGameStartConfiguration();

        /// <summary>
        /// Drives the start path for a prepared state.
        /// </summary>
        /// <param name="state">The state.</param>
        public ValueTask StartForTestAsync(PeriodicTaskGameServerState state) => this.OnStartedAsync(state);

        /// <inheritdoc />
        protected override PeriodicTaskGameServerState CreateState(IGameContext gameContext) => new(gameContext);
    }

    /// <summary>
    /// A minimal mini game start configuration for the countdown scope tests.
    /// </summary>
    private sealed class TestMiniGameStartConfiguration : MiniGameStartConfiguration
    {
    }

    /// <summary>
    /// A minimal periodic task which exposes the protected seams for the scope tests.
    /// </summary>
    private sealed class TestScopePlugIn : PeriodicTaskBasePlugIn<PeriodicTaskConfiguration, PeriodicTaskGameServerState>
    {
        /// <summary>
        /// Gets the number of finished runs.
        /// </summary>
        public int FinishedCount { get; private set; }

        /// <summary>
        /// Gets the state of the specified context.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public PeriodicTaskGameServerState GetState(GameContext gameContext) => this.GetStateByGameContext(gameContext);

        /// <inheritdoc />
        protected override PeriodicTaskGameServerState CreateState(IGameContext gameContext) => new(gameContext);

        /// <inheritdoc />
        protected override ValueTask OnPrepareEventAsync(PeriodicTaskGameServerState state) => ValueTask.CompletedTask;

        /// <inheritdoc />
        protected override ValueTask OnPreparedAsync(PeriodicTaskGameServerState state) => ValueTask.CompletedTask;

        /// <inheritdoc />
        protected override ValueTask OnStartedAsync(PeriodicTaskGameServerState state) => ValueTask.CompletedTask;

        /// <inheritdoc />
        protected override ValueTask OnFinishedAsync(PeriodicTaskGameServerState state)
        {
            this.FinishedCount++;
            return ValueTask.CompletedTask;
        }
    }
}
