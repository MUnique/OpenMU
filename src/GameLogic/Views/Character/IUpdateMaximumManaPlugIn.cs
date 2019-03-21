// <copyright file="IUpdateMaximumManaPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about an updated maximum mana.
    /// </summary>
    public interface IUpdateMaximumManaPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the maximum mana.
        /// </summary>
        void UpdateMaximumMana();
    }
}