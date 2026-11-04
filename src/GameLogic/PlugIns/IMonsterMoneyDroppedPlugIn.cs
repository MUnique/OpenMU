// <copyright file="IMonsterMoneyDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a killed monster dropped money.
/// </summary>
/// <remarks>
/// Items dropped by monsters are handled by the <see cref="IMonsterItemDroppedPlugIn"/>.
/// </remarks>
[Guid("6F2B8C1D-5E4A-4F9B-A3D7-0C1E2B3A4D5F")]
[PlugInPoint("Monster money dropped", "Plugins which will be executed when a killed monster dropped money.")]
public interface IMonsterMoneyDroppedPlugIn
{
    /// <summary>
    /// Is called when a killed monster dropped money to the ground.
    /// </summary>
    /// <param name="monster">The killed monster.</param>
    /// <param name="killer">The player who killed the monster.</param>
    /// <param name="droppedMoney">The dropped money, which is already on the map.</param>
    ValueTask MonsterMoneyDroppedAsync(AttackableNpcBase monster, Player killer, DroppedMoney droppedMoney);
}
