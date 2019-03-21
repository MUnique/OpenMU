// <copyright file="IItemDurabilityChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about a durability change of an item.
    /// </summary>
    public interface IItemDurabilityChangedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the durability of the item changed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="causedByConsumption">Flag which indicates if the durability change was caused by consuming the item.</param>
        void ItemDurabilityChanged(Item item, bool causedByConsumption);
    }
}