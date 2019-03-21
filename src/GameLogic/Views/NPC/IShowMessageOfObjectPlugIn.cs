// <copyright file="IShowMessageOfObjectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC
{
    /// <summary>
    /// Interface of a view whose implementation informs about a message from an object (e.g. NPC).
    /// </summary>
    public interface IShowMessageOfObjectPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the message of an object, e.g. NPC.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        void ShowMessageOfObject(string message, IIdentifiable sender);
    }
}