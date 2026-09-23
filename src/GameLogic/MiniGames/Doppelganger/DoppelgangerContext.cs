// <copyright file="DoppelgangerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// The context of a doppelganger event game.
/// </summary>
/// <remarks>
/// The players defend a magic circle against monsters which walk along a path towards it.
/// Herds of monsters spawn at the start of the path in a fixed interval, additional stronger
/// monsters at specific times. At some point, ice walkers appear on the path, which have to
/// be killed within a limited time.
/// The event fails for everyone when <see cref="DoppelgangerEventDefinition.MaximumGoalCount"/>
/// monsters reached the magic circle. It fails for a single player when the character
/// dies or leaves the event map, e.g. by warping or disconnecting.
/// When the game time is over and the defense didn't fail, the remaining players succeeded.
/// </remarks>
public sealed class DoppelgangerContext : MiniGameContext
{
    /// <summary>
    /// The duration of the countdown which the client shows for the ice walker mission.
    /// </summary>
    private static readonly TimeSpan IceWalkerCountdownDuration = TimeSpan.FromSeconds(30);

    private readonly IGameContext _gameContext;
    private readonly DoppelgangerEventDefinition _definition;
    private readonly IList<DoppelgangerPathArea> _path;

    /// <summary>
    /// The players for which the event already ended with a result, e.g. because they died.
    /// </summary>
    private readonly ConcurrentDictionary<Player, DoppelgangerResult> _playerResults = new();

    /// <summary>
    /// The monsters on the path, including the ice walkers.
    /// </summary>
    private readonly ConcurrentDictionary<Monster, DoppelgangerMonsterIntelligence> _pathMonsters = new();

    private readonly ConcurrentDictionary<Monster, byte> _iceWalkers = new();

    private DateTime _gameStartedUtc;
    private DateTime _gameEndsAtUtc;
    private int _goalCount;
    private int _lastShownMonsterPosition = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerContext"/> class.
    /// </summary>
    /// <param name="key">The key of this context.</param>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="gameContext">The game context, to which this game belongs.</param>
    /// <param name="mapInitializer">The map initializer, which is used when the event starts.</param>
    public DoppelgangerContext(
        MiniGameMapKey key,
        MiniGameDefinition definition,
        IGameContext gameContext,
        IMapInitializer mapInitializer)
        : base(key, definition, gameContext, mapInitializer)
    {
        this._gameContext = gameContext;
        this._definition = DoppelgangerEventDefinition.CreateDefault();
        var mapNumber = (short)this.Map.MapId;
        this._path = this._definition.Paths.FirstOrDefault(path => path.MapNumber == mapNumber)?.Areas ?? [];
    }

    /// <summary>
    /// Gets a value indicating whether the defense failed, because too many monsters reached the magic circle.
    /// </summary>
    public bool IsDefenseFailed => Volatile.Read(ref this._goalCount) >= this._definition.MaximumGoalCount;

    /// <inheritdoc />
    protected override TimeSpan RemainingTime
    {
        get
        {
            var remaining = this._gameEndsAtUtc - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Registers a monster which reached the magic circle. When too many monsters
    /// reached it, the defense fails and the event ends.
    /// </summary>
    public async ValueTask RegisterMonsterReachedMagicCircleAsync()
    {
        if (this.State != MiniGameState.Playing)
        {
            return;
        }

        var goalCount = Interlocked.Increment(ref this._goalCount);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowMonsterGoalAsync(goalCount, this._definition.MaximumGoalCount)).AsTask()).ConfigureAwait(false);

        if (goalCount == this._definition.MaximumGoalCount)
        {
            this.Logger.LogInformation("{context}: The defense failed, because {goalCount} monsters reached the magic circle.", this, goalCount);
            this.FinishEvent();
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        this._gameStartedUtc = DateTime.UtcNow;
        this._gameEndsAtUtc = this._gameStartedUtc.Add(this.Definition.GameDuration);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(async p =>
        {
            await p.ShowStateAsync(DoppelgangerState.Playing).ConfigureAwait(false);
            await p.ShowMonsterGoalAsync(0, this._definition.MaximumGoalCount).ConfigureAwait(false);
        }).AsTask()).ConfigureAwait(false);

        _ = Task.Run(() => this.RunPlayInfoLoopAsync(this.GameEndedToken), this.GameEndedToken);
        _ = Task.Run(() => this.RunMonsterSpawnsAsync(this.GameEndedToken), this.GameEndedToken);
    }

    /// <inheritdoc />
#pragma warning disable VSTHRD100 // Avoid async void methods
    protected override async void OnMonsterDied(object? sender, DeathInformation e)
#pragma warning restore VSTHRD100
    {
        try
        {
            base.OnMonsterDied(sender, e);

            if (sender is not Monster monster)
            {
                return;
            }

            this._pathMonsters.TryRemove(monster, out _);
            if (this._iceWalkers.TryRemove(monster, out _) && this._iceWalkers.IsEmpty)
            {
                this.Logger.LogDebug("{context}: All ice walkers were killed.", this);
                await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.HideIceWalkerAsync()).AsTask()).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in OnMonsterDied.", this);
        }
    }

    /// <inheritdoc />
#pragma warning disable VSTHRD100 // Avoid async void methods
    protected override async void OnPlayerDied(object? sender, DeathInformation e)
#pragma warning restore VSTHRD100
    {
        try
        {
            base.OnPlayerDied(sender, e);

            if (sender is Player player)
            {
                await this.FailPlayerAsync(player).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in OnPlayerDied.", this);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnObjectRemovedFromMapAsync((GameMap Map, ILocateable Object) args)
    {
        if (args.Object is Player player)
        {
            // Leaving the map during the game, e.g. by warping or disconnecting, fails the event for the player.
            await this.FailPlayerAsync(player).ConfigureAwait(false);
        }

        await base.OnObjectRemovedFromMapAsync(args).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask GameEndedAsync(ICollection<Player> finishers)
    {
        this._pathMonsters.Clear();
        this._iceWalkers.Clear();

        var result = this.IsDefenseFailed ? DoppelgangerResult.MonstersReachedMagicCircle : DoppelgangerResult.Success;
        foreach (var player in finishers)
        {
            if (!this._playerResults.TryAdd(player, result))
            {
                // The player already got its result, e.g. because it died.
                continue;
            }

            await player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(async p =>
            {
                await p.ShowResultAsync(result).ConfigureAwait(false);
                await p.ShowStateAsync(DoppelgangerState.Ended).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    /// <summary>
    /// Fails the event for the player, if the game is running and it didn't get a result yet.
    /// </summary>
    /// <param name="player">The player.</param>
    private async ValueTask FailPlayerAsync(Player player)
    {
        if (this.State != MiniGameState.Playing
            || !this._playerResults.TryAdd(player, DoppelgangerResult.Failed))
        {
            return;
        }

        this.Logger.LogDebug("{context}: The event failed for player {player}.", this, player);
        await player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.ShowResultAsync(DoppelgangerResult.Failed)).ConfigureAwait(false);
    }

    private async Task RunPlayInfoLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(this._definition.PlayInfoInterval);
            do
            {
                await this.ShowPlayInfoAsync().ConfigureAwait(false);
                await this.ShowMonsterPositionAsync().ConfigureAwait(false);
            }
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in the play info loop.", this);
        }
    }

    private async ValueTask ShowPlayInfoAsync()
    {
        var mapNumber = (short)this.Map.MapId;
        var playerPositions = new List<(Player Player, int Position)>();

        // The synchronous part of the action is executed sequentially for all players.
        await this.ForEachPlayerAsync(player =>
        {
            if (player.IsAlive && !this._playerResults.ContainsKey(player))
            {
                playerPositions.Add((player, this._definition.GetPathPosition(mapNumber, player.Position)));
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);

        var remainingTime = this.RemainingTime;
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowPlayInfoAsync(remainingTime, playerPositions)).AsTask()).ConfigureAwait(false);
    }

    /// <summary>
    /// Shows the position of the most advanced monster on the path, when it changed.
    /// The ice walkers are not considered, because they're shown separately.
    /// </summary>
    private async ValueTask ShowMonsterPositionAsync()
    {
        var position = this._pathMonsters.Values
            .Where(intelligence => intelligence.WalksAlongPath)
            .Select(intelligence => intelligence.PathPosition)
            .DefaultIfEmpty(0)
            .Max();
        if (Interlocked.Exchange(ref this._lastShownMonsterPosition, position) == position)
        {
            return;
        }

        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowMonsterPositionAsync(position)).AsTask()).ConfigureAwait(false);
    }

    private async Task RunMonsterSpawnsAsync(CancellationToken cancellationToken)
    {
        if (this._path.Count == 0)
        {
            this.Logger.LogError("{context}: No path defined for map {map}, so no monsters will spawn.", this, this.Map.MapId);
            return;
        }

        try
        {
            var pendingSpawns = this._definition.AdditionalSpawns.OrderBy(spawn => spawn.SpawnTime).ToList();
            var iceWalkersSpawned = false;
            using var timer = new PeriodicTimer(this._definition.HerdInterval);
            do
            {
                var elapsed = DateTime.UtcNow - this._gameStartedUtc;
                await this.SpawnHerdAsync(elapsed).ConfigureAwait(false);

                while (pendingSpawns.Count > 0 && pendingSpawns[0].SpawnTime <= elapsed)
                {
                    foreach (var monsterNumber in pendingSpawns[0].MonsterNumbers)
                    {
                        await this.SpawnMonsterAsync(monsterNumber, 0, true, true).ConfigureAwait(false);
                    }

                    pendingSpawns.RemoveAt(0);
                }

                if (!iceWalkersSpawned && elapsed >= this._definition.IceWalkerSpawnTime)
                {
                    iceWalkersSpawned = true;
                    _ = Task.Run(() => this.RunIceWalkerMissionAsync(cancellationToken), cancellationToken);
                }
            }
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error when spawning the monsters.", this);
        }
    }

    private async ValueTask SpawnHerdAsync(TimeSpan elapsed)
    {
        if (this._definition.HerdMonsterNumbers.Count == 0)
        {
            return;
        }

        var count = this._definition.GetHerdBaseCount(elapsed) + Math.Max(0, this.PlayerCount - 1);
        for (var i = 0; i < count; i++)
        {
            var monsterNumber = this._definition.HerdMonsterNumbers[Rand.NextInt(0, this._definition.HerdMonsterNumbers.Count)];
            var attacksFirst = this._definition.AlwaysAttackingMonsterNumbers.Contains(monsterNumber)
                               || Rand.NextRandomBool(this._definition.AttackFirstChance);
            await this.SpawnMonsterAsync(monsterNumber, 0, true, attacksFirst).ConfigureAwait(false);
        }
    }

    private async Task RunIceWalkerMissionAsync(CancellationToken cancellationToken)
    {
        try
        {
            var count = Math.Max(1, this.PlayerCount);
            var shownPosition = -1;
            for (var i = 0; i < count; i++)
            {
                var position = Rand.NextInt(this._definition.IceWalkerMinimumPosition, this._definition.IceWalkerMaximumPosition + 1);
                if (await this.SpawnMonsterAsync(this._definition.IceWalkerNumber, position, false, true).ConfigureAwait(false) is { } iceWalker)
                {
                    this._iceWalkers.TryAdd(iceWalker, 0);
                    shownPosition = shownPosition < 0 ? position : shownPosition;
                }
            }

            if (shownPosition < 0)
            {
                return;
            }

            this.Logger.LogDebug("{context}: {count} ice walkers appeared.", this, this._iceWalkers.Count);
            await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.ShowIceWalkerAsync(shownPosition)).AsTask()).ConfigureAwait(false);

            var countdownDuration = this._definition.IceWalkerMissionDuration < IceWalkerCountdownDuration
                ? this._definition.IceWalkerMissionDuration
                : IceWalkerCountdownDuration;
            await Task.Delay(this._definition.IceWalkerMissionDuration - countdownDuration, cancellationToken).ConfigureAwait(false);
            if (this._iceWalkers.IsEmpty)
            {
                return;
            }

            await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.ShowIceWalkerCountdownAsync()).AsTask()).ConfigureAwait(false);
            await Task.Delay(countdownDuration, cancellationToken).ConfigureAwait(false);

            var remainingIceWalkers = this._iceWalkers.Keys.ToList();
            if (remainingIceWalkers.Count == 0)
            {
                return;
            }

            this.Logger.LogDebug("{context}: The ice walkers weren't killed in time.", this);
            foreach (var iceWalker in remainingIceWalkers)
            {
                await this.RemoveMonsterAsync(iceWalker).ConfigureAwait(false);
            }

            await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.HideIceWalkerAsync()).AsTask()).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error in the ice walker mission.", this);
        }
    }

    private async ValueTask<Monster?> SpawnMonsterAsync(short monsterNumber, int pathPosition, bool walksAlongPath, bool attacksFirst)
    {
        if (this._gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == monsterNumber) is not { } monsterDefinition)
        {
            this.Logger.LogWarning("{context}: Monster definition {monsterNumber} not found.", this, monsterNumber);
            return null;
        }

        var area = this._path[pathPosition];
        var spawnArea = new MonsterSpawnArea
        {
            GameMap = this.Map.Definition,
            MonsterDefinition = monsterDefinition,
            SpawnTrigger = SpawnTrigger.OnceAtEventStart,
            Quantity = 1,
            X1 = (byte)(area.X1 + 1),
            X2 = area.X2,
            Y1 = (byte)(area.Y1 + 1),
            Y2 = area.Y2,
        };

        var intelligence = new DoppelgangerMonsterIntelligence(this._path, pathPosition, walksAlongPath, attacksFirst, this.OnMonsterReachedMagicCircleAsync, this.Logger);
        var monster = new Monster(spawnArea, monsterDefinition, this.Map, this.DropGenerator, intelligence, this._gameContext.PlugInManager, this._gameContext.PathFinderPool, this);
        intelligence.Npc = monster;
        try
        {
            monster.Initialize();
        }
        catch (InvalidOperationException ex)
        {
            this.Logger.LogWarning(ex, "{context}: Couldn't spawn monster {monsterNumber} at the path position {pathPosition}.", this, monsterNumber, pathPosition);
            monster.Dispose();
            return null;
        }

        this._pathMonsters.TryAdd(monster, intelligence);
        await this.Map.AddAsync(monster).ConfigureAwait(false);
        monster.OnSpawn();
        intelligence.Start();
        return monster;
    }

    private async ValueTask OnMonsterReachedMagicCircleAsync(Monster monster)
    {
        await this.RemoveMonsterAsync(monster).ConfigureAwait(false);
        await this.RegisterMonsterReachedMagicCircleAsync().ConfigureAwait(false);
    }

    private async ValueTask RemoveMonsterAsync(Monster monster)
    {
        this._pathMonsters.TryRemove(monster, out _);
        this._iceWalkers.TryRemove(monster, out _);
        await this.Map.RemoveAsync(monster).ConfigureAwait(false);
        monster.Dispose();
    }
}
