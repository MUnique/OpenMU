// <copyright file="IItemDestroyedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin interface which is called when an item got destroyed,
    /// e.g. because it has been consumed, destroyed by the chaos machine or disappeared as item drop.
    /// </summary>
    [PlugInPoint("Item destroyed", "Is called when an item got destroyed, e.g. because it has been consumed or destroyed by the chaos machine or disappeared as item drop.")]
    [Guid("C001640F-4630-45E2-9EA3-C574B62E7738")]
    public interface IItemDestroyedPlugIn
    {
        /// <summary>
        /// Is called when an item got destroyed, e.g. because it has been consumed or destroyed by the chaos machine
        /// or disappeared as item drop.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemDestroyed(Item item);
    }
}