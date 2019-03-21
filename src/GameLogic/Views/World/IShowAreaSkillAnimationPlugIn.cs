// <copyright file="IShowAreaSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Interface of a view whose implementation informs about area skill animations of objects.
    /// </summary>
    public interface IShowAreaSkillAnimationPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the area skill animation.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="point">The coordinates.</param>
        /// <param name="rotation">The rotation.</param>
        void ShowAreaSkillAnimation(Player player, Skill skill, Point point, byte rotation);
    }
}