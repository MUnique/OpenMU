// <copyright file="OpenStoreAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;

using MUnique.OpenMU.DataModel.Configuration.Items;
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
    public async ValueTask OpenStoreAsync(Player player, string storeName)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var character = player.SelectedCharacter;
        if (character is null)
        {
            player.Logger.LogWarning("OpenStore request failed: No character selected.");
            return;
        }

        if (player.ShopStorage?.Items.Any(i => !i.StorePrice.HasValue) ?? true)
        {
            player.Logger.LogWarning("OpenStore request failed: Not all store items have a price assigned. Player: [{0}], StoreName: [{1}]", character.Name, character.StoreName);
            return;
        }

        if (player.ShopStorage?.Items.Any(i => i.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption)) ?? true)
        {
            player.Logger.LogWarning("OpenStore request failed: Items with an harmony option can't be traded. Player: [{0}], StoreName: [{1}]", character.Name, character.StoreName);
            return;
        }

        character.StoreName = storeName;
        player.ShopStorage.StoreOpen = true;
        player.Logger.LogDebug("OpenStore: Player: [{0}], StoreName: [{1}]", character.Name, character.StoreName);
        await player.ForEachWorldObserverAsync<IPlayerShopOpenedPlugIn>(p => p.PlayerShopOpenedAsync(player), true).ConfigureAwait(false);
    }
}