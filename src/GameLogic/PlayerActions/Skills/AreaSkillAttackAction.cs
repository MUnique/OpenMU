// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action to attack with a skill which inflicts damage to an area of the current map of the player.
/// </summary>
public class AreaSkillAttackAction
{
    private const int UndefinedTarget = 0xFFFF;

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

        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget)
        {
            await this.PerformAutomaticHitsAsync(player, extraTargetId, targetAreaCenter, skillEntry!, skill, rotation).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(p => p.ShowAreaSkillAnimationAsync(player, skill, targetAreaCenter, rotation), true).ConfigureAwait(false);
    }

    private async ValueTask PerformAutomaticHitsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill, byte rotation)
    {
        if (player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsStunned] > 0)
        {
            player.Logger.LogWarning("Probably Hacker - player {player} is attacking in stunned state", player);
            return;
        }

        if (attributes[Stats.IsAsleep] > 0)
        {
            player.Logger.LogWarning("Probably Hacker - player {player} is attacking in sleep state", player);
            return;
        }

        if (player.IsAtSafezone())
        {
            player.Logger.LogWarning("Probably Hacker - player {player} is attacking from safezone", player);
            return;
        }

        bool isExtraTargetDefined = extraTargetId != UndefinedTarget;
        var extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;

        var attackablesInRange =
            skill.SkillType == SkillType.AreaSkillExplicitTarget
                ? null
                : player.CurrentMap?
                    .GetAttackablesInRange(targetAreaCenter, skill.Range)
                    .Where(a => a != player)
                    .Where(a => !a.IsAtSafezone());

        bool isCombo = false;
        if (player.ComboState is { } comboState)
        {
            isCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
        }

        if (attackablesInRange is not null)
        {
            if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillTargetFilter>(skill.Number) is { } filterPlugin)
            {
                attackablesInRange = attackablesInRange.Where(a => filterPlugin.IsTargetWithinBounds(player, a, targetAreaCenter, rotation));
            }

            if (!player.GameContext.Configuration.AreaSkillHitsPlayer)
            {
                attackablesInRange = attackablesInRange.Where(a => a is not Player);
            }

            foreach (var target in attackablesInRange)
            {
                await this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter, isCombo).ConfigureAwait(false);

                if (target != extraTarget)
                {
                    continue;
                }

                isExtraTargetDefined = false;
                extraTarget = null;
            }
        }

        if (isExtraTargetDefined
            && extraTarget is not null
            && player.IsInRange(extraTarget.Position, skill.Range + 2)
            && !player.IsAtSafezone())
        {
            await this.ApplySkillAsync(player, skillEntry, extraTarget, targetAreaCenter, isCombo).ConfigureAwait(false);
        }

        if (isCombo)
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, extraTarget), true).ConfigureAwait(false);
        }
    }

    private async ValueTask ApplySkillAsync(Player player, SkillEntry skillEntry, IAttackable target, Point targetAreaCenter, bool isCombo)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        if (target.CheckSkillTargetRestrictions(player, skillEntry.Skill))
        {
            var hitInfo = await target.AttackByAsync(player, skillEntry, isCombo).ConfigureAwait(false);
            await target.TryApplyElementalEffectsAsync(player, skillEntry).ConfigureAwait(false);
            var baseSkill = skillEntry.GetBaseSkill();

            if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(baseSkill.Number) is { } strategy)
            {
                await strategy.AfterTargetGotAttackedAsync(player, target, skillEntry, targetAreaCenter, hitInfo).ConfigureAwait(false);
            }
        }
    }
}