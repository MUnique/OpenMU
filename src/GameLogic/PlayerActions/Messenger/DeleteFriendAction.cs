// <copyright file="DeleteFriendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to delete a friend from the friendlist.
    /// </summary>
    public class DeleteFriendAction
    {
        private readonly IFriendServer friendServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFriendAction"/> class.
        /// </summary>
        /// <param name="friendServer">The friend server.</param>
        public DeleteFriendAction(IFriendServer friendServer)
        {
            this.friendServer = friendServer;
        }

        /// <summary>
        /// Deletes the friend.
        /// </summary>
        /// <param name="player">The player who wants to delete the friend from his friendlist.</param>
        /// <param name="friendName">Name of the friend which should get deleted from the friendlist.</param>
        public void DeleteFriend(Player player, string friendName)
        {
            this.friendServer.DeleteFriend(player.SelectedCharacter.Name, friendName);
            player.PlayerView.MessengerView.FriendDeleted(friendName);
        }
    }
}
