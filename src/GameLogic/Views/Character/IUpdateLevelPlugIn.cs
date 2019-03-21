// <copyright file="IUpdateLevelPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated character level.
    /// </summary>
    public interface IUpdateLevelPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the level.
        /// </summary>
        void UpdateLevel();
    }
}