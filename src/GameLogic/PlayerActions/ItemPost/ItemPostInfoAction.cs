// <copyright file="ItemPostInfoAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemPost;

using MUnique.OpenMU.GameLogic.ItemPost;
using MUnique.OpenMU.GameLogic.Views.ItemPost;

/// <summary>
/// Handles requests for posted item information.
/// </summary>
public class ItemPostInfoAction
{
    /// <summary>
    /// Sends the posted item information to the player if available.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="postId">The post identifier.</param>
    public async ValueTask SendItemInfoAsync(Player player, uint postId)
    {
        var postState = player.GameContext.GetItemPostState();
        if (!postState.TryGetItem(postId, out var item) || item is null)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<IItemPostInfoViewPlugIn>(p => p.ShowItemPostInfoAsync(postId, item)).ConfigureAwait(false);
    }
}
