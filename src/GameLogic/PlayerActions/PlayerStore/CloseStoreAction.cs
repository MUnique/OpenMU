// <copyright file="CloseStoreAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

using MUnique.OpenMU.GameLogic.Views.PlayerShop;

/// <summary>
/// Action to close the own player store.
/// </summary>
public class CloseStoreAction
{
    /// <summary>
    /// Closes the store of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask CloseStoreAsync(Player player)
    {
        if (player.ShopStorage is null)
        {
            return;
        }

        using (await player.ShopStorage.StoreLock.LockAsync())
        {
            player.ShopStorage.StoreOpen = false;
        }

        await player.ForEachWorldObserverAsync<IPlayerShopClosedPlugIn>(plugin => plugin.PlayerShopClosedAsync(player), true).ConfigureAwait(false);
    }
}