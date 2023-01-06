// <copyright file="ChaosCastleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;
using Nito.Disposables.Internals;

/// <summary>
/// The context of a chaos castle game.
/// </summary>
/// <remarks>
/// A chaos castle event works like that:
///   First, one to 70 players enter the chaos castle.
///   The map starts as a safezone.
///   Then the event starts:
///     * The map terrain is redefined of not being a safezone anymore.
///     * The map is filled up to 100 objects with NPCs
///   Then, these objects fight each other. If the player dies, the event is over for that player.
///   If a npc dies, it doesn't re-spawn. There is a 50% chance, that a died monster explodes and moves players nearby (range 3) by one coordinate.
///   During the game, the map is getting smaller (so-called "Trap Status") after a certain amount of objects died
///   at the following number of remaining objects:
///     * less than 40
///     * less than 30
///     * less than 20
/// A player has won, if:
///   1) It's the last remaining object on the map
///   2) The time is up and it has the highest kill count
/// Rewards: A jewel or an ancient item.
///
/// Different to the original chaos castle, we don't apply additional damage when players get moved.
/// </remarks>
public sealed class ChaosCastleContext : MiniGameContext
{
    private const int MaxObjectsCount = 100;

    private const int MonsterKillPoints = 2;
    private const int PlayerKillPoints = 1;

    private static readonly (int Min, int Max)[] BlowOutDistance =
    {
        (3, 4),
        (3, 4),
        (2, 3),
        (0, 1),
    };

    // private static readonly uint[] BlowOutDamage = { 15, 15, 10, 5 };

    private static readonly IImmutableList<(int Blesses, int Souls)> MonsterJewelDropsPerLevel = new List<(int Blesses, int Souls)>
    {
        new(0, 0), // Dummy
        new(0, 2), // Chaos Castle 1
        new(1, 1), // Chaos Castle 2
        new(1, 2), // Chaos Castle 3
        new(1, 2), // Chaos Castle 4
        new(2, 1), // Chaos Castle 5
        new(2, 2), // Chaos Castle 6
        new(2, 3), // Chaos Castle 7
    }.ToImmutableList();

    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;

    private readonly Dictionary<Monster, ItemDefinition> _monsterDrops = new();

    private readonly ConcurrentDictionary<string, PlayerGameState> _gameStates = new();

    private IReadOnlyCollection<(string Name, int Score, int BonusExp, int BonusMoney)>? _highScoreTable;
    private TimeSpan _remainingTime;
    private Player? _winner;
    private int _aliveMonstersCount;
    private ChaosCastleStatus _currentCastleStatus = ChaosCastleStatus.Running;

    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public ChaosCastleContext(MiniGameMapKey key, MiniGameDefinition definition, IGameContext gameContext, IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._gameContext = gameContext;
        this._mapInitializer = mapInitializer;
    }

    /// <inheritdoc />
    protected override Player? Winner => this._winner;

    /// <inheritdoc />
    protected override TimeSpan RemainingTime => this._remainingTime;

    /// <inheritdoc/>
    protected override async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            await this.UpdateStateAsync(ChaosCastleStatus.Ended, player).ConfigureAwait(false);
        }

        await base.OnObjectRemovedFromMapAsync(args).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnTerrainChangedAsync(MiniGameChangeEvent changeEvent)
    {
        await base.OnTerrainChangedAsync(changeEvent).ConfigureAwait(false);
        switch (changeEvent.Index)
        {
            case 1:
                this._currentCastleStatus = ChaosCastleStatus.RunningShrinkingStageOne;
                break;
            case 2:
                this._currentCastleStatus = ChaosCastleStatus.RunningShrinkingStageTwo;
                break;
            case 3:
                this._currentCastleStatus = ChaosCastleStatus.RunningShrinkingStageThree;
                break;
            default:
                // no action required;
                break;
        }

        //// TODO: Pull monsters inside
    }

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        foreach (var player in players)
        {
            this._gameStates.TryAdd(player.Name, new PlayerGameState(player));
        }

        // todo: change map to be non-safezone and send packet to all players about it -> by definition, MiniGameChangeEvent
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        await this.SpawnMonstersAsync().ConfigureAwait(false);

        _ = Task.Run(async () => await this.EventLoopAsync(this.GameEndedToken).ConfigureAwait(false), this.GameEndedToken);
    }

    /// <inheritdoc />
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        this._currentCastleStatus = ChaosCastleStatus.Ended;
        await this.UpdateStateForAllAsync().ConfigureAwait(false);
        //// TODO: Change terrain, make everything a safezone.

        var sortedFinishers = finishers
            .Select(f => this._gameStates[f.Name])
            .WhereNotNull()
            .OrderByDescending(state => state.Score)
            .ToList();

        var scoreList = new List<(string Name, int Score, int BonusExp, int BonusMoney)>();
        int rank = 0;
        foreach (var state in sortedFinishers)
        {
            this._winner ??= state.Player;
            rank++;
            state.Rank = rank;
            var (bonusScore, givenMoney) = await this.GiveRewardsAndGetBonusScoreAsync(state.Player, rank).ConfigureAwait(false);
            state.AddScore(bonusScore);

            scoreList.Add((
                state.Player.Name,
                state.Score,
                this.Definition.Rewards.FirstOrDefault(r => r.RewardType == MiniGameRewardType.Experience && (r.Rank is null || r.Rank == rank))?.RewardAmount ?? 0,
                givenMoney));
        }

        this._highScoreTable = scoreList.AsReadOnly();

        await this.SaveRankingAsync(sortedFinishers.Select(state => (state.Rank, state.Player.SelectedCharacter!, state.Score))).ConfigureAwait(false);
        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask ShowScoreAsync(Player player)
    {
        if (this._highScoreTable is { } table)
        {
            var isSuccessful = this._winner is not null;
            var (name, score, bonusMoney, bonusExp) = table.First(t => t.Name == player.Name);
            await player.InvokeViewPlugInAsync<IBloodCastleScoreTableViewPlugin>(p => p.ShowScoreTableAsync(isSuccessful, name, score, bonusExp, bonusMoney)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override void OnPlayerDied(object? sender, DeathInformation e)
    {
        base.OnPlayerDied(sender, e);

        if (this._gameStates.TryGetValue(e.KillerName, out var playerState))
        {
            playerState.AddScore(PlayerKillPoints);
        }
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    /// <inheritdoc />
    protected override async void OnMonsterDied(object? sender, DeathInformation e)
#pragma warning restore VSTHRD100 // Avoid async void methods
    {
        try
        {
            Interlocked.Decrement(ref this._aliveMonstersCount);
            base.OnMonsterDied(sender, e);
            if (sender is not Monster monster)
            {
                return;
            }

            if (this._monsterDrops.TryGetValue(monster, out var drop))
            {
                var item = new TemporaryItem
                {
                    Definition = drop,
                    Durability = 1,
                };
                var droppedItem = new DroppedItem(item, monster.Position, this.Map, this.Map.GetObject(e.KillerId) as Player, null);
                await this.Map.AddAsync(droppedItem).ConfigureAwait(false);
            }

            if (this._gameStates.TryGetValue(e.KillerName, out var playerState))
            {
                playerState.AddScore(MonsterKillPoints);
            }

            // There is a 50 % chance, that a died monster explodes and moves players nearby(range 3) by one coordinate.
            if (Rand.NextRandomBool(50))
            {
                return;
            }

            var position = monster.Position;
            var playersInRange = monster.CurrentMap.GetAttackablesInRange(position, 3).OfType<Player>();
            foreach (var player in playersInRange)
            {
                await this.MovePlayerByExplosionAsync(player, position).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected Error when handling a monster death.");
        }
    }

    // see also BlowObjsFromPoint
    private async ValueTask MovePlayerByExplosionAsync(Player player, Point explosionPosition)
    {
        var playerPosition = player.Position;
        var distance = (int)playerPosition.EuclideanDistanceTo(explosionPosition);
        if (distance > 3)
        {
            return;
        }

        int directionX;
        int directionY;
        if (playerPosition.X > explosionPosition.X)
        {
            directionX = 1;
        }
        else if (playerPosition.X < explosionPosition.X)
        {
            directionX = -1;
        }
        else
        {
            directionX = Rand.NextRandomBool() ? 1 : -1;
        }

        if (playerPosition.Y > explosionPosition.Y)
        {
            directionY = 1;
        }
        else if (playerPosition.Y < explosionPosition.Y)
        {
            directionY = -1;
        }
        else
        {
            directionY = Rand.NextRandomBool() ? 1 : -1;
        }

        for (int i = 0; i < 5; i++)
        {
            var (min, max) = BlowOutDistance[distance];
            var blowX = Rand.NextInt(min, max + 1);
            var blowY = Rand.NextInt(min, max + 1);
            if (Rand.NextRandomBool())
            {
                if (blowX >= max)
                {
                    blowX = max;
                    blowY = min - Rand.NextInt(0, 2);
                }
            }
            else if (blowY >= max)
            {
                blowY = max;
                blowX = min - Rand.NextInt(0, 2);
            }

            blowX = Math.Max(0, blowX);
            blowY = Math.Max(0, blowY);

            var targetX = playerPosition.X + (blowX * directionX);
            var targetY = playerPosition.Y + (blowY * directionY);
            targetX = Math.Max(0, targetX);
            targetX = Math.Min(0xFF, targetX);
            targetY = Math.Max(0, targetY);
            targetY = Math.Min(0xFF, targetY);

            var moved = await this.SetPlayerPositionAsync(player, new Point((byte)targetX, (byte)targetY)).ConfigureAwait(false);
            if (moved)
            {
                await this.CheckPlayerPositionAsync(player).ConfigureAwait(false);
                break;
            }
        }
    }

    private async ValueTask<bool> SetPlayerPositionAsync(Player player, Point point)
    {
        if (this.Map != player.CurrentMap)
        {
            return false;
        }

        if (player.IsTeleporting)
        {
            return true;
        }

        // TODO: If target point is blocked or occupied, return false
        /* This probably prevents that the player falls down, so it's not valid
         var targetAttr = this.Map.Terrain.WalkMap[point.X, point.Y];
        if (!targetAttr)
        {
            return false;
        }*/

        await player.MoveAsync(point).ConfigureAwait(false);

        return true;
    }

    private async ValueTask EventLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var timerInterval = TimeSpan.FromSeconds(1);
            using var timer = new PeriodicTimer(timerInterval);
            var maximumGameDuration = this.Definition.GameDuration;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(maximumGameDuration);
            var ending = DateTime.UtcNow.Add(maximumGameDuration);
            this._remainingTime = maximumGameDuration;
            this._currentCastleStatus = ChaosCastleStatus.Started;
            await this.UpdateStateForAllAsync().ConfigureAwait(false);
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.UpdateStateForAllAsync().ConfigureAwait(false);

                // await this.CheckForFallenUsers().ConfigureAwait(false);

                if (this.HasGameEnded())
                {
                    break;
                }

                this._remainingTime = ending.Subtract(DateTime.UtcNow);
            }

            this._remainingTime = TimeSpan.Zero;
        }
        catch (OperationCanceledException)
        {
            // Expected exception when the game ends before running into the timeout.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unexpected error during update chaos castle status: {0}", ex.Message);
        }
    }

    private bool HasGameEnded()
    {
        var playerCount = this.PlayerCount;
        if (playerCount <= 0)
        {
            this.Logger.LogInformation("Game ended - all players dead");
            return true;
        }

        if (playerCount == 1 && this._aliveMonstersCount == 0)
        {
            this.Logger.LogInformation("Game ended - last player remaining");
            return true;
        }

        return false;
    }

    //private async ValueTask CheckForFallenUsers()
    //{
    //    // Players which are on a terrain without ground, fall down
    //    await this.ForEachPlayerAsync(CheckPlayerPositionAsync).ConfigureAwait(false);
    //}

    private async ValueTask CheckPlayerPositionAsync(Player player)
    {
        var position = player.Position;
        var terrainIsWalkable = this.Map.Terrain.WalkMap[position.X, position.Y];
        if (terrainIsWalkable)
        {
            return;
        }

        await player.KillInstantlyAsync().ConfigureAwait(false);
    }

    private ValueTask UpdateStateForAllAsync()
    {
        return this.ForEachPlayerAsync(player => this.UpdateStateAsync(this._currentCastleStatus, player).AsTask());
    }

    private ValueTask UpdateStateAsync(ChaosCastleStatus status, Player player)
    {
        var objectCount = this._aliveMonstersCount + this.PlayerCount;
        return player.InvokeViewPlugInAsync<IChaosCastleStateViewPlugin>(
            p =>
                p.UpdateStateAsync(
                    status,
                    this._remainingTime,
                    MaxObjectsCount,
                    objectCount));
    }

    private async Task SpawnMonstersAsync()
    {
        var requiredMonsters = MaxObjectsCount - this.PlayerCount;
        var spawnAreas = this.Map.Definition.MonsterSpawns.AsList();

        var addedMonsters = new List<Monster>();
        this.Map.ObjectAdded += OnObjectAdded;
        try
        {
            for (var i = 0; i < requiredMonsters && i < spawnAreas.Count; i++)
            {
                var spawnArea = spawnAreas[i];

                await this._mapInitializer.InitializeSpawnAsync(this.Map, spawnArea, this).ConfigureAwait(false);
            }
        }
        finally
        {
            this.Map.ObjectAdded -= OnObjectAdded;
        }

        this._aliveMonstersCount = requiredMonsters;

        // TODO: This should rather be configurable...
        var blessDefinition = this._gameContext.Configuration.Items.First(item => item is { Group: 14, Number: 13 });
        var soulDefinition = this._gameContext.Configuration.Items.First(item => item is { Group: 14, Number: 14 });
        var drops = MonsterJewelDropsPerLevel[this.Definition.GameLevel];
        AddItems(drops.Blesses, blessDefinition);
        AddItems(drops.Souls, soulDefinition);

        void AddItems(int count, ItemDefinition definition)
        {
            for (var i = 0; i < count; i++)
            {
                bool assigned;
                do
                {
                    var randomMonster = addedMonsters.SelectRandom();
                    if (randomMonster is null)
                    {
                        // should never happen, but prevent endless loop
                        break;
                    }

                    assigned = this._monsterDrops.TryAdd(randomMonster, definition);
                }
                while (!assigned);
            }
        }

        ValueTask OnObjectAdded((GameMap Map, ILocateable Object) args)
        {
            if (args.Object is Monster monster)
            {
                addedMonsters.Add(monster);
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed class PlayerGameState
    {
        private int _score;

        public PlayerGameState(Player player)
        {
            if (player.SelectedCharacter?.CharacterClass is null)
            {
                throw new InvalidOperationException($"The player '{player}' is in the wrong state");
            }

            this.Player = player;
        }

        public Player Player { get; }

        public int Score => this._score;

        public int Rank { get; set; }

        public void AddScore(int value)
        {
            Interlocked.Add(ref this._score, value);
        }
    }
}