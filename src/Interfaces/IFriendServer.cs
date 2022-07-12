// -----------------------------------------------------------------------
// <copyright file="IFriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Defines some server ids which are used for some specific states.
/// </summary>
public enum SpecialServerId : byte
{
    /// <summary>
    /// Gets the server id which represents being offline.
    /// </summary>
    Offline = 0xFF,

    /// <summary>
    /// Gets the server id which represents being invisible (=offline to other players).
    /// </summary>
    Invisible = 0xFE,
}

/// <summary>
/// The friend server interface.
/// </summary>
public interface IFriendServer
{
    /// <summary>
    /// Forwards the letter.
    /// </summary>
    /// <param name="letter">The letter.</param>
    ValueTask ForwardLetterAsync(LetterHeader letter);

    /// <summary>
    /// Handles the friend request response.
    /// </summary>
    /// <param name="characterName">The character name of the responder.</param>
    /// <param name="friendName">The character name of the requester.</param>
    /// <param name="accepted">Indicating whether the request got accepted.</param>
    ValueTask FriendResponseAsync(string characterName, string friendName, bool accepted);

    /// <summary>
    /// Is called when a player entered the game.
    /// It will cause a response with <see cref="IFriendSystemSubscriber.InitializeMessengerAsync"/>
    /// and a state update for friends.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName);

    /// <summary>
    /// Is called when a player leaves the game.
    /// It will cause a state update for friends.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    ValueTask PlayerLeftGameAsync(Guid characterId, string characterName);

    /// <summary>
    /// Sets the online visibility state of a character.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="characterId">Id of the character.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="isVisible">If set to <c>true</c>, the character is visible as online. Otherwise, it appears as offline for other players, but is still online</param>
    ValueTask SetPlayerVisibilityStateAsync(byte serverId, Guid characterId, string characterName, bool isVisible);

    /// <summary>
    /// Sends a friend request to the friend, and adds a new friend view item to the players friend list.
    /// </summary>
    /// <param name="playerName">The name of the requesting player.</param>
    /// <param name="friendName">The name of the requested friend.</param>
    /// <returns>If a new friend view item got added to the players friend list.</returns>
    ValueTask<bool> FriendRequestAsync(string playerName, string friendName);

    /// <summary>
    /// Deletes the friend.
    /// </summary>
    /// <param name="name">The player who is deleting a friend from his friend list.</param>
    /// <param name="friendName">Name of the friend who should be deleted.</param>
    ValueTask DeleteFriendAsync(string name, string friendName);

    /// <summary>
    /// Creates a new chat room.
    /// </summary>
    /// <param name="playerName">Name of the player who is creating the chat room.</param>
    /// <param name="friendName">Name of the friend who should be invited to the chat room.</param>
    ValueTask CreateChatRoomAsync(string playerName, string friendName);

    /// <summary>
    /// Invites a friend to an existing chat room.
    /// </summary>
    /// <param name="selectedCharacterName">Name of the selected character.</param>
    /// <param name="friendName">Name of the friend.</param>
    /// <param name="roomNumber">The room number.</param>
    /// <returns>The success of the invitation.</returns>
    ValueTask<bool> InviteFriendToChatRoomAsync(string selectedCharacterName, string friendName, ushort roomNumber);
}