// <copyright file="AddResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using MUnique.OpenMU.GameLogic.Views.Messenger;

    /// <summary>
    /// Action to respond to a friend request.
    /// </summary>
    public class AddResponseAction
    {
        /// <summary>
        /// Proceeds the response.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="requesterName">Name of the requester.</param>
        /// <param name="accepted">if set to <c>true</c> the request has been accepted.</param>
        public void ProceedResponse(Player player, string requesterName, bool accepted)
        {
            if (string.IsNullOrEmpty(requesterName))
            {
                // this happens after a letter has been sent to an unknown character
                return;
            }

            var friendServer = (player.GameContext as IGameServerContext)?.FriendServer;
            if (friendServer != null)
            {
                if (accepted)
                {
                    player.ViewPlugIns.GetPlugIn<IFriendAddedPlugIn>()?.FriendAdded(requesterName);
                }

                friendServer.FriendResponse(player.SelectedCharacter.Name, requesterName, accepted);
            }
        }
    }
}
