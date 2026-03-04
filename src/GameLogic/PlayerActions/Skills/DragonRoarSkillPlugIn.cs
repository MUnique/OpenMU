// <copyright file="DragonRoarSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the dragon roar skill of the rage fighter class. Additionally to the attacked target, it will hit up to seven additional targets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.DragonRoarSkillPlugIn_Name), Description = nameof(PlugInResources.DragonRoarSkillPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A797A6AD-AC92-4731-A0FB-D46D4C1DD0DF")]
public class DragonRoarSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public virtual short Key => 264;

    /// <summary>
    /// Gets the range around the target, in which additional targets are searched.
    /// </summary>
    protected virtual short Range => 3;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        bool FilterTarget(IAttackable attackable)
        {
            if (attackable == target)
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

        var extraTargets = target.CurrentMap?.GetAttackablesInRange(target.Position, this.Range).Where(FilterTarget).Take(7);

        if (extraTargets is null)
        {
            return;
        }

        int i = 1;
        var skill = skillEntry.Skill!;
        foreach (var extraTarget in extraTargets)
        {
            if (i <= 3 || Rand.NextRandomBool())
            {
                // first three 100% chance, others 50% chance
                await extraTarget.AttackByAsync(attacker, skillEntry, false, 1, false).ConfigureAwait(false);
                await extraTarget.TryApplyElementalEffectsAsync(attacker, skillEntry).ConfigureAwait(false);

                for (int hit = 2; hit <= skill.NumberOfHitsPerAttack; hit++)
                {
                    await extraTarget.AttackByAsync(attacker, skillEntry, false, 1, hit == skill.NumberOfHitsPerAttack).ConfigureAwait(false);
                }
            }

            i++;
        }
    }
}