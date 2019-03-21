// <copyright file="IItemConsumedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which got consumed (or not).
    /// </summary>
    public interface IItemConsumedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that an item got consumed (or not).
        /// </summary>
        /// <param name="inventorySlot">The inventory slot.</param>
        /// <param name="success">If set to <c>true</c> the item got consumed; otherwise not.</param>
        void ItemConsumed(byte inventorySlot, bool success);
    }
}