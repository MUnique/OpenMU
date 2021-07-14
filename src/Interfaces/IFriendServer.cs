// -----------------------------------------------------------------------
// <copyright file="IFriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.Interfaces
{
    using System;
    using System.Collections.Generic;

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
        /// Gets the chat server ip.
        /// </summary>
        /// <returns>The chat server ip.</returns>
        string ChatServerIp { get; }

        /// <summary>
        /// Forwards the letter.
        /// </summary>
        /// <param name="letter">The letter.</param>
        void ForwardLetter(LetterHeader letter);

        /// <summary>
        /// Handles the friend request response.
        /// </summary>
        /// <param name="characterName">The character name of the responder.</param>
        /// <param name="friendName">The character name of the requester.</param>
        /// <param name="accepted">Indicating whether the request got accepted.</param>
        void FriendResponse(string characterName, string friendName, bool accepted);

        /// <summary>
        /// Creates a new chat room.
        /// </summary>
        /// <param name="playerName">Name of the player who is creating the chat room.</param>
        /// <param name="friendName">Name of the friend who should be invited to the chat room.</param>
        /// <returns>The authentication information for the <paramref name="playerName"/>.</returns>
        ChatServerAuthenticationInfo? CreateChatRoom(string playerName, string friendName);

        /// <summary>
        /// Gets the friend list of a character.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <returns>The friend list of a character.</returns>
        IEnumerable<string> GetFriendList(Guid characterId);

        /// <summary>
        /// Sets the online state of a character.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="serverId">The server identifier.</param>
        void SetOnlineState(Guid characterId, string characterName, int serverId);

        /// <summary>
        /// Sends a friend request to the friend, and adds a new friend view item to the players friend list.
        /// </summary>
        /// <param name="playerName">The name of the requesting player.</param>
        /// <param name="friendName">The name of the requested friend.</param>
        /// <returns>If a new friend view item got added to the players friend list.</returns>
        bool FriendRequest(string playerName, string friendName);

        /// <summary>
        /// Deletes the friend.
        /// </summary>
        /// <param name="name">The player who is deleting a friend from his friendlist.</param>
        /// <param name="friendName">Name of the friend who should be deleted.</param>
        void DeleteFriend(string name, string friendName);

        /// <summary>
        /// Returns the character names of all open friend requests.
        /// </summary>
        /// <param name="characterId">Character id.</param>
        /// <returns>The character names of all open friend requests.</returns>
        IEnumerable<string> GetOpenFriendRequests(Guid characterId);

        /// <summary>
        /// Invites a friend to an existing chat room.
        /// </summary>
        /// <param name="selectedCharacterName">Name of the selected character.</param>
        /// <param name="friendName">Name of the friend.</param>
        /// <param name="roomNumber">The room number.</param>
        /// <returns>The success of the invitation.</returns>
        bool InviteFriendToChatRoom(string selectedCharacterName, string friendName, ushort roomNumber);
    }
}
