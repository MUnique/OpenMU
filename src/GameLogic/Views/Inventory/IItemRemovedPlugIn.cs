// <copyright file="IItemRemovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which got removed from the inventory.
    /// </summary>
    public interface IItemRemovedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that an item got removed.
        /// </summary>
        /// <param name="inventorySlot">The inventory slot of the removed item.</param>
        void RemoveItem(byte inventorySlot);
    }
}