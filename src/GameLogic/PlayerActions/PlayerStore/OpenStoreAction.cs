// <copyright file="OpenStoreAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

using MUnique.OpenMU.GameLogic.Views.PlayerShop;

/// <summary>
/// Action to open a player store.
/// </summary>
public class OpenStoreAction
{
    /// <summary>
    /// Opens the player store.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="storeName">Name of the store.</param>
    public void OpenStore(Player player, string storeName)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.ShopStorage?.Items.Any(i => !i.StorePrice.HasValue) ?? true)
        {
            player.Logger.LogWarning("OpenStore request failed: Not all store items have a price assigned. Player: [{0}], StoreName: [{1}]", player.SelectedCharacter?.Name, player.ShopStorage?.StoreName);
            return;
        }

        player.ShopStorage.StoreName = storeName;
        player.ShopStorage.StoreOpen = true;
        player.Logger.LogDebug("OpenStore: Player: [{0}], StoreName: [{1}]", player.SelectedCharacter!.Name, player.ShopStorage.StoreName);
        player.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IPlayerShopOpenedPlugIn>()?.PlayerShopOpened(player), true);
    }
}