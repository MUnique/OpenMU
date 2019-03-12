// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Action to attack with a skill which inflicts damage to an area of the current map of the player.
    /// </summary>
    public class AreaSkillAttackAction
    {
        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSkillAttackAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AreaSkillAttackAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

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
            SkillEntry skillEntry = player.SkillList.GetSkill(skillId);
            var skill = skillEntry.Skill;
            if (skill.SkillType == SkillType.PassiveBoost)
            {
                return;
            }

            if (!player.TryConsumeForSkill(skill))
            {
                return;
            }

            if (skill.SkillType == SkillType.AreaSkillAutomaticHits)
            {
                this.PerformAutomaticHits(player, extraTargetId, targetAreaCenter, skillEntry, skill);
            }

            player.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IWorldView>()?.ShowAreaSkillAnimation(player, skill, targetAreaCenter, rotation), true);
        }

        private void PerformAutomaticHits(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill)
        {
            bool isExtraTargetDefined = extraTargetId == 0xFFFF;
            var attackablesInRange = player.CurrentMap.GetAttackablesInRange(targetAreaCenter, skill.Range);
            if (!this.gameContext.Configuration.AreaSkillHitsPlayer)
            {
                attackablesInRange = attackablesInRange.Where(a => !(a is Player));
                isExtraTargetDefined = false;
            }

            IAttackable extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;
            foreach (var target in attackablesInRange)
            {
                this.ApplySkill(player, skillEntry, skill, target);

                if (target == extraTarget)
                {
                    isExtraTargetDefined = false;
                    extraTarget = null;
                }
            }

            if (isExtraTargetDefined)
            {
                this.ApplySkill(player, skillEntry, skill, extraTarget);
            }
        }

        private void ApplySkill(Player player, SkillEntry skillEntry, Skill skill, IAttackable target)
        {
            if (target.CheckSkillTargetRestrictions(player, skill))
            {
                target.AttackBy(player, skillEntry);
                target.ApplyElementalEffects(player, skillEntry);
            }
        }
    }
}
