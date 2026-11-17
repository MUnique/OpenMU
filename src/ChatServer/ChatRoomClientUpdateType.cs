// <copyright file="ChatRoomClientUpdateType.cs" company="MUnique">
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
