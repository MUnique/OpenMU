// <copyright file="SpawnGateSelectionArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The arguments of the <see cref="IPlayerSpawnGateSelectionPlugIn"/>. Plugin points can't return
/// values, so the selected gate is passed back through this object.
/// </summary>
public sealed class SpawnGateSelectionArgs
{
    /// <summary>
    /// Gets or sets the gate at which the player should be spawned. When it stays <see langword="null"/>,
    /// the player is spawned at the safezone gate of its current map.
    /// </summary>
    /// <remarks>
    /// A plugin should not overwrite a gate which another plugin already selected.
    /// </remarks>
    public ExitGate? Gate { get; set; }
}
