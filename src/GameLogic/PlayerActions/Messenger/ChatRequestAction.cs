﻿// <copyright file="ChatRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to request a chat with a friend with the messenger.
    /// </summary>
    public class ChatRequestAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRequestAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChatRequestAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Requests the chat.
        /// </summary>
        /// <param name="player">The player who requests the chat.</param>
        /// <param name="friendName">Name of the friend, with which the player wants to chat.</param>
        public void RequestChat(Player player, string friendName)
        {
            var authenticationInfo = this.gameContext.FriendServer.CreateChatRoom(player.SelectedCharacter.Name, friendName);
            if (authenticationInfo != null)
            {
                player.ViewPlugIns.GetPlugIn<IMessengerView>()?.ChatRoomCreated(authenticationInfo, friendName, true);
            }
        }

        /// <summary>
        /// Invites the friend to an existing chat room.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="friendName">Name of the friend.</param>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        public void InviteFriendToChat(Player player, string friendName, ushort roomId, uint requestId)
        {
            var result = this.gameContext.FriendServer.InviteFriendToChatRoom(player.SelectedCharacter.Name, friendName, roomId);
            player.ViewPlugIns.GetPlugIn<IMessengerView>()?.ShowFriendInvitationResult(result, requestId);
        }
    }
}
