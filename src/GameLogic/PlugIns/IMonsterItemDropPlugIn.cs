// <copyright file="IMonsterItemDropPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a killed monster is about to drop an item.
/// </summary>
[Guid("2E90D8A9-96BD-41FF-82B9-7A9D97CE817E")]
[PlugInPoint("Monster item dropping", "Plugins which are called when a killed monster is about to drop an item. They can cancel the drop.")]
public interface IMonsterItemDropPlugIn
{
    /// <summary>
    /// Is called when a killed monster is about to drop an item.
    /// </summary>
    /// <param name="monster">The monster.</param>
    /// <param name="killer">The player which killed the monster.</param>
    /// <param name="item">The item which is about to drop.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. Allows to cancel the drop.</param>
    void ItemDropping(AttackableNpcBase monster, Player killer, Item item, CancelEventArgs eventArgs);
}
