// <copyright file="StoreItemListRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to request the item list of a player store.
    /// </summary>
    public class StoreItemListRequestAction
    {
        /// <summary>
        /// Requests the store item list.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="requestedPlayer">The requested player.</param>
        public void RequestStoreItemList(Player player, Player requestedPlayer)
        {
            if (!requestedPlayer.ShopStorage.StoreOpen)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Player's Store not open.", MessageType.BlueNormal);
                return;
            }

            player.ViewPlugIns.GetPlugIn<IShowShopItemListPlugIn>()?.ShowShopItemList(requestedPlayer, false);
        }
    }
}
