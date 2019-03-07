// <copyright file="IItemTradedToOtherPlayerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item got traded to another player.
    /// </summary>
    [PlugInPoint("Item traded", "Is called when an item got traded to another player.")]
    [Guid("30DC381B-3A32-4824-8A47-A24B985C3C62")]
    public interface IItemTradedToOtherPlayerPlugIn
    {
        /// <summary>
        /// Is called when an item got traded to another player.
        /// </summary>
        /// <param name="source">The source player.</param>
        /// <param name="target">The target player.</param>
        /// <param name="item">The traded item.</param>
        void ItemTraded(ITrader source, ITrader target, Item item);
    }
}