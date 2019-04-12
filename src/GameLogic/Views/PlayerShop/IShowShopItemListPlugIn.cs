// <copyright file="IShowShopItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.PlayerShop
{
    /// <summary>
    /// Interface of a view whose implementation informs about the shop item list which was requested by the player previously.
    /// </summary>
    public interface IShowShopItemListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the shop item list of the requested players shop.
        /// </summary>
        /// <param name="requestedPlayer">The requested player.</param>
        /// <param name="isUpdate">If set to <c>true</c>, the sent list is an update of an already received list, e.g. after an item has been sold.</param>
        void ShowShopItemList(Player requestedPlayer, bool isUpdate);
    }
}