// <copyright file="IUpdateCurrentHealthPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated current health.
    /// </summary>
    public interface IUpdateCurrentHealthPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the current health.
        /// </summary>
        void UpdateCurrentHealth();
    }
}