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
            // Old Target out of Range or sight?
            // Unlike a monster, a trap can't walk around a wall, so a target
            // which lost sight must be dropped; otherwise the trap would stay
            // latched and stop attacking everyone else in range.
            if (!this._currentTarget.IsActive()
                || this._currentTarget.IsAtSafezone()
                || !this.Trap.HasLineOfSightTo(this._currentTarget)
                || !await this.IsTargetInObserversAsync().ConfigureAwait(false))
            {
                this._currentTarget = await this.SearchNextTargetAsync().ConfigureAwait(false);
            }
        }
        else
        {
            this._currentTarget = await this.SearchNextTargetAsync().ConfigureAwait(false);
        }

        // no target?
        if (this._currentTarget is null)
        {
            return;
        }

        // Target in Attack Range with a clear line of sight?
        ushort dist = (ushort)this._currentTarget.GetDistanceTo(this.Trap);
        if (this.Trap.Definition.AttackRange + 1 >= dist
            && this.Trap.HasLineOfSightTo(this._currentTarget))
        {
            await this.Trap.AttackAsync(this._currentTarget).ConfigureAwait(false);  // yes, attack
        }
    }

    private async ValueTask<IAttackable?> SearchNextTargetAsync()
    {
        List<IWorldObserver> tempObservers;
        using (await this.Trap.ObserverLock.ReaderLockAsync())
        {
            tempObservers = new List<IWorldObserver>(this.Trap.Observers);
        }

        var candidates = tempObservers.OfType<IAttackable>()
            .Where(target => !this.Map.Terrain.SafezoneMap[target.Position.X, target.Position.Y]);

        // Prefer a visible target, fall back to the nearest one; a target which
        // lost sight is dropped by the caller, so the trap re-targets instead of
        // staying latched.
        return NpcTargetSelection.GetNearestPreferVisible(candidates, this.Trap, target => target.GetDistanceTo(this.Trap));
    }

    private async ValueTask<bool> IsTargetInObserversAsync()
    {
        using (await this.Trap.ObserverLock.ReaderLockAsync())
        {
            return this._currentTarget is IWorldObserver worldObserver && this.Trap.Observers.Contains(worldObserver);
        }
    }
}