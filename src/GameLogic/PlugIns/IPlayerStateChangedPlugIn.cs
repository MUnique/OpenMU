// <copyright file="IPlayerStateChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when the state of a player has been changed.
/// </summary>
/// <remarks>
/// Example cases:
/// - Player Entered Game
/// - Player Left Game
/// - Before saving player data
/// - Player Authenticated
/// - Trade finished.
/// </remarks>
[Guid("7AB20179-753F-423C-94F7-16DB03D4D046")]
[PlugInPoint("Player state changed", "Is called when a player state got changed.")]
public interface IPlayerStateChangedPlugIn
{
    /// <summary>
    /// Is called when the <see cref="Player.PlayerState" /> changed.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="previousState">The previous state.</param>
    /// <param name="currentState">The current state.</param>
    ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState);
}