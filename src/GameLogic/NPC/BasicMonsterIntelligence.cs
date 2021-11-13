// <copyright file="BasicMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using System.Diagnostics;
using System.Threading;

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
    protected IAttackable? CurrentTarget { get; private set; }

    private bool IsObservedByAttacker
    {
        get
        {
            this.Monster.ObserverLock.EnterReadLock();
            try
            {
                return this.Monster.Observers.OfType<IAttacker>().Any();
            }
            finally
            {
                this.Monster.ObserverLock.ExitReadLock();
            }
        }
    }

    /// <inheritdoc/>
    public void Start()
    {
        this._aiTimer = new Timer(_ => this.SafeTick(), null, this.Npc.Definition.AttackDelay, this.Npc.Definition.AttackDelay);
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
    protected virtual IAttackable? SearchNextTarget()
    {
        List<IWorldObserver> tempObservers;
        this.Npc.ObserverLock.EnterReadLock();
        try
        {
            tempObservers = new List<IWorldObserver>(this.Npc.Observers);
        }
        finally
        {
            this.Npc.ObserverLock.ExitReadLock();
        }

        var possibleTargets = tempObservers.OfType<IAttackable>()
            .Where(a => a.IsActive() && !a.IsAtSafezone())
            .ToList();
        var summons = possibleTargets.OfType<Player>()
            .Select(p => p.Summon?.Item1)
            .Where(s => s is not null)
            .Cast<IAttackable>()
            .WhereActive()
            .ToList();
        possibleTargets.AddRange(summons);
        var closestTarget = possibleTargets
            .OrderBy(a => a.GetDistanceTo(this.Npc))
            .FirstOrDefault();

        return closestTarget;

        // todo: check the walk distance
    }

    /// <summary>
    /// Determines whether this instance can attack.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can attack; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanAttack() => true;

    private bool IsTargetInObservers(IAttackable target)
    {
        this.Npc.ObserverLock.EnterReadLock();
        try
        {
            return target is IWorldObserver worldObserver && this.Npc.Observers.Contains(worldObserver);
        }
        finally
        {
            this.Npc.ObserverLock.ExitReadLock();
        }
    }

    private void SafeTick()
    {
        try
        {
            this.Tick();
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private void Tick()
    {
        if (!this.Monster.IsAlive)
        {
            return;
        }

        if (this.Monster.IsWalking)
        {
            return;
        }

        if (!this.CanAttack())
        {
            return;
        }

        var target = this.CurrentTarget;
        if (target != null)
        {
            // Old Target out of Range?
            if (!target.IsAlive
                || target.IsTeleporting
                || target.IsAtSafezone()
                || !target.IsInRange(this.Monster.Position, this.Npc.Definition.ViewRange)
                || (target is IWorldObserver && !this.IsTargetInObservers(target)))
            {
                target = this.CurrentTarget = this.SearchNextTarget();
            }
        }
        else
        {
            target = this.CurrentTarget = this.SearchNextTarget();
        }

        // no target?
        if (target is null)
        {
            // we move around randomly, so the monster does not look dead when watched from distance.
            if (this.IsObservedByAttacker)
            {
                this.Monster.RandomMove();
            }

            return;
        }

        // Target in Attack Range?
        if (target.IsInRange(this.Monster.Position, this.Monster.Definition.AttackRange + 1) && !this.Monster.IsAtSafezone())
        {
            this.Monster.Attack(target);  // yes, attack
        }

        // Target in View Range?
        else if (target.IsInRange(this.Monster.Position, this.Monster.Definition.ViewRange + 1))
        {
            // no, walk to the target
            var walkTarget = this.Monster.CurrentMap!.Terrain.GetRandomCoordinate(target.Position, this.Monster.Definition.AttackRange);
            this.Monster.WalkTo(walkTarget);
        }
        else
        {
            // we move around randomly, so the monster does not look dead when watched from distance.
            this.Monster.RandomMove();
        }
    }
}