// <copyright file="DoppelgangerMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The intelligence of the monsters of the doppelganger event.
/// They walk along the path towards the magic circle and only attack players
/// which are in their attack range, so that they don't leave the path.
/// </summary>
/// <remarks>
/// Unlike the <see cref="BasicMonsterIntelligence"/>, it keeps running when no player
/// observes the monster, because the monsters have to walk towards the magic circle anyway.
/// </remarks>
public sealed class DoppelgangerMonsterIntelligence : INpcIntelligence, IDisposable
{
    private readonly IList<DoppelgangerPathArea> _path;
    private readonly Func<Monster, ValueTask> _onMagicCircleReached;
    private readonly ILogger _logger;
    private Timer? _timer;
    private Monster? _monster;
    private IAttackable? _attacker;
    private int _pathPosition;
    private int _hasReachedMagicCircle;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerMonsterIntelligence"/> class.
    /// </summary>
    /// <param name="path">The areas of the path.</param>
    /// <param name="pathPosition">The position index on the path where the monster spawns.</param>
    /// <param name="walksAlongPath">If set to <c>true</c>, the monster walks along the path; otherwise, it stays at its position.</param>
    /// <param name="attacksFirst">If set to <c>true</c>, the monster attacks players in its range; otherwise, only players which attacked it.</param>
    /// <param name="onMagicCircleReached">The callback which is called when the monster reached the magic circle.</param>
    /// <param name="logger">The logger.</param>
    public DoppelgangerMonsterIntelligence(
        IList<DoppelgangerPathArea> path,
        int pathPosition,
        bool walksAlongPath,
        bool attacksFirst,
        Func<Monster, ValueTask> onMagicCircleReached,
        ILogger logger)
    {
        this._path = path;
        this._pathPosition = pathPosition;
        this.WalksAlongPath = walksAlongPath;
        this.AttacksFirst = attacksFirst;
        this._onMagicCircleReached = onMagicCircleReached;
        this._logger = logger;
    }

    /// <summary>
    /// Gets a value indicating whether the monster walks along the path.
    /// </summary>
    public bool WalksAlongPath { get; }

    /// <summary>
    /// Gets a value indicating whether the monster attacks players in its range without being attacked first.
    /// </summary>
    public bool AttacksFirst { get; }

    /// <summary>
    /// Gets the current position index of the monster on the path.
    /// </summary>
    public int PathPosition => Volatile.Read(ref this._pathPosition);

    /// <inheritdoc />
    public NonPlayerCharacter Npc
    {
        get => this.Monster;
        set => this.Monster = (Monster)value;
    }

    /// <summary>
    /// Gets or sets the monster.
    /// </summary>
    public Monster Monster
    {
        get => this._monster ?? throw new InvalidOperationException("Instance is not initialized with a Monster yet");
        set => this._monster = value;
    }

    /// <inheritdoc />
    /// <remarks>The magic circle may be located in a safezone.</remarks>
    public bool CanWalkOnSafezone => true;

    /// <inheritdoc />
    public void RegisterHit(IAttacker attacker)
    {
        if (attacker is IAttackable attackable)
        {
            this._attacker = attackable;
        }
    }

    /// <inheritdoc />
    public void Start()
    {
        this._timer ??= new Timer(state => _ = this.SafeTickAsync(), null, this.Monster.Definition.AttackDelay, this.Monster.Definition.AttackDelay);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The monster keeps walking when no player observes it, so pausing is ignored.
    /// </remarks>
    public void Pause()
    {
        // intentionally left blank.
    }

    /// <inheritdoc />
    public bool CanWalkOn(Point target)
    {
        return (this.Monster.CurrentMap.Terrain.AIgrid[target.X, target.Y] & 1) == 1;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._timer?.Dispose();
        this._timer = null;
    }

    /// <summary>
    /// Gets the center of the area, which is the target of the monsters walking to this area.
    /// </summary>
    /// <param name="area">The area.</param>
    /// <returns>The center of the area.</returns>
    internal static Point GetCenter(DoppelgangerPathArea area)
    {
        return new Point((byte)(area.X1 + ((area.X2 - area.X1) / 2)), (byte)(area.Y1 + ((area.Y2 - area.Y1) / 2)));
    }

    /// <summary>
    /// Executes one step of the intelligence. It's called periodically by a timer.
    /// </summary>
    internal async ValueTask TickAsync()
    {
        if (this._monster is not { IsAlive: true } monster
            || Volatile.Read(ref this._hasReachedMagicCircle) != 0)
        {
            return;
        }

        // Only the monsters which walk along the path can reach the magic circle. Others, like the
        // larvae of a chest, might just be standing next to it.
        if (this.WalksAlongPath && this.UpdatePathPosition(monster.Position) == this._path.Count - 1)
        {
            if (Interlocked.Exchange(ref this._hasReachedMagicCircle, 1) == 0)
            {
                this.Dispose();
                await this._onMagicCircleReached.Invoke(monster).ConfigureAwait(false);
            }

            return;
        }

        if (monster.IsWalking
            || monster.Attributes[Stats.IsStunned] > 0
            || monster.Attributes[Stats.IsAsleep] > 0)
        {
            return;
        }

        if (await this.SearchTargetAsync(monster).ConfigureAwait(false) is { } target)
        {
            await monster.AttackAsync(target).ConfigureAwait(false);
            return;
        }

        if (this.WalksAlongPath && monster.Attributes[Stats.IsFrozen] <= 0)
        {
            await monster.WalkToAsync(this.GetWalkTarget(this._path[this.PathPosition + 1])).ConfigureAwait(false);
        }
    }

    private async Task SafeTickAsync()
    {
        try
        {
            await this.TickAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown.
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error in the doppelganger monster intelligence of {monster}.", this._monster);
        }
    }

    /// <summary>
    /// Updates the position index on the path. The index only increases, when the monster
    /// entered one of the next areas, because the areas overlap.
    /// </summary>
    private int UpdatePathPosition(Point position)
    {
        var current = this.PathPosition;
        for (var i = this._path.Count - 1; i > current; i--)
        {
            if (this._path[i].Contains(position))
            {
                Volatile.Write(ref this._pathPosition, i);
                return i;
            }
        }

        return current;
    }

    private async ValueTask<IAttackable?> SearchTargetAsync(Monster monster)
    {
        if (this._attacker is { } attacker && this.IsValidTarget(monster, attacker))
        {
            return attacker;
        }

        this._attacker = null;
        if (!this.AttacksFirst)
        {
            return null;
        }

        List<IAttackable> candidates;
        using (await monster.ObserverLock.ReaderLockAsync())
        {
            candidates = monster.Observers.OfType<IAttackable>().Where(candidate => this.IsValidTarget(monster, candidate)).ToList();
        }

        return candidates.MinBy(candidate => candidate.GetDistanceTo(monster));
    }

    private bool IsValidTarget(Monster monster, IAttackable target)
    {
        return target.IsActive()
               && target is not Player { IsInvisible: true }
               && !target.IsAtSafezone()
               && target.IsInRange(monster.Position, monster.Definition.AttackRange)
               && monster.HasLineOfSightTo(target);
    }

    /// <summary>
    /// Gets the point to which the monster walks to reach the area. It's the center of the
    /// area, or another walkable point of it, if the center isn't walkable.
    /// </summary>
    private Point GetWalkTarget(DoppelgangerPathArea area)
    {
        var center = GetCenter(area);
        if (this.CanWalkOn(center))
        {
            return center;
        }

        for (var x = area.X1 + 1; x <= area.X2; x++)
        {
            for (var y = area.Y1 + 1; y <= area.Y2; y++)
            {
                var point = new Point((byte)x, (byte)y);
                if (this.CanWalkOn(point))
                {
                    return point;
                }
            }
        }

        return center;
    }
}
