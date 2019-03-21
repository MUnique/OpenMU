// <copyright file="IShowSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Interface of a view whose implementation informs about skill animations of objects.
    /// </summary>
    public interface IShowSkillAnimationPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the skill animation.
        /// </summary>
        /// <param name="attackingPlayer">The attacking player.</param>
        /// <param name="target">The target.</param>
        /// <param name="skill">The skill.</param>
        void ShowSkillAnimation(Player attackingPlayer, IAttackable target, Skill skill);
    }
}