// <copyright file="IUpdateRotationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated rotation of the own player.
    /// </summary>
    public interface IUpdateRotationPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the rotation of the own player.
        /// </summary>
        void UpdateRotation();
    }
}