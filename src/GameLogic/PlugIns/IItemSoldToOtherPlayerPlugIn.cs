// <copyright file="IItemSoldToOtherPlayerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item got sold to another player through the personal store.
    /// </summary>
    [PlugInPoint("Item sold to other player", "Is called when an item got sold to another player through the personal store.")]
    [Guid("5899A530-FC78-4A6E-82EB-7267A95107D6")]
    public interface IItemSoldToOtherPlayerPlugIn
    {
        /// <summary>
        /// Is called when an item got sold to another player through the personal store.
        /// </summary>
        /// <param name="seller">The seller.</param>
        /// <param name="item">The item.</param>
        /// <param name="buyer">The buyer.</param>
        void ItemSold(Player seller, Item item, Player buyer);
    }
}