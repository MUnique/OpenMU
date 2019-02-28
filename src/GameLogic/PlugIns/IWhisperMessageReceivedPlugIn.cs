// <copyright file="IWhisperMessageReceivedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which is called when a whispered chat message got received.
    /// It allows to cancel the forwarding of the message to the receiver.
    /// </summary>
    [Guid("A4BA9FA4-AEDA-49D2-800C-139639C3D901")]
    [PlugInPoint("Whisper chat message received", "Plugins which will be executed when a whispered chat message arrives.")]
    public interface IWhisperMessageReceivedPlugIn
    {
        /// <summary>
        /// Is called when a whispered chat message got received from a player.
        /// </summary>
        /// <param name="sender">The sending player.</param>
        /// <param name="receiver">The receiving player.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancelEventArgs">
        /// The <see cref="CancelEventArgs" /> instance containing the event data.
        /// It allows to cancel the forwarding of the message to the receiver.
        /// </param>
        void WhisperMessageReceived(Player sender, Player receiver, string message, CancelEventArgs cancelEventArgs);
    }
}