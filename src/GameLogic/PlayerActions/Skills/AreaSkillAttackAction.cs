// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action to attack with a skill which inflicts damage to an area of the current map of the player.
/// </summary>
public class AreaSkillAttackAction
{
    /// <summary>
    /// Performs the skill by the player at the specified area. Additionally to the target area, a target object can be specified.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targetAreaCenter">The coordinates of the center of the target area.</param>
    /// <param name="rotation">The rotation in which the player is looking. It's not really relevant for the hitted objects yet, but for some directed skills in the future it might be.</param>
    public async ValueTask AttackAsync(Player player, ushort extraTargetId, ushort skillId, Point targetAreaCenter, byte rotation)
    {
        var skillEntry = player.SkillList?.GetSkill(skillId);
        var skill = skillEntry?.Skill;
        if (skill is null || skill.SkillType == SkillType.PassiveBoost)
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skill))
        {
            return;
        }

        if (skill.SkillType == SkillType.AreaSkillAutomaticHits)
        {
            await this.PerformAutomaticHitsAsync(player, extraTargetId, targetAreaCenter, skillEntry!, skill);
        }

        await player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(p => p.ShowAreaSkillAnimationAsync(player, skill, targetAreaCenter, rotation), true).ConfigureAwait(false);
    }

    private async ValueTask PerformAutomaticHitsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill)
    {
        bool isExtraTargetDefined = extraTargetId != 0xFFFF;
        var attackablesInRange = player.CurrentMap?.GetAttackablesInRange(targetAreaCenter, skill.Range).Where(a => a != player) ?? Enumerable.Empty<IAttackable>();
        if (!player.GameContext.Configuration.AreaSkillHitsPlayer)
        {
            attackablesInRange = attackablesInRange.Where(a => a is not Player);
        }

        var extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;
        foreach (var target in attackablesInRange)
        {
            await this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter);

            if (target == extraTarget)
            {
                isExtraTargetDefined = false;
                extraTarget = null;
            }
        }

        if (isExtraTargetDefined && extraTarget is not null && player.IsInRange(extraTarget.Position, skill.Range + 2))
        {
            await this.ApplySkillAsync(player, skillEntry, extraTarget, targetAreaCenter);
        }
    }

    private async ValueTask ApplySkillAsync(Player player, SkillEntry skillEntry, IAttackable target, Point targetAreaCenter)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        if (target.CheckSkillTargetRestrictions(player, skillEntry.Skill))
        {
            await target.AttackByAsync(player, skillEntry);
            await target.TryApplyElementalEffectsAsync(player, skillEntry);
            if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(skillEntry.Skill.Number) is { } strategy)
            {
                await strategy.AfterTargetGotAttackedAsync(player, target, skillEntry, targetAreaCenter);
            }
        }
    }
}