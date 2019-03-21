// <copyright file="ILetterSendResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about the success of the previously sent letter.
    /// </summary>
    public interface ILetterSendResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the letter send result, of the previously sent letter.
        /// </summary>
        /// <param name="success">The success.</param>
        /// <param name="letterId">The client side id of the letter.</param>
        void LetterSendResult(LetterSendSuccess success, uint letterId);
    }
}