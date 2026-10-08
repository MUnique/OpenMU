// <copyright file="IMiniGameEndedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a mini game ended.
/// </summary>
[Guid("FFB2B5E3-DF02-43FB-A713-7FAB7EDFEE0A")]
[PlugInPoint("Mini game ended", "Plugins which will be executed when a mini game ended.")]
public interface IMiniGameEndedPlugIn
{
    /// <summary>
    /// Is called when a mini game ended.
    /// </summary>
    /// <param name="miniGame">The mini game.</param>
    /// <param name="winner">The winner of the game, if the game has one, e.g. the last survivor of Chaos Castle.</param>
    /// <param name="finishers">The players which were still in the game when it ended.</param>
    ValueTask MiniGameEndedAsync(MiniGameContext miniGame, Player? winner, IReadOnlyCollection<Player> finishers);
}
