// <copyright file="IItemUpgradedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an upgraded item in the inventory.
    /// </summary>
    public interface IItemUpgradedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Item has been upgraded.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemUpgraded(Item item);
    }
}