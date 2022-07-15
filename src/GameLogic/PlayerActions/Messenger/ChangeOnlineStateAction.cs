// <copyright file="ChangeOnlineStateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

/// <summary>
/// Action to change the online state for the messenger.
/// </summary>
public class ChangeOnlineStateAction
{
    /// <summary>
    /// Sets the online state.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="online">If set to <c>true</c> the player is visible as online with its server id to his friends. Otherwise, not.</param>
    public async ValueTask SetOnlineStateAsync(Player player, bool online)
    {
        player.OnlineAsFriend = online;
        if (player.GameContext is IGameServerContext gameServerContext && player.SelectedCharacter is { } character)
        {
            await gameServerContext.FriendServer.SetPlayerVisibilityStateAsync(gameServerContext.Id, character.Id, player.SelectedCharacter.Name, player.OnlineAsFriend).ConfigureAwait(false);
        }
    }
}