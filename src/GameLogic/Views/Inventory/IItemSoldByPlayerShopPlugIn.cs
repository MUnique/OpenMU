// <copyright file="IItemSoldByPlayerShopPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which got sold to another player through the players shop.
    /// </summary>
    public interface IItemSoldByPlayerShopPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that his item from the player shop has been sold to another player.
        /// </summary>
        /// <param name="slot">The slot of the item.</param>
        /// <param name="buyer">The buyer.</param>
        void ItemSoldByPlayerShop(byte slot, Player buyer);
    }
}