// <copyright file="IItemDropPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an item has been dropped to the ground by the player.
/// </summary>
[Guid("EDB62A52-96BA-4ACA-9355-244834D53437")]
[PlugInPoint("Item dropping", "Plugins which are called when an item has been dropped to the ground by the player.")]
public interface IItemDropPlugIn
{
    /// <summary>
    /// Is called when an item has been dropped to the ground by the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="item">The item.</param>
    /// <param name="target">The target point on the ground.</param>
    /// <param name="dropArgs">The <see cref="ItemDropArguments" /> instance containing the event data.
    /// A plugin can define what's happening after the plugin has been executed.</param>
    /// <returns>
    ///   <c>true</c>, when the item should be removed and deleted from the inventory; otherwise, <c>false</c>.
    /// </returns>
    ValueTask HandleItemDropAsync(Player player, Item item, Point target, ItemDropArguments dropArgs);

    /// <summary>
    /// Arguments for handling the item drop.
    /// </summary>
    public class ItemDropArguments : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the item drop request was handled.
        /// In this case <see cref="CancelEventArgs.Cancel"/> is set to <c>true</c> to prevent further plugins to execute.
        /// </summary>
        public bool WasHandled
        {
            get => this.Cancel;
            set => this.Cancel = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item should be removed and deleted from the inventory
        /// when also <see cref="CancelEventArgs.Cancel"/> is set to <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remove item]; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }
    }
}