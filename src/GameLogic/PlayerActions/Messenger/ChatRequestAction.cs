// <copyright file="ChatRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
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
                player.PlayerView.MessengerView.ChatRoomCreated(authenticationInfo, friendName, true);
            }
        }
    }
}
