// <copyright file="IItemDropResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which got dropped from the inventory (or not).
    /// </summary>
    public interface IItemDropResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that an item got dropped from the inventory (or not).
        /// </summary>
        /// <param name="slot">The slot from which the item has been dropped (or not).</param>
        /// <param name="success">If set to <c>true</c> the item has been dropped; otherwise not.</param>
        void ItemDropResult(byte slot, bool success);
    }
}