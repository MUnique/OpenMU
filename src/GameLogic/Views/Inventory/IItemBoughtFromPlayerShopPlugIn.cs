// <copyright file="IItemBoughtFromPlayerShopPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an item which has been bought
    /// from another players shop.
    /// </summary>
    public interface IItemBoughtFromPlayerShopPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that an item has been bought from another players shop.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemBoughtFromPlayerShop(Item item);
    }
}