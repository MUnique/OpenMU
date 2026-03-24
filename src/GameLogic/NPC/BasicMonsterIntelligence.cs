// <copyright file="BasicMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A basic monster AI which is pretty basic.
/// </summary>
public class BasicMonsterIntelligence : INpcIntelligence, IDisposable
{
    private Timer? _aiTimer;
    private Monster? _monster;

    /// <summary>
    /// Finalizes an instance of the <see cref="BasicMonsterIntelligence"/> class.
    /// </summary>
    ~BasicMonsterIntelligence()
    {
        this.Dispose();
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance can walk on safezone.
    /// </summary>
    public bool CanWalkOnSafezone { get; protected set; }

    /// <inheritdoc/>
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

    /// <summary>
    /// Gets the current target.
    /// </summary>
    public IAttackable? CurrentTarget { get; private set; }

    /// <inheritdoc/>
    public void Start()
    {
        var startDelay = this.Npc.Definition.AttackDelay + TimeSpan.FromMilliseconds(Rand.NextInt(0, 100));
        this.OnStart();
        this._aiTimer ??= new Timer(_ => this.SafeTick(), null, startDelay, this.Npc.Definition.AttackDelay);
    }

    /// <inheritdoc/>
    public void Pause()
    {
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual void RegisterHit(IAttacker attacker)
    {
        if (this.CurrentTarget is null && attacker is IAttackable attackable)
        {
            this.CurrentTarget = attackable;
        }
    }

    /// <inheritdoc/>
    public virtual bool CanWalkOn(Point target)
    {
        return this.Monster.CurrentMap.Terrain.AIgrid[target.X, target.Y] == 1;
    }

    /// <inheritdoc/>
    public bool IsTargetingPlayer(Player player) => this.CurrentTarget == player;

    /// <summary>
    /// Called when the intelligence starts.
    /// </summary>
    protected virtual void OnStart()
    {
        // can be overwritten for additional logic.
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool managed)
    {
        this._aiTimer?.Dispose();
        this._aiTimer = null;
    }

    /// <summary>
    /// Searches the next target in view range.
    /// </summary>
    /// <returns>The next target.</returns>
    protected virtual async ValueTask<IAttackable?> SearchNextTargetAsync()
    {
        List<IWorldObserver> tempObservers;
        using (await this.Npc.ObserverLock.ReaderLockAsync())
        {
            if (this.Npc.Observers.Count == 0)
            {
                return null;
            }

            tempObservers = new List<IWorldObserver>(this.Npc.Observers);
        }

        var possibleTargets = tempObservers.OfType<IAttackable>()
            .Where(a => a.IsActive() && !a.IsAtSafezone() && a is not Player { IsInvisible: true })
            .ToList();

        // Also consider summoned monsters belonging to players in range.
        var summons = possibleTargets.OfType<Player>()
            .Select(p => p.Summon?.Item1)
            .Where(s => s is not null)
            .Cast<IAttackable>()
            .WhereActive()
            .ToList();

        possibleTargets.AddRange(summons);

        // todo: check the walk distance
        return possibleTargets.MinBy(a => a.GetDistanceTo(this.Npc));
    }

    /// <summary>
    /// Determines whether this instance can attack this tick.
    /// </summary>
    protected virtual ValueTask<bool> CanAttackAsync() => ValueTask.FromResult(true);

    /// <summary>
    /// Handles the tick when no target is available, moves the monster randomly.
    /// </summary>
    protected virtual async ValueTask TickWithoutTargetAsync()
    {
        if (this.Monster.Attributes[Stats.IsFrozen] > 0)
        {
            return;
        }

        if (await this.IsObservedByAttackerAsync().ConfigureAwait(false))
        {
            await this.Monster.RandomMoveAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Determines whether the handled monster is observed by any attacker.
    /// </summary>
    protected async ValueTask<bool> IsObservedByAttackerAsync()
    {
        using var readerLock = await this.Monster.ObserverLock.ReaderLockAsync();
        return this.Monster.Observers.OfType<IAttacker>().Any();
    }

    private async ValueTask<bool> IsTargetInObserversAsync(IAttackable target)
    {
        using (await this.Npc.ObserverLock.ReaderLockAsync())
        {
            return target is IWorldObserver worldObserver && this.Npc.Observers.Contains(worldObserver);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void SafeTick()
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
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private async ValueTask TickAsync()
    {
        if (!this.Monster.IsAlive)
        {
            this.CurrentTarget = null;
            return;
        }

        if (this.Monster.IsWalking)
        {
            return;
        }

        if (this.Monster.Attributes[Stats.IsStunned] > 0 || this.Monster.Attributes[Stats.IsAsleep] > 0)
        {
            return;
        }

        if (!await this.CanAttackAsync().ConfigureAwait(false))
        {
            return;
        }

        this.CurrentTarget = await this.ResolveTargetAsync().ConfigureAwait(false);

        if (this.CurrentTarget is null)
        {
            await this.TickWithoutTargetAsync().ConfigureAwait(false);
            return;
        }

        // Target in attack range — attack.
        if (this.CurrentTarget.IsInRange(this.Monster.Position, this.Monster.Definition.AttackRange)
            && !this.Monster.IsAtSafezone())
        {
            await this.Monster.AttackAsync(this.CurrentTarget).ConfigureAwait(false);
            return;
        }

        if (this.Monster.Attributes[Stats.IsFrozen] > 0)
        {
            return;
        }

        // Target visible but outside attack range, walk toward it.
        if (this.CurrentTarget.IsInRange(this.Monster.Position, this.Monster.Definition.ViewRange + 1))
        {
            var walkTarget = this.Monster.CurrentMap!.Terrain.GetRandomCoordinate(this.CurrentTarget.Position, this.Monster.Definition.AttackRange);
            if (await this.Monster.WalkToAsync(walkTarget).ConfigureAwait(false))
            {
                return;
            }
        }

        // Nothing else to do, wander randomly.
        await this.Monster.RandomMoveAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the current target if still valid, otherwise searches for a new one.
    /// </summary>
    private async ValueTask<IAttackable?> ResolveTargetAsync()
    {
        if (this.CurrentTarget is not null && this.IsCurrentTargetValid())
        {
            // Double-check the target is still within the observer list (needed for
            // players who have moved out of view range server-side).
            if (!await this.IsTargetInObserversAsync(this.CurrentTarget).ConfigureAwait(false)
                && this.CurrentTarget is IWorldObserver)
            {
                return await this.SearchNextTargetAsync().ConfigureAwait(false);
            }

            return this.CurrentTarget;
        }

        return await this.SearchNextTargetAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Returns <c>true</c> if the current target is still a valid attack candidate.
    /// </summary>
    private bool IsCurrentTargetValid()
    {
        return this.CurrentTarget is not null
               && this.CurrentTarget.IsAlive
               && this.CurrentTarget is not Player { IsInvisible: true }
               && !this.CurrentTarget.IsTeleporting
               && !this.CurrentTarget.IsAtSafezone()
               && this.CurrentTarget.IsInRange(this.Monster.Position, this.Npc.Definition.ViewRange);
    }
}