// <copyright file="IItemPickUpFailedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which didn't get picked up.
    /// </summary>
    public interface IItemPickUpFailedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the previous attempt to pick up an item failed.
        /// </summary>
        /// <param name="reason">The reason for the fail.</param>
        void ItemPickUpFailed(ItemPickFailReason reason);
    }
}