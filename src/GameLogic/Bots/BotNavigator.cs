// <copyright file="BotNavigator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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

    /// <summary>
    /// Below this level a bot never warps: it stays on its class starting map (e.g. elves in Noria,
    /// summoners in Elvenland), so the newbie maps stay populated instead of everyone drifting to one map.
    /// </summary>
    private const int MinWarpLevel = 30;

    /// <summary>Width of the level band of areas we randomize between, so bots don't all stack on one spot.</summary>
    private const int BandWidth = 3;

    /// <summary>Range (tiles) around the origin that counts as "at the hunting ground".</summary>
    private const int HuntingRange = 6;

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
    /// starved the path finder pool once dozens of bots travelled at the same time.
    /// </summary>
    private const int PathReuseTolerance = 4;

    /// <summary>How close (tiles) the bot must be to the merchant to trade.</summary>
    private const int MerchantTalkRange = 3;

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

    /// <summary>A party member keeps within this distance (tiles) of its leader; beyond it, it walks back.</summary>
    private const int FollowDistance = 10;

    /// <summary>
    /// Hunting grounds closer than this (tiles) to a recent death site are avoided after the same
    /// player killed the bot repeatedly (see <see cref="OfflinePlayer.TryGetDeathSiteToAvoid"/>),
    /// so the bot farms somewhere else instead of walking back into the same lost fight.
    /// </summary>
    private const int DeathSiteAvoidanceRange = 30;

    private static readonly TimeSpan EvaluationInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan EmptyGroundGrace = TimeSpan.FromSeconds(8);

    /// <summary>How often the bot checks its backpack for equippable upgrades.</summary>
    private static readonly TimeSpan EquipCheckInterval = TimeSpan.FromSeconds(8);

    /// <summary>How often the bot considers whether it needs a shopping trip.</summary>
    private static readonly TimeSpan ShoppingCheckInterval = TimeSpan.FromSeconds(30);

    /// <summary>Cooldown after a shopping trip before considering the next one.</summary>
    private static readonly TimeSpan ShoppingCooldown = TimeSpan.FromMinutes(10);

    /// <summary>Minimum time between two cross-map warps, so a bot does not bounce between maps (a real player does not hop maps every minute either).</summary>
    private static readonly TimeSpan WarpCooldown = TimeSpan.FromMinutes(3);

    /// <summary>Cooldown for following the party leader to another map (shorter, so the group regroups quickly).</summary>
    private static readonly TimeSpan FollowWarpCooldown = TimeSpan.FromSeconds(20);

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
    /// If the bot's position has not changed for this long it is considered stuck (a wedged walk, or a
    /// monster it can't reach). The navigator then forces a fresh destination so the bot gets going again.
    /// </summary>
    private static readonly TimeSpan StuckTimeout = TimeSpan.FromSeconds(8);

    /// <summary>
    /// Stuck timeout when huntable monsters are nearby: much longer, because a bot standing still while
    /// monsters are around is usually just fighting them from one tile. It still eventually fires, which
    /// breaks the rare deadlock of a monster that is in range but cannot be attacked or reached.
    /// </summary>
    private static readonly TimeSpan StuckWithMonstersTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// A pool of full-map path finders for long-distance travel. The pooled (scoped) path finder rejects
    /// start/end further apart than ~16 tiles, so travel uses a <see cref="FullGridNetwork"/> which resolves
    /// routes across the whole map. A path finder is not safe for concurrent use, so we keep a small pool and
    /// hand one out per call: several bots can plan routes in parallel (a single shared instance serialized
    /// every bot and starved navigation once the population reached the hundreds), while the pool size caps
    /// how many CPU cores whole-map searches may occupy at once.
    /// </summary>
    private static readonly int TravelPathFinderPoolSize = Math.Clamp(Environment.ProcessorCount / 3, 2, 6);

    private static readonly SemaphoreSlim TravelPathFinderPool = new(TravelPathFinderPoolSize, TravelPathFinderPoolSize);

    private static readonly ConcurrentBag<PathFinder> TravelPathFinders = CreateTravelPathFinderPool();

    private readonly OfflinePlayer _player;
    private readonly CancellationTokenSource _cts = new();

    private Timer? _timer;
    private DateTime? _emptyGroundSince;
    private int _isEvaluating;
    private Point _destination;
    private bool _hasDestination;
    private DateTime _lastWarpUtc = DateTime.MinValue;
    private Point _lastPosition;
    private DateTime _lastMoveUtc = DateTime.MinValue;
    private DateTime _nextEquipCheckUtc = DateTime.MinValue;
    private IList<PathResultNode>? _travelPath;
    private int _travelPathIndex;
    private Point _travelPathTarget;
    private Point? _shoppingTarget;
    private DateTime _nextShoppingCheckUtc = DateTime.MinValue;
    private DateTime? _resetDueAtUtc;
    private short _leaderMapNumber;
    private DateTime _leaderOnMapSinceUtc = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BotNavigator"/> class.
    /// </summary>
    /// <param name="player">The bot player.</param>
    public BotNavigator(OfflinePlayer player)
    {
        this._player = player;
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

    private static int GroundWeight(MonsterSpawnArea area, Point from)
    {
        var proximity = Math.Max(1, 300 - GroundDistance(area, from));
        return Math.Max(1, (int)area.Quantity) * proximity;
    }

    /// <summary>Manhattan distance between the center of the spawn area and the given point.</summary>
    private static int GroundDistance(MonsterSpawnArea area, Point from)
    {
        var centerX = (area.X1 + area.X2) / 2;
        var centerY = (area.Y1 + area.Y2) / 2;
        return Math.Abs(from.X - centerX) + Math.Abs(from.Y - centerY);
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
            || this._player.CurrentMap is not { } map)
        {
            return;
        }

        // Inside a mini game event the whole normal routine below is suspended - the event
        // dictates the pace, and every branch which could move the bot off the event map
        // (shopping, revenge, warping, ground picking) must not run.
        if (this._player.CurrentMiniGame is not null)
        {
            await this.EvaluateInsideMiniGameAsync(map, cancellationToken).ConfigureAwait(false);
            return;
        }

        // Party bookkeeping: answer a pending invitation from a player once its human-like delay
        // passed, and leave a party with a human again when the bot got bored (see BotPartyHandler).
        await BotPartyHandler.ProcessAsync(this._player).ConfigureAwait(false);

        // Make sure the bot always carries healing and mana potions. Without them the offline handlers
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

            // Wings don't drop, so the loot-driven equipment progression above never provides them;
            // they are earned at the classic level milestones instead (see BotWingHandler). Queued
            // for the same reason: equipping mounts item power-ups.
            this._player.PendingBotActions.Enqueue(() => BotWingHandler.TryAdvanceWingsAsync(this._player));
        }

        // Keep the combat centre on the bot's current position, so the combat handler always engages
        // monsters right next to the bot - both at the hunting ground and while travelling through hostile
        // territory (self-defence). This is what keeps a travelling bot alive. The travel destination is
        // tracked separately in _destination.
        var inSafezone = map.Terrain.SafezoneMap[this._player.Position.X, this._player.Position.Y];

        // Combat centre: normally the bot's own position (self-defence while travelling and at the hunting
        // ground). But while inside the safezone we aim it at the destination, so the combat handler does NOT
        // make the bot stand in town swinging at monsters just outside (which it can't damage from a safezone) -
        // it should simply walk out of town instead.
        this._player.HuntingOrigin = inSafezone && this._hasDestination ? this._destination : this._player.Position;

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
            .Count(m => m.IsAlive && !m.IsAtSafezone() && CombatHandler.IsSafeTarget(this._player, m.Definition));

        // The bot counts as stuck when it has been frozen on the spot: quickly when there is nothing to
        // fight (a wedged walk or blocked path), and after a much longer grace when huntable monsters are
        // around - standing still next to monsters usually just means fighting, but if the position stays
        // frozen well beyond that, something is wedged (e.g. a monster in range that cannot be reached).
        var stuckTimeout = monstersNearby > 0 ? StuckWithMonstersTimeout : StuckTimeout;
        var stuck = DateTime.UtcNow - this._lastMoveUtc > stuckTimeout;
        if (stuck)
        {
            // Break free: pick a fresh hunting ground and walk to it now. Issuing the walk resets the wedged
            // walker, and a new destination avoids re-stalling on the same blocked path.
            this._lastMoveUtc = DateTime.UtcNow;
            this._emptyGroundSince = null;
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
        // evolution itself is handled by the feature plugin's maintenance pass, because it restarts the bot.
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

        // An armed revenge (a player killed this bot; it respawned on the same map) overrides the
        // whole normal routine below, including following the party leader: the bot marches back to
        // the place of its death. A leader on a revenge march still drags its followers along - the
        // posse a wronged player would bring. Once the revenge is spent or times out, the bot falls
        // back into the routine (and a follower rejoins its group).
        if (await this.TryRevengeMarchAsync(map, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // Party members follow their leader instead of roaming on their own: to the leader's map when
        // it warped away, and towards the leader when it moved off. Near the leader they hunt normally
        // (the party heal/buff handlers and the party experience bonus need the group to stay together).
        // Followers never warp on their own - the leader (the lowest-level member, so it only ever picks
        // maps the whole group can hunt on) decides where the party goes.
        var leaderToFollow = this.GetPartyLeaderToFollow();
        if (leaderToFollow is not null)
        {
            if (await this.TryFollowLeaderAsync(map, leaderToFollow, cancellationToken).ConfigureAwait(false))
            {
                return;
            }
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
        // once within the combat range the branch above takes over and the combat handler engages. This
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
        // route turned out to be impossible (e.g. across a river), drop it and fall through to pick another
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
        // In the safezone we skip the grace so a freshly-spawned bot heads out of town straight away.
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
                    this._player.Logger.LogInformation("Bot '{Name}' gives up its shopping trip: no route to the merchant at {Target}.", this._player.Name, target);
                    this._shoppingTarget = null;
                    this._player.IsOnShoppingTrip = false;
                    this._nextShoppingCheckUtc = DateTime.UtcNow + ShoppingCooldown;
                }

                return true;
            }

            // The trade itself runs in the MU Helper tick, not here: selling and buying mutate the
            // inventory and the Zen of a character whose combat tick may be running at this very moment
            // (the open NPC dialog only holds off the NEXT tick) - and both write through the same,
            // not-thread-safe persistence context. Same rule as for every other bot-initiated mutation,
            // see PendingBotActions. The jewels are spent right after the trade, in the same tick.
            this._player.PendingBotActions.Enqueue(() => this.TradeAndUpgradeAsync(map, target));

            this._shoppingTarget = null;
            this._player.IsOnShoppingTrip = false;
            this._nextShoppingCheckUtc = DateTime.UtcNow + ShoppingCooldown;
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

        if (BotShoppingHandler.FindMerchantPosition(map) is not { } merchantPosition)
        {
            // No merchant lives on this map at all (the Dungeon has no town). Shopping here would
            // starve the bot's logistics for as long as it stays - no potion restock, no selling, a
            // slowly silting backpack. Like a player pulling a town scroll, it warps to its class
            // home town and shops there; the better-map logic takes it back hunting afterwards.
            if (this.TryGetHomeEscapeGate(out var homeGate, out var homeMap, out _))
            {
                this._travelPath = null;
                this._hasDestination = false;
                this._lastWarpUtc = DateTime.UtcNow;
                this._nextShoppingCheckUtc = DateTime.UtcNow; // start the trip on the new map right away
                this._player.Logger.LogInformation("Bot '{Name}' warps home to {Map} for a shopping trip - no merchant on its map.", this._player.Name, homeMap.Name);
                await this._player.WarpToAsync(homeGate).ConfigureAwait(false);
                return true;
            }

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
        this._player.Logger.LogInformation("Bot '{Name}' heads to the merchant for a shopping trip.", this._player.Name);
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
    /// Marches the bot back to the place of its death while a revenge is armed (see
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
    /// The bot's routine while it takes part in a mini game event (Blood Castle, Devil Square,
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

        // The party boredom timer must not run down (let alone fire) mid-event; the window
        // restarts once the event is over.
        this._player.PartyBoredomAtUtc = null;

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
    /// Whether the object is something this bot would fight inside a mini game event. Unlike in the
    /// open world there is no safe-level filtering - the event's level bracket already matched the
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
            Monster monster => monster.Definition.ObjectKind == NpcObjectKind.Monster,
            Player player => BotPvpRules.IsLegalPvpTarget(this._player, player),
            _ => false,
        };
    }

    /// <summary>
    /// Finds the spot of a nearby opponent inside a mini game event, mirroring
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
    /// or the leader is currently not followable (dead, no map, or inside another mini game instance -
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
    /// Keeps a party member with its leader: warps to the leader's map when the leader warped away,
    /// and walks towards the leader when it moved out of the follow range.
    /// </summary>
    /// <returns>True, if following consumed this tick; false, if the bot is close enough and should hunt normally.</returns>
    private async ValueTask<bool> TryFollowLeaderAsync(GameMap map, Player leader, CancellationToken cancellationToken)
    {
        if (!ReferenceEquals(leader.CurrentMap, map))
        {
            ExitGate? warpListGate = null;
            if (leader.CurrentMap is { } targetMap
                && !this.TryGetLegalWarpGate(targetMap.Definition, out warpListGate)
                && targetMap.Definition.Number != this._player.SelectedCharacter?.CharacterClass?.HomeMap?.Number)
            {
                // The leader moved to a map the bot's plain character level cannot legally enter
                // (level gates map access, the same rule as everywhere else). Rather than trail
                // behind unreachable or sneak in through a back door, the bot leaves the group.
                if (this._player.Party is { } party)
                {
                    this._player.Logger.LogInformation(
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
            if (DateTime.UtcNow - this._lastWarpUtc >= FollowWarpCooldown
                && leader.CurrentMap is { } leaderMap
                && (leaderMap.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() ?? warpListGate) is { } leaderGate)
            {
                this._lastWarpUtc = DateTime.UtcNow;
                this._hasDestination = false;
                this._travelPath = null;
                this._player.Logger.LogInformation(
                    "Bot {Character} following party leader {Leader} to map {Map}.",
                    this._player.Name,
                    leader.Name,
                    leaderMap.Definition.Name);
                await this._player.WarpToAsync(leaderGate).ConfigureAwait(false);
                await this.TryPersistCurrentMapAsync(leaderMap.Definition).ConfigureAwait(false);
            }

            return true;
        }

        if (this._player.GetDistanceTo(leader.Position) > FollowDistance)
        {
            this._destination = leader.Position;
            this._hasDestination = true;
            await this.TravelTowardAsync(map, leader.Position, cancellationToken).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Whether the leader has been on his current map long enough (<see cref="LeaderSettleTime"/>) for the
    /// bot to follow him there. Tracks the leader's map per bot, so each follower makes the call on its own.
    /// </summary>
    private bool HasLeaderSettled(Player leader)
    {
        if (leader.CurrentMap?.Definition is not { } leaderMap)
        {
            return false;
        }

        if (this._leaderMapNumber != leaderMap.Number)
        {
            this._leaderMapNumber = leaderMap.Number;
            this._leaderOnMapSinceUtc = DateTime.UtcNow;
        }

        return DateTime.UtcNow - this._leaderOnMapSinceUtc >= LeaderSettleTime;
    }

    /// <summary>
    /// Performs the bot's character reset once it is due (reset feature enabled, required level
    /// reached, reset limit not exhausted). The reset is not executed the moment the bot becomes
    /// eligible: a random grace delay (<see cref="MinResetDelay"/>..<see cref="MaxResetDelay"/>) is
    /// rolled first, during which the bot hunts on normally. After the reset the navigation state is
    /// cleared, so the bot starts its next cycle fresh - re-investing its points (queued by the reset
    /// handler) and heading for a hunting ground that suits it again.
    /// </summary>
    /// <returns>True, if a reset was performed.</returns>
    private async ValueTask<bool> TryResetCharacterAsync()
    {
        if (BotPartyHandler.HasHumanCompanion(this._player))
        {
            // Not in the middle of a hunting session with a player: the reset would wipe the bot's
            // strength and teleport it home, deserting the group. It resets right after the party
            // ends (the boredom timer of BotPartyHandler bounds how long that takes).
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
        this._lastWarpUtc = DateTime.MinValue;
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
                           && !this.TryGetLegalWarpGate(currentMap, out _)
                           && currentMap.Number != this._player.SelectedCharacter?.CharacterClass?.HomeMap?.Number;
        var mustEscape = mapIsHostile || mapIsIllegal;

        if ((plainLevel < MinWarpLevel && !mustEscape)
            || DateTime.UtcNow - this._lastWarpUtc < WarpCooldown)
        {
            return false;
        }

        // When escaping, any legal map with something safe to hunt beats staying - and with no legal
        // warp target at all (a freshly reset veteran may be below every warp requirement), the bot
        // retreats to its class home town, like a player using a town scroll.
        if (!this.TryPickBetterMap(mustEscape, out var targetGate, out var targetMap, out var targetLevel)
            && !(mustEscape && this.TryGetHomeEscapeGate(out targetGate, out targetMap, out targetLevel)))
        {
            return false;
        }

        this._lastWarpUtc = DateTime.UtcNow;
        this._hasDestination = false;
        this._travelPath = null;
        this._player.Logger.LogInformation(
            "Bot {Character} (level {Level}) warping to map {Map} (monsters ~{MonsterLevel}).",
            this._player.Name,
            botLevel,
            targetMap.Name,
            targetLevel);
        await this._player.WarpToAsync(targetGate).ConfigureAwait(false);
        await this.TryPersistCurrentMapAsync(targetMap).ConfigureAwait(false);
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
        // CPU and starved the shared path finder pool once dozens of bots travelled at the same time.
        if (this._travelPath is { } cached
            && this._travelPathIndex < cached.Count
            && destination.EuclideanDistanceTo(this._travelPathTarget) <= PathReuseTolerance
            && position.EuclideanDistanceTo(cached[this._travelPathIndex].Point) <= 1.5)
        {
            await this.WalkCachedStepsAsync(position).ConfigureAwait(false);
            return true;
        }

        this._travelPath = null;

        IList<PathResultNode>? path;

        // Observe the navigator's shutdown token while queuing for a shared path finder: on dispose the
        // wait is abandoned at once (the OperationCanceledException is expected and handled in
        // SafeEvaluateAsync) instead of holding the tick until a finder frees up.
        await TravelPathFinderPool.WaitAsync(cancellationToken).ConfigureAwait(false);
        PathFinder? finder = null;
        try
        {
            if (!TravelPathFinders.TryTake(out finder))
            {
                finder = CreateTravelPathFinder();
            }

            finder.ResetPathFinder();
            path = finder.FindPath(position, destination, map.Terrain.AIgrid, true);
        }
        finally
        {
            if (finder is not null)
            {
                TravelPathFinders.Add(finder);
            }

            TravelPathFinderPool.Release();
        }

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
    }

    /// <summary>
    /// Picks a hunting ground and starts walking to it. If the chosen ground turns out to be unreachable the
    /// destination is dropped, so the next call (next tick) simply picks another one.
    /// </summary>
    private async ValueTask PickGroundAndTravelAsync(GameMap map, CancellationToken cancellationToken)
    {
        if (!this.TryPickHuntingGround(map, out var ground, out var groundLevel))
        {
            this._hasDestination = false;
            this._player.Logger.LogDebug("Bot {Character}: no hunting ground found on map {Map}.", this._player.Name, map.Definition.Name);
            return;
        }

        this._destination = ground;
        this._hasDestination = true;
        this._player.Logger.LogInformation(
            "Bot {Character} (level {Level}) heading to hunting ground {Ground} (monster level ~{MonsterLevel}).",
            this._player.Name,
            this.GetBotLevel(),
            ground,
            groundLevel);

        if (!await this.TravelTowardAsync(map, ground, cancellationToken).ConfigureAwait(false))
        {
            this._hasDestination = false;
        }
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

        // A stack holds as many charges as the item definition allows - no more (see EmergencyPotionCharges).
        var stackSize = Math.Max((byte)1, definition.Durability);
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
    /// Finds the position of the nearest live, attackable monster within <see cref="MonsterSeekRadius"/> that
    /// the bot can safely fight, so the bot heads straight for a real monster instead of a random tile in a
    /// huge spawn area. Returns false when none are loaded nearby (or all are above the safe level cap), in
    /// which case the caller falls back to coarse spawn-area travel / warping to relocate.
    /// </summary>
    private bool TryFindNearestMonsterGround(GameMap map, out Point ground)
    {
        ground = default;
        var position = this._player.Position;
        var candidates = new List<(Monster Monster, double Distance)>();
        foreach (var attackable in map.GetAttackablesInRange(position, MonsterSeekRadius))
        {
            if (attackable is not Monster monster
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

        // Head for one of the few nearest monsters, chosen randomly, instead of strictly the nearest:
        // with many bots on one ground, deterministic nearest-first would send them all to the same
        // monster and they would roam the map as one pack - a very bot-like look.
        var chosen = candidates
            .OrderBy(c => c.Distance)
            .Take(3)
            .Select(c => c.Monster)
            .SelectRandom();
        if (chosen is null)
        {
            return false;
        }

        // The monster stands on a walkable tile, so head straight for it; TravelTowardAsync walks a few steps
        // per tick and re-homes as the monster roams, closing the gap until combat range is reached.
        ground = chosen.Position;
        return true;
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
    /// by this - the game gates warps by the plain character level (see <see cref="TryGetLegalWarpGate"/>),
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
    /// <param name="gate">The warp target gate, if a legal entry exists.</param>
    /// <returns>True, if the bot may warp to the map.</returns>
    private bool TryGetLegalWarpGate(GameMapDefinition mapDefinition, [MaybeNullWhen(false)] out ExitGate gate)
    {
        gate = null;
        if (this._player.SelectedCharacter is not { } character)
        {
            return false;
        }

        var plainLevel = (int)(this._player.Attributes?[Stats.Level] ?? 1);
        gate = this._player.GameContext.Configuration.WarpList
            .Where(w => w.Gate?.Map?.Number == mapDefinition.Number
                        && character.GetEffectiveMoveLevelRequirement(w.LevelRequirement) <= plainLevel)
            .Select(w => w.Gate!)
            .FirstOrDefault();
        return gate is not null;
    }

    /// <summary>
    /// Gets the escape gate to the bot's class home town, for when it must leave its current map but is
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
    /// The lowest monster level which still earns this bot anything: a master-class character at the
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
    /// Picks the legally warpable map (other than the current one) that offers the strongest monsters the
    /// bot can still safely handle, if it is meaningfully better than the current map, with its warp gate.
    /// In escape mode (fleeing a hostile or illegal map) any legal map with something safe to hunt counts,
    /// no matter how it compares to the current one.
    /// </summary>
    private bool TryPickBetterMap(bool escape, out ExitGate gate, out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        // A mastered bot earns nothing below the master-experience floor, so it first looks for a map
        // which pays at all. Only when no map within its reach offers monsters above the floor that it
        // can safely fight does it fall back to hunting by safety alone: standing on a map it cannot
        // survive would not earn it master experience either, and its gear still improves meanwhile.
        var floor = this.MasterExperienceFloor();
        return (floor > 0 && this.TryPickBetterMapCore(escape, floor, out gate, out mapDefinition, out monsterLevel))
               || this.TryPickBetterMapCore(escape, 0, out gate, out mapDefinition, out monsterLevel);
    }

    /// <summary>
    /// Picks the best map (see <see cref="TryPickBetterMap"/>), counting only monsters of at least
    /// <paramref name="minimumMonsterLevel"/>.
    /// </summary>
    private bool TryPickBetterMapCore(bool escape, int minimumMonsterLevel, out ExitGate gate, out GameMapDefinition mapDefinition, out int monsterLevel)
    {
        gate = default!;
        mapDefinition = default!;
        monsterLevel = 0;

        var current = this._player.CurrentMap?.Definition;
        var threshold = escape ? 1 : (current is null ? 0 : this.BestSafeLevel(current, minimumMonsterLevel)) + WarpImprovementMargin;

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

            if (this.TryGetLegalWarpGate(candidate, out var warpGate))
            {
                gate = warpGate;
                mapDefinition = candidate;
                monsterLevel = best;
                threshold = best + 1; // keep only a strictly-stronger map after this one
            }
        }

        return mapDefinition is not null;
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
        // long as this map holds such monsters it hunts them - the weaker grounds would pay it nothing.
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
        // EVERY candidate lies near the death site are they all kept - relocating within the band is
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
        // (shorter, safer travel) while keeping some variety.
        var position = this._player.Position;
        var chosen = band.SelectWeightedRandom(band.Select(x => GroundWeight(x.Area, position)));
        if (chosen.Area is null)
        {
            return false;
        }

        groundLevel = chosen.Level;
        return this.TryPickWalkablePoint(map, chosen.Area, siteToAvoid, out ground);
    }

    private bool TryPickWalkablePoint(GameMap map, MonsterSpawnArea area, Point? avoidCenter, out Point point)
    {
        var minX = Math.Min(area.X1, area.X2);
        var maxX = Math.Max(area.X1, area.X2);
        var minY = Math.Min(area.Y1, area.Y2);
        var maxY = Math.Max(area.Y1, area.Y2);

        for (var attempt = 0; attempt < MaxPointPickAttempts; attempt++)
        {
            var x = Rand.NextInt(minX, maxX + 1);
            var y = Rand.NextInt(minY, maxY + 1);
            if (!map.Terrain.WalkMap[x, y] || map.Terrain.SafezoneMap[x, y])
            {
                continue;
            }

            // MU spawn areas can span most of the map, so a random point of an otherwise "far" area
            // may still land right at the death site to avoid - re-roll such points. If the area
            // offers nothing else, the attempts run out and the pick fails like for any other
            // unusable area (the caller then simply retries next tick).
            if (avoidCenter is { } avoid
                && Math.Abs(x - avoid.X) + Math.Abs(y - avoid.Y) <= DeathSiteAvoidanceRange)
            {
                continue;
            }

            point = new Point((byte)x, (byte)y);
            return true;
        }

        point = default;
        return false;
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
}
