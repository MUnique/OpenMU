// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System.Linq;
    using MUnique.OpenMU.DataModel;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
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
        public void Attack(Player player, ushort extraTargetId, ushort skillId, Point targetAreaCenter, byte rotation)
        {
            var skillEntry = player.SkillList?.GetSkill(skillId);
            var skill = skillEntry?.Skill;
            if (skill is null || skill.SkillType == SkillType.PassiveBoost)
            {
                return;
            }

            if (!player.TryConsumeForSkill(skill))
            {
                return;
            }

            if (skill.SkillType == SkillType.AreaSkillAutomaticHits)
            {
                this.PerformAutomaticHits(player, extraTargetId, targetAreaCenter, skillEntry!, skill);
            }

            player.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowAreaSkillAnimationPlugIn>()?.ShowAreaSkillAnimation(player, skill, targetAreaCenter, rotation), true);
        }

        private void PerformAutomaticHits(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill)
        {
            bool isExtraTargetDefined = extraTargetId == 0xFFFF;
            var attackablesInRange = player.CurrentMap?.GetAttackablesInRange(targetAreaCenter, skill.Range) ?? Enumerable.Empty<IAttackable>();
            if (!player.GameContext.Configuration.AreaSkillHitsPlayer)
            {
                attackablesInRange = attackablesInRange.Where(a => a is not Player);
                isExtraTargetDefined = false;
            }

            var extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;
            foreach (var target in attackablesInRange)
            {
                this.ApplySkill(player, skillEntry, target, targetAreaCenter);

                if (target == extraTarget)
                {
                    isExtraTargetDefined = false;
                    extraTarget = null;
                }
            }

            if (isExtraTargetDefined && extraTarget is not null)
            {
                this.ApplySkill(player, skillEntry, extraTarget, targetAreaCenter);
            }
        }

        private void ApplySkill(Player player, SkillEntry skillEntry, IAttackable target, Point targetAreaCenter)
        {
            skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

            if (target.CheckSkillTargetRestrictions(player, skillEntry.Skill))
            {
                target.AttackBy(player, skillEntry);
                target.TryApplyElementalEffects(player, skillEntry);
                player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(skillEntry.Skill.Number)?.AfterTargetGotAttacked(player, target, skillEntry, targetAreaCenter);
            }
        }
    }
}
