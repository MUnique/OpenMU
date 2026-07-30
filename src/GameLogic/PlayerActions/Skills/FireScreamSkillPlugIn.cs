// <copyright file="FireScreamSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the fire scream skill of the dark lord class. Based on a chance, it does an additional damage (explosion) to any targets in a radius which origin is the target itself.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.FireScreamSkillPlugIn_Name), Description = nameof(PlugInResources.FireScreamSkillPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7E2F9B4A-D6C1-4F8E-A3B5-21E8C7F9D541")]
public class FireScreamSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public short Key => 78;

    private short Radius => 2;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        if (hitInfo is not { } hit || !Rand.NextRandomBool(0.3))
        {
            return;
        }

        var attackDamage = hit.HealthDamage + hit.ShieldDamage;
        var explosionDamage = attackDamage / 10;
        if (explosionDamage < 1)
        {
            return;
        }

        bool FilterTarget(IAttackable attackable)
        {
            if (attackable == attacker)
            {
                return false;
            }

            if (attackable is Monster { SummonedBy: null } or Destructible)
            {
                return true;
            }

            if (attackable is Monster { SummonedBy: not null } summoned)
            {
                return FilterTarget(summoned.SummonedBy);
            }

            if (attackable is Player { DuelRoom.State: DuelState.DuelStarted } targetPlayer
                && attacker is Player { DuelRoom.State: DuelState.DuelStarted } duelPlayer
                && targetPlayer.DuelRoom == duelPlayer.DuelRoom
                && targetPlayer.DuelRoom.IsDuelist(targetPlayer) && targetPlayer.DuelRoom.IsDuelist(duelPlayer))
            {
                return true;
            }

            if (attackable is Player { GuildWarContext.State: GuildWarState.Started } guildWarTarget
                && attacker is Player { GuildWarContext.State: GuildWarState.Started } guildWarAttacker
                && guildWarTarget.GuildWarContext == guildWarAttacker.GuildWarContext)
            {
                return true;
            }

            return false;
        }

        var explosionTargets = target.CurrentMap?
            .GetAttackablesInRange(target.Position, this.Radius)
            .Where(a => a.GetDistanceTo(target) <= this.Radius)
            .Where(FilterTarget) ?? [];
        if (!explosionTargets.Any())
        {
            return;
        }

        // Delay the explosion a little bit, so the client can show the hit values staggered
        await Task.Delay(100).ConfigureAwait(false);

        foreach (var explosionTarget in explosionTargets)
        {
            if (explosionTarget.IsActive())
            {
                // We just need to apply the damage, so we can resort to the bleeding damage method which has DamageAttributes.Undefined
                await explosionTarget.ApplyBleedingDamageAsync(attacker, explosionDamage).ConfigureAwait(false);
            }
        }
    }
}