// <copyright file="IUpdateCurrentManaPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated current mana.
    /// </summary>
    public interface IUpdateCurrentManaPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the current mana.
        /// </summary>
        void UpdateCurrentMana();
    }
}