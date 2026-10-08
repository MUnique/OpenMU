// <copyright file="IMiniGameStartedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a mini game started.
/// </summary>
/// <remarks>
/// It's not called when the game didn't start, e.g. because not enough players entered.
/// </remarks>
[Guid("38D44383-179F-4271-932B-3B83CA3ABCB8")]
[PlugInPoint("Mini game started", "Plugins which will be executed when a mini game started.")]
public interface IMiniGameStartedPlugIn
{
    /// <summary>
    /// Is called when a mini game started.
    /// </summary>
    /// <param name="miniGame">The mini game.</param>
    /// <param name="players">The players which are playing the game.</param>
    ValueTask MiniGameStartedAsync(MiniGameContext miniGame, IReadOnlyCollection<Player> players);
}
