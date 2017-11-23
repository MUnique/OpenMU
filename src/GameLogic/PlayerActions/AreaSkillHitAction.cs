// <copyright file="AreaSkillHitAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to hit targets with an area skill, which requires explicit hits <seealso cref="SkillType.AreaSkillExplicitHits"/>.
    /// TODO: It's usually required to perform a <see cref="AreaSkillAttackAction"/> before, so this check has to be implemented.
    ///       Each animation and hit is usually referenced due a counter value in the packets.
    /// </summary>
    public class AreaSkillHitAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSkillHitAction"/> class.
        /// </summary>
        public AreaSkillHitAction()
        {
        }

        /// <summary>
        /// Attacks the target by the player with the specified skill.
        /// </summary>
        /// <param name="player">The player who is performing the skill.</param>
        /// <param name="target">The target.</param>
        /// <param name="skill">The skill.</param>
        public void AttackTarget(Player player, IAttackable target, SkillEntry skill)
        {
            if (skill.Skill.SkillType != SkillType.AreaSkillExplicitHits
                || target == null
                || !target.Alive)
            {
                return;
            }

            target.AttackBy(player, skill);
        }
    }
}
