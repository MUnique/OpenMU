// <copyright file="ITargetedSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills
{
    /// <summary>
    /// Interface for an action of a specific skill.
    /// </summary>
    public interface ITargetedSkillAction
    {
        /// <summary>
        /// Gets the skill id assigned for this action.
        /// </summary>
        ushort Key { get; }

        /// <summary>
        /// Callback function to trigger when specific skill is invoked.
        /// <param name="player">The player invoking the skill.</param>
        /// <param name="target">The target of the skill.</param>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>Returns the coroutine for the skill action.</returns>
        /// </summary>
        ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId);
    }
}
