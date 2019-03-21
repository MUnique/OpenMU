// <copyright file="IShowHitPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    /// <summary>
    /// Interface of a view whose implementation informs about a hit on an object.
    /// </summary>
    public interface IShowHitPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the hit damage over of an object.
        /// </summary>
        /// <param name="hitReceiver">The hit receiver.</param>
        /// <param name="hitInfo">The hit information.</param>
        void ShowHit(IAttackable hitReceiver, HitInfo hitInfo);
    }
}