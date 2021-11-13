// <copyright file="IChatClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

/// <summary>
/// Type of the chat room client update message.
/// </summary>
public enum ChatRoomClientUpdateType : byte
{
    /// <summary>
    /// A client joined the chat room. Then the client which receives the message adds this client to it's local chat room client list.
    /// </summary>
    Joined = 0,

    /// <summary>
    /// A client left the chat room. Then the client which receives the message removes this client from it's local chat room client list.
    /// </summary>
    Left = 1,
}

/// <summary>
/// Interface for a chat client.
/// </summary>
public interface IChatClient
{
    /// <summary>
    /// Gets or sets the index of the client in the room.
    /// </summary>
    byte Index { get; set; }

    /// <summary>
    /// Gets the authentication token which was sent by the client.
    /// </summary>
    string? AuthenticationToken { get; }

    /// <summary>
    /// Gets or sets the nickname.
    /// </summary>
    string? Nickname { get; set; }

    /// <summary>
    /// Gets the last activity.
    /// </summary>
    DateTime LastActivity { get; }

    /// <summary>
    /// Logs the chat client off, which means it removes it from it's current chat room and closes the connection.
    /// </summary>
    void LogOff();

    /// <summary>
    /// Sends the message to this chat client.
    /// </summary>
    /// <param name="senderId">The sender identifier.</param>
    /// <param name="message">The message.</param>
    void SendMessage(byte senderId, string message);

    /// <summary>
    /// Sends the client list of the chat room to this client.
    /// </summary>
    /// <param name="clients">The chat room clients.</param>
    void SendChatRoomClientList(IReadOnlyCollection<IChatClient> clients);

    /// <summary>
    /// Notifies the client that another client has joined the chat room.
    /// </summary>
    /// <param name="updatedClientId">The joined client identifier.</param>
    /// <param name="updatedClientName">Name of the joined client.</param>
    /// <param name="updateType">Type of the update (join or leave).</param>
    void SendChatRoomClientUpdate(byte updatedClientId, string updatedClientName, ChatRoomClientUpdateType updateType);
}