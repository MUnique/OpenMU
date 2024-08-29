// <copyright file="ChainLightningSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;

using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the chain lightning skill of the summoner class. Additionally to the attacked target, it will hit up to two additional targets.
/// </summary>
[PlugIn(nameof(ChainLightningSkillPlugIn), "Handles the chain lightning skill of the summoner class. Additionally to the attacked target, it will hit up to two additional targets.")]
[Guid("298C5FF8-03A2-476B-B064-A59E73DFCEB9")]
public class ChainLightningSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public short Key => 215;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        bool FilterTarget(IAttackable attackable)
        {
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

        var secondTarget = target.CurrentMap?.GetAttackablesInRange(target.Position, 2).FirstOrDefault(FilterTarget) ?? target;
        var thirdTarget = secondTarget.CurrentMap?.GetAttackablesInRange(secondTarget.Position, 2).Where(FilterTarget).FirstOrDefault(t => t != secondTarget && t != target) ?? secondTarget;

        var observable = attacker as IObservable;
        if (observable is null)
        {
            return;
        }

        await observable.ForEachWorldObserverAsync<IShowChainLightningPlugIn>(o => o.ShowLightningChainAnimationAsync(attacker, skillEntry.Skill!, [target, secondTarget, thirdTarget]), true).ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            await Task.Delay(300).ConfigureAwait(false);

            // first attack 70 %
            await secondTarget.AttackByAsync(attacker, skillEntry, false, 0.7).ConfigureAwait(false);
            await secondTarget.TryApplyElementalEffectsAsync(attacker, skillEntry).ConfigureAwait(false);

            await Task.Delay(300).ConfigureAwait(false);

            // second attack 50%
            await thirdTarget.AttackByAsync(attacker, skillEntry, false, 0.5).ConfigureAwait(false);
            await thirdTarget.TryApplyElementalEffectsAsync(attacker, skillEntry).ConfigureAwait(false);
        });
    }
}