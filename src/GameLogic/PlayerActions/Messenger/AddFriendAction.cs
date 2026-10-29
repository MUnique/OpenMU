// <copyright file="AddFriendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

using MUnique.OpenMU.GameLogic.Views.Messenger;

/// <summary>
/// Action to add a friend to the friend list.
/// </summary>
public class AddFriendAction
{
    /// <summary>
    /// Adds the friend to the friend list.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="friendName">Name of the friend.</param>
    public async ValueTask AddFriendAsync(Player player, string friendName)
    {
        var friendServer = (player.GameContext as IGameServerContext)?.FriendServer;
        if (friendServer != null && player.SelectedCharacter is { } character)
        {
            // A name which no character has would fail silently further down (the friend row gets an
            // empty FriendId and is not saved), while the client already said "request sent".
            if (!await CharacterExistsAsync(player, friendName).ConfigureAwait(false))
            {
                await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), friendName).ConfigureAwait(false);
                return;
            }

            bool isNewFriend = await friendServer.FriendRequestAsync(character.Name, friendName).ConfigureAwait(false);
            if (isNewFriend)
            {
                await player.InvokeViewPlugInAsync<IFriendAddedPlugIn>(p => p.FriendAddedAsync(friendName)).ConfigureAwait(false);
            }
        }
    }

    private static async ValueTask<bool> CharacterExistsAsync(Player player, string characterName)
    {
        using var context = player.GameContext.PersistenceContextProvider.CreateNewFriendServerContext();
        return await context.CharacterExistsAsync(characterName).ConfigureAwait(false);
    }
}
