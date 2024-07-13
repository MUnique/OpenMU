// <copyright file="ChaosCastleContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System;
using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
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
///   If a npc dies, it doesn't re-spawn. There is a 50% chance, that a died monster explodes and moves players nearby (range 3)
///   by <see cref="BlowOutDistance"/>, depending on the distance to the killed monster.
///   During the game, the map is getting smaller (so-called "Trap Status") after a certain amount of objects died
///   at the following number of remaining objects:
///     * less than 40
///     * less than 30
///     * less than 20
/// A player has won, if:
///   1) It's the last remaining object on the map
///   2) The time is up and it has the highest kill count
/// Rewards: A jewel and/or an ancient item.
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

    private readonly IGameContext _gameContext;
    private readonly IMapInitializer _mapInitializer;
    private readonly ConcurrentDictionary<string, PlayerGameState> _gameStates = new();
    private readonly ConcurrentDictionary<Monster, byte> _monsters = new();

    private IReadOnlyCollection<(string Name, int Score, int BonusExp, int BonusMoney)>? _highScoreTable;
    private TimeSpan _remainingTime;
    private Player? _winner;
    private int _aliveMonstersCount;
    private ChaosCastleStatus _currentCastleStatus = ChaosCastleStatus.Running;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleContext"/> class.
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

        this.Logger.LogDebug("Event {0} created, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
    }

    /// <inheritdoc />
    public override bool AllowPlayerKilling => true;

    /// <inheritdoc />
    protected override Player? Winner => this._winner;

    /// <inheritdoc />
    protected override TimeSpan RemainingTime => this._remainingTime;

    /// <inheritdoc />
    protected override int MinimumPlayerCount => 1;

    /// <inheritdoc />
    public override bool IsItemAllowedToEquip(Item item)
    {
        if (item.Definition is not { } definition)
        {
            return false;
        }

        switch (definition.Group, definition.Number)
        {
            case (13, 2): // Uniria
            case (13, 3): // Dino
            case (13, 37): // Fenrir
                return false;
            default:
                if (definition.BasePowerUpAttributes.Any(a => a.TargetAttribute == Stats.TransformationSkin))
                {
                    return false;
                }

                break;
        }

        return base.IsItemAllowedToEquip(item);
    }

    /// <inheritdoc />
    public override bool IsSkillAllowed(Skill skill, Player attacker, IAttackable target)
    {
        if (!base.IsSkillAllowed(skill, attacker, target))
        {
            return false;
        }

        if (skill.SkillType == SkillType.SummonMonster)
        {
            // It's not allowed to summon a monster.
            return false;
        }

        if (skill.SkillType == SkillType.Buff && target != attacker)
        {
            // It's only allowed to buff the own player.
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            await this.UpdateStateAsync(ChaosCastleStatus.Ended, player, 0).ConfigureAwait(false);
        }

        await base.OnObjectRemovedFromMapAsync(args).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnTerrainChangingAsync(MiniGameChangeEvent changeEvent)
    {
        await base.OnTerrainChangingAsync(changeEvent).ConfigureAwait(false);
        this.Logger.LogDebug("Terrain changing by event index {0}. Event: {1}, game id {2}", changeEvent.Index, this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
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
                // no action required
                break;
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnTerrainChangedAsync(MiniGameChangeEvent changeEvent)
    {
        await base.OnTerrainChangedAsync(changeEvent).ConfigureAwait(false);

        if (changeEvent.TerrainChanges.All(c => c.TerrainAttribute != TerrainAttributeType.NoGround))
        {
            return;
        }

        await this.CheckForFallenUsersAsync().ConfigureAwait(false);
        await this.PullMonstersAsync(changeEvent).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        this.Logger.LogDebug("Starting the game... Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
        foreach (var player in players)
        {
            this._gameStates.TryAdd(player.Name, new PlayerGameState(player));
        }

        await base.OnGameStartAsync(players).ConfigureAwait(false);

        await this.SpawnMonstersAsync().ConfigureAwait(false);

        _ = Task.Run(async () => await this.EventLoopAsync(this.GameEndedToken).ConfigureAwait(false), this.GameEndedToken);
    }

    /// <inheritdoc />
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        this.Logger.LogDebug("Game ended... Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
        this._currentCastleStatus = ChaosCastleStatus.Ended;
        await this.UpdateStateForAllAsync().ConfigureAwait(false);

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

            this._monsters.Remove(monster, out _);

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
            else
            {
                // leave the random values like they are
            }

            blowX = Math.Max(0, blowX);
            blowY = Math.Max(0, blowY);

            var targetX = playerPosition.X + (blowX * directionX);
            var targetY = playerPosition.Y + (blowY * directionY);
            targetX = Math.Max(0, targetX);
            targetX = Math.Min(0xFF, targetX);
            targetY = Math.Max(0, targetY);
            targetY = Math.Min(0xFF, targetY);

            var targetPoint = new Point((byte)targetX, (byte)targetY);
            var moved = await this.SetPlayerPositionAsync(player, targetPoint).ConfigureAwait(false);
            if (moved)
            {
                this.Logger.LogDebug("Player {player} was moved to {targetPoint}.", player, targetPoint);
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

        await player.MoveAsync(point).ConfigureAwait(false);

        return true;
    }

    private async ValueTask EventLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            this.Logger.LogDebug("Starting event loop... Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
            var timerInterval = TimeSpan.FromSeconds(1);
            using var timer = new PeriodicTimer(timerInterval);
            var maximumGameDuration = this.Definition.GameDuration;
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(maximumGameDuration);
            var ending = DateTime.UtcNow.Add(maximumGameDuration);
            this._remainingTime = maximumGameDuration;
            this._currentCastleStatus = ChaosCastleStatus.Started;
            await this.UpdateStateForAllAsync().ConfigureAwait(false);
            this._currentCastleStatus = ChaosCastleStatus.Running;
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.UpdateStateForAllAsync().ConfigureAwait(false);

                if (this.HasGameEnded())
                {
                    this.FinishEvent();
                    break;
                }

                this._remainingTime = ending.Subtract(DateTime.UtcNow);
            }

            this._remainingTime = TimeSpan.Zero;
        }
        catch (OperationCanceledException ex)
        {
            // Expected exception when the game ends before running into the timeout.
            this.Logger.LogDebug(ex, "Stopped Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
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
            this.Logger.LogDebug("Game ended - all players dead. Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
            return true;
        }

        if (playerCount == 1 && this._aliveMonstersCount == 0)
        {
            this.Logger.LogDebug("Game ended - last player remaining. Event: {0}, game id {1}", this.Definition.Name, (this._gameContext as IGameServerContext)?.Id ?? 0);
            return true;
        }

        return false;
    }

    private async ValueTask CheckForFallenUsersAsync()
    {
        // Players which are on a terrain without ground, fall down
        await this.ForEachPlayerAsync(this.CheckPlayerPositionAsync).ConfigureAwait(false);
    }

    private async Task CheckPlayerPositionAsync(Player player)
    {
        var position = player.Position;
        var terrainIsWalkable = this.Map.Terrain.WalkMap[position.X, position.Y];
        if (terrainIsWalkable)
        {
            return;
        }

        this.Logger.LogDebug("Player {0} is at a blocked position, it will be killed instantly.", player);
        await player.KillInstantlyAsync().ConfigureAwait(false);
    }

    private async ValueTask UpdateStateForAllAsync()
    {
        var objectCount = this._aliveMonstersCount + this.PlayerCount;
        var currentStatus = this._currentCastleStatus;
        this.Logger.LogDebug("UpdateState {0} for all players. Object Count: {1}", currentStatus, objectCount);
        await this.ForEachPlayerAsync(player => this.UpdateStateAsync(currentStatus, player, objectCount)).ConfigureAwait(false);
        if (currentStatus is ChaosCastleStatus.RunningShrinkingStageOne or ChaosCastleStatus.RunningShrinkingStageTwo or ChaosCastleStatus.RunningShrinkingStageThree)
        {
            await this.ForEachPlayerAsync(player => this.UpdateStateAsync(ChaosCastleStatus.Running, player, objectCount)).ConfigureAwait(false);
        }
    }

    private async Task UpdateStateAsync(ChaosCastleStatus status, Player player, int objectCount)
    {
        await player.InvokeViewPlugInAsync<IChaosCastleStateViewPlugin>(
            p =>
                p.UpdateStateAsync(
                    status,
                    this._remainingTime,
                    MaxObjectsCount,
                    objectCount))
            .ConfigureAwait(false);
    }

    private async Task SpawnMonstersAsync()
    {
        var requiredMonsters = MaxObjectsCount - this.PlayerCount;

        this.Logger.LogDebug("{0}: Spawning {1} monsters...", this.Definition.Description, requiredMonsters);
        var spawnAreas = this.Map.Definition.MonsterSpawns.AsList();
        this.DropGenerator = new ChaosCastleDropGenerator(this._gameContext, this, requiredMonsters);

        for (var i = 0; i < requiredMonsters && i < spawnAreas.Count; i++)
        {
            var spawnArea = spawnAreas[i];

            if (await this._mapInitializer.InitializeSpawnAsync(i, this.Map, spawnArea, this, this.DropGenerator).ConfigureAwait(false) is Monster monster)
            {
                this._monsters.TryAdd(monster, default);
            }
        }

        this._aliveMonstersCount = requiredMonsters;
        this.Logger.LogDebug("Monsters created.");
    }

    private async ValueTask PullMonstersAsync(MiniGameChangeEvent changeEvent)
    {
        // Determine middle:
        var minX = changeEvent.TerrainChanges.Min(c => c.StartX);
        var maxX = changeEvent.TerrainChanges.Max(c => c.EndX);
        var minY = changeEvent.TerrainChanges.Min(c => c.StartY);
        var maxY = changeEvent.TerrainChanges.Max(x => x.EndY);

        var middlePoint = new Point((byte)((minX + maxX) / 2), (byte)((minY + maxY) / 2));

        var walkMap = this.Map.Terrain.WalkMap;

        var monstersNeedPull = this._monsters.Keys
            .Where(m => !walkMap[m.Position.X, m.Position.Y])
            .Where(m => m.IsAlive);

        bool MoveByX(byte distance, ref Point resultTarget, bool setTargetAnyway = false)
        {
            var offset = new Point(0, distance);
            var target = resultTarget;
            if (target.Y < middlePoint.Y)
            {
                target += offset;
            }
            else if (target.Y > middlePoint.Y)
            {
                target -= offset;
            }
            else
            {
                // we're already good to go!
            }

            return EvaluateTarget(target, ref resultTarget, setTargetAnyway);
        }

        bool MoveByY(byte distance, ref Point resultTarget, bool setTargetAnyway = false)
        {
            var offset = new Point(distance, 0);
            var target = resultTarget;
            if (target.X < middlePoint.X)
            {
                target += offset;
            }
            else if (target.X > middlePoint.X)
            {
                target -= offset;
            }
            else
            {
                // we're already good to go!
            }

            return EvaluateTarget(target, ref resultTarget, setTargetAnyway);
        }

        bool MoveByXandY(byte distance, ref Point resultTarget)
        {
            var target = resultTarget;
            MoveByX(distance, ref target, true);
            MoveByY(distance, ref target, true);
            return EvaluateTarget(target, ref resultTarget, false);
        }

        bool EvaluateTarget(Point target, ref Point resultTarget, bool setTargetAnyway)
        {
            if (walkMap[target.X, target.Y])
            {
                resultTarget = target;
                return true;
            }

            if (setTargetAnyway)
            {
                resultTarget = target;
            }

            return false;
        }

        foreach (var monster in monstersNeedPull)
        {
            var moved = false;
            for (byte dist = 1; dist <= 5; dist++)
            {
                var target = monster.Position;
                if (MoveByX(dist, ref target)
                    || MoveByY(dist, ref target)
                    || MoveByXandY(dist, ref target))
                {
                    this.Logger.LogDebug("Moving {monster} by {dist} steps to {target}", monster, dist, target);
                    await monster.MoveAsync(target).ConfigureAwait(false);
                    moved = true;
                    break;
                }
            }

            if (!moved)
            {
                this.Logger.LogDebug("Couldn't move monster {monster} to valid coordinate.", monster);
            }
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