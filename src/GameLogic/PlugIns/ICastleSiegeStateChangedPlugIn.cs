// <copyright file="ICastleSiegeStateChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when the state of the castle siege changed.
/// </summary>
[Guid("FD8B1313-F26E-4D12-9A7D-71DAFBB30880")]
[PlugInPoint("Castle siege state changed", "Plugins which will be executed when the state of the castle siege changed, e.g. when the battle started.")]
public interface ICastleSiegeStateChangedPlugIn
{
    /// <summary>
    /// Is called when the state of the castle siege changed.
    /// </summary>
    /// <param name="gameContext">The game context of the castle siege.</param>
    /// <param name="castleSiege">
    /// The castle siege. It provides the new <see cref="CastleSiegeContext.CurrentState"/>, its start and end time,
    /// and with <see cref="CastleSiegeContext.SiegeData"/> the owner of the castle - which is the winner after the battle ended.
    /// </param>
    /// <param name="previousState">The previous state.</param>
    ValueTask CastleSiegeStateChangedAsync(IGameContext gameContext, CastleSiegeContext castleSiege, CastleSiegeState previousState);
}
