// <copyright file="IExternalChatMessageReceivedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a chat message arrives from outside of the game, e.g. from Discord,
/// before it's sent into the game. It's the counterpart of <see cref="IChatMessageReceivedPlugIn"/>, for senders
/// which aren't players, so that e.g. a filter can cover all chats.
/// </summary>
[Guid("CC5DC7AF-9896-40DB-8BBA-1FAA935521AB")]
[PlugInPoint("External chat message received", "Plugins which will be executed when a chat message arrives from outside of the game, e.g. from Discord.")]
public interface IExternalChatMessageReceivedPlugIn
{
    /// <summary>
    /// Is called when a chat message arrives from outside of the game.
    /// </summary>
    /// <param name="senderName">The name of the sender, e.g. the name of the character of a linked Discord user.</param>
    /// <param name="message">The message.</param>
    /// <param name="channel">The chat of the game into which the message is sent.</param>
    /// <param name="cancelEventArgs">
    /// The <see cref="CancelEventArgs"/> instance containing the event data.
    /// It allows to cancel the forwarding of the message into the game.
    /// </param>
    void ExternalChatMessageReceived(string senderName, string message, GameChatChannel channel, CancelEventArgs cancelEventArgs);
}
