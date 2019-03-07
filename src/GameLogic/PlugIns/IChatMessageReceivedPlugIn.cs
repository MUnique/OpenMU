// <copyright file="IChatMessageReceivedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which is called when a chat message got received.
    /// It allows to cancel the forwarding of the message to the receivers.
    /// </summary>
    [Guid("026BF30C-6602-444E-862C-FAE19308E5EA")]
    [PlugInPoint("Chat message received", "Plugins which will be executed when a chat message arrives.")]
    public interface IChatMessageReceivedPlugIn
    {
        /// <summary>
        /// Is called when a chat message got received from a player.
        /// </summary>
        /// <param name="sender">The sending player.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancelEventArgs">
        /// The <see cref="CancelEventArgs"/> instance containing the event data.
        /// It allows to cancel the forwarding of the message to the receivers.
        /// </param>
        void ChatMessageReceived(Player sender, string message, CancelEventArgs cancelEventArgs);
    }
}