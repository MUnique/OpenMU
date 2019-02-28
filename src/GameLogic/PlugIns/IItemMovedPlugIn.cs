// <copyright file="IItemMovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item got moved by the player.
    /// </summary>
    [Guid("DA6AD004-9343-423A-A669-F4DC2CCE3CA1")]
    [PlugInPoint("Item moved", "Plugins which are called when an item has been moved by the player.")]
    public interface IItemMovedPlugIn
    {
        /// <summary>
        /// Is called when an item has been moved by the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="item">The item.</param>
        void ItemMoved(Player player, Item item);
    }
}