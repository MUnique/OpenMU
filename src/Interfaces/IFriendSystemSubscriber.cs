// <copyright file="IFriendSystemSubscriber.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Immutable;

/// <summary>
/// Interface for an object which subscribes for changes in the friends system.
/// </summary>
public interface IFriendSystemSubscriber
{
    /// <summary>
    /// Notifies the game server that a letter got received for an online player.
    /// </summary>
    /// <param name="letter">The letter header.</param>
    ValueTask LetterReceivedAsync(LetterHeader letter);

    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="requester">The requester.</param>
    /// <param name="receiver">The receiver.</param>
    ValueTask FriendRequestAsync(string requester, string receiver);

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="player">The player who is playing on the server, and needs to get notified.</param>
    /// <param name="friend">The friend whose state changed.</param>
    /// <param name="serverId">The new server identifier of the <paramref name="friend"/>.</param>
    ValueTask FriendOnlineStateChangedAsync(string player, string friend, int serverId);

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="playerAuthenticationInfo">Authentication information of the player who should get notified about the created chat room.</param>
    /// <param name="friendName">Name of the friend player which is expected to be in the chat room.</param>
    ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName);

    /// <summary>
    /// Initializes the messenger of a player.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    ValueTask InitializeMessengerAsync(MessengerInitializationData initializationData);
}

/// <summary>
/// The data of a messenger initialization for <see cref="IFriendSystemSubscriber.InitializeMessengerAsync"/>.
/// </summary>
public record MessengerInitializationData(string PlayerName, IImmutableList<string> Friends, IImmutableList<string> OpenFriendRequests);