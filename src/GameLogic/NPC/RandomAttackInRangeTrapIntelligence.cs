// <copyright file="RandomAttackInRangeTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// An AI which attacks a target which is in range of the trap.
/// </summary>
public class RandomAttackInRangeTrapIntelligence : TrapIntelligenceBase
{
    private IAttackable? _currentTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomAttackInRangeTrapIntelligence"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    public RandomAttackInRangeTrapIntelligence(GameMap map)
        : base(map)
    {
    }

    /// <inheritdoc />
    protected override async ValueTask TickAsync()
    {
        if (this._currentTarget != null)
        {
            // Old Target out of Range?
            if (!this._currentTarget.IsActive()
                || this._currentTarget.IsAtSafezone()
                || !await this.IsTargetInObserversAsync())
            {
                this._currentTarget = await this.SearchNextTarget();
            }
        }
        else
        {
            this._currentTarget = await this.SearchNextTarget();
        }

        // no target?
        if (this._currentTarget is null)
        {
            return;
        }

        // Target in Attack Range?
        ushort dist = (ushort)this._currentTarget.GetDistanceTo(this.Trap);
        if (this.Trap.Definition.AttackRange + 1 >= dist)
        {
            await this.Trap.AttackAsync(this._currentTarget);  // yes, attack
        }
    }

    private async ValueTask<IAttackable?> SearchNextTarget()
    {
        List<IWorldObserver> tempObservers;
        using (await this.Trap.ObserverLock.ReaderLockAsync())
        {
            tempObservers = new List<IWorldObserver>(this.Trap.Observers);
        }

        double closestDistance = 100;
        IAttackable? closest = null;

        foreach (var target in tempObservers.OfType<IAttackable>())
        {
            if (this.Map.Terrain.SafezoneMap[target.Position.X, target.Position.Y])
            {
                continue;
            }

            double d = target.GetDistanceTo(this.Trap);
            if (closestDistance > d)
            {
                closest = target;
                closestDistance = d;
            }
        }

        return closest;
    }

    private async ValueTask<bool> IsTargetInObserversAsync()
    {
        using (await this.Trap.ObserverLock.ReaderLockAsync())
        {
            return this._currentTarget is IWorldObserver worldObserver && this.Trap.Observers.Contains(worldObserver);
        }
    }
}