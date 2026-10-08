// <copyright file="MiniGamePlugInPointTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the plugin points of the mini game lifecycle:
/// <see cref="IMiniGameEntranceOpenedPlugIn"/>, <see cref="IMiniGameStartedPlugIn"/> and <see cref="IMiniGameEndedPlugIn"/>.
/// </summary>
[TestFixture]
public class MiniGamePlugInPointTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private readonly List<MiniGameContext> _gamesToDispose = new();

    /// <summary>
    /// Disposes the games, so that their background loops don't leak into other tests.
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        foreach (var game in this._gamesToDispose)
        {
            await game.DisposeAsync().ConfigureAwait(false);
        }

        this._gamesToDispose.Clear();
    }

    /// <summary>
    /// Tests that the plugins are notified about the opened entrance, the start and the end of a game.
    /// </summary>
    [Test]
    public async Task LifecycleIsReportedAsync()
    {
        var (plugInManager, contextMock) = CreateGameContext();
        var plugIn = new RecordingPlugIn();
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameEntranceOpenedPlugIn>(plugIn);
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameStartedPlugIn>(plugIn);
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameEndedPlugIn>(plugIn);

        var game = this.CreateGame(contextMock.Object, minimumPlayerCount: 0);
        await WaitUntilAsync(() => game.State == MiniGameState.Playing).ConfigureAwait(false);
        game.Finish();
        await WaitUntilAsync(() => plugIn.Calls.Contains("Ended")).ConfigureAwait(false);

        Assert.That(plugIn.Calls, Is.EqualTo(new[] { "Opened", "Started", "Ended" }));
        Assert.That(plugIn.Games, Is.All.SameAs(game));
    }

    /// <summary>
    /// Tests that the start and end aren't reported when the game doesn't start, because not enough players entered.
    /// </summary>
    [Test]
    public async Task GameWithoutPlayersReportsOnlyOpenedEntranceAsync()
    {
        var (plugInManager, contextMock) = CreateGameContext();
        var plugIn = new RecordingPlugIn();
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameEntranceOpenedPlugIn>(plugIn);
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameStartedPlugIn>(plugIn);
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameEndedPlugIn>(plugIn);

        var game = this.CreateGame(contextMock.Object, minimumPlayerCount: 1);
        await WaitUntilAsync(() => game.State == MiniGameState.Disposed).ConfigureAwait(false);

        Assert.That(plugIn.Calls, Is.EqualTo(new[] { "Opened" }));
    }

    /// <summary>
    /// Tests that a failing plugin doesn't break the game.
    /// </summary>
    [Test]
    public async Task FailingPlugInDoesNotBreakTheGameAsync()
    {
        var (plugInManager, contextMock) = CreateGameContext();
        plugInManager.RegisterPlugInAtPlugInPoint<IMiniGameEntranceOpenedPlugIn>(new FailingPlugIn());

        var game = this.CreateGame(contextMock.Object, minimumPlayerCount: 0);
        await WaitUntilAsync(() => game.State == MiniGameState.Playing).ConfigureAwait(false);

        Assert.That(game.IsDisposed, Is.False);
    }

    private static (PlugInManager PlugInManager, Mock<IGameContext> Context) CreateGameContext()
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var contextMock = new Mock<IGameContext>();
        contextMock.SetupGet(c => c.LoggerFactory).Returns(NullLoggerFactory.Instance);
        contextMock.SetupGet(c => c.Configuration).Returns(new GameConfiguration());
        contextMock.SetupGet(c => c.DropGenerator).Returns(NullDropGenerator.Instance);
        contextMock.SetupGet(c => c.PlugInManager).Returns(plugInManager);
        contextMock.SetupGet(c => c.MiniGames).Returns(Mock.Of<IMiniGameManager>());
        return (plugInManager, contextMock);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.Add(Timeout);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(20).ConfigureAwait(false);
        }

        Assert.That(condition(), Is.True, "The condition wasn't met in time.");
    }

    private TestGame CreateGame(IGameContext gameContext, int minimumPlayerCount)
    {
        var mapDefinition = new GameMapDefinition { Number = 0 };
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.ChangeEvents).Returns(new List<MiniGameChangeEvent>());
        definitionMock.SetupGet(d => d.Entrance).Returns(new ExitGate { Map = mapDefinition });
        var definition = definitionMock.Object;
        definition.Type = MiniGameType.BloodCastle;
        definition.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        definition.EnterDuration = TimeSpan.Zero;
        definition.GameDuration = TimeSpan.FromMinutes(5);
        definition.ExitDuration = TimeSpan.FromMinutes(1);
        definition.MaximumPlayerCount = 10;

        var mapInitializerMock = new Mock<IMapInitializer>();
        mapInitializerMock.Setup(m => m.CreateGameMap(It.IsAny<GameMapDefinition>()))
            .Returns<GameMapDefinition>(map => new GameMap(map, TimeSpan.FromMinutes(1), 16));

        var game = new TestGame(new MiniGameMapKey(0, 0, string.Empty), definition, gameContext, mapInitializerMock.Object, minimumPlayerCount);
        game.EnsureGameLoopRunning();
        this._gamesToDispose.Add(game);
        return game;
    }

    /// <summary>
    /// A game which starts without waiting and can be finished by the test.
    /// </summary>
    private sealed class TestGame : MiniGameContext
    {
        private readonly int _minimumPlayerCount;

        public TestGame(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer, int minimumPlayerCount)
            : base(key, definition, gameContext, mapInitializer)
        {
            this._minimumPlayerCount = minimumPlayerCount;
        }

        protected override TimeSpan MinimumEnterDuration => TimeSpan.Zero;

        protected override TimeSpan CountdownDuration => TimeSpan.Zero;

        protected override int MinimumPlayerCount => this._minimumPlayerCount;

        public void Finish() => this.FinishEvent();
    }

    [Guid("C892C41E-517F-4CC9-9CCC-E078049E5355")]
    private sealed class RecordingPlugIn : IMiniGameEntranceOpenedPlugIn, IMiniGameStartedPlugIn, IMiniGameEndedPlugIn
    {
        public List<string> Calls { get; } = new();

        public List<MiniGameContext> Games { get; } = new();

        public ValueTask MiniGameEntranceOpenedAsync(MiniGameContext miniGame)
        {
            this.Record("Opened", miniGame);
            return ValueTask.CompletedTask;
        }

        public ValueTask MiniGameStartedAsync(MiniGameContext miniGame, IReadOnlyCollection<Player> players)
        {
            this.Record("Started", miniGame);
            return ValueTask.CompletedTask;
        }

        public ValueTask MiniGameEndedAsync(MiniGameContext miniGame, Player? winner, IReadOnlyCollection<Player> finishers)
        {
            this.Record("Ended", miniGame);
            return ValueTask.CompletedTask;
        }

        private void Record(string call, MiniGameContext miniGame)
        {
            lock (this.Calls)
            {
                this.Calls.Add(call);
                this.Games.Add(miniGame);
            }
        }
    }

    [Guid("D80AB130-8A88-4641-B928-24870345AFC2")]
    private sealed class FailingPlugIn : IMiniGameEntranceOpenedPlugIn
    {
        public ValueTask MiniGameEntranceOpenedAsync(MiniGameContext miniGame)
        {
            throw new InvalidOperationException("Test");
        }
    }
}
