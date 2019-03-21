// <copyright file="IShowLetterPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about the content of a requested letter.
    /// </summary>
    public interface IShowLetterPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the letter body.
        /// </summary>
        /// <param name="letterBody">The letter body.</param>
        void ShowLetter(LetterBody letterBody);
    }
}