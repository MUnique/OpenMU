// <copyright file="AreaSkillHitAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to hit targets with an area skill, which requires explicit hits <seealso cref="SkillType.AreaSkillExplicitHits"/>.
    /// </summary>
    public class AreaSkillHitAction
    {
        /// <summary>
        /// Attacks the target by the player with the specified skill.
        /// </summary>
        /// <param name="player">The player who is performing the skill.</param>
        /// <param name="target">The target.</param>
        /// <param name="skill">The skill.</param>
        public void AttackTarget(Player player, IAttackable target, SkillEntry skill)
        {
            if (skill.Skill?.SkillType != SkillType.AreaSkillExplicitHits
                || target is null
                || !target.IsAlive)
            {
                return;
            }

            if (target.CheckSkillTargetRestrictions(player, skill.Skill))
            {
                target.AttackBy(player, skill);
                target.TryApplyElementalEffects(player, skill);
            }
        }
    }
}
