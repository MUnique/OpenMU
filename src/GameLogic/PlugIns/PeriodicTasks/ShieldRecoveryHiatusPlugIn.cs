// <copyright file="ShieldRecoveryHiatusPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates and resets the shield recovery hiatus attribute.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShieldRecoveryHiatusPlugIn_Name), Description = nameof(PlugInResources.ShieldRecoveryHiatusPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7B4A9E2D-C1F5-48E3-A6B2-1E7D4C8F3A5B")]
public class ShieldRecoveryHiatusPlugIn : IPeriodicTaskPlugIn, IAttackableMovedPlugIn, IAttackableGotHitPlugIn
{
    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        await gameContext.ForEachPlayerAsync(player =>
        {
            if (player.SelectedCharacter != null
                && !player.PlayerState.CurrentState.IsDisconnectedOrFinished()
                && player.Attributes is { } attributes
                && attributes[Stats.MaximumShield] > 0)
            {
                attributes.AddElement(new SimpleElement(1, AggregateType.AddRaw), Stats.ShieldRecoveryHiatus);
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        // do nothing.
    }

    /// <inheritdoc />
    public void AttackableGotHit(IAttackable attackable, IAttacker attacker, HitInfo hitInfo)
    {
        var defender = attackable as Player ?? (attackable as Monster)?.SummonedBy;
        var attackerPlayer = attacker as Player ?? (attacker as Monster)?.SummonedBy;
        if (defender is null || attackerPlayer is null || defender == attackerPlayer || defender.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.MaximumShield] > 0 && hitInfo is { ShieldDamage: > 0 })
        {
            attributes.GetComposableAttribute(Stats.ShieldRecoveryHiatus)?.RemoveAllElements();
        }
    }

    /// <inheritdoc />
    public void AttackableMoved(IAttackable attackable)
    {
        if (attackable is not Player player || player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.MaximumShield] > 0 &&
            (attributes[Stats.IsShieldRecoveryActive] < 1 || attributes[Stats.CurrentShield] == attributes[Stats.MaximumShield]))
        {
            attributes.GetComposableAttribute(Stats.ShieldRecoveryHiatus)?.RemoveAllElements();
        }
    }
}