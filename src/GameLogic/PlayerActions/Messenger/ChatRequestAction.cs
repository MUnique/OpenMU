// <copyright file="ChatRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

using MUnique.OpenMU.GameLogic.Views.Messenger;

/// <summary>
/// Action to request a chat with a friend with the messenger.
/// </summary>
public class ChatRequestAction
{
    /// <summary>
    /// Requests the chat.
    /// </summary>
    /// <param name="player">The player who requests the chat.</param>
    /// <param name="friendName">Name of the friend, with which the player wants to chat.</param>
    public async ValueTask RequestChatAsync(Player player, string friendName)
    {
        var character = player.SelectedCharacter;
        if (character is null)
        {
            return;
        }

        var friendServer = (player.GameContext as IGameServerContext)?.FriendServer;
        if (friendServer is { })
        {
            await friendServer.CreateChatRoomAsync(character.Name, friendName).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Invites the friend to an existing chat room.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="friendName">Name of the friend.</param>
    /// <param name="roomId">The room identifier.</param>
    /// <param name="requestId">The request identifier.</param>
    public async ValueTask InviteFriendToChatAsync(Player player, string friendName, ushort roomId, uint requestId)
    {
        var character = player.SelectedCharacter;
        if (character is null)
        {
            return;
        }

        var friendServer = (player.GameContext as IGameServerContext)?.FriendServer;
        if (friendServer is { })
        {
            var result = await friendServer.InviteFriendToChatRoomAsync(character.Name, friendName, roomId).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowFriendInvitationResultPlugIn>(p => p.ShowFriendInvitationResultAsync(result, requestId)).ConfigureAwait(false);
        }
    }
}