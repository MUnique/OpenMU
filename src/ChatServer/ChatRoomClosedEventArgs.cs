// <copyright file="ChatRoomClosedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

/// <summary>
/// Event arguments which contains the chat room which has been closed.
/// </summary>
/// <seealso cref="System.EventArgs" />
internal class ChatRoomClosedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomClosedEventArgs"/> class.
    /// </summary>
    /// <param name="room">The chat room.</param>
    public ChatRoomClosedEventArgs(ChatRoom room)
    {
        this.ChatRoom = room;
    }

    /// <summary>
    /// Gets the chat room which has been closed.
    /// </summary>
    public ChatRoom ChatRoom { get; }
}