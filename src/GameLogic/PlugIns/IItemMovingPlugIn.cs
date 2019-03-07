// <copyright file="IItemMovingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item is about to be moved by the player.
    /// </summary>
    [Guid("B652F4DF-6BE9-430E-BD83-DD5FBB1228D2")]
    [PlugInPoint("Item moving", "Plugins which are called when an item is about to be moved by the player.")]
    public interface IItemMovingPlugIn
    {
        /// <summary>
        /// Is called when an item is about to be moved by the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The item.</param>
        /// <param name="targetStorage">The target storage.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The move can be cancelled by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.</param>
        void ItemMoving(Player player, Item item, Storages targetStorage, int slot, CancelEventArgs eventArgs);
    }
}