// <copyright file="IItemConsumedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called after an item got consumed.
    /// </summary>
    [Guid("C6407782-58C7-460D-B01D-CA6D15C742E1")]
    [PlugInPoint("Item consumed", "Plugins which will be executed when an item got consumed.")]
    public interface IItemConsumedPlugIn
    {
        /// <summary>
        /// Is called after an item got consumed.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The item.</param>
        /// <param name="targetItem">The target item, e.g. an armor piece which gets upgraded by the consumption of a jewel.</param>
        void ItemConsumed(Player player, Item item, Item targetItem);
    }
}