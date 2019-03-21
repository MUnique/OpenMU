// <copyright file="ILetterDeletedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about a deleted letter in the inbox.
    /// </summary>
    public interface ILetterDeletedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// The letter has been deleted.
        /// </summary>
        /// <param name="letterIndex">The letter header index.</param>
        void LetterDeleted(ushort letterIndex);
    }
}