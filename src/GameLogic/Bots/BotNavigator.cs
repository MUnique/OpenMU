// <copyright file="BotNavigator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Drives a bot's high-level navigation. It picks a hunting ground that suits the bot's level from
/// the monster spawn areas of the current map and points the bot's <see cref="OfflinePlayer.HuntingOrigin"/>
/// at it. The existing offline movement handler then walks the bot there (using the server pathfinder)
/// and the combat handler hunts locally - so a bot which spawns in town finds its own way to the monsters.
/// </summary>
internal sealed class BotNavigator : AsyncDisposable
{
    private static readonly TimeSpan StartDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan EvaluationInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan EmptyGroundGrace = TimeSpan.FromSeconds(8);

    /// <summary>Monsters up to this many levels below the bot still grant full experience (see <see cref="AttackableExtensions"/>).</summary>
    private const int FullExpLowerHeadroom = 10;

    /// <summary>Preferred upper bound: monsters a few levels above the bot are still reasonably safe to fight.</summary>
    private const int SafeUpperHeadroom = 5;

    /// <summary>Width of the level band of areas we randomize between, so bots don't all stack on one spot.</summary>
    private const int BandWidth = 3;

    /// <summary>Range (tiles) around the origin that counts as "at the hunting ground".</summary>
    private const int HuntingRange = 6;

    /// <summary>Number of path steps to issue per travel hop. Short hops let the bot stop and fight between hops.</summary>
    private const int StepsPerHop = 3;

    /// <summary>
    /// If a monster is within this range the bot stops travelling and lets the combat handler engage.
    /// MUST equal the combat hunting range: the combat handler only ever targets monsters within
    /// <see cref="CombatHandler.HuntingRange"/> of the bot, so stopping for anything further away would
    /// hand control to the combat handler for a monster it never fights - the bot would freeze in place
    /// (stop travelling, but find nothing to attack) until the monster happened to wander off.
    /// </summary>
    private const int TravelStopRange = HuntingRange;

    /// <summary>Search limit for the travel path finder, generous enough for a full cross-map route.</summary>
    private const int TravelSearchLimit = 20000;

    /// <summary>Heuristic weight while travelling: high enough to make the A* search greedy and cheap over long routes.</summary>
    private const int TravelHeuristicWeight = 12;

    private const int MaxPointPickAttempts = 25;

    /// <summary>Minimum time between two cross-map warps, so a bot does not bounce between maps.</summary>
    private static readonly TimeSpan WarpCooldown = TimeSpan.FromSeconds(60);

    /// <summary>
    /// A shared full-map path finder for long-distance travel. The pooled path finder uses a small scoped
    /// grid (it rejects start/end further apart than ~16 tiles), so it can only do local movement; this one
    /// uses a <see cref="FullGridNetwork"/> which resolves routes across the whole map. A single instance is
    /// reused under <see cref="TravelPathFinderLock"/> because a path finder is not safe for concurrent use.
    /// </summary>
    private static readonly PathFinder TravelPathFinder = CreateTravelPathFinder();

    private static readonly SemaphoreSlim TravelPathFinderLock = new(1, 1);

    private readonly OfflinePlayer _player;
    private readonly CancellationTokenSource _cts = new();

    private Timer? _timer;
    private DateTime? _emptyGroundSince;
    private int _isEvaluating;
    private Point _destination;
    private bool _hasDestination;
    private DateTime _lastWarpUtc = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotNavigator"/> class.
    /// </summary>
    /// <param name="player">The bot player.</param>
    public BotNavigator(OfflinePlayer player)
    {
        this._player = player;
    }

    /// <summary>
    /// Starts the periodic navigation evaluation.
    /// </summary>
    public void Start()
    {
        this._timer ??= new Timer(
            _ => _ = this.SafeEvaluateAsync(this._cts.Token),
            null,
            StartDelay,
            EvaluationInterval);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await this._cts.CancelAsync().ConfigureAwait(false);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._timer?.Dispose();
            this._timer = null;
            this._cts.Dispose();
        }

        base.Dispose(disposing);
    }

    private async Task SafeEvaluateAsync(CancellationToken cancellationToken)
    {
        // The timer fires on a fixed interval regardless of whether the previous evaluation finished;
        // skip overlapping runs so a single bot never has two travel walks / searches in flight.
        if (Interlocked.CompareExchange(ref this._isEvaluating, 1, 0) != 0)
        {
            return;
        }

        try
        {
            await this.EvaluateAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Bot navigator error for {Account}.", this._player.AccountLoginName);
        }
        finally
        {
            this._isEvaluating = 0;
        }
    }

    private async ValueTask EvaluateAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested
            || this._player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || !this._player.IsAlive
            || this._player.CurrentMap is not { } map)
        {
            return;
        }

        // Keep the combat centre on the bot's current position, so the combat handler always engages
        // monsters right next to the bot - both at the hunting ground and while travelling through hostile
        // territory (self-defence). This is what keeps a travelling bot alive. The travel destination is
        // tracked separately in _destination.
        this._player.HuntingOrigin = this._player.Position;

        // A walk (travel hop or local combat move) is already in progress.
        if (this._player.IsWalking)
        {
            return;
        }

        var monstersNearby = map.GetAttackablesInRange(this._player.Position, TravelStopRange)
            .OfType<Monster>()
            .Count(m => m.IsAlive && !m.IsAtSafezone());

        // Monsters right here: stop and let the combat handler fight them (don't walk away into more danger).
        if (monstersNearby > 0)
        {
            this._emptyGroundSince = null;
            return;
        }

        // Nothing to fight here. If we still have a destination to reach, keep walking towards it.
        if (this._hasDestination && this._player.GetDistanceTo(this._destination) > HuntingRange)
        {
            await this.TravelTowardAsync(map, this._destination).ConfigureAwait(false);
            return;
        }

        // Arrived (or no destination) and the area is empty: wait out a short grace, then pick a new ground.
        this._hasDestination = false;
        this._emptyGroundSince ??= DateTime.UtcNow;
        if (DateTime.UtcNow - this._emptyGroundSince < EmptyGroundGrace)
        {
            return;
        }

        this._emptyGroundSince = null;

        // If the current map offers no level-appropriate monsters, warp to a map that does, so the bot
        // earns useful experience instead of grinding monsters far below (or above) its level. It warps
        // to the destination map's safezone and then walks out to a hunting ground like a real player.
        var botLevel = this.GetBotLevel();
        if (DateTime.UtcNow - this._lastWarpUtc >= WarpCooldown
            && !MapHasIdealBand(map.Definition, botLevel)
            && this.TryPickTargetMap(botLevel, out var targetGate, out var targetMap))
        {
            this._lastWarpUtc = DateTime.UtcNow;
            this._hasDestination = false;
            this._player.Logger.LogInformation(
                "Bot {Character} (level {Level}) warping to map {Map} for a better level match.",
                this._player.Name,
                botLevel,
                targetMap.Name);
            await this._player.WarpToAsync(targetGate).ConfigureAwait(false);
            return;
        }

        if (this.TryPickHuntingGround(map, out var ground, out var groundLevel))
        {
            this._destination = ground;
            this._hasDestination = true;
            this._player.Logger.LogInformation(
                "Bot {Character} (level {Level}) heading to hunting ground {Ground} (monster level ~{MonsterLevel}).",
                this._player.Name,
                this.GetBotLevel(),
                ground,
                groundLevel);
        }
        else
        {
            this._player.Logger.LogDebug("Bot {Character}: no reachable hunting ground found on map {Map}.", this._player.Name, map.Definition.Name);
        }
    }

    /// <summary>
    /// Walks the next stretch towards the destination. A full route is resolved with a heuristic search
    /// (so it goes around walls and out of the town gate) and the first <see cref="StepsPerHop"/> steps are
    /// issued; the remainder is recomputed from the new position on the next tick. The safe zone is allowed
    /// in the path so the bot can leave town.
    /// </summary>
    private async ValueTask TravelTowardAsync(GameMap map, Point destination)
    {
        var position = this._player.Position;

        IList<PathResultNode>? path;
        await TravelPathFinderLock.WaitAsync().ConfigureAwait(false);
        try
        {
            TravelPathFinder.ResetPathFinder();
            path = TravelPathFinder.FindPath(position, destination, map.Terrain.AIgrid, true);
        }
        finally
        {
            TravelPathFinderLock.Release();
        }

        this._player.Logger.LogDebug(
            "Bot {Character}: travel {From} -> dest {Dest}, pathLen={PathLen}.",
            this._player.Name,
            position,
            destination,
            path?.Count ?? -1);
        if (path is null || path.Count == 0)
        {
            return;
        }

        var stepsCount = Math.Min(path.Count, StepsPerHop);
        var steps = new WalkingStep[stepsCount];
        for (var i = 0; i < stepsCount; i++)
        {
            var node = path[i];
            var previous = i == 0 ? position : steps[i - 1].To;
            steps[i] = new WalkingStep(previous, node.Point, previous.GetDirectionTo(node.Point));
        }

        await this._player.WalkToAsync(steps[stepsCount - 1].To, steps).ConfigureAwait(false);
    }

    private static PathFinder CreateTravelPathFinder()
    {
        return new PathFinder(new FullGridNetwork(true))
        {
            Heuristic = new ManhattanTravelHeuristic(),
            HeuristicEstimate = TravelHeuristicWeight,
            SearchLimit = TravelSearchLimit,
        };
    }

    private int GetBotLevel() => (int)(this._player.Attributes?[Stats.Level] ?? 1);

    /// <summary>
    /// Determines whether the map has at least one monster spawn within the bot's full-experience,
    /// reasonably safe level band - i.e. whether it is worth hunting on for that level.
    /// </summary>
    private static bool MapHasIdealBand(GameMapDefinition mapDefinition, int botLevel)
    {
        return mapDefinition.MonsterSpawns
            .Where(a => a is { Quantity: > 0, MonsterDefinition.ObjectKind: NpcObjectKind.Monster })
            .Select(a => GetMonsterLevel(a.MonsterDefinition!))
            .Any(level => level > 0 && level >= botLevel - FullExpLowerHeadroom && level <= botLevel + SafeUpperHeadroom);
    }

    /// <summary>
    /// Picks a random enterable map (other than the current one) whose monsters fit the bot's level,
    /// together with one of its safezone spawn gates to warp to.
    /// </summary>
    private bool TryPickTargetMap(int botLevel, out ExitGate gate, out GameMapDefinition mapDefinition)
    {
        gate = default!;
        mapDefinition = default!;
        var currentDefinition = this._player.CurrentMap?.Definition;
        var candidates = new List<(GameMapDefinition Map, ExitGate Gate)>();
        foreach (var candidate in this._player.GameContext.Configuration.Maps)
        {
            if (ReferenceEquals(candidate, currentDefinition)
                || !MapHasIdealBand(candidate, botLevel)
                || candidate.TryGetRequirementError(this._player, out _))
            {
                continue;
            }

            if (candidate.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is { } spawnGate)
            {
                candidates.Add((candidate, spawnGate));
            }
        }

        if (candidates.Count == 0)
        {
            return false;
        }

        var chosen = candidates[Rand.NextInt(0, candidates.Count)];
        gate = chosen.Gate;
        mapDefinition = chosen.Map;
        return true;
    }

    /// <summary>
    /// Picks a hunting ground point from the map's monster spawn areas, preferring monsters that suit the
    /// bot's level: ideally within [level-10, level+5] (full experience, safe); otherwise the closest band
    /// below (over-leveled bot) or the lowest band above (under-leveled bot).
    /// </summary>
    private bool TryPickHuntingGround(GameMap map, out Point ground, out int groundLevel)
    {
        ground = default;
        groundLevel = 0;

        var botLevel = this.GetBotLevel();
        var candidates = map.Definition.MonsterSpawns
            .Where(a => a is { Quantity: > 0, MonsterDefinition.ObjectKind: NpcObjectKind.Monster })
            .Select(a => (Area: a, Level: GetMonsterLevel(a.MonsterDefinition!)))
            .Where(x => x.Level > 0)
            .ToList();
        if (candidates.Count == 0)
        {
            return false;
        }

        var ideal = candidates.Where(x => x.Level >= botLevel - FullExpLowerHeadroom && x.Level <= botLevel + SafeUpperHeadroom).ToList();
        List<(MonsterSpawnArea Area, int Level)> band;
        if (ideal.Count > 0)
        {
            var top = ideal.Max(x => x.Level);
            band = ideal.Where(x => x.Level >= top - BandWidth).ToList();
        }
        else
        {
            var below = candidates.Where(x => x.Level < botLevel - FullExpLowerHeadroom).ToList();
            if (below.Count > 0)
            {
                var top = below.Max(x => x.Level);
                band = below.Where(x => x.Level >= top - BandWidth).ToList();
            }
            else
            {
                var bottom = candidates.Min(x => x.Level);
                band = candidates.Where(x => x.Level <= bottom + BandWidth).ToList();
            }
        }

        // Weight the choice by spawn quantity and proximity, so the bot prefers nearby, dense grounds
        // (shorter, safer travel) while keeping some variety.
        var position = this._player.Position;
        var chosen = band.SelectWeightedRandom(band.Select(x => GroundWeight(x.Area, position)));
        if (chosen.Area is null)
        {
            return false;
        }

        groundLevel = chosen.Level;
        return this.TryPickWalkablePoint(map, chosen.Area, out ground);
    }

    private static int GetMonsterLevel(MonsterDefinition definition)
        => (int)(definition.Attributes.FirstOrDefault(a => a.AttributeDefinition == Stats.Level)?.Value ?? 0f);

    private static int GroundWeight(MonsterSpawnArea area, Point from)
    {
        var centerX = (area.X1 + area.X2) / 2;
        var centerY = (area.Y1 + area.Y2) / 2;
        var distance = Math.Abs(from.X - centerX) + Math.Abs(from.Y - centerY);
        var proximity = Math.Max(1, 300 - distance);
        return Math.Max(1, (int)area.Quantity) * proximity;
    }

    /// <summary>
    /// A Manhattan distance heuristic (the path finder applies its own estimate multiplier), used to turn the
    /// otherwise unguided search into A* for long-distance bot travel.
    /// </summary>
    private sealed class ManhattanTravelHeuristic : IHeuristic
    {
        public int HeuristicEstimateMultiplier { get; set; }

        public int CalculateHeuristicDistance(Point location, Point target)
            => this.HeuristicEstimateMultiplier * (Math.Abs(location.X - target.X) + Math.Abs(location.Y - target.Y));
    }

    private bool TryPickWalkablePoint(GameMap map, MonsterSpawnArea area, out Point point)
    {
        var minX = Math.Min(area.X1, area.X2);
        var maxX = Math.Max(area.X1, area.X2);
        var minY = Math.Min(area.Y1, area.Y2);
        var maxY = Math.Max(area.Y1, area.Y2);

        for (var attempt = 0; attempt < MaxPointPickAttempts; attempt++)
        {
            var x = Rand.NextInt(minX, maxX + 1);
            var y = Rand.NextInt(minY, maxY + 1);
            if (map.Terrain.WalkMap[x, y] && !map.Terrain.SafezoneMap[x, y])
            {
                point = new Point((byte)x, (byte)y);
                return true;
            }
        }

        point = default;
        return false;
    }
}
