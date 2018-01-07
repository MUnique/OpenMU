// <copyright file="SetItemPriceAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    /// <summary>
    /// Action to set the price of an item of the player store.
    /// </summary>
    public class SetItemPriceAction
    {
        /// <summary>
        /// Sets the price of an item.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="slot">The slot of the item in the store (0 to 31).</param>
        /// <param name="price">The price.</param>
        public void SetPrice(Player player, int slot, uint price)
        {
            player.ShopStorage.StorePrices[slot] = price;

            // todo: response?
        }
    }
}
