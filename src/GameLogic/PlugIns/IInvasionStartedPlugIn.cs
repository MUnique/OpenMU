// <copyright file="IInvasionStartedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an invasion started, e.g. the golden invasion.
/// </summary>
[Guid("027BE944-C349-4538-9FE7-7097337CE878")]
[PlugInPoint("Invasion started", "Plugins which will be executed when an invasion started.")]
public interface IInvasionStartedPlugIn
{
    /// <summary>
    /// Is called when an invasion started and its monsters have been spawned.
    /// </summary>
    /// <param name="gameContext">The game context in which the invasion started.</param>
    /// <param name="invasion">The plugin which runs the invasion. Its type identifies the kind of invasion.</param>
    /// <param name="maps">The maps which are announced to the players as invaded.</param>
    ValueTask InvasionStartedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps);
}
