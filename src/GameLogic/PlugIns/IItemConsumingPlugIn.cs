// <copyright file="IItemConsumingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item is about to get consumed.
    /// </summary>
    [Guid("73BB132C-F5E1-4A38-9FD8-370ABD4CD856")]
    [PlugInPoint("Item consuming", "Plugins which will be executed when an item is about to get consumed.")]
    public interface IItemConsumingPlugIn
    {
        /// <summary>
        /// Is called when an item is about to get consumed.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The item.</param>
        /// <param name="targetItem">The target item, e.g. an armor piece which gets upgraded by the consumption of a jewel.</param>
        /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. Allows to cancel the consumption.</param>
        void ItemConsuming(Player player, Item item, Item targetItem, CancelEventArgs eventArgs);
    }
}