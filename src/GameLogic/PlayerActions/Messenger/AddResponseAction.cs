// <copyright file="AddResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    /// <summary>
    /// Action to respond to a friend request.
    /// </summary>
    public class AddResponseAction
    {
        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddResponseAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AddResponseAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Proceeds the reponse.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="requesterName">Name of the requester.</param>
        /// <param name="accepted">if set to <c>true</c> the request has been accepted.</param>
        public void ProceedReponse(Player player, string requesterName, bool accepted)
        {
            if (accepted)
            {
                player.PlayerView.MessengerView.FriendAdded(requesterName);
            }

            this.gameContext.FriendServer.FriendResponse(player.SelectedCharacter.Name, requesterName, accepted);
        }
    }
}
