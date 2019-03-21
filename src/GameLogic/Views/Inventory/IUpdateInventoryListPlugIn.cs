// <copyright file="IUpdateInventoryListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about the current inventory item list.
    /// </summary>
    public interface IUpdateInventoryListPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the inventory list.
        /// </summary>
        void UpdateInventoryList();
    }
}