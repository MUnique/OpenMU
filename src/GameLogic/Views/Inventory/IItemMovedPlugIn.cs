// <copyright file="IItemMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an item which has moved in the inventory.
    /// </summary>
    public interface IItemMovedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// An item got moved.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="toSlot">The new slot index.</param>
        /// <param name="storage">The new storage.</param>
        void ItemMoved(Item item, byte toSlot, Storages storage);
    }
}
