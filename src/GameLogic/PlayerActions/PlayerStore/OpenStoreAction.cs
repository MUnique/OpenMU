// <copyright file="OpenStoreAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;

    /// <summary>
    /// Action to open a player store.
    /// </summary>
    public class OpenStoreAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenStoreAction));

        /// <summary>
        /// Opens the player store.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="storeName">Name of the store.</param>
        public void OpenStore(Player player, string storeName)
        {
            if (player.ShopStorage.Items.Any(i => !i.StorePrice.HasValue))
            {
                Log.WarnFormat("OpenStore request failed: Not all store items have a price assigned. Player: [{0}], StoreName: [{1}]", player.SelectedCharacter.Name, player.ShopStorage.StoreName);
                return;
            }

            player.ShopStorage.StoreName = storeName;
            player.ShopStorage.StoreOpen = true;
            Log.DebugFormat("OpenStore: Player: [{0}], StoreName: [{1}]", player.SelectedCharacter.Name, player.ShopStorage.StoreName);
            player.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IPlayerShopOpenedPlugIn>()?.PlayerShopOpened(player), true);
        }
    }
}
