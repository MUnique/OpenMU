// <copyright file="IItemSoldToNpcPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory
{
    /// <summary>
    /// Interface of a view whose implementation informs about an item which got sold to the npc merchant (or not).
    /// </summary>
    public interface IItemSoldToNpcPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Notifies the client that the item got sold to the npc (or not).
        /// </summary>
        /// <param name="success">If set to <c>true</c> the item has been sold; otherwise not.</param>
        void ItemSoldToNpc(bool success);
    }
}