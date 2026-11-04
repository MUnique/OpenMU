// <copyright file="IMonsterItemDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a killed monster dropped an item.
/// </summary>
/// <remarks>
/// It allows to react on rare drops, e.g. to announce excellent or ancient items.
/// Item drops of players are handled by the <see cref="IItemDropPlugIn"/>.
/// </remarks>
[Guid("0AA658CC-0716-455E-800F-7275EEAD6752")]
[PlugInPoint("Monster item dropped", "Plugins which will be executed when a killed monster dropped an item.")]
public interface IMonsterItemDroppedPlugIn
{
    /// <summary>
    /// Is called when a killed monster dropped an item to the ground.
    /// </summary>
    /// <param name="monster">The killed monster.</param>
    /// <param name="killer">The player who killed the monster.</param>
    /// <param name="droppedItem">The dropped item, which is already on the map.</param>
    ValueTask MonsterItemDroppedAsync(AttackableNpcBase monster, Player killer, DroppedItem droppedItem);
}
