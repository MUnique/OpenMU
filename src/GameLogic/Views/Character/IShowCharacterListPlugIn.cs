// <copyright file="IShowCharacterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about the available characters in the character selection screen.
    /// </summary>
    public interface IShowCharacterListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the character list.
        /// </summary>
        void ShowCharacterList();
    }
}