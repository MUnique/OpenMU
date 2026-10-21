// <copyright file="IChatMessageSentPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called after a chat message of a player has been delivered.
/// </summary>
/// <remarks>
/// Unlike <see cref="IChatMessageReceivedPlugIn"/>, it's called only for messages which actually got delivered,
/// e.g. not for messages of chat banned players or messages which got cancelled by another plugin.
/// It's not called for chat commands.
/// </remarks>
[Guid("2CD21C08-8C0E-4534-A6C4-730FE77A9427")]
[PlugInPoint("Chat message sent", "Plugins which will be executed after a chat message of a player has been delivered.")]
public interface IChatMessageSentPlugIn
{
    /// <summary>
    /// Is called after a chat message of a player has been delivered.
    /// </summary>
    /// <param name="sender">The sending player.</param>
    /// <param name="message">The message, as it was delivered. Depending on the <paramref name="messageType"/>, it includes the prefix of the type, e.g. <c>@</c> for guild messages.</param>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="receiver">The receiver of a <see cref="ChatMessageType.Whisper"/> message; <see langword="null"/> for all other types.</param>
    ValueTask ChatMessageSentAsync(Player sender, string message, ChatMessageType messageType, Player? receiver);
}
