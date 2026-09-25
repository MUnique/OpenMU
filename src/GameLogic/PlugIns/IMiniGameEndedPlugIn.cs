// <copyright file="IMiniGameEndedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a mini game (e.g. Blood Castle, Devil Square, Chaos Castle) has been ended.
/// </summary>
[Guid("B7C3E1A2-8D4F-4E6B-A915-2C7D3F8E6B10")]
[PlugInPoint("Mini game ended", "Plugins which will be executed when a mini game has been ended.")]
public interface IMiniGameEndedPlugIn
{
    /// <summary>
    /// Is called when a mini game has been ended.
    /// </summary>
    /// <param name="miniGame">The mini game.</param>
    /// <param name="finishers">The players which stayed in the game until its end.</param>
    ValueTask MiniGameEndedAsync(MiniGameContext miniGame, ICollection<Player> finishers);
}
