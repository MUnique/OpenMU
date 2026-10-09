// <copyright file="MiniGameRejoinTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Tests that a player which lost its connection during a mini game can rejoin it,
/// as long as the game is still running.
/// </summary>
[TestFixture]
public class MiniGameRejoinTests
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
    /// Tests that a disconnected player rejoins the running game at its previous position, and only once.
    /// </summary>
    [Test]
    public async Task DisconnectedPlayerRejoinsRunningGameAsync()
    {
        var (game, player) = await this.CreateRunningGameWithDisconnectedPlayerAsync().ConfigureAwait(false);
        Assert.That(player.CurrentMiniGame, Is.Null);

        var reconnected = await CreateReconnectedPlayerAsync(player).ConfigureAwait(false);
        var reconnectedCharacter = reconnected.SelectedCharacter!;

        Assert.That(await game.TryRejoinAsync(reconnected).ConfigureAwait(false), Is.True);
        Assert.That(reconnected.CurrentMiniGame, Is.SameAs(game));
        Assert.That(reconnectedCharacter.CurrentMap, Is.SameAs(game.Map.Definition));
        Assert.That(reconnectedCharacter.PositionX, Is.EqualTo(10));
        Assert.That(reconnectedCharacter.PositionY, Is.EqualTo(20));

        var secondAttempt = await CreateReconnectedPlayerAsync(player).ConfigureAwait(false);
        Assert.That(await game.TryRejoinAsync(secondAttempt).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a player which didn't lose its connection during the game can't rejoin it.
    /// </summary>
    [Test]
    public async Task PlayerWhichWasNotInTheGameCantRejoinAsync()
    {
        var (game, player) = await this.CreateRunningGameWithDisconnectedPlayerAsync().ConfigureAwait(false);
        var stranger = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        stranger.SelectedCharacter!.Id = Guid.NewGuid();

        Assert.That(await game.TryRejoinAsync(stranger).ConfigureAwait(false), Is.False);
        Assert.That(stranger.CurrentMiniGame, Is.Null);
    }

    /// <summary>
    /// Tests that a disconnected player can't rejoin a game which already ended,
    /// and that its character is left untouched then.
    /// </summary>
    [Test]
    public async Task DisconnectedPlayerCantRejoinEndedGameAsync()
    {
        var (game, player) = await this.CreateRunningGameWithDisconnectedPlayerAsync().ConfigureAwait(false);
        game.Finish();
        await WaitUntilAsync(() => game.State is MiniGameState.Ended or MiniGameState.Disposed).ConfigureAwait(false);

        var reconnected = await CreateReconnectedPlayerAsync(player).ConfigureAwait(false);
        var reconnectedCharacter = reconnected.SelectedCharacter!;
        var mapBefore = reconnectedCharacter.CurrentMap;

        Assert.That(await game.TryRejoinAsync(reconnected).ConfigureAwait(false), Is.False);
        Assert.That(reconnected.CurrentMiniGame, Is.Null);
        Assert.That(reconnectedCharacter.CurrentMap, Is.SameAs(mapBefore));
        Assert.That(reconnectedCharacter.PositionX, Is.EqualTo(0));
        Assert.That(reconnectedCharacter.PositionY, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that a player which was dead when it disconnected can't rejoin, because it would have left the game anyway.
    /// </summary>
    [Test]
    public async Task DeadPlayerIsNotRememberedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.Id = Guid.NewGuid();
        var game = await this.CreateRunningGameAsync(player.GameContext).ConfigureAwait(false);
        await EnterAsync(game, await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false)).ConfigureAwait(false);
        await EnterAsync(game, player).ConfigureAwait(false);

        player.IsAlive = false;
        game.RememberDisconnectedPlayer(player);

        var reconnected = await CreateReconnectedPlayerAsync(player).ConfigureAwait(false);
        Assert.That(await game.TryRejoinAsync(reconnected).ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests that a rejoined player gets the terrain changes which were applied while it was gone,
    /// because its client loads the map with the original terrain, e.g. with a closed bridge.
    /// </summary>
    [Test]
    public async Task RejoinedPlayerGetsAppliedTerrainChangesAsync()
    {
        var terrainChange = new MiniGameTerrainChange
        {
            TerrainAttribute = TerrainAttributeType.Blocked,
            SetTerrainAttribute = true,
            IsClientUpdateRequired = true,
            StartX = 5,
            StartY = 5,
            EndX = 6,
            EndY = 6,
        };
        var changeEventMock = new Mock<MiniGameChangeEvent>();
        changeEventMock.SetupGet(e => e.TerrainChanges).Returns(new List<MiniGameTerrainChange> { terrainChange });
        var (game, player) = await this.CreateRunningGameWithDisconnectedPlayerAsync(changeEventMock.Object).ConfigureAwait(false);
        await WaitUntilAsync(() => !game.Map.Terrain.WalkMap[5, 5]).ConfigureAwait(false);

        var reconnected = await CreateReconnectedPlayerAsync(player).ConfigureAwait(false);
        Assert.That(await game.TryRejoinAsync(reconnected).ConfigureAwait(false), Is.True);
        await game.Map.AddAsync(reconnected).ConfigureAwait(false);

        var terrainView = Mock.Get(reconnected.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()!);
        terrainView.Verify(
            v => v.ChangeAttributesAsync(
                TerrainAttributeType.Blocked,
                true,
                It.Is<IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)>>(areas => IsSingleArea(areas, 5, 5, 6, 6))),
            Times.Once);
    }

    private static bool IsSingleArea(IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> areas, byte startX, byte startY, byte endX, byte endY)
    {
        return areas.Count == 1 && areas.Single() == (startX, startY, endX, endY);
    }

    private static async Task EnterAsync(MiniGameContext game, Player player)
    {
        Assert.That(await game.TryEnterAsync(player).ConfigureAwait(false), Is.EqualTo(EnterResult.Success));
    }

    private static async ValueTask<Player> CreateReconnectedPlayerAsync(Player disconnectedPlayer)
    {
        // The new connection gets a new player object, which selects the same character again.
        var reconnected = await PlayerTestHelper.CreatePlayerAsync(disconnectedPlayer.GameContext).ConfigureAwait(false);
        reconnected.SelectedCharacter!.Id = disconnectedPlayer.SelectedCharacter!.Id;
        return reconnected;
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

    private async Task<(TestGame Game, Player DisconnectedPlayer)> CreateRunningGameWithDisconnectedPlayerAsync(params MiniGameChangeEvent[] changeEvents)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.Id = Guid.NewGuid();
        var game = await this.CreateRunningGameAsync(player.GameContext, changeEvents).ConfigureAwait(false);

        // Another player stays in the game, so that it goes on when the disconnected player leaves.
        var remainingPlayer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        remainingPlayer.SelectedCharacter!.Id = Guid.NewGuid();
        await EnterAsync(game, remainingPlayer).ConfigureAwait(false);
        await EnterAsync(game, player).ConfigureAwait(false);

        player.IsAlive = true;
        player.SelectedCharacter.PositionX = 10;
        player.SelectedCharacter.PositionY = 20;
        await game.Map.AddAsync(player).ConfigureAwait(false);

        // That's what happens when the connection gets lost: the game remembers the
        // player, and then the player leaves the map.
        game.RememberDisconnectedPlayer(player);
        await game.Map.RemoveAsync(player).ConfigureAwait(false);
        Assert.That(game.State, Is.EqualTo(MiniGameState.Playing));
        return (game, player);
    }

    private async Task<TestGame> CreateRunningGameAsync(IGameContext gameContext, params MiniGameChangeEvent[] changeEvents)
    {
        var mapDefinition = gameContext.Configuration.Maps.First(m => m.Number == 0);
        var definitionMock = new Mock<MiniGameDefinition>();
        definitionMock.SetupGet(d => d.Rewards).Returns(new List<MiniGameReward>());
        definitionMock.SetupGet(d => d.SpawnWaves).Returns(new List<MiniGameSpawnWave>());
        definitionMock.SetupGet(d => d.ChangeEvents).Returns(changeEvents.ToList());
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

        var game = new TestGame(new MiniGameMapKey(0, 0, string.Empty), definition, gameContext, mapInitializerMock.Object);
        this._gamesToDispose.Add(game);
        game.EnsureGameLoopRunning();
        await WaitUntilAsync(() => game.State == MiniGameState.Playing).ConfigureAwait(false);
        return game;
    }

    /// <summary>
    /// A game which starts without waiting, can be entered while it's running and can be finished by the test.
    /// </summary>
    private sealed class TestGame : MiniGameContext
    {
        public TestGame(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
            : base(key, definition, gameContext, mapInitializer)
        {
        }

        protected override TimeSpan MinimumEnterDuration => TimeSpan.Zero;

        protected override TimeSpan CountdownDuration => TimeSpan.Zero;

        protected override int MinimumPlayerCount => 0;

        protected override bool AllowEnterWhilePlaying => true;

        public void Finish() => this.FinishEvent();
    }
}
