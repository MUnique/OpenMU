// <copyright file="IShowAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Interface of a view whose implementation informs about object animations.
    /// </summary>
    public interface IShowAnimationPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the animation.
        /// </summary>
        /// <param name="animatingObj">The animating object.</param>
        /// <param name="animation">The animation.</param>
        /// <param name="targetObj">The target object.</param>
        /// <param name="direction">The direction.</param>
        void ShowAnimation(IIdentifiable animatingObj, byte animation, IIdentifiable? targetObj, Direction direction);

        /// <summary>
        /// Shows the monster attack animation.
        /// </summary>
        /// <param name="animatingObj">The animating monster.</param>
        /// <param name="targetObj">The target object.</param>
        /// <param name="direction">The direction.</param>
        void ShowMonsterAttackAnimation(IIdentifiable animatingObj, IIdentifiable? targetObj, Direction direction);
    }
}