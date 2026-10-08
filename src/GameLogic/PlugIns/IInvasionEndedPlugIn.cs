// <copyright file="IInvasionEndedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an invasion ended, e.g. the golden invasion.
/// </summary>
[Guid("34D3F779-A173-4DCE-A3EA-58AC72274C7E")]
[PlugInPoint("Invasion ended", "Plugins which will be executed when an invasion ended.")]
public interface IInvasionEndedPlugIn
{
    /// <summary>
    /// Is called when an invasion ended, before its remaining monsters are removed.
    /// </summary>
    /// <param name="gameContext">The game context in which the invasion ended.</param>
    /// <param name="invasion">The plugin which runs the invasion. Its type identifies the kind of invasion.</param>
    /// <param name="maps">The maps which were announced to the players as invaded.</param>
    ValueTask InvasionEndedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps);
}
