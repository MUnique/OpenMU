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
    public async ValueTask AttackTargetAsync(Player player, IAttackable target, SkillEntry skill)
    {
        if (skill.Skill?.SkillType != SkillType.AreaSkillExplicitHits
            || target is null
            || !target.IsAlive
            || target.IsAtSafezone())
        {
            return;
        }

        if (player.IsAtSafezone())
        {
            // It's possible, when the player did some area skill (Evil Spirit), and walked into the safezone.
            // We don't log it as hacker attempt, since the AreaSkillAttackAction already does handle this.
        }

        if (target.CheckSkillTargetRestrictions(player, skill.Skill))
        {
            await target.AttackByAsync(player, skill, false).ConfigureAwait(false);
            await target.TryApplyElementalEffectsAsync(player, skill).ConfigureAwait(false);
        }
    }
}