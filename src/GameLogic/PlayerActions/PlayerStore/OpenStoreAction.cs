// <copyright file="OpenStoreAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using log4net;

    /// <summary>
    /// Action to open a player store.
    /// </summary>
    public class OpenStoreAction
    {
        private static ILog log = LogManager.GetLogger(typeof(OpenStoreAction));

        /// <summary>
        /// Opens the player store.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="storeName">Name of the store.</param>
        public void OpenStore(Player player, string storeName)
        {
            player.ShopStorage.StoreName = storeName;
            player.ShopStorage.StoreOpen = true;
            log.DebugFormat("OpenStore: Player: [{0}], StoreName: [{1}]", player.SelectedCharacter.Name, player.ShopStorage.StoreName);
            player.ForEachObservingPlayer(p => p.PlayerView.PlayerShopOpened(player), true);
        }
    }
}
