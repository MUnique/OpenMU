// <copyright file="BotNavigator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Drives a bot's high-level navigation. It picks a hunting ground that suits the bot's level from
/// the monster spawn areas of the current map and points the bot's <see cref="OfflinePlayer.HuntingOrigin"/>
/// at it. The existing offline movement handler then walks the bot there (using the server pathfinder)
/// and the combat handler hunts locally - so a bot which spawns in town finds its own way to the monsters.
/// </summary>
internal sealed class BotNavigator : AsyncDisposable
{
    /// <summary>
    /// A bot only warps to another map if it offers safe monsters at least this many levels stronger.
    /// Sized as a hysteresis band: the safety verdict for a borderline monster flips when the bot's
    /// defense changes with its buffs, and a small margin let two maps leapfrog each other on every
    /// re-evaluation - the bot ping-ponged between them on warp cooldown.
    /// </summary>
    private const int WarpImprovementMargin = 8;

    /// <summary>Fallback when the server defines no warps: the lowest stock requirement.</summary>
    private const int FallbackMinWarpLevel = 10;

    /// <summary>Width of the level band of areas we randomize between, so bots don't all stack on one spot.</summary>
    private const int BandWidth = 3;

    /// <summary>Range (tiles) around the origin that counts as "at the hunting ground".</summary>
    private const int HuntingRange = BotMuHelperSettings.DefaultHuntingRange;

    /// <summary>
    /// Distance (tiles) at which a hunting ground counts as half as attractive as one the bot is standing on.
    /// See <see cref="GroundWeight"/>.
    /// </summary>
    private const int ProximityFalloff = 64;

    /// <summary>
    /// Width (tiles) of the box a hunting ground point is sampled from when the spawn area itself is smaller.
    /// See <see cref="TryPickWalkablePoint"/>.
    /// </summary>
    private const int GroundScatterSpan = 2 * HuntingRange;

    /// <summary>
    /// Share of the shopping cooldown the next trip is spread over, so that bots which shopped together
    /// don't come back together. See <see cref="NextShoppingCheckUtc"/>.
    /// </summary>
    private const double ShoppingSpread = 0.4;

    /// <summary>
    /// Share of the buff check interval the next check is spread over, so bots don't all walk to the
    /// buff NPC at once. See <see cref="NextBuffCheckUtc"/>.
    /// </summary>
    private const double BuffSpread = 0.4;

    /// <summary>
    /// How far the bot looks for an actual live monster to home in on. MU spawn areas span most of the map
    /// (e.g. Lorencia's are ~86x233 / ~105x68 tiles) with only a few dozen monsters each, so their monsters
    /// sit ~20 tiles apart. Walking to a random tile inside such an area almost never lands within the 6-tile
    /// combat range of a real monster - the bot arrives, finds nothing to fight, and wanders off. So once the
    /// bot is anywhere in the region it scans this radius for the nearest live monster and heads straight for
    /// it. Kept modest so the per-tick area-of-interest scan stays cheap even with hundreds of bots.
    /// </summary>
    private const int MonsterSeekRadius = 40;

    /// <summary>Item group of potions (healing and mana potions share group 14).</summary>
    private const int HealPotionGroup = 14;

    /// <summary>Item number of the Large Healing Potion within <see cref="HealPotionGroup"/>.</summary>
    private const int HealPotionNumber = 3;

    /// <summary>Item number of the Large Mana Potion within <see cref="HealPotionGroup"/>.</summary>
    private const int ManaPotionNumber = 6;

    /// <summary>
    /// How many charges an emergency refill stocks up (the potion item's Durability holds the remaining
    /// count; one is spent per heal). Spread over as many item stacks as the definition's stack size
    /// needs: a Large Healing Potion holds three charges per stack, so a bot handed a single 255-charge
    /// "potion" carried an item the game cannot produce - its stack never looked low again (the shopping
    /// loop for that potion kind went dead), and stacking it with a bought one underflowed the count.
    /// </summary>
    private const int EmergencyPotionCharges = 21;

    /// <summary>
    /// Emergency refill threshold: normally the bot restocks potions by BUYING them at a merchant (see
    /// <see cref="BotShoppingHandler"/>, triggered well above this level); only when a stack still runs
    /// this low (e.g. no Zen, merchant unreachable) is it topped up out of thin air as a last resort,
    /// so the bot never dies over an empty bottle.
    /// </summary>
    private const int HealPotionRefillBelow = 10;

    /// <summary>Number of path steps to issue per travel hop. Short hops let the bot stop and fight between hops.</summary>
    private const int StepsPerHop = 3;

    /// <summary>
    /// How far the travel destination may drift (e.g. a roaming monster the bot homes in on) before the
    /// cached route is re-planned. Re-planning a whole-map A* every tick just to consume three steps
    /// starved the pathfinder pool once dozens of bots traveled at the same time.
    /// </summary>
    private const int PathReuseTolerance = 4;

    /// <summary>How close (tiles) the bot must be to the merchant to trade.</summary>
    private const int MerchantTalkRange = 3;

    /// <summary>
    /// If a monster is within this range the bot stops traveling and lets the combat handler engage.
    /// MUST equal the combat hunting range: the combat handler only ever targets monsters within
    /// <see cref="CombatHandler.HuntingRange"/> of the bot, so stopping for anything further away would
    /// hand control to the combat handler for a monster it never fights - the bot would freeze in place
    /// (stop travelling, but find nothing to attack) until the monster happened to wander off.
    /// </summary>
    private const int TravelStopRange = HuntingRange;

    /// <summary>Search limit for the travel pathfinder, generous enough for a full cross-map route.</summary>
    private const int TravelSearchLimit = 20000;

    /// <summary>Heuristic weight while traveling: high enough to make the A* search greedy and cheap over long routes.</summary>
    private const int TravelHeuristicWeight = 12;

    private const int MaxPointPickAttempts = 25;

    /// <summary>
    /// Follow trigger for mini-game events: a participant further than this (tiles) from its
    /// leader walks back. Open-world party formation uses <see cref="PartyRegroupDistance"/> instead.
    /// </summary>
    private const int FollowDistance = 10;

    /// <summary>
    /// Open-world formation radius: with nothing to fight around the leader, a follower closes back
    /// in to within this distance (tiles). Deliberately tighter than <see cref="HuntingRange"/>, so the
    /// party stacks up when idle; the regroup walk itself is gated on "no fightable monster near the
    /// leader" (see <see cref="EvaluateFollowerHuntingAsync"/>), so combat drift out to hunting range
    /// never triggers a yank-back mid-fight.
    /// </summary>
    private const int PartyRegroupDistance = 2;

    /// <summary>
    /// Distance (tiles) from its leader beyond which a follower walks back even while there are monsters
    /// near the leader. Twice the hunting range: the combat handler only fights targets within
    /// <see cref="HuntingRange"/> of the leader, so no fight keeps a follower further away than that.
    /// </summary>
    private const int FollowerLeashDistance = 2 * HuntingRange;

    /// <summary>
    /// Hunting grounds closer than this (tiles) to a recent death site are avoided after the same
    /// player killed the bot repeatedly (see <see cref="OfflinePlayer.TryGetDeathSiteToAvoid"/>),
    /// so the bot farms somewhere else instead of walking back into the same lost fight.
    /// </summary>
    private const int DeathSiteAvoidanceRange = 30;

    /// <summary>
    /// Share of the picks the best ranked map takes when the bot chooses where to hunt (see
    /// <see cref="PickByRank"/>); the runners-up split what is left, on the same terms. Below a half on
    /// purpose: at a half the fourth map of a level band and everything behind it share a few percent
    /// between them, which left Karutan, Icarus and Kanturu with three or four bots each while the top
    /// map held a hundred. The bot still prefers the best ground - every candidate it draws from is
    /// already a step up from where it stands - it just does not treat the runner-up as worthless.
    /// </summary>
    private const double BestCandidateShare = 0.35;

    /// <summary>The game's own warp action, so a bot travels on exactly a player's terms - fare included.</summary>
    private static readonly WarpAction WarpAction = new();

    /// <summary>
    /// Cache of which maps' safezone spawn gates have a walkable tile, so a bot may safely be sent
    /// there. Keyed by the resolved safezone map's id, not its number: map numbers are not unique -
    /// the Devil Squares share number 9, and a server can host several game configurations whose
    /// maps carry the same numbers over different terrain. See <see cref="HasWalkableSpawnGate"/>.
    /// </summary>
    private static readonly ConcurrentDictionary<Guid, bool> WalkableSpawnGateCache = new();

    /// <summary>
    /// How often the bot takes its navigation decisions (where to go, whether to warp, shop, follow).
    /// Same pace as the MU Helper tick: a bot standing next to its leader sets off at most half a second
    /// after him. The errands checked on each decision keep their own, much longer intervals.
    /// </summary>
    private static readonly TimeSpan EvaluationInterval = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// How often the navigator timer fires. Most shots end right away; they only exist to notice that a
    /// travel hop has ended, so the next one starts without waiting for <see cref="EvaluationInterval"/>
    /// (see <see cref="TryBeginEvaluation"/>).
    /// </summary>
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(200);

    private static readonly TimeSpan EmptyGroundGrace = TimeSpan.FromSeconds(8);

    /// <summary>How often the bot checks its backpack for equippable upgrades.</summary>
    private static readonly TimeSpan EquipCheckInterval = TimeSpan.FromSeconds(8);

    /// <summary>How often the bot considers whether it needs a shopping trip.</summary>
    private static readonly TimeSpan ShoppingCheckInterval = TimeSpan.FromSeconds(30);

    /// <summary>Cooldown after a shopping trip before considering the next one.</summary>
    private static readonly TimeSpan ShoppingCooldown = TimeSpan.FromMinutes(10);

    /// <summary>
    /// How often the bot re-evaluates whether it should claim a buff NPC's free buff: a level-eligible
    /// bot could claim it right away, so the check re-fires quickly while the buff has not yet been applied.
    /// </summary>
    private static readonly TimeSpan BuffCheckInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Cooldown after a buff trip before considering the next one, so bots which just got the buff
    /// (and level-eligible ones which could not claim it) don't re-walk to the NPC every check interval.
    /// </summary>
    private static readonly TimeSpan BuffCooldown = TimeSpan.FromMinutes(10);

    /// <summary>
    /// How often the bot considers a visit to the chaos machine (see <see cref="BotChaosMachineHandler"/>).
    /// Short, so a bot standing at the goblin mixes its next piece right away; planning is cheap.
    /// </summary>
    private static readonly TimeSpan ChaosMachineCheckInterval = TimeSpan.FromSeconds(30);

    /// <summary>Cooldown after a chaos machine trip which came to nothing (no route, no warp there, or a refused mix).</summary>
    private static readonly TimeSpan ChaosMachineCooldown = TimeSpan.FromMinutes(10);

    /// <summary>Minimum time between two cross-map warps, so a bot does not bounce between maps (a real player does not hop maps every minute either).</summary>
    private static readonly TimeSpan WarpCooldown = TimeSpan.FromMinutes(3);

    /// <summary>
    /// How long a bot may go without landing a single hit before it treats its map as barren and steps
    /// down (see <see cref="TryPickEasierMap"/>). Long enough that a walk across a map, a merchant trip
    /// or a quiet stretch between spawns does not count as one.
    /// </summary>
    private static readonly TimeSpan BarrenMapDuration = TimeSpan.FromMinutes(3);

    /// <summary>Cooldown for following the party leader to another map (shorter, so the group regroups quickly).</summary>
    private static readonly TimeSpan FollowWarpCooldown = TimeSpan.FromSeconds(20);

    /// <summary>Cooldown between expensive same-map gate searches after walking to the leader failed.</summary>
    private static readonly TimeSpan FollowWarpSearchCooldown = TimeSpan.FromSeconds(20);

    /// <summary>
    /// How long the party leader has to stay on a map before its bots follow him there. A leader who is
    /// only passing through (a town trip for supplies, a gate on the way somewhere) must not drag his
    /// whole party across the world and back - real party members wait to see where he settles, too.
    /// </summary>
    private static readonly TimeSpan LeaderSettleTime = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Bounds of the random delay between becoming eligible for a character reset and performing it.
    /// A real player finishes the fight, walks to town, maybe trades first - and 250 bots resetting
    /// the very second they hit the required level would be an obvious give-away (and a thundering
    /// herd of simultaneous re-gearing). The delay is rolled per bot when it becomes eligible.
    /// </summary>
    private static readonly TimeSpan MinResetDelay = TimeSpan.FromMinutes(1);

    /// <summary>Upper bound of the random reset delay, see <see cref="MinResetDelay"/>.</summary>
    private static readonly TimeSpan MaxResetDelay = TimeSpan.FromMinutes(10);

    /// <summary>
    /// If the bot's position has not changed for this long, it is considered stuck (a wedged walk, or a
    /// monster it can't reach). The navigator then forces a fresh destination, so the bot gets going again.
    /// </summary>
    private static readonly TimeSpan StuckTimeout = TimeSpan.FromSeconds(8);

    /// <summary>
    /// Stuck timeout when huntable monsters are nearby: much longer, because a bot standing still while
    /// monsters are around is usually just fighting them from one tile. It still eventually fires, which
    /// breaks the rare deadlock of a monster that is in range but cannot be attacked or reached.
    /// </summary>
    private static readonly TimeSpan StuckWithMonstersTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// A pool of full-map pathfinders for long-distance travel. The pooled (scoped) pathfinder rejects
    /// start/end further apart than ~16 tiles, so travel uses a <see cref="FullGridNetwork"/> which resolves
    /// routes across the whole map. A pathfinder is not safe for concurrent use, so we keep a small pool and
    /// hand one out per call: several bots can plan routes in parallel (a single shared instance serialized
    /// every bot and starved navigation once the population reached the hundreds), while the pool size caps
    /// how many CPU cores whole-map searches may occupy at once.
    /// </summary>
    private static readonly int TravelPathFinderPoolSize = Math.Clamp(Environment.ProcessorCount / 3, 2, 6);

    private static readonly SemaphoreSlim TravelPathFinderPool = new(TravelPathFinderPoolSize, TravelPathFinderPoolSize);

    private static readonly ConcurrentBag<PathFinder> TravelPathFinders = CreateTravelPathFinderPool();

    private readonly OfflinePlayer _player;
    private readonly TimeProvider _timeProvider;
    private readonly CancellationTokenSource _cts = new();

    private Timer? _timer;
    private DateTime? _emptyGroundSince;
    private int _isEvaluating;
    private Point _destination;
    private bool _hasDestination;
    private DateTimeOffset _lastWarpUtc = DateTimeOffset.MinValue;
    private DateTimeOffset _nextFollowWarpSearchUtc = DateTimeOffset.MinValue;
    private Point _lastPosition;
    private DateTime _lastMoveUtc = DateTime.MinValue;
    private DateTime _nextEquipCheckUtc = DateTime.MinValue;
    private IList<PathResultNode>? _travelPath;
    private int _travelPathIndex;
    private Point _travelPathTarget;
    private Point? _shoppingTarget;

    /// <summary>Target position of the current buff trip, or null, if there is none.</summary>
    private Point? _buffTarget;

    /// <summary>Target position of the current chaos machine trip, or null, if there is none.</summary>
    private Point? _chaosMachineTarget;

    /// <summary>The walkable tile next to the goblin the current chaos machine trip walks to.</summary>
    private Point _chaosMachineStandingPoint;

    /// <summary>
    /// When the bot considers a chaos machine trip again; the first one is spread over the cooldown, so
    /// a population does not crowd the goblin at once after a restart.
    /// </summary>
    private DateTime _nextChaosMachineCheckUtc = DateTime.UtcNow + (ChaosMachineCooldown * Rand.NextDouble());

    /// <summary>
    /// When the bot considers a buff trip again. Like the shopping check, the first one is spread over the
    /// cooldown (not "now"), so bots don't all march to the Elf Soldier at once.
    /// </summary>
    private DateTime _nextBuffCheckUtc = DateTime.UtcNow + (BuffCooldown * Rand.NextDouble());

    /// <summary>
    /// When this bot looks at its supplies again. Bots come up with the server, so starting them all at
    /// "now" sent the whole population to the same merchant in the same minute - and since the cooldown
    /// was the same for everyone, nothing ever broke that herd apart again. The first check is therefore
    /// spread over the cooldown, and every following one is spread around it.
    /// </summary>
    private DateTime _nextShoppingCheckUtc = DateTime.UtcNow + (ShoppingCooldown * Rand.NextDouble());
    private DateTime? _resetDueAtUtc;
    private GameConfiguration? _warpFloorConfiguration;
    private CharacterClass? _warpFloorClass;
    private int _minWarpLevel = FallbackMinWarpLevel;
    private Guid _leaderMapId;
    private DateTimeOffset _leaderOnMapSinceUtc = DateTimeOffset.MinValue;
    private DateTimeOffset _nextEvaluationUtc = DateTimeOffset.MinValue;
    private bool _isHopInProgress;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotNavigator"/> class.
    /// </summary>
    /// <param name="player">The bot player.</param>
    public BotNavigator(OfflinePlayer player)
        : this(player, TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BotNavigator"/> class.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="timeProvider">The time provider.</param>
    internal BotNavigator(OfflinePlayer player, TimeProvider timeProvider)
    {
        this._player = player;
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Starts the periodic navigation evaluation. The start delay is jittered per bot, so hundreds of
    /// navigators do not all fire on the same second boundary (smoother load, less robotic synchrony).
    /// </summary>
    public void Start()
    {
        // The token is captured ONCE, not read from the (later disposed) source on every shot: a timer
        // callback which is already running when Dispose gets rid of the source would throw an
        // ObjectDisposedException right in the callback - unhandled, on a thread pool thread, taking the
        // whole server process down with it. Same pattern as the MU Helper tick.
        var cancellationToken = this._cts.Token;
        this._timer ??= new Timer(
            state => _ = this.SafeEvaluateAsync(cancellationToken),
            null,
            TimeSpan.FromMilliseconds(2000 + Rand.NextInt(0, 1500)),
            PollInterval);
    }

    /// <summary>
    /// Rates a spawn area as a hunting ground for a bot standing at the given point.
    /// </summary>
    /// <param name="area">The spawn area.</param>
    /// <param name="from">The point the bot measures from.</param>
    /// <returns>The relative weight of the area.</returns>
    internal static int GroundWeight(MonsterSpawnArea area, Point from)
    {
        // Manhattan distances on a 256x256 map reach ~460, so a linear "300 - distance" term rated a
        // ground next to the entrance up to 299 while everything past the halfway point clamped to 1.
        // Bots enter a map through the same gate and therefore all measure from the same spot, so that
        // ratio made them queue up on the handful of spawns nearest to it. A hyperbolic falloff keeps
        // the preference for close grounds - travel is still time not spent hunting - without writing
        // off the rest of the map.
        var proximity = (ProximityFalloff * ProximityFalloff) / (ProximityFalloff + GroundDistance(area, from));
        return Math.Max(1, (int)area.Quantity) * Math.Max(1, proximity);
    }

    /// <summary>
    /// Whether a bot may safely be sent to the map: the safezone spawn gate it would be recovered to
    /// has at least one walkable tile. A warp which lands a bot on a blocked tile is recovered by
    /// <see cref="Player.WarpToSafezoneAsync"/>, and for a connection-less bot that recovery runs
    /// inline (see <see cref="OfflineMapChangePlugIn"/>) - so a spawn gate with no walkable tile at
    /// all used to recurse until the stack overflowed. The player's map transitions now bound that
    /// recovery, so this filter is no longer what keeps the server alive; it keeps the bot from
    /// picking a map it cannot stand in and being bounced straight back out of it.
    /// </summary>
    /// <param name="map">The map the bot considers warping to.</param>
    /// <returns>True, if the map's resolved safezone spawn gate has a walkable tile.</returns>
    /// <remarks>
    /// The verdict is cached for the lifetime of the process, keyed by the resolved safezone map -
    /// terrain is static configuration, so it is parsed once and shared by every bot. Terrain edited
    /// through the admin panel therefore needs a server restart to be taken into account.
    /// Only maps whose spawn gate has *zero* walkable tiles are excluded. A gate with a few walkable
    /// tiles can still, under PlaceAtGateAsync's single random roll, miss and recover to town - a
    /// quality glitch, not a crash, accepted by design.
    /// </remarks>
    internal static bool HasWalkableSpawnGate(GameMapDefinition map)
    {
        // The bot is not recovered to the destination map's own gate but to its safezone map's -
        // mirroring Player.GetSpawnGateOfCurrentMapAsync. Many maps (dungeons, event maps, Icarus,
        // Karutan 2, ...) point somewhere else entirely, so checking the destination itself would
        // both reject safe maps and miss the ones which actually strand a bot.
        var safezoneMap = map.SafezoneMap ?? map;
        var mapId = safezoneMap.GetId();
        if (mapId == Guid.Empty)
        {
            // GetId falls back to Guid.Empty for anything it cannot identify, so caching under it
            // would file every such map under one entry and hand the first map's verdict to all the
            // others - a single blocked one would then reject every map there is.
            return HasWalkableSpawnGateCore(safezoneMap);
        }

        return WalkableSpawnGateCache.GetOrAdd(mapId, _ => HasWalkableSpawnGateCore(safezoneMap));
    }

    /// <summary>
    /// The uncached terrain check behind <see cref="HasWalkableSpawnGate"/>, on an already resolved
    /// safezone map.
    /// </summary>
    /// <param name="safezoneMap">The resolved safezone map.</param>
    /// <returns>True, if the map's spawn gate has a walkable tile.</returns>
    internal static bool HasWalkableSpawnGateCore(GameMapDefinition safezoneMap)
    {
        // The map is usually not loaded when a bot weighs it up, so its GameMap (which would expose
        // Terrain and SafeZoneSpawnGate ready-made) may not exist - and forcing it to load just for
        // this check would be far more expensive than parsing the terrain once.
        var terrain = new GameMapTerrain(safezoneMap);
        return terrain.GetWalkableCoordinate(safezoneMap.GetSafezoneGate(terrain)) is not null;
    }

    /// <summary>
    /// Follows the party leader across maps after the leader has settled at the destination.
    /// </summary>
    /// <param name="map">The bot's current map.</param>
    /// <param name="leader">The party leader.</param>
    /// <returns>True, if evaluation should stop because a follow action was taken or the leader is on
    /// another map; false, if the bot can continue its normal local behavior.</returns>
    internal async ValueTask<bool> TryHandleCrossMapLeaderFollowAsync(GameMap map, Player leader)
    {
        if (ReferenceEquals(leader.CurrentMap, map))
        {
            return false;
        }

        ExitGate? warpListGate = null;
        if (leader.CurrentMap is { } targetMap
            && (this.TryGetLegalWarp(targetMap.Definition, out var leaderWarp)
                ? (warpListGate = leaderWarp.Gate) is null
                : targetMap.Definition.Number != this._player.SelectedCharacter?.CharacterClass?.HomeMap?.Number))
        {
            // The leader moved to a map the bot's plain character level cannot legally enter
            // (level gates map access, the same rule as everywhere else). Rather than trail
            // behind unreachable or sneak in through a back door, the bot leaves the group.
            if (this._player.Party is { } party)
            {
                this._player.Logger.LogDebug(
                    "Bot {Character} leaves its party: it cannot legally follow '{Leader}' to map {Map}.",
                    this._player.Name,
                    leader.Name,
                    targetMap.Definition.Name);
                await party.KickMySelfAsync(this._player).ConfigureAwait(false);
            }

            return true;
        }

        // Only follow a leader who SETTLED on the new map: a leader in transit (pulling a town
        // scroll for shopping, passing through a gate, hopping back and forth) used to drag its
        // whole party along on every hop - and since the followers land on the map's spawn gate
        // rather than next to him, they were still walking over when he warped away again.
        if (!this.HasLeaderSettled(leader))
        {
            return true;
        }

        // Prefer a spawn gate of the leader's map; maps without one (the Dungeon has no town)
        // fall back to the warp list gate - the same spot the leader warped to. Without the
        // fallback a follower could neither warp after its leader nor hunt (following consumed
        // its ticks), and only the stuck watchdog kept it twitching between hunting grounds.
        if (this._timeProvider.GetUtcNow() - this._lastWarpUtc >= FollowWarpCooldown
            && leader.CurrentMap is { } leaderMap
            && (leaderMap.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() ?? warpListGate) is { } leaderGate)
        {
            this._lastWarpUtc = this._timeProvider.GetUtcNow();
            this._nextFollowWarpSearchUtc = DateTimeOffset.MinValue;
            this._hasDestination = false;
            this._travelPath = null;
            this._player.Logger.LogDebug(
                "Bot {Character} following party leader {Leader} to map {Map}.",
                this._player.Name,
                leader.Name,
                leaderMap.Definition.Name);
            await this._player.WarpToAsync(leaderGate).ConfigureAwait(false);
            await this.TryPersistCurrentMapAsync(leaderMap.Definition).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Regroups with a leader on the same map by walking, or by using a reachable legal warp when
    /// disconnected floors make walking impossible.
    /// </summary>
    /// <param name="map">The shared map of the bot and its leader.</param>
    /// <param name="leader">The party leader to regroup with.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if a walk or warp was performed; otherwise false.</returns>
    /// <remarks>
    /// The result is observed by focused regression tests. Production follower hunting returns after
    /// the regroup attempt regardless of whether an action was available.
    /// </remarks>
    internal async ValueTask<bool> TryRegroupWithLeaderOnSameMapAsync(GameMap map, Player leader, CancellationToken cancellationToken)
    {
        // Discard any previous hunting destination before resolving the moving leader again. Only
        // publish the leader as a destination after a route was found and a walk was actually issued.
        this._hasDestination = false;
        if (await this.TravelTowardAsync(map, leader.Position, cancellationToken).ConfigureAwait(false))
        {
            this._nextFollowWarpSearchUtc = DateTimeOffset.MinValue;
            this._destination = leader.Position;
            this._hasDestination = true;
            return true;
        }

        var now = this._timeProvider.GetUtcNow();
        if (now - this._lastWarpUtc < FollowWarpCooldown
            || now < this._nextFollowWarpSearchUtc)
        {
            return false;
        }

        // Some maps represent several floors as disconnected regions on the same map id
        // (Dungeon, Lost Tower, ...). If walking cannot reach the leader, regroup through the
        // closest legal warp entry whose complete landing area can reach him. Requiring every possible
        // landing point is deliberate: the warp performs one unvalidated random placement, so accepting
        // a partially usable gate would make following succeed or fail by chance.
        var leaderWarp = this.FindBestReachableLegalWarp(map, leader.Position, cancellationToken);
        if (leaderWarp?.Gate is not { } leaderGate)
        {
            // Start the full retry delay after a completed, unsuccessful scan. Keep this separate
            // from _lastWarpUtc, which only records real warps and controls other warp decisions.
            this._nextFollowWarpSearchUtc = this._timeProvider.GetUtcNow() + FollowWarpSearchCooldown;
            return false;
        }

        this._lastWarpUtc = this._timeProvider.GetUtcNow();
        this._travelPath = null;
        this._player.Logger.LogDebug(
            "Bot {Character} following party leader {Leader} within map {Map} through warp {Warp}.",
            this._player.Name,
            leader.Name,
            map.Definition.Name,
            leaderWarp.Name);
        await this._player.WarpToAsync(leaderGate).ConfigureAwait(false);
        await this.TryPersistCurrentMapAsync(map.Definition).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Whether the leader has been on his current map long enough (<see cref="LeaderSettleTime"/>) for the
    /// bot to follow him there. Tracks the leader's map instance per bot, so each follower makes the call on its own.
    /// </summary>
    /// <param name="leader">The party leader.</param>
    /// <returns>True, if the leader has settled long enough.</returns>
    internal bool HasLeaderSettled(Player leader)
    {
        if (leader.CurrentMap is not { } leaderMap)
        {
            return false;
        }

        if (this._leaderMapId != leaderMap.Id)
        {
            this._leaderMapId = leaderMap.Id;
            this._leaderOnMapSinceUtc = this._timeProvider.GetUtcNow();
        }

        return this._timeProvider.GetUtcNow() - this._leaderOnMapSinceUtc >= LeaderSettleTime;
    }

    /// <summary>
    /// Picks a hunting ground and starts walking to it. If the chosen ground turns out to be unreachable, the
    /// destination and its claim are dropped, so the next call (next tick) simply picks another one.
    /// </summary>
    /// <param name="map">The map to pick a hunting ground on.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async ValueTask PickGroundAndTravelAsync(GameMap map, CancellationToken cancellationToken)
    {
        if (!this.TryPickHuntingGround(map, out var ground, out var groundLevel))
        {
            this._hasDestination = false;
            this._player.HuntingSpot = null;
            this._player.Logger.LogDebug("Bot {Character}: no hunting ground found on map {Map}.", this._player.Name, map.Definition.Name);
            return;
        }

        this._player.HuntingSpot = new HuntingSpot(ground, map.Definition.Number);
        this._destination = ground;
        this._hasDestination = true;
        this._player.Logger.LogDebug(
            "Bot {Character} (level {Level}) heading to hunting ground {Ground} (monster level ~{MonsterLevel}).",
            this._player.Name,
            this.GetBotLevel(),
            ground,
            groundLevel);

        if (!await this.TravelTowardAsync(map, ground, cancellationToken).ConfigureAwait(false))
        {
            this._hasDestination = false;
            this._player.HuntingSpot = null;
        }
    }

    /// <summary>
    /// Finds the position of the nearest live, attackable monster within <see cref="MonsterSeekRadius"/> that
    /// the bot can safely fight, so the bot heads straight for a real monster instead of a random tile in a
    /// huge spawn area. Returns false when none are loaded nearby (or all are above the safe level cap), in
    /// which case the caller falls back to coarse spawn-area travel / warping to relocate.
    /// </summary>
    /// <param name="map">The map to search on.</param>
    /// <param name="ground">The position of the chosen monster.</param>
    /// <returns>True, if a safe monster was found.</returns>
    internal bool TryFindNearestMonsterGround(GameMap map, out Point ground)
    {
        ground = default;
        var position = this._player.Position;
        var attackables = map.GetAttackablesInRange(position, MonsterSeekRadius);
        var candidates = new List<(Monster Monster, double Distance)>();
        foreach (var attackable in attackables)
        {
            if (attackable is not Monster { SummonedBy: null } monster
                || !monster.IsAlive
                || monster.IsAtSafezone())
            {
                continue;
            }

            if (!CombatHandler.IsSafeTarget(this._player, monster.Definition))
            {
                // Too dangerous (judged by its real damage, not its level) - leave it and let the warp /
                // area logic relocate the bot to a map whose monsters it can handle.
                continue;
            }

            candidates.Add((monster, monster.GetDistanceTo(position)));
        }

        if (candidates.Count == 0)
        {
            return false;
        }

        // Prefer monsters no other bot outside the party is already hunting. If every nearby monster is
        // contested, the ground is saturated - sharing it beats standing around, so the bot still picks one.
        var competitorSpots = HuntingSpotOccupancy.GetCompetitorSpots(this._player, attackables.OfType<Player>(), map.Definition.Number);
        var unclaimed = candidates
            .Where(c => !HuntingSpotOccupancy.IsMonsterClaimed(c.Monster.Position, competitorSpots, HuntingRange))
            .ToList();
        if (unclaimed.Count == 0)
        {
            unclaimed = candidates;
        }

        // Head for one of the few nearest monsters, chosen randomly, instead of strictly the nearest:
        // with many bots on one ground, deterministic nearest-first would send them all to the same
        // monster, and they would roam the map as one pack - a very bot-like look.
        var chosen = unclaimed
            .OrderBy(c => c.Distance)
            .Take(3)
            .Select(c => c.Monster)
            .SelectRandom();
        if (chosen is null)
        {
            return false;
        }

        // The monster stands on a walkable tile, so head straight for it; TravelTowardAsync walks a few steps
        // per tick and re-homes as the monster roams, closing the gap until combat range is reached. Claiming
        // the target keeps other bots from converging on the same monster while this one is still on its way.
        this._player.HuntingSpot = new HuntingSpot(chosen.Position, map.Definition.Number);
        ground = chosen.Position;
        return true;
    }

    /// <summary>
    /// Determines whether the navigator evaluates on this timer shot, and if so, starts the next interval:
    /// once per <see cref="EvaluationInterval"/>, and additionally as soon as a travel hop has ended.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <returns><c>true</c>, if the bot should take its next navigation decision now.</returns>
    /// <remarks>
    /// A hop of <see cref="StepsPerHop"/> steps takes about a second. When the hop still ran at a decision,
    /// that decision was skipped, and the bot stood until the next one - it walked in bursts, at about half
    /// the speed of a player. Continuing right after the hop keeps it moving, while it still stops between
    /// hops when a monster comes into range.
    /// </remarks>
    internal bool TryBeginEvaluation(DateTimeOffset now)
    {
        if (now < this._nextEvaluationUtc && !(this._isHopInProgress && !this._player.IsWalking))
        {
            return false;
        }

        this._nextEvaluationUtc = now + EvaluationInterval;
        this._isHopInProgress = false;
        return true;
    }

    /// <summary>
    /// Hunts around the party leader on the same map: monsters within hunting range
    /// (<see cref="HuntingRange"/>, mirroring the follower's effective combat range of
    /// <see cref="Bots.BotMuHelperSettings.HuntingRange"/>) of the leader are fought via the combat
    /// handler (tethered through <see cref="OfflinePlayer.HuntingOrigin"/>, which is also set where the
    /// origin is decided so it holds on walking ticks), and only with nothing fightable near the leader
    /// does the bot close back in to within <see cref="PartyRegroupDistance"/>. Followers never pick
    /// independent hunting grounds - the leader decides where the party hunts.
    /// </summary>
    /// <remarks>
    /// Two cases close in regardless of monsters: a follower in the safezone, where it cannot fight, and
    /// a follower further than <see cref="FollowerLeashDistance"/> from its leader.
    /// </remarks>
    /// <param name="map">The map of the bot and its leader.</param>
    /// <param name="leader">The party leader.</param>
    /// <param name="inSafezone">Whether the bot stands in the safezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async ValueTask EvaluateFollowerHuntingAsync(GameMap map, Player leader, bool inSafezone, CancellationToken cancellationToken)
    {
        // Tether combat to the leader, so the bot fights what is near him instead of wandering off.
        this._player.HuntingOrigin = leader.Position;
        var distanceToLeader = this._player.GetDistanceTo(leader.Position);

        if (inSafezone)
        {
            // Nothing to fight in town, so the bot keeps up with its leader - also when he heads out to the
            // monsters at the gate. It used to stand still here and only moved when the stuck watchdog
            // fired: three steps every few seconds.
            if (distanceToLeader > PartyRegroupDistance)
            {
                _ = await this.TryRegroupWithLeaderOnSameMapAsync(map, leader, cancellationToken).ConfigureAwait(false);
                return;
            }

            this._hasDestination = false;
            this._emptyGroundSince = null;
            return;
        }

        var monsterNearLeader = map.GetAttackablesInRange(leader.Position, HuntingRange)
            .OfType<Monster>()
            .Any(m => m.IsAlive && !m.IsAtSafezone() && m.SummonedBy is null && CombatHandler.IsSafeTarget(this._player, m.Definition));
        if (monsterNearLeader && distanceToLeader <= FollowerLeashDistance)
        {
            // Something fightable near the leader: hold position and let the combat handler engage,
            // even when drifted out to hunting range - walking back now would interrupt the fight.
            this._hasDestination = false;
            this._emptyGroundSince = null;
            return;
        }

        // Nothing to fight around the leader, or the bot fell so far behind that no fight explains it
        // (e.g. it holds for a monster it cannot reach while the leader runs on): close back into formation.
        if (distanceToLeader > PartyRegroupDistance)
        {
            // The result is used by direct regression tests. This production flow returns after the
            // regroup attempt either way, so there is no subsequent behavior to condition on it.
            _ = await this.TryRegroupWithLeaderOnSameMapAsync(map, leader, cancellationToken).ConfigureAwait(false);
            return;
        }

        this._hasDestination = false;
        this._emptyGroundSince = null;
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._timer is { } timer)
        {
            this._timer = null;
            await timer.DisposeAsync().ConfigureAwait(false);
        }

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

    private static PathFinder CreateTravelPathFinder()
    {
        return new PathFinder(new FullGridNetwork(true))
        {
            Heuristic = new ManhattanTravelHeuristic(),
            HeuristicEstimate = TravelHeuristicWeight,
            SearchLimit = TravelSearchLimit,
        };
    }

    private static ConcurrentBag<PathFinder> CreateTravelPathFinderPool()
    {
        var pool = new ConcurrentBag<PathFinder>();
        for (var i = 0; i < TravelPathFinderPoolSize; i++)
        {
            pool.Add(CreateTravelPathFinder());
        }

        return pool;
    }

    private static int GetMonsterLevel(MonsterDefinition definition)
        => (int)(definition.Attributes.FirstOrDefault(a => a.AttributeDefinition == Stats.Level)?.Value ?? 0f);

    /// <summary>Manhattan distance between the center of the spawn area and the given point.</summary>
    /// <summary>
    /// Returns the time of the next supply check, spread around <see cref="ShoppingCooldown"/> so that bots
    /// which just shopped together don't queue up at the merchant together again.
    /// </summary>
    /// <returns>The time of the next supply check.</returns>
    private static DateTime NextShoppingCheckUtc()
    {
        var spread = 1 + (((Rand.NextDouble() * 2) - 1) * ShoppingSpread);
        return DateTime.UtcNow + (ShoppingCooldown * spread);
    }

    /// <summary>
    /// Returns the time of the next buff consideration, spread around <see cref="BuffCooldown"/> so that bots
    /// which just went to the buff NPC together don't line up there again.
    /// </summary>
    /// <returns>The time of the next buff consideration.</returns>
    private static DateTime NextBuffCheckUtc()
    {
        var spread = 1 + (((Rand.NextDouble() * 2) - 1) * BuffSpread);
        return DateTime.UtcNow + (BuffCooldown * spread);
    }

    private static int GroundDistance(MonsterSpawnArea area, Point from)
    {
        var center = GroundCenter(area);
        return Math.Abs(from.X - center.X) + Math.Abs(from.Y - center.Y);
    }

    private static Point GroundCenter(MonsterSpawnArea area)
        => new((byte)((area.X1 + area.X2) / 2), (byte)((area.Y1 + area.Y2) / 2));

    private static int GateDistance(Gate gate, Point target)
    {
        var centerX = (gate.X1 + gate.X2) / 2;
        var centerY = (gate.Y1 + gate.Y2) / 2;
        return Math.Abs(target.X - centerX) + Math.Abs(target.Y - centerY);
    }

    /// <summary>
    /// Picks an index into a list ranked best-first, each rank taking
    /// <see cref="BestCandidateShare"/> of what the ranks before it left over. Deliberately not a
    /// uniform draw - a bot should prefer the best ground it can hunt, just not to the exclusion of
    /// everything else.
    /// </summary>
    /// <param name="count">The number of ranked candidates.</param>
    private static int PickByRank(int count)
    {
        for (var rank = 0; rank < count - 1; rank++)
        {
            if (Rand.NextDouble() < BestCandidateShare)
            {
                return rank;
            }
        }

        return count - 1;
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
            if (!this.TryBeginEvaluation(this._timeProvider.GetUtcNow()))
            {
                return;
            }

            await this.EvaluateAsync(cancellationToken).ConfigureAwait(false);
            this._player.OnAiTickSucceeded();
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
        catch (Exception ex)
        {
            this._player.Logger.LogError(ex, "Bot navigator error for {Account}.", this._player.AccountLoginName);
            this._player.OnAiTickFailed();
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
            || this._player.CurrentMap is not { } map
            || this._player.SelectedCharacter is null)
        {
            return;
        }

        // Inside a mini-game event the whole normal routine below is suspended - the event
        // dictates the pace, and every branch that could move the bot off the event map
        // (shopping, revenge, warping, ground picking) must not run.
        if (this._player.CurrentMiniGame is not null)
        {
            await this.EvaluateInsideMiniGameAsync(map, cancellationToken).ConfigureAwait(false);
            return;
        }

        // Party bookkeeping: answer a pending invitation from a player once its human-like delay
        // passed (see BotPartyHandler).
        await BotPartyHandler.ProcessAsync(this._player).ConfigureAwait(false);

        // Make sure the bot always carries healing and mana potions. Without them, the offline handlers
        // have nothing to drink: the bot dies to sustained damage, and casters degrade to weak melee
        // once their mana runs dry. This also tops up already-generated bots at runtime.
        // Queued into the MuHelper tick: an ammunition refill lands in an equipped hand slot, which
        // mounts item power-ups into the attribute system - and any attribute mutation from this
        // (separate) timer racing the combat handler's recalculations can corrupt the attribute
        // graph permanently (see PendingBotActions).
        this._player.PendingBotActions.Enqueue(() => this.EnsurePotionsAsync());

        // Periodically evaluate looted gear and equip upgrades (drops the replaced piece), so the bot's
        // equipment progresses over its lifetime like a real player's. Queued for the same reason:
        // equipping mounts/unmounts item power-ups.
        if (DateTime.UtcNow >= this._nextEquipCheckUtc)
        {
            this._nextEquipCheckUtc = DateTime.UtcNow + EquipCheckInterval;
            this._player.PendingBotActions.Enqueue(() => BotEquipmentHandler.TryEquipUpgradesAsync(this._player));

            // Looted skill orbs and scrolls are consumed into new skills like a human would (see
            // BotSkillHandler). Queued for the same reason: learning mutates the skill list the combat
            // handler may be enumerating on its own timer.
            this._player.PendingBotActions.Enqueue(() => BotSkillHandler.TryLearnSkillsAsync(this._player));

            // Wings don't drop, so the loot-driven equipment progression above never provides them;
            // they are earned at the classic level milestones instead (see BotWingHandler). Queued
            // for the same reason: equipping mount item power-ups.
            this._player.PendingBotActions.Enqueue(() => BotWingHandler.TryAdvanceWingsAsync(this._player));
        }

        // Keep the combat centre on the bot's current position, so the combat handler always engages
        // monsters right next to the bot - both at the hunting ground and while travelling through hostile
        // territory (self-defence). This is what keeps a travelling bot alive. The travel destination is
        // tracked separately in _destination.
        var inSafezone = map.Terrain.SafezoneMap[this._player.Position.X, this._player.Position.Y];

        // Resolved here (not just before the follow branch) so the tether below holds on every tick,
        // including ticks that return early while walking or on an errand.
        var leaderToFollow = this.GetPartyLeaderToFollow();
        var isTetheredToLeader = leaderToFollow is not null
            && ReferenceEquals(leaderToFollow.CurrentMap, map)
            && !inSafezone;

        // Followers hunt around their leader and never pick an independent ground. Dropping a claim
        // from before the bot joined the party also keeps it from blocking other bots.
        if (leaderToFollow is not null)
        {
            this._player.HuntingSpot = null;
        }

        // Combat centre: normally the bot's own position (self-defence while travelling and at the hunting
        // ground). A follower on the leader's map hunts tethered to the leader instead, so the combat
        // handler only ever picks targets near him - this holds continuously, not just on idle ticks.
        // While inside the safezone we aim it at the destination, so the combat handler does NOT
        // make the bot stand in town swinging at monsters just outside (which it can't damage from a safezone) -
        // it should simply walk out of town instead.
        this._player.HuntingOrigin = isTetheredToLeader
            ? leaderToFollow!.Position
            : inSafezone && this._hasDestination ? this._destination : this._player.Position;

        // Watchdog bookkeeping: remember when the bot last actually changed position.
        if (this._player.Position != this._lastPosition)
        {
            this._lastPosition = this._player.Position;
            this._lastMoveUtc = DateTime.UtcNow;
        }

        // Only monsters the bot would actually fight count (the combat AI ignores ones failing its
        // damage-based safety check), so a too-strong monster next to the bot neither stops its travel
        // nor suppresses the stuck watchdog.
        var monstersNearby = map.GetAttackablesInRange(this._player.Position, TravelStopRange)
            .OfType<Monster>()
            .Count(m => m.IsAlive && !m.IsAtSafezone() && m.SummonedBy is null && CombatHandler.IsSafeTarget(this._player, m.Definition));

        // The bot counts as stuck when it has been frozen on the spot: quickly when there is nothing to
        // fight (a wedged walk or blocked path), and after a much longer grace when huntable monsters are
        // around - standing still next to monsters usually just means fighting, but if the position stays
        // frozen well beyond that, something is wedged (e.g. a monster in range that cannot be reached).
        var stuckTimeout = monstersNearby > 0 ? StuckWithMonstersTimeout : StuckTimeout;
        var stuck = DateTime.UtcNow - this._lastMoveUtc > stuckTimeout;
        if (stuck)
        {
            // Break free: pick a fresh hunting ground and walk to it now. Issuing the walk resets the wedged
            // walker, and a new destination avoids re-stalling on the same blocked path. A follower
            // regroups on its leader instead of roaming to a random ground - the leader decides where
            // the party hunts.
            this._lastMoveUtc = DateTime.UtcNow;
            this._emptyGroundSince = null;
            if (leaderToFollow is not null && ReferenceEquals(leaderToFollow.CurrentMap, map))
            {
                await this.TravelTowardAsync(map, leaderToFollow.Position, cancellationToken).ConfigureAwait(false);
                return;
            }

            await this.PickGroundAndTravelAsync(map, cancellationToken).ConfigureAwait(false);
            return;
        }

        // A walk (travel hop or local combat move) is already in progress.
        if (this._player.IsWalking)
        {
            return;
        }

        // On servers with the reset feature, a due character reset outranks the whole routine below:
        // the bot has reached the required level, so hunting on only gains it nothing until it resets.
        // It keeps fighting normally through the random grace delay, then resets and starts its next
        // cycle (see BotResetHandler).
        if (await this.TryResetCharacterAsync().ConfigureAwait(false))
        {
            return;
        }

        // A master bot invests freshly earned master points (one per master level). Queued into the
        // MuHelper tick like the level-up progression - learning a skill mutates the skill list, which
        // must never happen while the combat handler enumerates it (see PendingBotActions). The master
        // evolution itself is handled by the feature plugin's maintenance pass because it restarts the bot.
        if (BotMasterHandler.HasMasterPointsToSpend(this._player))
        {
            this._player.PendingBotActions.Enqueue(() => BotMasterHandler.TrySpendMasterPointsAsync(this._player));
        }

        // A shopping trip (selling junk loot, restocking potions with Zen) runs before everything else,
        // so it completes even for party members - the follow logic below would otherwise pull them
        // back to the leader mid-trade.
        if (await this.TryShoppingAsync(map, inSafezone, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // A buff trip (claiming the free defense/damage buff a dialog NPC like the Elf Soldier offers)
        // is an errand like shopping - it also completes even for party members, and does not mix with
        // the follow logic below pulling the bot back to the leader mid-visit.
        if (await this.TryBuffTripAsync(map, inSafezone, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // A chaos machine trip (upgrading a lucky +9..+14 piece at the goblin) is an errand like the
        // others: it completes even for party members.
        if (await this.TryChaosMachineTripAsync(map, inSafezone, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // Armed revenge (a player killed this bot; it respawned on the same map) overrides the
        // whole normal routine below, including following the party leader: the bot marches back to
        // the place of its death. A leader on a revenge march still drags its followers along - the
        // posse a wronged player would bring. Once the revenge is spent or times out, the bot falls
        // back into the routine (and a follower rejoins its group).
        if (await this.TryRevengeMarchAsync(map, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // Party members follow their leader instead of roaming on their own: to the leader's map when
        // it warped away (see TryHandleCrossMapLeaderFollowAsync), and around him while on the same map (see
        // EvaluateFollowerHuntingAsync - hunt within range while there is something to fight, regroup
        // into formation when idle). Followers never roam to independent hunting grounds, warp on
        // their own, or step down barren maps - the leader (the lowest-level member, so it only ever
        // picks maps the whole group can hunt on) decides where the party goes. Trade-off, by design:
        // if the leader idles in town or is wedged, its followers idle with him.
        if (leaderToFollow is not null)
        {
            if (await this.TryHandleCrossMapLeaderFollowAsync(map, leaderToFollow).ConfigureAwait(false))
            {
                return;
            }

            await this.EvaluateFollowerHuntingAsync(map, leaderToFollow, inSafezone, cancellationToken).ConfigureAwait(false);
            return;
        }

        // A bot whose map is no longer the best it could safely hunt warps away with priority - even
        // though trivial monsters are still around. Without this, the nearest-monster homing always
        // finds SOMETHING huntable on the starter map, the warp logic below is never reached, and
        // high-level bots farm level-5 mobs on Lorencia forever (with the experience penalty) instead
        // of moving on to their level's maps. TryWarpToBetterMapAsync itself only fires when another
        // map is meaningfully better (margin + cooldown), so a bot on an adequate map stays put.
        if (leaderToFollow is null
            && await this.TryWarpToBetterMapAsync().ConfigureAwait(false))
        {
            return;
        }

        // Inside the safezone the bot can't fight anyway, so it must keep walking out of town instead of
        // freezing at the gate when a monster is just outside. Only stop to fight once it has left the safezone.
        if (!inSafezone && monstersNearby > 0)
        {
            this._emptyGroundSince = null;
            return;
        }

        // Home in on a nearby live monster instead of walking to a random tile in a map-sized spawn area
        // (where the bot would almost never arrive within combat range of an actual monster). This is what
        // makes the bot converge on real monsters and actually fight. The bot walks toward the monster;
        // once within the combat range, the branch above takes over and the combat handler engages. This
        // also runs inside the safezone, so a bot in town heads straight for the monsters just outside the
        // gate instead of a random far-away spawn point. Only when nothing is loaded within the seek radius
        // (deep in town, or all nearby monsters too strong) does it fall back to the coarse spawn-area /
        // warp logic below to relocate.
        if (this.TryFindNearestMonsterGround(map, out var monsterGround))
        {
            this._destination = monsterGround;
            this._hasDestination = true;
            if (await this.TravelTowardAsync(map, monsterGround, cancellationToken).ConfigureAwait(false))
            {
                this._emptyGroundSince = null;
                return;
            }

            // Unreachable monster (e.g. across a river): drop it and fall through to pick a spawn-area ground
            // or warp, instead of re-targeting the same unreachable monster every tick.
            this._hasDestination = false;
        }

        // Nothing to fight here. If we still have a destination to reach, keep walking towards it. If the
        // route turns out to be impossible (e.g. across a river), drop it and fall through to pick another
        // ground in this same tick instead of standing still.
        if (this._hasDestination && this._player.GetDistanceTo(this._destination) > HuntingRange)
        {
            if (!await this.TravelTowardAsync(map, this._destination, cancellationToken).ConfigureAwait(false))
            {
                // Impossible route: drop the destination and wait out the empty-ground grace before
                // picking another one. Re-picking immediately turned a pocket of unreachable picks
                // into a full-tick churn - a new far-away ground every second, hammering the path
                // finder without the bot ever moving.
                this._hasDestination = false;
                this._emptyGroundSince ??= DateTime.UtcNow;
            }

            return;
        }

        // Arrived (or no destination) and the area is empty: wait out a short grace, then pick a new ground.
        // In the safezone we skip the grace so a freshly spawned bot heads out of town straight away.
        this._hasDestination = false;
        if (!inSafezone)
        {
            this._emptyGroundSince ??= DateTime.UtcNow;
            if (DateTime.UtcNow - this._emptyGroundSince < EmptyGroundGrace)
            {
                return;
            }
        }

        this._emptyGroundSince = null;

        if (leaderToFollow is null && await this.TryWarpToBetterMapAsync().ConfigureAwait(false))
        {
            return;
        }

        await this.PickGroundAndTravelAsync(map, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Drives a shopping trip: when the backpack fills up or potions run low, the bot heads to a town
    /// merchant (warping to the map's safezone first when out in the field, like using a town portal),
    /// walks up to it and trades - selling junk loot and restocking potions with Zen.
    /// </summary>
    /// <returns>True, if the shopping trip consumed this tick.</returns>
    private async ValueTask<bool> TryShoppingAsync(GameMap map, bool inSafezone, CancellationToken cancellationToken)
    {
        if (this._shoppingTarget is { } target)
        {
            if (this._player.GetDistanceTo(target) > MerchantTalkRange)
            {
                if (!await this.TravelTowardAsync(map, target, cancellationToken).ConfigureAwait(false))
                {
                    // No way to the merchant from here - give up this trip and don't retry every
                    // check interval (an unreachable merchant stays unreachable for a while; a bot
                    // retrying twice a minute wastes its hunting time on futile marches).
                    this._player.Logger.LogDebug("Bot '{Name}' gives up its shopping trip: no route to the merchant at {Target}.", this._player.Name, target);
                    this._shoppingTarget = null;
                    this._player.IsOnShoppingTrip = false;
                    this._nextShoppingCheckUtc = NextShoppingCheckUtc();
                }

                return true;
            }

            // The trade itself runs in the MU Helper tick, not here: selling and buying mutate the
            // inventory and the Zen of a character whose combat tick may be running at this very moment
            // (the open NPC dialog only holds off the NEXT tick) - and both write through the same,
            // not-thread-safe persistence context. The same rule as for every other bot-initiated mutation,
            // see PendingBotActions. The jewels are spent right after the trade, in the same tick.
            this._player.PendingBotActions.Enqueue(() => this.TradeAndUpgradeAsync(map, target));

            this._shoppingTarget = null;
            this._player.IsOnShoppingTrip = false;
            this._nextShoppingCheckUtc = NextShoppingCheckUtc();
            this._lastMoveUtc = DateTime.UtcNow; // standing at the shop is not "stuck"
            return true;
        }

        if (DateTime.UtcNow < this._nextShoppingCheckUtc)
        {
            return false;
        }

        this._nextShoppingCheckUtc = DateTime.UtcNow + ShoppingCheckInterval;
        if (!BotShoppingHandler.NeedsShopping(this._player))
        {
            return false;
        }

        if (BotShoppingHandler.FindMerchantPosition(this._player, map) is not { } merchantPosition)
        {
            // No merchant lives on this map at all (the Dungeon has no town). Shopping here would
            // starve the bot's logistics for as long as it stays - no potion restock, no selling, a
            // slowly silting backpack. Like a player pulling a town scroll, it warps to its class
            // hometown and shops there; the better-map logic takes it back hunting afterward.
            if (this.TryGetHomeEscapeGate(out var homeGate, out var homeMap, out _))
            {
                this._travelPath = null;
                this._hasDestination = false;
                this._lastWarpUtc = this._timeProvider.GetUtcNow();
                this._nextShoppingCheckUtc = DateTime.UtcNow; // start the trip on the new map right away
                this._player.Logger.LogDebug("Bot '{Name}' warps home to {Map} for a shopping trip - no merchant on its map.", this._player.Name, homeMap.Name);
                await this._player.WarpToAsync(homeGate).ConfigureAwait(false);
                return true;
            }

            this._nextShoppingCheckUtc = NextShoppingCheckUtc();
            return false;
        }

        // Merchants stand in town: when out in the field, warp to the map's safezone first (like using
        // a town portal scroll), then walk over to the merchant.
        if (!inSafezone
            && map.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is { } townGate)
        {
            this._travelPath = null;
            this._hasDestination = false;
            await this._player.WarpToAsync(townGate).ConfigureAwait(false);
        }

        this._shoppingTarget = merchantPosition;
        this._player.IsOnShoppingTrip = true;
        this._player.Logger.LogDebug("Bot '{Name}' heads to the merchant for a shopping trip.", this._player.Name);
        return true;
    }

    /// <summary>
    /// The trade at the merchant, as it runs inside the MU Helper tick: sell the junk, restock the
    /// potions and - still standing safely at the shop with the dialog closed - spend a few of the
    /// looted jewels on the own gear, the player-like moment for it (see <see cref="BotJewelHandler"/>).
    /// </summary>
    private async ValueTask TradeAndUpgradeAsync(GameMap map, Point merchantPosition)
    {
        if (await BotShoppingHandler.TryTradeAsync(this._player, map, merchantPosition).ConfigureAwait(false))
        {
            await BotJewelHandler.TryUpgradeGearAsync(this._player).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Drives a buff trip: when the server has a dialog NPC offering a buff the bot qualifies for (the
    /// Elf Soldier grants fresh characters a defense and a damage buff, but exists only in Season Six
    /// data), the bot heads to it - warping to the map's safezone first when out in the field, like
    /// using a town portal - walks up to it and claims the buff like a real player would. Without data
    /// to configure the NPC the check simply never fires, so this is a no-op on earlier seasons.
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="inSafezone">Indicates whether the bot currently is in the safezone of the map.</param>
    /// <param name="cancellationToken">The token which aborts the travel wait on shutdown.</param>
    /// <returns>True, if the buff trip consumed this tick.</returns>
    private async ValueTask<bool> TryBuffTripAsync(GameMap map, bool inSafezone, CancellationToken cancellationToken)
    {
        if (this._buffTarget is { } target)
        {
            if (this._player.GetDistanceTo(target) > MerchantTalkRange)
            {
                if (!await this.TravelTowardAsync(map, target, cancellationToken).ConfigureAwait(false))
                {
                    // No way to the buff NPC from here - give up this trip and don't retry every
                    // check interval: an unreachable NPC stays unreachable for a while, and a bot
                    // retrying twice a minute wastes its hunting time on futile marches.
                    this._player.Logger.LogDebug("Bot '{Name}' gives up its buff trip: no route to the buff NPC at {Target}.", this._player.Name, target);
                    this._buffTarget = null;
                    this._player.IsOnBuffTrip = false;
                    this._nextBuffCheckUtc = NextBuffCheckUtc();
                }

                return true;
            }

            // The dialog itself runs in the MU Helper tick, not here: it pauses the combat AI on the
            // next tick and applies the effect to the character, a player-state mutation which must run
            // where all other bot mutations run (see PendingBotActions).
            this._player.PendingBotActions.Enqueue(() => this.ClaimBuffAsync(map, target));

            this._buffTarget = null;
            this._player.IsOnBuffTrip = false;
            this._nextBuffCheckUtc = NextBuffCheckUtc();
            this._lastMoveUtc = DateTime.UtcNow; // standing at the buff NPC is not "stuck"
            return true;
        }

        if (DateTime.UtcNow < this._nextBuffCheckUtc)
        {
            return false;
        }

        this._nextBuffCheckUtc = DateTime.UtcNow + BuffCheckInterval;
        if (BotBuffHandler.TryGetApplicableBuff(this._player) is null)
        {
            // No buff dialog NPC configured (data without an Elf Soldier) or the bot does not qualify
            // (already buffed, or outside the buff's level range): a no-op on such servers.
            return false;
        }

        if (BotBuffHandler.FindBuffNpcPosition(this._player, map) is not { } buffNpcPosition)
        {
            // No buff NPC lives on this map at all. Like a player pulling a town scroll, the bot warps
            // to its class hometown, where the buff is offered; the better-map logic takes it back
            // hunting afterward.
            if (this.TryGetHomeEscapeGate(out var homeGate, out var homeMap, out _))
            {
                this._travelPath = null;
                this._hasDestination = false;
                this._lastWarpUtc = this._timeProvider.GetUtcNow();
                this._nextBuffCheckUtc = DateTime.UtcNow; // start the trip on the new map right away
                this._player.Logger.LogDebug("Bot '{Name}' warps home to {Map} for its buff - no buff NPC on its map.", this._player.Name, homeMap.Name);
                await this._player.WarpToAsync(homeGate).ConfigureAwait(false);
                return true;
            }

            this._nextBuffCheckUtc = NextBuffCheckUtc();
            return false;
        }

        // Buff NPCs stand in town (in or near the safezone): when out in the field, warp to the map's
        // safezone first (like using a town portal scroll), then walk over to the NPC.
        if (!inSafezone
            && map.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is { } townGate)
        {
            this._travelPath = null;
            this._hasDestination = false;
            await this._player.WarpToAsync(townGate).ConfigureAwait(false);
        }

        this._buffTarget = buffNpcPosition;
        this._player.IsOnBuffTrip = true;
        this._player.Logger.LogDebug("Bot '{Name}' heads to the buff NPC for its free buff.", this._player.Name);
        return true;
    }

    /// <summary>
    /// Claims the buff at the buff NPC, as it runs inside the MU Helper tick: open the dialog, request
    /// the buff, close the dialog - the player-like visit, see <see cref="BotBuffHandler.TryRequestBuffAsync"/>.
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="buffNpcPosition">The position of the buff NPC.</param>
    private async ValueTask ClaimBuffAsync(GameMap map, Point buffNpcPosition)
    {
        await BotBuffHandler.TryRequestBuffAsync(this._player, map, buffNpcPosition).ConfigureAwait(false);
    }

    /// <summary>
    /// Drives a chaos machine trip: when the bot carries what an upgrade of a worn lucky piece needs (see
    /// <see cref="BotChaosMachineHandler.PlanNextMix"/>), it heads to the goblin - warping to a map with
    /// one first, through the regular warp action with its fare, when its own map has none (in Season Six
    /// only Noria does) - walks up to it and mixes. The mix itself runs in the MU Helper tick, like the
    /// trades and buffs (see <see cref="OfflinePlayer.PendingBotActions"/>).
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="inSafezone">Indicates whether the bot currently is in the safezone of the map.</param>
    /// <param name="cancellationToken">The token which aborts the travel wait on shutdown.</param>
    /// <returns>True, if the trip consumed this tick.</returns>
    private async ValueTask<bool> TryChaosMachineTripAsync(GameMap map, bool inSafezone, CancellationToken cancellationToken)
    {
        if (this._chaosMachineTarget is { } target)
        {
            if (this._player.GetDistanceTo(target) > MerchantTalkRange)
            {
                if (!await this.TravelTowardAsync(map, this._chaosMachineStandingPoint, cancellationToken).ConfigureAwait(false))
                {
                    this._player.Logger.LogDebug("Bot '{Name}' gives up its chaos machine trip: no route to the goblin at {Target}.", this._player.Name, target);
                    this._chaosMachineTarget = null;
                    this._player.IsOnChaosMachineTrip = false;
                    this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCooldown;
                }

                return true;
            }

            this._player.PendingBotActions.Enqueue(() => this.MixAtChaosMachineAsync(map, target));
            this._chaosMachineTarget = null;
            this._player.IsOnChaosMachineTrip = false;
            this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCheckInterval;
            this._lastMoveUtc = DateTime.UtcNow; // standing at the goblin is not "stuck"
            return true;
        }

        if (DateTime.UtcNow < this._nextChaosMachineCheckUtc)
        {
            return false;
        }

        this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCheckInterval;
        if (BotChaosMachineHandler.GetChaosMachine(this._player.GameContext.Configuration) is not { } goblin
            || BotChaosMachineHandler.PlanNextMix(this._player, goblin) is not { } plan)
        {
            return false;
        }

        if (BotChaosMachineHandler.FindChaosMachinePosition(map) is not { } goblinPosition)
        {
            if (this._timeProvider.GetUtcNow() - this._lastWarpUtc < WarpCooldown)
            {
                // Just warped (e.g. to its hunting map): wait, rather than bouncing between maps.
                return false;
            }

            if (this.TryGetChaosMachineWarp(plan.Price, out var warp))
            {
                this._travelPath = null;
                this._hasDestination = false;
                this._player.HuntingSpot = null;
                this._lastWarpUtc = this._timeProvider.GetUtcNow();
                this._nextChaosMachineCheckUtc = DateTime.UtcNow; // start the trip on the new map right away
                this._player.Logger.LogDebug("Bot '{Name}' warps to {Map} for the chaos machine.", this._player.Name, warp.Gate?.Map?.Name);
                await WarpAction.WarpToAsync(this._player, warp).ConfigureAwait(false);
                return true;
            }

            this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCooldown;
            return false;
        }

        if (BotChaosMachineHandler.FindStandingPoint(map.Terrain, goblinPosition, MerchantTalkRange) is not { } standingPoint)
        {
            this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCooldown;
            return false;
        }

        // The goblin stands in town: when out in the field, warp to the map's safezone first.
        if (!inSafezone
            && map.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is { } townGate)
        {
            this._travelPath = null;
            this._hasDestination = false;
            await this._player.WarpToAsync(townGate).ConfigureAwait(false);
        }

        // The bot leaves its hunting ground for the trip; its claim would keep other bots off it meanwhile.
        this._player.HuntingSpot = null;
        this._chaosMachineTarget = goblinPosition;
        this._chaosMachineStandingPoint = standingPoint;
        this._player.IsOnChaosMachineTrip = true;
        this._player.Logger.LogDebug("Bot '{Name}' heads to the chaos machine.", this._player.Name);
        return true;
    }

    /// <summary>
    /// Mixes at the chaos machine, as it runs inside the MU Helper tick (see <see cref="BotChaosMachineHandler.TryMixAsync"/>).
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="goblinPosition">The position of the chaos machine NPC.</param>
    private async ValueTask MixAtChaosMachineAsync(GameMap map, Point goblinPosition)
    {
        if (!await BotChaosMachineHandler.TryMixAsync(this._player, map, goblinPosition).ConfigureAwait(false))
        {
            // A refused mix (e.g. the castle tax on top of the price) would fail the same way in 30 s.
            this._nextChaosMachineCheckUtc = DateTime.UtcNow + ChaosMachineCooldown;
        }
    }

    /// <summary>
    /// Gets a warp the bot may take to a map where a chaos machine NPC stands, and can afford while
    /// keeping the price of the mix - otherwise it arrives without the Zen to mix.
    /// </summary>
    /// <param name="mixPrice">The price of the planned mix.</param>
    /// <param name="warp">The warp.</param>
    /// <returns>True, if there is one.</returns>
    private bool TryGetChaosMachineWarp(long mixPrice, [MaybeNullWhen(false)] out WarpInfo warp)
    {
        warp = this._player.GameContext.Configuration.Maps
            .Where(m => m.MonsterSpawns.Any(s => s.MonsterDefinition is { } npc && BotChaosMachineHandler.IsChaosMachine(npc)))
            .SelectMany(this.GetLegalWarps)
            .Where(w => this._player.Money >= w.Costs + mixPrice)
            .OrderBy(w => w.Costs)
            .FirstOrDefault();
        return warp is not null;
    }

    /// <summary>
    /// Marches the bot back to the place of its death while revenge is armed (see
    /// <see cref="OfflinePlayer.TryGetRevengeDestination"/>). The single attempt is spent as soon as
    /// the bot arrives or runs into its killer on the way - no extra combat logic is needed here,
    /// because the re-armed aggressor memory already keeps the combat handler prioritizing the
    /// killer, struck only when legal (see <see cref="BotPvpRules"/>).
    /// Monsters along the route do not stop the march (self-defense still engages ones right
    /// next to the bot); a bot that stopped to farm would never arrive within the revenge time.
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="cancellationToken">The token which aborts the travel wait on shutdown.</param>
    /// <returns>True, if the revenge march consumed this tick.</returns>
    private async ValueTask<bool> TryRevengeMarchAsync(GameMap map, CancellationToken cancellationToken)
    {
        if (!this._player.TryGetRevengeDestination(map.Definition, out var deathSite))
        {
            return false;
        }

        if (this._player.RecentAggressor is { } aggressor
            && ReferenceEquals(aggressor.CurrentMap, map)
            && this._player.GetDistanceTo(aggressor.Position) <= TravelStopRange)
        {
            // Ran into the killer before reaching the site - that is who we came for. Stop travelling
            // and let the combat handler (which already prioritizes the aggressor) take over.
            this._player.ExpireRevenge("the bot caught up with its killer");
            return true;
        }

        if (this._player.GetDistanceTo(deathSite) <= HuntingRange)
        {
            // Arrived. If the killer still stands here, the armed aggressor memory makes the combat
            // handler engage; otherwise the bot simply hunts on normally from this spot.
            this._player.ExpireRevenge("the bot arrived at the death site");
            return false;
        }

        this._destination = deathSite;
        this._hasDestination = true;
        if (!await this.TravelTowardAsync(map, deathSite, cancellationToken).ConfigureAwait(false))
        {
            // No walkable route back (e.g. the respawn gate is across a river) - drop the attempt
            // instead of re-issuing an impossible walk every tick.
            this._player.ExpireRevenge("no route back to the death site");
            return false;
        }

        return true;
    }

    /// <summary>
    /// The bot's routine while it takes part in a mini-game event (Blood Castle, Devil Square,
    /// Chaos Castle): fight whatever the event throws at it, keep up with the party leader on the
    /// way through the course, and close in on the action when it moved away - nothing else. Death
    /// and the event's end need no handling here: the engine respawns a dead bot at the map's
    /// safezone like a player (which removes it from the event), and the event warps the remaining
    /// participants out when it ends.
    /// </summary>
    private async ValueTask EvaluateInsideMiniGameAsync(GameMap map, CancellationToken cancellationToken)
    {
        // Keep the drinking supplies topped up - an event without potions ends quickly.
        this._player.PendingBotActions.Enqueue(() => this.EnsurePotionsAsync());

        // Fight from wherever the bot stands; the combat handler engages the targets around it.
        this._player.HuntingOrigin = this._player.Position;

        if (this._player.IsWalking)
        {
            return;
        }

        // Stay with the leader on the way through the event's course (the Blood Castle bridge) ...
        if (this.GetPartyLeaderToFollow() is { } leader
            && ReferenceEquals(leader.CurrentMap, map)
            && this._player.GetDistanceTo(leader.Position) > FollowDistance)
        {
            await this.TravelTowardAsync(map, leader.Position, cancellationToken).ConfigureAwait(false);
            return;
        }

        // ... and otherwise close in on the nearest opposition instead of idling at the entrance
        // when the wave spawned (or the Chaos Castle crowd moved) beyond the combat range.
        if (!map.GetAttackablesInRange(this._player.Position, TravelStopRange).Any(this.IsEventTarget)
            && this.TryFindNearestEventTargetGround(map, out var ground))
        {
            await this.TravelTowardAsync(map, ground, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Whether the object is something this bot would fight inside a mini-game event. Unlike in the
    /// open world, there is no safe-level filtering - the event's level bracket already matched the
    /// bot, and the event dictates the opposition. Players count as targets only where the event
    /// allows player killing (Chaos Castle, see <see cref="BotPvpRules"/>).
    /// </summary>
    private bool IsEventTarget(IAttackable attackable)
    {
        if (ReferenceEquals(attackable, this._player)
            || !attackable.IsAlive
            || attackable.IsAtSafezone())
        {
            return false;
        }

        return attackable switch
        {
            Monster monster => monster.SummonedBy is null && monster.Definition.ObjectKind == NpcObjectKind.Monster,
            Player player => BotPvpRules.IsLegalPvpTarget(this._player, player),
            _ => false,
        };
    }

    /// <summary>
    /// Finds the spot of a nearby opponent inside a mini-game event, mirroring
    /// <see cref="TryFindNearestMonsterGround"/> (random among the two nearest, so many bots
    /// don't dogpile the same target).
    /// </summary>
    private bool TryFindNearestEventTargetGround(GameMap map, out Point ground)
    {
        ground = default;
        var position = this._player.Position;
        var candidates = map.GetAttackablesInRange(position, MonsterSeekRadius)
            .Where(this.IsEventTarget)
            .OrderBy(a => a.GetDistanceTo(position))
            .Take(2)
            .ToList();
        if (candidates.Count == 0)
        {
            return false;
        }

        ground = candidates.SelectRandom()!.Position;
        return true;
    }

    /// <summary>
    /// Gets the party leader this bot should follow, or null if the bot is solo, is the leader itself,
    /// or the leader is currently not followable (dead, no map, or inside another mini-game instance -
    /// following into an event goes through the regular entry of <see cref="BotMiniGameHandler"/>,
    /// never through a warp).
    /// </summary>
    private Player? GetPartyLeaderToFollow()
    {
        if (this._player.Party is not { } party
            || party.PartyMaster is not Player leader
            || ReferenceEquals(leader, this._player)
            || !leader.IsAlive
            || leader.CurrentMap is null
            || !ReferenceEquals(leader.CurrentMiniGame, this._player.CurrentMiniGame))
        {
            return null;
        }

        return leader;
    }

    /// <summary>
    /// Performs the bot's character reset once it is due (reset feature enabled, required level
    /// reached, reset limit not exhausted). The reset is not executed the moment the bot becomes
    /// eligible: a random grace delay (<see cref="MinResetDelay"/>..<see cref="MaxResetDelay"/>) is
    /// rolled first, during which the bot hunts on normally. After the reset, the navigation state is
    /// cleared, so the bot starts its next cycle fresh - re-investing its points (queued by the reset
    /// handler) and heading for a hunting ground that suits it again.
    /// </summary>
    /// <returns>True, if a reset was performed.</returns>
    private async ValueTask<bool> TryResetCharacterAsync()
    {
        if (BotPartyHandler.HasHumanCompanion(this._player))
        {
            // Not in the middle of a hunting session with a player: the reset would wipe the bot's
            // strength and teleport it home, deserting the group. It resets once the party with the
            // human companion ends.
            return false;
        }

        if (BotResetHandler.GetResetConfiguration(this._player.GameContext) is not { } resetConfiguration
            || !BotResetHandler.IsResetDue(this._player, resetConfiguration))
        {
            this._resetDueAtUtc = null;
            return false;
        }

        if (this._resetDueAtUtc is not { } dueAt)
        {
            var delay = MinResetDelay + TimeSpan.FromSeconds(Rand.NextInt(0, (int)(MaxResetDelay - MinResetDelay).TotalSeconds + 1));
            this._resetDueAtUtc = DateTime.UtcNow + delay;
            this._player.Logger.LogDebug(
                "Bot {Character} reached the reset level; resetting in {Delay}.",
                this._player.Name,
                delay);
            return false;
        }

        if (DateTime.UtcNow < dueAt)
        {
            return false;
        }

        this._resetDueAtUtc = null;
        var payCosts = this._player.GameContext.FeaturePlugIns.GetPlugIn<BotFeaturePlugIn>()?.Configuration?.BotsPayResetCosts == true;

        // Queued into the MuHelper tick: the reset writes the level/reset/stat attributes, and such
        // mutations must not race the combat handler's recalculations (see PendingBotActions). The
        // queued call re-validates the eligibility itself; if it refuses (e.g. unaffordable costs
        // when BotsPayResetCosts is enabled), the check above re-schedules a fresh grace delay.
        var player = this._player;
        player.PendingBotActions.Enqueue(async () =>
        {
            await BotResetHandler.TryResetAsync(player, resetConfiguration, payCosts).ConfigureAwait(false);
        });

        // Start the next cycle fresh: no stale destination or route, and no warp cooldown - a freshly
        // reset player heads straight back out, and the point pool (queued by the reset handler into
        // the bot's AI tick) is invested before the next evaluation gets this far. Cleared
        // optimistically - if the queued reset refuses, the bot merely re-picks its hunting ground.
        this._hasDestination = false;
        this._travelPath = null;
        this._emptyGroundSince = null;
        this._lastWarpUtc = DateTimeOffset.MinValue;
        this._nextFollowWarpSearchUtc = DateTimeOffset.MinValue;
        return true;
    }

    /// <summary>
    /// Warps to the map that offers the strongest monsters this bot can still safely handle, so it
    /// earns the best experience without being slaughtered. It arrives at the destination safezone
    /// and walks out to a hunting ground like a real player.
    /// </summary>
    /// <returns>True, if a warp was performed.</returns>
    private async ValueTask<bool> TryWarpToBetterMapAsync()
    {
        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || this._player.SelectedCharacter is null)
        {
            return false;
        }

        var botLevel = this.GetBotLevel();
        var plainLevel = (int)(this._player.Attributes?[Stats.Level] ?? 1);

        // Two situations force the bot OFF its current map, bypassing the usual minimum warp level:
        // a map without a single safe monster is hostile territory (aggressive monsters kill the bot
        // while it cannot fight back), and a map the bot could not legally warp to with its plain
        // level is off-limits regardless of its strength - a real character of that level cannot be
        // there, so neither may the bot (e.g. a veteran stranded on a high map after its reset).
        var currentMap = this._player.CurrentMap?.Definition;
        var mapIsHostile = currentMap is not null && this.BestSafeLevel(currentMap) == 0;
        var mapIsIllegal = currentMap is not null
                           && !this.TryGetLegalWarp(currentMap, out _)
                           && currentMap.Number != this._player.SelectedCharacter?.CharacterClass?.HomeMap?.Number;
        var mustEscape = mapIsHostile || mapIsIllegal;

        // A map can pass every check above and still pay the bot nothing: the safe monsters are a rare
        // kind among ones it must refuse, or stronger bots empty the grounds before it arrives. The bot
        // notices the way a player would - it has not landed a hit in minutes - and steps DOWN to easier
        // ground to earn its way back up, instead of walking between hunting grounds forever.
        var mapIsBarren = DateTime.UtcNow - this._player.LastAttackUtc > BarrenMapDuration;

        if ((plainLevel < this.GetMinWarpLevel() && !mustEscape && !mapIsBarren)
            || this._timeProvider.GetUtcNow() - this._lastWarpUtc < WarpCooldown)
        {
            return false;
        }

        // When escaping, any legal map with something safe to hunt beats staying. The home retreat is
        // the last resort AND the safety valve: a bot which cannot afford a single warp - or is below
        // every warp requirement, as a freshly reset veteran is - would otherwise be stranded on ground
        // that pays it nothing, unable to earn the fare out of it. Walking home is free, like a player
        // taking the long way back, and its home map is a starter ground it can always hunt.
        WarpInfo? fare = null;
        ExitGate targetGate;
        GameMapDefinition? targetMap;
        int targetLevel;
        if ((mapIsBarren && this.TryPickEasierMap(out var easierWarp, out targetMap, out targetLevel))
            || this.TryPickBetterMap(mustEscape, out easierWarp, out targetMap, out targetLevel))
        {
            fare = easierWarp;
            targetGate = easierWarp.Gate!;
        }
        else if ((mustEscape || mapIsBarren)
                 && this.TryGetHomeEscapeGate(out var homeGate, out targetMap, out targetLevel))
        {
            targetGate = homeGate;
        }
        else
        {
            return false;
        }

        // The barren timer restarts with the move: the new map deserves the same chance to prove itself
        // before it is judged, and without this every following tick would warp again.
        this._player.LastAttackUtc = DateTime.UtcNow;
        this._lastWarpUtc = this._timeProvider.GetUtcNow();
        this._hasDestination = false;
        this._travelPath = null;
        this._player.HuntingSpot = null;
        this._player.Logger.LogDebug(
            "Bot {Character} (level {Level}) warping to map {Map} (monsters ~{MonsterLevel}, fare {Fare} zen).",
            this._player.Name,
            botLevel,
            targetMap!.Name,
            targetLevel,
            fare?.Costs ?? 0);

        if (fare is not null)
        {
            // Through the player action, so the bot pays the fare and passes every check the game makes
            // of a player warping there - including any this navigator does not know about.
            await WarpAction.WarpToAsync(this._player, fare).ConfigureAwait(false);
        }
        else
        {
            await this._player.WarpToAsync(targetGate).ConfigureAwait(false);
        }

        // Where the character ACTUALLY ended up, not where it was sent: the warp action re-checks what
        // the candidate filter checked and may refuse, and remembering a refused trip as a move would
        // reload the bot onto a map it never reached.
        await this.TryPersistCurrentMapAsync(this._player.CurrentMap?.Definition ?? targetMap).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Walks the next stretch towards the destination. A full route is resolved with a heuristic search
    /// (so it goes around walls and out of the town gate) and the first <see cref="StepsPerHop"/> steps are
    /// issued; the remainder is recomputed from the new position on the next tick. The safe zone is allowed
    /// in the path so the bot can leave town.
    /// </summary>
    private async ValueTask<bool> TravelTowardAsync(GameMap map, Point destination, CancellationToken cancellationToken)
    {
        var position = this._player.Position;

        // Follow the cached route as long as it still leads to (roughly) the same destination and the
        // bot is still on it. Re-planning a whole-map A* every tick just to consume three steps wasted
        // CPU and starved the shared pathfinder pool once dozens of bots traveled at the same time.
        if (this._travelPath is { } cached
            && this._travelPathIndex < cached.Count
            && destination.EuclideanDistanceTo(this._travelPathTarget) <= PathReuseTolerance
            && position.EuclideanDistanceTo(cached[this._travelPathIndex].Point) <= 1.5)
        {
            await this.WalkCachedStepsAsync(position).ConfigureAwait(false);
            return true;
        }

        this._travelPath = null;

        var path = await this.FindTravelPathAsync(map, position, destination, cancellationToken).ConfigureAwait(false);

        if (path is null || path.Count == 0)
        {
            // No route to this ground (e.g. across a river): report it so the navigator picks another one
            // straight away instead of standing still re-issuing an impossible walk.
            return false;
        }

        this._travelPath = path;
        this._travelPathIndex = 0;
        this._travelPathTarget = destination;
        await this.WalkCachedStepsAsync(position).ConfigureAwait(false);
        return true;
    }

    private async ValueTask<IList<PathResultNode>?> FindTravelPathAsync(
        GameMap map,
        Point start,
        Point destination,
        CancellationToken cancellationToken)
    {
        // Observe the navigator's shutdown token while queuing for a shared pathfinder: on disposal the
        // wait is abandoned at once instead of holding the tick until a finder frees up. Cancellation from
        // this wait or FindPath is expected during shutdown and handled by SafeEvaluateAsync.
        await TravelPathFinderPool.WaitAsync(cancellationToken).ConfigureAwait(false);
        PathFinder? finder = null;
        try
        {
            if (!TravelPathFinders.TryTake(out finder))
            {
                finder = CreateTravelPathFinder();
            }

            finder.ResetPathFinder();
            return finder.FindPath(start, destination, map.Terrain.AIgrid, true, cancellationToken);
        }
        finally
        {
            if (finder is not null)
            {
                TravelPathFinders.Add(finder);
            }

            TravelPathFinderPool.Release();
        }
    }

    /// <summary>
    /// Issues the next few steps of the cached route (short hops, so the bot can stop and fight between
    /// them) and advances the route cursor.
    /// </summary>
    private async ValueTask WalkCachedStepsAsync(Point position)
    {
        var path = this._travelPath!;
        var stepsCount = Math.Min(path.Count - this._travelPathIndex, StepsPerHop);
        var steps = new WalkingStep[stepsCount];
        for (var i = 0; i < stepsCount; i++)
        {
            var node = path[this._travelPathIndex + i];
            var previous = i == 0 ? position : steps[i - 1].To;
            steps[i] = new WalkingStep(previous, node.Point, previous.GetDirectionTo(node.Point));
        }

        this._travelPathIndex += stepsCount;
        await this._player.WalkToAsync(steps[stepsCount - 1].To, steps).ConfigureAwait(false);

        // A walk refused by the game (e.g. the bot is frozen) must not trigger a decision on every timer shot.
        this._isHopInProgress = this._player.IsWalking;
    }

    /// <summary>
    /// Ensures the bot keeps a stack of healing and a stack of mana potions in its inventory. Creates the
    /// stacks the first time (covering bots generated before they were granted) and refills them once they
    /// run low, so the bot never goes without a way to heal or to keep casting.
    /// </summary>
    private async ValueTask EnsurePotionsAsync()
    {
        await this.EnsurePotionStackAsync(HealPotionNumber).ConfigureAwait(false);
        await this.EnsurePotionStackAsync(ManaPotionNumber).ConfigureAwait(false);
        await this.EnsureAmmunitionAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Keeps an archer's ammunition topped up. With the starter bow the offline attack path does not
    /// consume arrows, but an upgraded bow can carry an ammunition consumption rate - without this the
    /// bot would eventually shoot its quiver empty and its bow skills would stop working.
    /// </summary>
    private async ValueTask EnsureAmmunitionAsync()
    {
        if (this._player.Inventory is not { } inventory)
        {
            return;
        }

        if (inventory.EquippedAmmunitionItem is { } ammo)
        {
            if (ammo.Durability < HealPotionRefillBelow && ammo.Definition is { } ammoDefinition)
            {
                ammo.Durability = Math.Max((byte)1, ammoDefinition.Durability);
            }

            return;
        }

        // Ammo ran out completely (an empty stack gets destroyed): if a bow is equipped, put a fresh
        // stack of arrows into the other hand.
        var bow = inventory.EquippedItems.FirstOrDefault(i =>
            i.Definition is { Group: 4, IsAmmunition: false });
        if (bow is null)
        {
            return;
        }

        var arrows = this._player.GameContext.Configuration.Items
            .FirstOrDefault(d => d.Group == 4 && d.Number == 15);
        if (arrows is null)
        {
            return;
        }

        var item = this._player.PersistenceContext.CreateNew<Item>();
        item.Definition = arrows;
        item.Durability = Math.Max((byte)1, arrows.Durability);
        var targetSlot = bow.ItemSlot == InventoryConstants.RightHandSlot
            ? InventoryConstants.LeftHandSlot
            : InventoryConstants.RightHandSlot;
        if (!await inventory.AddItemAsync(targetSlot, item).ConfigureAwait(false))
        {
            await this._player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        }
    }

    private async ValueTask EnsurePotionStackAsync(int potionNumber)
    {
        if (this._player.Inventory is not { } inventory
            || this._player.GameContext.Configuration.Items
                .FirstOrDefault(d => d.Group == HealPotionGroup && d.Number == potionNumber) is not { } definition)
        {
            return;
        }

        var potions = inventory.Items
            .Where(i => i.Definition?.Group == HealPotionGroup && i.Definition.Number == potionNumber)
            .ToList();
        var charges = potions.Sum(p => (int)p.Durability);
        if (charges >= HealPotionRefillBelow)
        {
            return;
        }

        // Only ever up to the emergency amount, never a full stack: a Large Healing Potion holds 255
        // charges, four times what a bot stocks at a merchant, so handing out whole stacks here made the
        // fallback the bot's normal potion supply and the shopping trip never had a reason to restock.
        var stackSize = Math.Min(Math.Max((byte)1, definition.Durability), (byte)EmergencyPotionCharges);
        foreach (var potion in potions.Where(p => p.Durability < stackSize))
        {
            charges += (int)(stackSize - potion.Durability);
            potion.Durability = stackSize;
            if (charges >= EmergencyPotionCharges)
            {
                return;
            }
        }

        while (charges < EmergencyPotionCharges)
        {
            var item = this._player.PersistenceContext.CreateNew<Item>();
            item.Definition = definition;
            item.Durability = stackSize;
            if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
            {
                // Backpack full - drop the throw-away item again; what is stocked has to do.
                await this._player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                return;
            }

            charges += stackSize;
        }
    }

    /// <summary>
    /// Re-resolves the warp destination map through the bot's own persistence context and assigns that
    /// instance to the character. The warp itself assigns the GLOBAL configuration instance, which the
    /// bot's context does not track - the CurrentMapId foreign key silently never got saved, so a
    /// restarted bot always woke up on its home map and warped all over again (warp burst on restart).
    /// </summary>
    private async ValueTask TryPersistCurrentMapAsync(GameMapDefinition mapDefinition)
    {
        try
        {
            if (this._player.SelectedCharacter is not { } character)
            {
                return;
            }

            var ownInstance = await this._player.PersistenceContext
                .GetByIdAsync<GameMapDefinition>(mapDefinition.GetId()).ConfigureAwait(false);
            if (ownInstance is not null)
            {
                character.CurrentMap = ownInstance;
            }
        }
        catch (Exception ex)
        {
            // Not worth failing the warp over - worst case the map is (as before) not persisted.
            this._player.Logger.LogDebug(ex, "Could not persist current map for bot {Character}.", this._player.Name);
        }
    }

    /// <summary>
    /// The bot's reset-aware effective level (see <see cref="BotResetHandler.GetEffectiveLevel"/>),
    /// used for hunting decisions (which monsters pay off). Map ACCESS is deliberately not decided
    /// by this - the game gates warps by the plain character level (see <see cref="TryGetLegalWarp"/>),
    /// and the bots must obey the same rule.
    /// </summary>
    private int GetBotLevel() => BotResetHandler.GetEffectiveLevel(this._player);

    /// <summary>
    /// Gets the gate through which the bot may legally warp to the given map, exactly like a player
    /// using the in-game warp window: the map must have an entry in the server's warp list whose level
    /// requirement (class-reduced, see <see cref="CharacterExtensions.GetEffectiveMoveLevelRequirement"/>)
    /// is met by the bot's PLAIN character level. The reset-aware effective level intentionally plays
    /// no role here - a freshly reset veteran cannot warp to high maps either, no matter its strength.
    /// </summary>
    /// <param name="mapDefinition">The target map.</param>
    /// <param name="warp">The warp info, if a legal entry exists.</param>
    /// <returns>True, if the bot may warp to the map.</returns>
    private bool TryGetLegalWarp(GameMapDefinition mapDefinition, [MaybeNullWhen(false)] out WarpInfo warp)
    {
        warp = this.GetLegalWarps(mapDefinition)
            .FirstOrDefault();
        return warp is not null;
    }

    /// <summary>
    /// Gets the legal warp entry whose complete landing area has the shortest walkable route to a target.
    /// </summary>
    /// <param name="map">The target map.</param>
    /// <param name="target">The target position.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The best reachable warp, or <c>null</c> if no legal gate can reach the target.</returns>
    /// <remarks>
    /// A candidate is deliberately rejected when any coordinate which
    /// <see cref="ExitGateExtensions.GetRandomPoint(ExitGate)"/> may select is blocked or cannot reach
    /// the target. <see cref="Player.WarpToAsync(ExitGate)"/> performs that random placement once and
    /// does not retry or validate it, so accepting a partially usable gate would make following
    /// probabilistic. This fail-closed policy can reject a configured gate with many usable coordinates
    /// and one bad coordinate; keeping that trade-off local avoids changing map-transition behavior for
    /// other warp features.
    /// The reverse flood fill can inspect the whole map, whereas the travel pathfinder stops after
    /// <see cref="TravelSearchLimit"/> expanded nodes. A gate whose shortest landing route already exceeds
    /// that budget is therefore rejected before warping.
    /// </remarks>
    private WarpInfo? FindBestReachableLegalWarp(GameMap map, Point target, CancellationToken cancellationToken)
    {
        var candidates = this.GetLegalWarps(map.Definition)
            .Where(candidate => candidate.Gate is not null)
            .Select(candidate => (Warp: candidate, Gate: candidate.Gate!, LandingPoints: candidate.Gate!.GetPossibleLandingPoints().ToArray()))
            .Where(candidate => !candidate.LandingPoints.Contains(this._player.Position))
            .ToArray();
        if (candidates.Length == 0)
        {
            return null;
        }

        // Movement on the walk map is bidirectional. One reverse flood fill from the leader therefore
        // gives both reachability and shortest distance for every possible gate landing point, without
        // acquiring a full-map pathfinder once per point.
        // Cancellation from this flood fill is expected during shutdown and handled by SafeEvaluateAsync.
        cancellationToken.ThrowIfCancellationRequested();
        var walkMap = map.Terrain.WalkMap;
        var width = walkMap.GetLength(0);
        var coordinateCount = width * walkMap.GetLength(1);
        var distancesFromTarget = ArrayPool<int>.Shared.Rent(coordinateCount);
        var pendingCoordinates = ArrayPool<int>.Shared.Rent(coordinateCount);
        try
        {
            Array.Clear(distancesFromTarget, 0, coordinateCount);
            this.PopulateWalkableDistancesFrom(
                walkMap,
                target,
                distancesFromTarget,
                pendingCoordinates,
                cancellationToken);
            WarpInfo? bestWarp = null;
            var bestWorstCaseDistance = int.MaxValue;
            foreach (var candidate in candidates)
            {
                var worstCaseDistance = this.GetWorstLandingDistance(candidate.LandingPoints, distancesFromTarget, width);
                if (worstCaseDistance is null
                    || worstCaseDistance > TravelSearchLimit
                    || worstCaseDistance > bestWorstCaseDistance
                    || (worstCaseDistance == bestWorstCaseDistance
                        && bestWarp?.Gate is { } bestGate
                        && GateDistance(candidate.Gate, target) >= GateDistance(bestGate, target)))
                {
                    continue;
                }

                bestWarp = candidate.Warp;
                bestWorstCaseDistance = worstCaseDistance.Value;
            }

            return bestWarp;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(distancesFromTarget);
            ArrayPool<int>.Shared.Return(pendingCoordinates);
        }
    }

    private int? GetWorstLandingDistance(
        IReadOnlyCollection<Point> landingPoints,
        int[] distancesFromTarget,
        int mapWidth)
    {
        var worstCaseDistance = 0;
        foreach (var landingPoint in landingPoints)
        {
            var encodedDistance = distancesFromTarget[(landingPoint.Y * mapWidth) + landingPoint.X];
            if (encodedDistance == 0)
            {
                return null;
            }

            // Zero means unreachable, so stored distances are offset by one.
            worstCaseDistance = Math.Max(worstCaseDistance, encodedDistance - 1);
        }

        return worstCaseDistance;
    }

    private void PopulateWalkableDistancesFrom(
        bool[,] walkMap,
        Point start,
        int[] distances,
        int[] pendingCoordinates,
        CancellationToken cancellationToken)
    {
        if (!walkMap[start.X, start.Y])
        {
            return;
        }

        var width = walkMap.GetLength(0);
        var height = walkMap.GetLength(1);
        var readIndex = 0;
        var writeIndex = 0;
        var startIndex = (start.Y * width) + start.X;
        distances[startIndex] = 1;
        pendingCoordinates[writeIndex++] = startIndex;
        while (readIndex < writeIndex)
        {
            if ((readIndex & 0xFF) == 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var currentIndex = pendingCoordinates[readIndex++];
            var currentX = currentIndex % width;
            var currentY = currentIndex / width;
            var nextEncodedDistance = distances[currentIndex] + 1;
            for (var offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (var offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    var nextX = currentX + offsetX;
                    var nextY = currentY + offsetY;
                    if ((uint)nextX >= width
                        || (uint)nextY >= height
                        || !walkMap[nextX, nextY]
                        || distances[(nextY * width) + nextX] != 0)
                    {
                        continue;
                    }

                    var nextIndex = (nextY * width) + nextX;
                    distances[nextIndex] = nextEncodedDistance;
                    pendingCoordinates[writeIndex++] = nextIndex;
                }
            }
        }
    }

    private IEnumerable<WarpInfo> GetLegalWarps(GameMapDefinition mapDefinition)
    {
        if (this._player.SelectedCharacter is not { } character
            || BotFeaturePlugIn.GetConfiguration(this._player.GameContext)?.IsMapForbidden(mapDefinition) == true)
        {
            // A map forbidden for bots has no legal warp at all: no hunting ground, no leader to follow
            // there, no errand - and a bot which logged in on it treats it as illegal and leaves.
            return [];
        }

        // Runtime/instanced maps are not guaranteed to reference the same persisted entity as the
        // global warp list, so map-number matching remains the compatibility fallback. When the exact
        // definition is present, however, use only its entries to avoid mixing distinct definitions
        // which happen to share a number.
        var sameNumberWarps = this._player.GameContext.Configuration.WarpList
            .Where(w => w.Gate?.Map?.Number == mapDefinition.Number)
            .ToArray();
        var mapDefinitionId = mapDefinition.GetId();
        var exactMapWarps = mapDefinitionId == Guid.Empty
            ? []
            : sameNumberWarps.Where(w => w.Gate!.Map!.GetId() == mapDefinitionId).ToArray();
        var mapWarps = exactMapWarps.Length > 0 ? exactMapWarps : sameNumberWarps;

        // Select the identity tier before applying access requirements. Otherwise an exact entry
        // which is too high-level could incorrectly fall through to an easier same-number entry.
        var plainLevel = (int)(this._player.Attributes?[Stats.Level] ?? 1);
        return mapWarps.Where(w => character.GetEffectiveMoveLevelRequirement(w.LevelRequirement) <= plainLevel);
    }

    /// <summary>
    /// Lowest level at which this bot warps on its own: its lowest
    /// class-reduced warp requirement, so special classes keep their edge.
    /// Cached per configuration and class; both change rarely.
    /// </summary>
    private int GetMinWarpLevel()
    {
        var configuration = this._player.GameContext.Configuration;
        var characterClass = this._player.SelectedCharacter?.CharacterClass;
        if (!ReferenceEquals(configuration, this._warpFloorConfiguration)
            || !ReferenceEquals(characterClass, this._warpFloorClass))
        {
            this._warpFloorConfiguration = configuration;
            this._warpFloorClass = characterClass;
            this._minWarpLevel = configuration.WarpList
                .Select(warp => this._player.SelectedCharacter?.GetEffectiveMoveLevelRequirement(warp.LevelRequirement) ?? warp.LevelRequirement)
                .DefaultIfEmpty(FallbackMinWarpLevel)
                .Min();
        }

        return this._minWarpLevel;
    }

    /// <summary>
    /// Whether the bot can pay the warp's fare. A bot travels on the same terms as a player - it pays
    /// what the warp costs - so a map it cannot afford is not offered to it in the first place, rather
    /// than chosen and then refused by <see cref="WarpAction"/>.
    /// </summary>
    /// <param name="warp">The warp to pay for.</param>
    private bool CanAffordWarp(WarpInfo warp) => this._player.Money >= warp.Costs;

    /// <summary>
    /// Gets the escape gate to the bot's class hometown, for when it must leave its current map but is
    /// below every warp requirement (e.g. a freshly reset veteran) - the equivalent of a town scroll.
    /// </summary>
    /// <param name="gate">The home map safezone gate.</param>
    /// <param name="mapDefinition">The home map.</param>
    /// <param name="monsterLevel">The level of the strongest safe monster there (informational).</param>
    /// <returns>True, if the bot has a home map it is not already on.</returns>
    private bool TryGetHomeEscapeGate([MaybeNullWhen(false)] out ExitGate gate, [MaybeNullWhen(false)] out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        gate = null;
        mapDefinition = null;
        monsterLevel = 0;

        var homeMap = this._player.SelectedCharacter?.CharacterClass?.HomeMap;
        if (homeMap is null
            || homeMap.Number == this._player.CurrentMap?.Definition.Number
            || BotFeaturePlugIn.GetConfiguration(this._player.GameContext)?.IsMapForbidden(homeMap) == true
            || homeMap.GetSafezoneGate() is not { } safezoneGate)
        {
            return false;
        }

        gate = safezoneGate;
        mapDefinition = homeMap;
        monsterLevel = this.BestSafeLevel(homeMap);
        return true;
    }

    /// <summary>
    /// The lowest monster level that still earns this bot anything: a master-class character at the
    /// game's maximum level gains master experience only from monsters of at least
    /// <c>GameConfiguration.MinimumMonsterLevelForMasterExperience</c> (the experience path grants it
    /// nothing at all below that) and no regular experience either, because it is at the maximum level
    /// already. Hunting below that floor is therefore
    /// not the safe choice for a mastered bot, it is a dead end - so the map and hunting ground choice
    /// look above the floor first. 0 for every other bot, which hunts by safety alone.
    /// </summary>
    private int MasterExperienceFloor()
    {
        var configuration = this._player.GameContext.Configuration;
        if (this._player.SelectedCharacter?.CharacterClass?.IsMasterClass != true
            || (this._player.Attributes?[Stats.Level] ?? 0) < configuration.MaximumLevel)
        {
            return 0;
        }

        return configuration.MinimumMonsterLevelForMasterExperience;
    }

    /// <summary>
    /// The level of the strongest monster on the map which this bot can safely fight (judged by the
    /// monster's real damage against the bot's defense and health, see <see cref="CombatHandler.IsSafeTarget"/>);
    /// 0 if the map has none. The level of the safe monsters remains the measure of a map's experience
    /// value when comparing maps - it just no longer decides what is SAFE. Monsters below
    /// <paramref name="minimumLevel"/> are not counted at all, which lets a mastered bot judge a map by
    /// what it actually earns there (see <see cref="MasterExperienceFloor"/>).
    /// </summary>
    /// <param name="mapDefinition">The map to judge.</param>
    /// <param name="minimumLevel">The lowest monster level worth counting.</param>
    private int BestSafeLevel(GameMapDefinition mapDefinition, int minimumLevel = 0)
    {
        var best = 0;
        foreach (var area in mapDefinition.MonsterSpawns)
        {
            if (!this.IsPermanentMonsterSpawn(area))
            {
                continue;
            }

            var level = GetMonsterLevel(area.MonsterDefinition!);
            if (level >= minimumLevel && level > best && CombatHandler.IsSafeTarget(this._player, area.MonsterDefinition!))
            {
                best = level;
            }
        }

        return best;
    }

    /// <summary>
    /// Whether the spawn area holds monsters which actually stand on the map at all times. The event
    /// spawns (golden monsters, the invasion and wave areas) are configured on the regular maps too, but
    /// only exist while their event runs - a bot judging a map by them would travel to hunting grounds
    /// which are empty most of the day, and would think its current map holds far stronger monsters than
    /// it can find there. Same class of mistake as walking to a merchant that is only spawned now and
    /// then, see <see cref="BotShoppingHandler.FindMerchantPosition"/>.
    /// </summary>
    private bool IsPermanentMonsterSpawn(MonsterSpawnArea area)
    {
        return area is { Quantity: > 0, SpawnTrigger: SpawnTrigger.Automatic, MonsterDefinition.ObjectKind: NpcObjectKind.Monster };
    }

    /// <summary>
    /// Picks the legally warpable map with the strongest monsters the bot can safely fight among those
    /// EASIER than where it stands now - one step down, not a retreat to the starter map. Used when the
    /// current map pays nothing (see <see cref="BarrenMapDuration"/>): the bot keeps stepping down every
    /// barren stretch until it finds ground it can actually farm, and the regular map choice carries it
    /// back up as its gear and level recover.
    /// </summary>
    /// <param name="warp">The warp info of the chosen map.</param>
    /// <param name="mapDefinition">The chosen map.</param>
    /// <param name="monsterLevel">The level of the strongest safe monster there.</param>
    private bool TryPickEasierMap(out WarpInfo warp, out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        warp = default!;
        mapDefinition = default!;
        monsterLevel = 0;

        var current = this._player.CurrentMap?.Definition;

        // With nothing safe here at all, every huntable map is a step down - the comparison must not
        // rule them all out.
        var currentLevel = current is null ? int.MaxValue : this.BestSafeLevel(current);
        if (currentLevel <= 0)
        {
            currentLevel = int.MaxValue;
        }

        foreach (var candidate in this._player.GameContext.Configuration.Maps)
        {
            if (ReferenceEquals(candidate, current))
            {
                continue;
            }

            var best = this.BestSafeLevel(candidate);
            if (best <= 0 || best >= currentLevel || best <= monsterLevel)
            {
                continue;
            }

            if (!candidate.TryGetRequirementError(this._player, out _)
                && this.TryGetLegalWarp(candidate, out var candidateWarp)
                && this.CanAffordWarp(candidateWarp)
                && HasWalkableSpawnGate(candidate))
            {
                warp = candidateWarp;
                mapDefinition = candidate;
                monsterLevel = best;
            }
        }

        return mapDefinition is not null;
    }

    /// <summary>
    /// Picks the legally warpable map (other than the current one) that offers the strongest monsters the
    /// bot can still safely handle if it is meaningfully better than the current map, with its warp gate.
    /// In escape mode (fleeing a hostile or illegal map) any legal map with something safe to hunt counts,
    /// no matter how it compares to the current one.
    /// </summary>
    private bool TryPickBetterMap(bool escape, out WarpInfo warp, out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        // A mastered bot earns nothing below the master-experience floor, so it first looks for a map
        // which pays at all. Only when no map within its reach offers monsters above the floor that it
        // can safely fight does it fall back to hunting by safety alone: standing on a map it cannot
        // survive would not earn it master experience either, and its gear still improves meanwhile.
        var floor = this.MasterExperienceFloor();
        return (floor > 0 && this.TryPickBetterMapCore(escape, floor, out warp, out mapDefinition, out monsterLevel))
               || this.TryPickBetterMapCore(escape, 0, out warp, out mapDefinition, out monsterLevel);
    }

    /// <summary>
    /// Picks the best map (see <see cref="TryPickBetterMap"/>), counting only monsters of at least
    /// <paramref name="minimumMonsterLevel"/>.
    /// </summary>
    private bool TryPickBetterMapCore(bool escape, int minimumMonsterLevel, out WarpInfo warp, out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        warp = default!;
        mapDefinition = default!;
        monsterLevel = 0;

        var current = this._player.CurrentMap?.Definition;
        var threshold = escape ? 1 : (current is null ? 0 : this.BestSafeLevel(current, minimumMonsterLevel)) + WarpImprovementMargin;

        var candidates = new List<(WarpInfo Warp, GameMapDefinition Map, int Level)>();
        foreach (var candidate in this._player.GameContext.Configuration.Maps)
        {
            if (ReferenceEquals(candidate, current))
            {
                continue;
            }

            var best = this.BestSafeLevel(candidate, minimumMonsterLevel);
            if (best < threshold || candidate.TryGetRequirementError(this._player, out _))
            {
                continue;
            }

            if (this.TryGetLegalWarp(candidate, out var candidateWarp)
                && this.CanAffordWarp(candidateWarp)
                && HasWalkableSpawnGate(candidate))
            {
                candidates.Add((candidateWarp, candidate, best));
            }
        }

        if (candidates.Count == 0)
        {
            return false;
        }

        // Always taking the single strongest map emptied the world: the maps of a level band differ by a
        // few monster levels, so one of them is every bot's answer, and the rest stand deserted - hundreds
        // of bots on Vulcanus while Tarkan, Karutan and Kanturu, which their level opens, see nobody. The
        // strongest map still wins most of the picks (see BestCandidateShare); the runners-up share the
        // rest, so the population spreads over the maps a player of that band would choose between.
        var ranked = candidates.OrderByDescending(c => c.Level).ToList();
        var picked = ranked[PickByRank(ranked.Count)];
        warp = picked.Warp;
        mapDefinition = picked.Map;
        monsterLevel = picked.Level;
        return true;
    }

    /// <summary>
    /// Picks a hunting ground point from the map's monster spawn areas, preferring the strongest monsters
    /// the bot can safely fight (see <see cref="CombatHandler.IsSafeTarget"/>) - best experience without
    /// being slaughtered. If nothing on the map is safe, it falls back to the weakest monsters available
    /// (the bot should warp away anyway).
    /// </summary>
    private bool TryPickHuntingGround(GameMap map, out Point ground, out int groundLevel)
    {
        ground = default;
        groundLevel = 0;

        var candidates = map.Definition.MonsterSpawns
            .Where(this.IsPermanentMonsterSpawn)
            .Select(a => (Area: a, Level: GetMonsterLevel(a.MonsterDefinition!)))
            .Where(x => x.Level > 0)
            .ToList();
        if (candidates.Count == 0)
        {
            return false;
        }

        // Prefer the strongest monsters the bot can safely handle (best experience); if none on this map
        // pass the safety check, fall back to the weakest available (the bot should warp away anyway).
        var safe = candidates.Where(x => CombatHandler.IsSafeTarget(this._player, x.Area.MonsterDefinition!)).ToList();

        // A mastered bot only earns master experience above the floor (see MasterExperienceFloor), so as
        // long as this map holds such monsters, it hunts them - the weaker grounds would pay it nothing.
        // Above the floor it then takes the WEAKEST monsters rather than the strongest: master experience
        // hardly grows with the monster's level, so the cheapest kill above the floor is the best one.
        var floor = this.MasterExperienceFloor();
        var masterGrounds = floor > 0 ? safe.Where(x => x.Level >= floor).ToList() : [];

        List<(MonsterSpawnArea Area, int Level)> band;
        if (masterGrounds.Count > 0)
        {
            var cheapest = masterGrounds.Min(x => x.Level);
            band = masterGrounds.Where(x => x.Level <= cheapest + BandWidth).ToList();
        }
        else if (safe.Count > 0)
        {
            var top = safe.Max(x => x.Level);
            band = safe.Where(x => x.Level >= top - BandWidth).ToList();
        }
        else
        {
            var bottom = candidates.Min(x => x.Level);
            band = candidates.Where(x => x.Level <= bottom + BandWidth).ToList();
        }

        // After repeated deaths against the same player, the area around the death site is off the
        // menu for a while (see OfflinePlayer.TryGetDeathSiteToAvoid): drop grounds close to it, so
        // the bot farms somewhere else instead of walking back into the same lost fight. Only if
        // EVERY candidate lies near the death site, are they all kept - relocating within the band is
        // still better than not picking any ground at all (the point pick below still steers away).
        Point? siteToAvoid = this._player.TryGetDeathSiteToAvoid(map.Definition, out var avoidPoint) ? avoidPoint : null;
        if (siteToAvoid is { } avoid)
        {
            var farGrounds = band.Where(x => GroundDistance(x.Area, avoid) > DeathSiteAvoidanceRange).ToList();
            if (farGrounds.Count > 0)
            {
                band = farGrounds;
            }
        }

        // Weight the choice by spawn quantity and proximity, so the bot prefers nearby, dense grounds
        // (shorter, safer travel) while keeping some variety. Grounds another bot outside the party
        // already works are dropped first, so two independent bots do not farm the same monsters.
        var competitorSpots = HuntingSpotOccupancy.GetCompetitorSpots(this._player, map.GetPlayers(), map.Definition.Number);
        var freeBand = band.Where(x => !HuntingSpotOccupancy.IsSpotOccupied(GroundCenter(x.Area), competitorSpots)).ToList();
        if (freeBand.Count > 0)
        {
            band = freeBand;
        }

        var position = this._player.Position;
        var chosen = band.SelectWeightedRandom(band.Select(x => GroundWeight(x.Area, position)));
        if (chosen.Area is null)
        {
            return false;
        }

        groundLevel = chosen.Level;
        return this.TryPickWalkablePoint(map, chosen.Area, siteToAvoid, competitorSpots, out ground);
    }

    private bool TryPickWalkablePoint(GameMap map, MonsterSpawnArea area, Point? avoidCenter, IReadOnlyList<HuntingSpot> competitorSpots, out Point point)
    {
        int minX = Math.Min(area.X1, area.X2);
        int maxX = Math.Max(area.X1, area.X2);
        int minY = Math.Min(area.Y1, area.Y2);
        int maxY = Math.Max(area.Y1, area.Y2);

        // Not every map states its spawns as rectangles: LaCleon, for one, lists every spawn as a single
        // tile, and so do 44 of the 55 maps which hold more than ten of them. Sampling "inside" such an
        // area can only ever return that one tile, so every bot that picks it walks onto the very same
        // spot. Scatter around it instead - the bot still arrives within combat range of the monster.
        var isSingleSpot = (maxX - minX) < GroundScatterSpan && (maxY - minY) < GroundScatterSpan;
        var center = new Point((byte)((minX + maxX) / 2), (byte)((minY + maxY) / 2));

        Point? occupiedFallback = null;
        for (var attempt = 0; attempt < MaxPointPickAttempts; attempt++)
        {
            // GetRandomCoordinate already keeps to the map, retries unwalkable picks and falls back to the
            // point itself when the surroundings offer nothing - so a spawn on a ledge stays usable.
            var candidate = isSingleSpot
                ? map.Terrain.GetRandomCoordinate(center, HuntingRange)
                : new Point((byte)Rand.NextInt(minX, maxX + 1), (byte)Rand.NextInt(minY, maxY + 1));
            int x = candidate.X;
            int y = candidate.Y;
            if (!map.Terrain.WalkMap[x, y] || map.Terrain.SafezoneMap[x, y])
            {
                continue;
            }

            // MU spawn areas can span most of the map, so a random point of an otherwise "far" area
            // may still land right at the death site to avoid - re-roll such points. If the area
            // offers nothing else, the attempts run out, and the pick fails like for any other
            // unusable area (the caller then simply retries the next tick).
            if (avoidCenter is { } avoid
                && Math.Abs(x - avoid.X) + Math.Abs(y - avoid.Y) <= DeathSiteAvoidanceRange)
            {
                continue;
            }

            var pointCandidate = new Point((byte)x, (byte)y);
            if (HuntingSpotOccupancy.IsSpotOccupied(pointCandidate, competitorSpots))
            {
                // Remember the point as a last resort: if this whole area is taken, sharing it is still
                // better than leaving the bot without a destination.
                occupiedFallback ??= pointCandidate;
                continue;
            }

            point = pointCandidate;
            return true;
        }

        if (occupiedFallback is { } fallback)
        {
            point = fallback;
            return true;
        }

        point = default;
        return false;
    }

    /// <summary>
    /// A Manhattan distance heuristic (the pathfinder applies its own estimate multiplier), used to turn the
    /// otherwise unguided search into A* for long-distance bot travel.
    /// </summary>
    private sealed class ManhattanTravelHeuristic : IHeuristic
    {
        public int HeuristicEstimateMultiplier { get; set; }

        public int CalculateHeuristicDistance(Point location, Point target)
            => this.HeuristicEstimateMultiplier * (Math.Abs(location.X - target.X) + Math.Abs(location.Y - target.Y));
    }
}
