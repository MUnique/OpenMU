// <copyright file="IShowCharacterCreationFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about that the requested character creation failed.
    /// </summary>
    public interface IShowCharacterCreationFailedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows that the character creation failed.
        /// </summary>
        void ShowCharacterCreationFailed();
    }
}