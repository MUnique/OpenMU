// <copyright file="DeleteFriendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

using MUnique.OpenMU.GameLogic.Views.Messenger;

/// <summary>
/// Action to delete a friend from the friend list.
/// </summary>
public class DeleteFriendAction
{
    /// <summary>
    /// Deletes the friend.
    /// </summary>
    /// <param name="player">The player who wants to delete the friend from his friend list.</param>
    /// <param name="friendName">Name of the friend which should get deleted from the friend list.</param>
    public async ValueTask DeleteFriendAsync(Player player, string friendName)
    {
        var friendServer = (player.GameContext as IGameServerContext)?.FriendServer;
        if (friendServer != null && player.SelectedCharacter is { } character)
        {
            await friendServer.DeleteFriendAsync(character.Name, friendName).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IFriendDeletedPlugIn>(p => p.FriendDeletedAsync(friendName)).ConfigureAwait(false);
        }
    }
}