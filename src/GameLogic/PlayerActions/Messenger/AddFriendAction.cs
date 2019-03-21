// <copyright file="AddFriendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using MUnique.OpenMU.GameLogic.Views.Messenger;

    /// <summary>
    /// Action to add a friend to the friendlist.
    /// </summary>
    public class AddFriendAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFriendAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AddFriendAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Adds the friend to the friendlist.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="friendName">Name of the friend.</param>
        public void AddFriend(Player player, string friendName)
        {
            bool isNewFriend = this.gameContext.FriendServer.FriendRequest(player.SelectedCharacter.Name, friendName);
            if (isNewFriend)
            {
                player.ViewPlugIns.GetPlugIn<IFriendAddedPlugIn>()?.FriendAdded(friendName);
            }
        }
    }
}
