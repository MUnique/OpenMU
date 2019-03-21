// <copyright file="IItemMoveFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an item which couldn't be moved in the inventory.
    /// </summary>
    public interface IItemMoveFailedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Moving an item failed.
        /// </summary>
        /// <param name="item">The item which could not be moved. Null, if requested item could not be determined.</param>
        void ItemMoveFailed(Item item);
    }
}