// <copyright file="IChatRoomRequestPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for publishing chat room creation requests to a pub/sub system.
/// </summary>
public interface IChatRoomRequestPublisher
{
    /// <summary>
    /// Publishes a request to create a chat room for two friends.
    /// </summary>
    /// <param name="playerName">The name of the first player.</param>
    /// <param name="friendName">The name of the friend.</param>
    /// <param name="playerServerId">The server ID where the player is located.</param>
    /// <param name="friendServerId">The server ID where the friend is located.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask PublishChatRoomCreationRequestAsync(string playerName, string friendName, int playerServerId, int friendServerId);

    /// <summary>
    /// Publishes a request to invite a friend to an existing chat room.
    /// </summary>
    /// <param name="playerName">The name of the player sending the invitation.</param>
    /// <param name="friendName">The name of the friend being invited.</param>
    /// <param name="roomId">The ID of the chat room.</param>
    /// <param name="playerServerId">The server ID where the player is located.</param>
    /// <param name="friendServerId">The server ID where the friend is located.</param>
    /// <returns>A task representing the asynchronous operation with success status.</returns>
    ValueTask<bool> PublishChatRoomInvitationRequestAsync(string playerName, string friendName, ushort roomId, int playerServerId, int friendServerId);
}