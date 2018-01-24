// <copyright file="SetItemPriceAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using System.Linq;

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
        public void SetPrice(Player player, int slot, int price)
        {
            var item = player.SelectedCharacter?.Inventory?.Items?.FirstOrDefault(i => i.ItemSlot == slot);
            if (item != null)
            {
                item.StorePrice = price > 0 ? price : (int?)null;
            }

            // todo: response?
        }
    }
}
