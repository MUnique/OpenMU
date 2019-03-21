// <copyright file="IApplyKeyConfigurationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about the key configuration of the character.
    /// </summary>
    public interface IApplyKeyConfigurationPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Applies the key configuration on the view.
        /// </summary>
        void ApplyKeyConfiguration();
    }
}