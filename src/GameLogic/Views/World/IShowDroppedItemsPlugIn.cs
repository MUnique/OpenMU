// <copyright file="IShowDroppedItemsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface of a view whose implementation informs about dropped items.
    /// </summary>
    public interface IShowDroppedItemsPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the dropped items.
        /// </summary>
        /// <param name="droppedItems">The dropped items.</param>
        /// <param name="freshDrops">if set to <c>true</c> this items are fresh drops; Otherwise they are already laying on the ground when reaching a newly discovered part of the map.</param>
        void ShowDroppedItems(IEnumerable<DroppedItem> droppedItems, bool freshDrops);
    }
}