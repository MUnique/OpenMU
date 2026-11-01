// <copyright file="RageSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action for rage attacks.
/// </summary>
public class RageSkillAttackAction
{
    /// <summary>
    /// The maximum number of targets per attack.
    /// </summary>
    public const int MaximumTargetsPerAttack = 5;

    private const int HitsPerTarget = 2;

    /// <summary>
    /// Selects the targets of a rage attack: the explicit target first, then the
    /// nearest other monsters around the <paramref name="searchCenter"/>.
    /// </summary>
    /// <param name="player">The attacking player.</param>
    /// <param name="explicitTarget">The explicit target, if any.</param>
    /// <param name="searchCenter">The point around which the additional targets are searched.</param>
    /// <param name="candidates">The attackables in range of the <paramref name="searchCenter"/>.</param>
    /// <returns>The selected targets, at most <see cref="MaximumTargetsPerAttack"/>.</returns>
    public static List<IAttackable> SelectTargets(IAttackable player, IAttackable? explicitTarget, Point searchCenter, IEnumerable<IAttackable> candidates)
    {
        var targets = new List<IAttackable>(MaximumTargetsPerAttack);
        if (explicitTarget is not null)
        {
            targets.Add(explicitTarget);
        }

        targets.AddRange(
            candidates
                .Where(t => t != explicitTarget && t != player)
                .Where(t => t is not Player)
                .OrderBy(t => t.GetDistanceTo(searchCenter))
                .Take(MaximumTargetsPerAttack - targets.Count));
        return targets;
    }

    /// <summary>
    /// Attacks the target with a rage skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="explicitTargetId">The explicit target identifier.</param>
    /// <param name="skillId">The skill identifier.</param>
    public async ValueTask AttackAsync(Player player, ushort explicitTargetId, ushort skillId)
    {
        if (player.CurrentMap is not { } map)
        {
            // The player is changing the map, e.g. by a warp.
            return;
        }

        var explicitTarget = player.GetObject(explicitTargetId) as IAttackable;
        if (player.SkillList is null || player.SkillList.GetSkill(skillId) is not { Skill: { } usedSkill } skill
                                     || (explicitTarget is not null && !explicitTarget.IsInRange(player.Position, usedSkill.Range)))
        {
            return;
        }

        // The additional targets are searched around the explicit target, so the skill also hits
        // a group of monsters when the fighter attacks it from the maximum distance.
        var searchCenter = explicitTarget?.Position ?? player.Position;
        var targets = SelectTargets(
            player,
            explicitTarget,
            searchCenter,
            map.GetAttackablesInRange(searchCenter, usedSkill.Range));

        await player.InvokeViewPlugInAsync<IShowRageAttackRangePlugIn>(p => p.ShowRageAttackRangeAsync(skillId, targets)).ConfigureAwait(false);

        if (!targets.Any())
        {
            return;
        }

        if (!await AttackActionValidator.CanAttackAsync(player, usedSkill).ConfigureAwait(false))
        {
            return;
        }

        if (explicitTarget is null)
        {
            await player.ForEachWorldObserverAsync<IShowRageAttackPlugIn>(p => p.ShowAttackAsync(player, targets.First(), skillId), true).ConfigureAwait(false);
        }

        bool isCombo = false;
        if (player.ComboState is { } comboState)
        {
            isCombo = await comboState.RegisterSkillAsync(skill.Skill).ConfigureAwait(false);
            if (isCombo)
            {
                await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, explicitTarget), true).ConfigureAwait(false);
            }
        }

        _ = this.RunAttacksAsync(player, skill, targets, isCombo);
    }

    private async ValueTask RunAttacksAsync(Player player, SkillEntry skill, List<IAttackable> targets, bool isCombo)
    {
        try
        {
            for (int i = 0; i < HitsPerTarget; i++)
            {
                await Task.Delay(200).ConfigureAwait(false);
                foreach (var target in targets)
                {
                    await target.AttackByAsync(player, skill, isCombo).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error running rage attack.");
        }
    }
}