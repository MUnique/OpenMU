// <copyright file="DoppelgangerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The context of a doppelganger event game.
/// </summary>
/// <remarks>
/// The players defend a magic circle against monsters which walk along a path towards it.
/// Herds of monsters spawn at the start of the path in a fixed interval, additional stronger
/// monsters at specific times. At some point, ice walkers appear on the path, which have to
/// be killed within a limited time.
/// The monsters get stronger with the level and the number of the players. When the ice walkers
/// weren't killed in time, the following monsters get even stronger.
/// Killed butchers leave interim reward chests behind, of which only one can be opened by talking to it.
/// It contains either items or larvae. When the defense succeeded, a final reward chest appears.
/// The event fails for everyone when <see cref="DoppelgangerEventDefinition.MaximumGoalCount"/>
/// monsters reached the magic circle. It fails for a single player when the character
/// dies or leaves the event map, e.g. by warping or disconnecting.
/// When the game time is over and the defense didn't fail, the remaining players succeeded.
/// </remarks>
public sealed class DoppelgangerContext : MiniGameContext
{
    /// <summary>
    /// The health of the chests. They're opened by talking to them, so they shouldn't be destroyed by attacks.
    /// </summary>
    private const int ChestHealth = 1_000_000_000;

    /// <summary>
    /// The maximum distance of a player to a chest to open it.
    /// </summary>
    private const int MaximumChestOpenDistance = 4;

    /// <summary>
    /// The duration of the countdown which the client shows for the ice walker mission.
    /// </summary>
    private static readonly TimeSpan IceWalkerCountdownDuration = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The time after which an opened chest is removed, so that the client can show its opening.
    /// </summary>
    private static readonly TimeSpan OpenedChestRemovalDelay = TimeSpan.FromSeconds(2);

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

    /// <summary>
    /// The interim reward chests, with the group of chests which appeared together.
    /// </summary>
    private readonly ConcurrentDictionary<Destructible, InterimChestGroup> _interimChests = new();

    private DateTime _gameStartedUtc;
    private DateTime _gameEndsAtUtc;
    private int _goalCount;
    private int _lastShownMonsterPosition = -1;
    private int _initialPlayerCount;
    private MonsterMultipliers _monsterMultipliers = MonsterMultipliers.None;
    private int _isIceWalkerMissionFailed;
    private Destructible? _finalChest;
    private int _isFinalChestOpened;

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

        // The definition is resolved once, so that a configuration change doesn't affect a running game.
        this._definition = GetEventDefinition(gameContext);
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

        // When several monsters reach the magic circle at the same time, the count can exceed the maximum.
        var shownGoalCount = Math.Min(goalCount, this._definition.MaximumGoalCount);
        await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p =>
            p.ShowMonsterGoalAsync(shownGoalCount, this._definition.MaximumGoalCount)).AsTask()).ConfigureAwait(false);

        if (goalCount == this._definition.MaximumGoalCount)
        {
            this.Logger.LogInformation("{context}: The defense failed, because {goalCount} monsters reached the magic circle.", this, goalCount);
            this.FinishEvent();
        }
    }

    /// <summary>
    /// Determines whether the destructible is a reward chest of this game, which wasn't opened yet.
    /// </summary>
    /// <param name="chest">The chest.</param>
    /// <returns><c>true</c>, if the destructible is a reward chest of this game; otherwise, <c>false</c>.</returns>
    public bool IsRewardChest(Destructible chest)
    {
        return this._interimChests.ContainsKey(chest) || chest == this._finalChest;
    }

    /// <summary>
    /// Opens the reward chest. Of the interim chests which appeared together, only one can be opened.
    /// An interim chest contains either larvae or items, the final chest always contains items.
    /// </summary>
    /// <param name="player">The player who opens the chest.</param>
    /// <param name="chest">The chest.</param>
    public async ValueTask OpenRewardChestAsync(Player player, Destructible chest)
    {
        if (player.CurrentMiniGame != this
            || !chest.IsAlive
            || chest.GetDistanceTo(player) > MaximumChestOpenDistance)
        {
            return;
        }

        var containsLarvae = false;
        if (this._interimChests.TryRemove(chest, out var group))
        {
            if (!group.TryOpen())
            {
                return;
            }

            containsLarvae = Rand.NextRandomBool(this._definition.LarvaChance);
            foreach (var otherChest in group.Chests.Where(c => c != chest))
            {
                if (this._interimChests.TryRemove(otherChest, out _))
                {
                    await this.RemoveNpcAsync(otherChest).ConfigureAwait(false);
                }
            }
        }
        else if (chest != this._finalChest || Interlocked.Exchange(ref this._isFinalChestOpened, 1) != 0)
        {
            return;
        }
        else
        {
            // It's the final chest, which always contains items.
        }

        this.Logger.LogDebug("{context}: {player} opened {chest}, which contains {content}.", this, player, chest, containsLarvae ? "larvae" : "items");

        // The client shows the opening of the chest as its death.
        await chest.ForEachWorldObserverAsync<IObjectGotKilledPlugIn>(p => p.ObjectGotKilledAsync(chest, player), true).ConfigureAwait(false);
        if (containsLarvae)
        {
            var area = GetAreaAround(chest.Position, 2);
            for (var i = 0; i < Math.Max(1, this._initialPlayerCount); i++)
            {
                await this.SpawnMonsterAsync(this._definition.Larva, area, 0, false, true).ConfigureAwait(false);
            }
        }
        else
        {
            await this.DropChestItemsAsync(player, chest).ConfigureAwait(false);
        }

        // The talk to the chest isn't awaited, so waiting here for the opening animation doesn't block anything.
        await Task.Delay(OpenedChestRemovalDelay).ConfigureAwait(false);
        await this.RemoveNpcAsync(chest).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnGameStartAsync(ICollection<Player> players)
    {
        await base.OnGameStartAsync(players).ConfigureAwait(false);

        this._gameStartedUtc = DateTime.UtcNow;
        this._initialPlayerCount = players.Count;
        this._monsterMultipliers = this.GetMonsterMultipliers(players);
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
            if (this.State == MiniGameState.Playing && this._definition.InterimChestMonsters.Any(m => IsSameMonster(m, monster.Definition)))
            {
                await this.SpawnInterimChestsAsync(monster.Position).ConfigureAwait(false);
            }

            if (this._iceWalkers.TryRemove(monster, out _) && this._iceWalkers.IsEmpty)
            {
                this.Logger.LogDebug("{context}: All ice walkers were killed.", this);
                await this.ShowGoldenMessageAsync(nameof(PlayerMessage.DoppelgangerIceWalkerKilled)).ConfigureAwait(false);
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
        this._interimChests.Clear();

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

        if (result == DoppelgangerResult.Success && finishers.FirstOrDefault(player => player.IsAlive) is { } firstFinisher)
        {
            this._finalChest = await this.SpawnDestructibleAsync(this._definition.FinalRewardChest, GetAreaAround(firstFinisher.Position, 1)).ConfigureAwait(false);
        }

        await base.GameEndedAsync(finishers).ConfigureAwait(false);
    }

    private static DoppelgangerEventDefinition GetEventDefinition(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<DoppelgangerFeaturePlugIn>()?.Configuration
               ?? DoppelgangerEventDefinition.CreateDefault(gameContext.Configuration);
    }

    /// <summary>
    /// Determines whether the monster definitions describe the same monster. They're compared
    /// by their number, because the configured definition may be a different instance than the
    /// one of the spawned monster.
    /// </summary>
    private static bool IsSameMonster(MonsterDefinition? first, MonsterDefinition? second)
    {
        return first is not null && second is not null && first.Number == second.Number;
    }

    private static DoppelgangerPathArea GetAreaAround(Point point, byte radius)
    {
        // The lower bounds of a path area are exclusive.
        return new DoppelgangerPathArea(
            (byte)Math.Max(point.X - radius - 1, 0),
            (byte)Math.Max(point.Y - radius - 1, 0),
            (byte)Math.Min(point.X + radius, byte.MaxValue),
            (byte)Math.Min(point.Y + radius, byte.MaxValue));
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
                    foreach (var monster in pendingSpawns[0].Monsters)
                    {
                        await this.SpawnMonsterAsync(monster, 0, true, true, true).ConfigureAwait(false);
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
        if (this._definition.HerdMonsters.Count == 0)
        {
            return;
        }

        var count = this._definition.GetHerdBaseCount(elapsed) + Math.Max(0, this.PlayerCount - 1);
        for (var i = 0; i < count; i++)
        {
            var monster = this._definition.HerdMonsters[Rand.NextInt(0, this._definition.HerdMonsters.Count)];
            var attacksFirst = this._definition.AlwaysAttackingMonsters.Any(m => IsSameMonster(m, monster))
                               || Rand.NextRandomBool(this._definition.AttackFirstChance);
            await this.SpawnMonsterAsync(monster, 0, true, attacksFirst, true).ConfigureAwait(false);
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
                if (await this.SpawnMonsterAsync(this._definition.IceWalker, position, false, true).ConfigureAwait(false) is { } iceWalker)
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
            await this.ShowGoldenMessageAsync(nameof(PlayerMessage.DoppelgangerIceWalkerAppeared)).ConfigureAwait(false);
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

            this.Logger.LogDebug("{context}: The ice walkers weren't killed in time, so the following monsters get stronger.", this);
            Interlocked.Exchange(ref this._isIceWalkerMissionFailed, 1);
            foreach (var iceWalker in remainingIceWalkers)
            {
                await this.RemoveMonsterAsync(iceWalker).ConfigureAwait(false);
            }

            await this.ForEachPlayerAsync(player => player.InvokeViewPlugInAsync<IDoppelgangerEventViewPlugIn>(p => p.HideIceWalkerAsync()).AsTask()).ConfigureAwait(false);
            await this.ShowGoldenMessageAsync(nameof(PlayerMessage.DoppelgangerIceWalkerEscaped)).ConfigureAwait(false);
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

    private ValueTask<Monster?> SpawnMonsterAsync(MonsterDefinition? monsterDefinition, int pathPosition, bool walksAlongPath, bool attacksFirst, bool isAffectedByMissionFailure = false)
    {
        return this.SpawnMonsterAsync(monsterDefinition, this._path[pathPosition], pathPosition, walksAlongPath, attacksFirst, isAffectedByMissionFailure);
    }

    private async ValueTask<Monster?> SpawnMonsterAsync(MonsterDefinition? monsterDefinition, DoppelgangerPathArea area, int pathPosition, bool walksAlongPath, bool attacksFirst, bool isAffectedByMissionFailure = false)
    {
        if (this.CreateSpawnArea(monsterDefinition, area) is not { } spawnArea)
        {
            return null;
        }

        var intelligence = new DoppelgangerMonsterIntelligence(this._path, pathPosition, walksAlongPath, attacksFirst, this.OnMonsterReachedMagicCircleAsync, this.Logger);
        var monster = new Monster(spawnArea, spawnArea.MonsterDefinition!, this.Map, this.DropGenerator, intelligence, this._gameContext.PlugInManager, this._gameContext.PathFinderPool, this);

        // The multipliers have to be applied before the monster is initialized, which sets its health.
        var penalty = isAffectedByMissionFailure && Volatile.Read(ref this._isIceWalkerMissionFailed) != 0
            ? this._definition.IceWalkerMissionFailedMultiplier
            : 1;
        this._monsterMultipliers.ApplyTo(monster, penalty);
        try
        {
            monster.Initialize();
        }
        catch (InvalidOperationException ex)
        {
            this.Logger.LogWarning(ex, "{context}: Couldn't spawn monster {monster} at the path position {pathPosition}.", this, monsterDefinition, pathPosition);
            monster.Dispose();
            return null;
        }

        this._pathMonsters.TryAdd(monster, intelligence);
        await this.Map.AddAsync(monster).ConfigureAwait(false);
        monster.OnSpawn();
        intelligence.Start();
        return monster;
    }

    private async ValueTask SpawnInterimChestsAsync(Point position)
    {
        var group = new InterimChestGroup();
        var area = GetAreaAround(position, 2);
        for (var i = 0; i < this._definition.InterimChestCount; i++)
        {
            if (await this.SpawnDestructibleAsync(this._definition.InterimRewardChest, area).ConfigureAwait(false) is { } chest)
            {
                group.Chests.Add(chest);
                this._interimChests.TryAdd(chest, group);
            }
        }

        if (group.Chests.Count > 0)
        {
            _ = Task.Run(() => this.RemoveUnopenedChestsAsync(group, this.GameEndedToken), this.GameEndedToken);
        }
    }

    private async Task RemoveUnopenedChestsAsync(InterimChestGroup group, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(this._definition.InterimChestDuration, cancellationToken).ConfigureAwait(false);
            foreach (var chest in group.Chests)
            {
                if (this._interimChests.TryRemove(chest, out _))
                {
                    await this.RemoveNpcAsync(chest).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The game ended.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "{context}: Unexpected error when removing the unopened chests.", this);
        }
    }

    private async ValueTask DropChestItemsAsync(Player player, Destructible chest)
    {
        var (items, _) = await this.DropGenerator.GenerateItemDropsAsync(chest.Definition, 0, player).ConfigureAwait(false);
        var owners = player.Party?.PartyList.AsEnumerable() ?? player.GetAsEnumerable();
        var isFirstItem = true;
        foreach (var item in items)
        {
            var position = isFirstItem ? chest.Position : this.Map.Terrain.GetRandomCoordinate(chest.Position, 2);
            isFirstItem = false;
            var droppedItem = new DroppedItem(item, position, this.Map, null, owners);
            await this.Map.AddAsync(droppedItem).ConfigureAwait(false);
        }
    }

    private async ValueTask<Destructible?> SpawnDestructibleAsync(MonsterDefinition? definition, DoppelgangerPathArea area)
    {
        if (this.CreateSpawnArea(definition, area) is not { } spawnArea)
        {
            return null;
        }

        spawnArea.MaximumHealthOverride = ChestHealth;

        var destructible = new Destructible(spawnArea, spawnArea.MonsterDefinition!, this.Map, this, this.DropGenerator, this._gameContext.PlugInManager);
        try
        {
            destructible.Initialize();
        }
        catch (InvalidOperationException ex)
        {
            this.Logger.LogWarning(ex, "{context}: Failed to spawn {definition} around {area}.", this, definition, area);
            destructible.Dispose();
            return null;
        }

        await this.Map.AddAsync(destructible).ConfigureAwait(false);
        destructible.OnSpawn();
        return destructible;
    }

    private MonsterSpawnArea? CreateSpawnArea(MonsterDefinition? monsterDefinition, DoppelgangerPathArea area)
    {
        if (monsterDefinition is null)
        {
            this.Logger.LogWarning("{context}: A monster of the event isn't configured.", this);
            return null;
        }

        return new MonsterSpawnArea
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
    }

    private async ValueTask RemoveNpcAsync(NonPlayerCharacter npc)
    {
        await this.Map.RemoveAsync(npc).ConfigureAwait(false);
        npc.Dispose();
    }

    /// <summary>
    /// Gets the multipliers for the monsters, depending on the highest level of the players and their number.
    /// </summary>
    private MonsterMultipliers GetMonsterMultipliers(ICollection<Player> players)
    {
        var playerLevel = players
            .Select(player => (int)((player.Attributes?[Stats.Level] ?? 0) + (player.Attributes?[Stats.MasterLevel] ?? 0)))
            .DefaultIfEmpty(1)
            .Max();
        if (this._definition.GetMonsterScaling(playerLevel) is not { } scaling)
        {
            return MonsterMultipliers.None;
        }

        var index = Math.Clamp(players.Count, 1, 5) - 1;
        var multipliers = new MonsterMultipliers(
            GetMultiplier(scaling.LevelMultipliers, index),
            GetMultiplier(scaling.HealthMultipliers, index),
            GetMultiplier(scaling.DamageMultipliers, index),
            GetMultiplier(scaling.DefenseMultipliers, index));
        this.Logger.LogDebug("{context}: Monster multipliers for player level {playerLevel} and {playerCount} players: {multipliers}", this, playerLevel, players.Count, multipliers);
        return multipliers;

        static float GetMultiplier(IList<float> multipliers, int index) =>
            multipliers.Count == 0 ? 1 : multipliers[Math.Min(index, multipliers.Count - 1)];
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
        await this.RemoveNpcAsync(monster).ConfigureAwait(false);
    }

    /// <summary>
    /// The multipliers for the monsters of a game.
    /// </summary>
    /// <param name="Level">The multiplier for the level.</param>
    /// <param name="Health">The multiplier for the health.</param>
    /// <param name="Damage">The multiplier for the damage.</param>
    /// <param name="Defense">The multiplier for the defense.</param>
    private sealed record MonsterMultipliers(float Level, float Health, float Damage, float Defense)
    {
        /// <summary>
        /// Gets the multipliers which don't change anything.
        /// </summary>
        public static MonsterMultipliers None { get; } = new(1, 1, 1, 1);

        /// <summary>
        /// Applies the multipliers to the monster.
        /// </summary>
        /// <param name="monster">The monster.</param>
        /// <param name="penalty">An additional multiplier for the health, damage and defense.</param>
        public void ApplyTo(Monster monster, float penalty)
        {
            Multiply(monster, Stats.Level, this.Level);
            Multiply(monster, Stats.MaximumHealth, this.Health * penalty);
            Multiply(monster, Stats.MinimumPhysBaseDmg, this.Damage * penalty);
            Multiply(monster, Stats.MaximumPhysBaseDmg, this.Damage * penalty);
            Multiply(monster, Stats.DefenseBase, this.Defense * penalty);
        }

        private static void Multiply(Monster monster, AttributeDefinition attribute, float multiplier)
        {
            if (Math.Abs(multiplier - 1) > 0.001f)
            {
                monster.Attributes.AddElement(new SimpleElement(multiplier, AggregateType.Multiplicate), attribute);
            }
        }
    }

    /// <summary>
    /// A group of interim reward chests, which appeared together. Only one of them can be opened.
    /// </summary>
    private sealed class InterimChestGroup
    {
        private int _isOpened;

        /// <summary>
        /// Gets the chests of the group.
        /// </summary>
        public List<Destructible> Chests { get; } = new();

        /// <summary>
        /// Tries to open the group. It succeeds only once.
        /// </summary>
        /// <returns><c>true</c>, if the group was opened; otherwise, <c>false</c>.</returns>
        public bool TryOpen() => Interlocked.Exchange(ref this._isOpened, 1) == 0;
    }
}
