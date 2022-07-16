// <copyright file="IFriendNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for a class which notifies a player about changes in the friend system.
/// </summary>
public interface IFriendNotifier
{
    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="requester">The requester.</param>
    /// <param name="receiver">The receiver.</param>
    /// <param name="serverId">The server identifier of the receiver.</param>
    ValueTask FriendRequestAsync(string requester, string receiver, int serverId);

    /// <summary>
    /// Notifies the game server that a letter got received for an online player.
    /// </summary>
    /// <param name="letter">The letter header.</param>
    ValueTask LetterReceivedAsync(LetterHeader letter);

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="playerServerId">The player server identifier.</param>
    /// <param name="player">The player who is playing on the server, and needs to get notified.</param>
    /// <param name="friend">The friend whose state changed.</param>
    /// <param name="friendServerId">The friend server identifier.</param>
    ValueTask FriendOnlineStateChangedAsync(int playerServerId, string player, string friend, int friendServerId);

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="playerAuthenticationInfo">Authentication information of the player who should get notified about the created chat room.</param>
    /// <param name="friendName">Name of the friend player which is expected to be in the chat room.</param>
    ValueTask ChatRoomCreatedAsync(int serverId, ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName);

    /// <summary>
    /// Initializes the messenger for a connected player.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="initializationData">The initialization data.</param>
    ValueTask InitializeMessengerAsync(int serverId, MessengerInitializationData initializationData);
}