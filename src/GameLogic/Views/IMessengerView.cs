// <copyright file="IMessengerView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// The letter send success flag.
    /// </summary>
    public enum LetterSendSuccess : byte
    {
        /// <summary>
        /// There was a problem and he should try again.
        /// </summary>
        TryAgain = 0,

        /// <summary>
        /// The letter has been sent successfully.
        /// </summary>
        Success = 1,

        /// <summary>
        /// The mailbox of the recipient is full.
        /// </summary>
        MailboxFull = 2,

        /// <summary>
        /// The receiver does not exist.
        /// </summary>
        ReceiverNotExists = 3,

        /// <summary>
        /// A letter can't be sent to yourself.
        /// </summary>
        CantSendToYourself = 4
    }

    /// <summary>
    /// The view for the messenger (MUssenger).
    /// </summary>
    public interface IMessengerView
    {
        /// <summary>
        /// Initializes the messenger. Adds the letters and the friends to the view.
        /// </summary>
        /// <param name="maxLetters">The maximum number of letters a player can have in its inbox.</param>
        void InitializeMessenger(int maxLetters);

        /// <summary>
        /// Adds a letter to the view.
        /// </summary>
        /// <param name="letter">Letter which should be added.</param>
        /// <param name="index">The index of the letter in the letter list.</param>
        /// <param name="newLetter">Determines if this letter is new, that means it got just sent.</param>
        void AddToLetterList(LetterHeader letter, ushort index, bool newLetter);

        /// <summary>
        /// Shows the friend request.
        /// </summary>
        /// <param name="requester">The requesters character name.</param>
        void ShowFriendRequest(string requester);

        /// <summary>
        /// Updates the online state (server id) of a friend.
        /// </summary>
        /// <param name="friend">The friends character name.</param>
        /// <param name="serverId">The server identifier.</param>
        void FriendStateUpdate(string friend, int serverId);

        /// <summary>
        /// A friend has been added to the friendlist.
        /// </summary>
        /// <param name="friendname">The character name of the friend.</param>
        void FriendAdded(string friendname);

        /// <summary>
        /// A friend has been deleted from the friendlist.
        /// </summary>
        /// <param name="deletedFriend">The deleted friend.</param>
        void FriendDeleted(string deletedFriend);

        /// <summary>
        /// A chat room has been created for this player and his friend.
        /// </summary>
        /// <param name="authenticationInfo">The id of the created chat room.</param>
        /// <param name="friendName">The character name of the friend which is expected in the chat room.</param>
        /// <param name="success">If set to <c>true</c>, the chat room has been created successfully; Otherwise not.</param>
        void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string friendName, bool success);

        /// <summary>
        /// Shows the letter body.
        /// </summary>
        /// <param name="letterBody">The letter body.</param>
        void ShowLetter(LetterBody letterBody);

        /// <summary>
        /// The letter has been deleted.
        /// </summary>
        /// <param name="letterIndex">The letter header index.</param>
        void LetterDeleted(ushort letterIndex);

        /// <summary>
        /// Shows the letter send result, of the previously sent letter.
        /// </summary>
        /// <param name="success">The success.</param>
        void LetterSendResult(LetterSendSuccess success);
    }
}
