// <copyright file="AreaSkillHitAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

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
    /// <param name="animationCounter">The animation counter of the hit, if the client sends one.</param>
    public async ValueTask AttackTargetAsync(Player player, IAttackable target, SkillEntry skill, byte animationCounter = 0)
    {
        if (skill.Skill is not { } usedSkill
            || usedSkill.SkillType != SkillType.AreaSkillExplicitHits
            || player.IsAtSafezone()
            || !target.IsAlive
            || target.IsAtSafezone()
            || (target is Player && !player.GameContext.Configuration.AreaSkillHitsPlayer))
        {
            return;
        }

        if (!player.SkillHitValidator.IsTargetInArea((ushort)usedSkill.Number, target.Position, animationCounter))
        {
            return;
        }

        if (target.CheckSkillTargetRestrictions(player, usedSkill))
        {
            var hitInfo = await target.AttackByAsync(player, skill, false).ConfigureAwait(false);
            await target.TryApplyElementalEffectsAsync(player, skill, hitInfo).ConfigureAwait(false);
        }
    }
}