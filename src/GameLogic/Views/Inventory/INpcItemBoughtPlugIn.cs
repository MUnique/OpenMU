// <copyright file="INpcItemBoughtPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about an item which got bought from an npc merchant.
    /// </summary>
    public interface INpcItemBoughtPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the item got bought from the npc.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        void NpcItemBought(Item newItem);
    }
}