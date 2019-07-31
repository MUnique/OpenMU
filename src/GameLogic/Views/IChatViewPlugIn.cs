// <copyright file="IChatViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    /// <summary>
    /// Interface for the chat view.
    /// </summary>
    public interface IChatViewPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Sends a Chat Message to the Player.
        /// </summary>
        /// <param name="message">The message which should be shown in the view.</param>
        /// <param name="sender">The character name of the sender.</param>
        /// <param name="type">The type of the chat message.</param>
        void ChatMessage(string message, string sender, ChatMessageType type);
    }
}
