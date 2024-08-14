// <copyright file="IPeriodicMiniGameStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Interface for a mini game start plugin.
/// </summary>
[PlugInPoint("Periodic Mini Game Start Plugins", "Plugin Container for mini game start plugins, e.g. for chaos castle.")]
[Guid("D88FFCCF-5C57-49E0-8E05-1C75A95A9AF2")]
public interface IPeriodicMiniGameStartPlugIn : IStrategyPlugIn<MiniGameType>, IPeriodicTaskPlugIn
{
    /// <summary>
    /// Gets the duration until next start.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <returns>
    /// The duration until next start.
    /// </returns>
    ValueTask<TimeSpan?> GetDurationUntilNextStartAsync(IGameContext gameContext, MiniGameDefinition miniGameDefinition);

    /// <summary>
    /// Gets the mini game context, if available.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="miniGameDefinition">The mini game definition.</param>
    /// <returns>The mini game context, if available.</returns>
    ValueTask<MiniGameContext?> GetMiniGameContextAsync(IGameContext gameContext, MiniGameDefinition miniGameDefinition);
}