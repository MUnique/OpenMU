﻿// <copyright file="SummonedMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The intelligence for a summoned monster.
/// It's basically the same as for the <see cref="BasicMonsterIntelligence"/>, but chooses monsters as targets.
/// </summary>
public sealed class SummonedMonsterIntelligence : BasicMonsterIntelligence
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SummonedMonsterIntelligence"/> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    public SummonedMonsterIntelligence(Player owner)
    {
        this.Owner = owner;
        // Summons should be allowed to walk within safezones to follow their owner.
        this.CanWalkOnSafezone = true;
    }

    /// <summary>
    /// Gets the owner of the summoned monster.
    /// </summary>
    public Player Owner { get; }

    /// <inheritdoc />
    public override void RegisterHit(IAttacker attacker)
    {
        // Do not aggro against the owner, even if the owner hits the summon intentionally.
        if (attacker is Player p && p == this.Owner)
        {
            return;
        }

        if (this.CurrentTarget is null
            || attacker.IsInRange(this.Npc.Position, this.Npc.Definition.AttackRange))
        {
            base.RegisterHit(attacker);
        }
    }

    /// <inheritdoc />
    protected override ValueTask<IAttackable?> SearchNextTargetAsync()
    {
        var currentMap = this.Owner.CurrentMap;

        var nextTarget = currentMap?.GetAttackablesInRange(this.Owner.Position, 8)
            .Where(o => o is Monster { SummonedBy: null, IsAlive: true })
            .OfType<NonPlayerCharacter>()
            .OrderBy(o => o.GetDistanceTo(this.Owner))
            .FirstOrDefault() as IAttackable;

        return ValueTask.FromResult(nextTarget);
    }

    /// <inheritdoc />
    protected override async ValueTask<bool> CanAttackAsync()
    {
        var maxDistance = this.CurrentTarget is null ? 2 : 5;
        var distanceToOwner = this.Owner.GetDistanceTo(this.Npc);
        if (distanceToOwner > maxDistance && this.Owner.IsAlive)
        {
            // follow the player
            var target = this.Monster.CurrentMap!.Terrain.GetRandomCoordinate(this.Owner.Position, 2);
            if (this.Monster.Observers.Contains(this.Owner))
            {
                await this.Monster.WalkToAsync(target).ConfigureAwait(false);
            }
            else
            {
                await this.Monster.MoveAsync(target).ConfigureAwait(false);
            }

            return false;
        }

        return true;
    }
}
