// <copyright file="IAddToLetterListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Interface of a view whose implementation informs about an added letter in the messenger.
    /// </summary>
    public interface IAddToLetterListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Adds a letter to the view.
        /// </summary>
        /// <param name="letter">Letter which should be added.</param>
        /// <param name="index">The index of the letter in the letter list.</param>
        /// <param name="newLetter">Determines if this letter is new, that means it got just sent.</param>
        void AddToLetterList(LetterHeader letter, ushort index, bool newLetter);
    }
}