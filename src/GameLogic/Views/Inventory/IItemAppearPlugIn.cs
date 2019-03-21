// <copyright file="IItemAppearPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about a new item which appeared in the inventory.
    /// </summary>
    public interface IItemAppearPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that a new item appears in the inventory.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        void ItemAppear(Item newItem);
    }
}