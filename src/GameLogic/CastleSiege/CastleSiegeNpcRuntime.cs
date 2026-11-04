// <copyright file="CastleSiegeNpcRuntime.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Holds the runtime state of a configured Castle Siege NPC.
/// </summary>
public class CastleSiegeNpcRuntime
{
    /// <summary>
    /// Gets or sets the NPC definition.
    /// </summary>
    public CastleSiegeNpcDefinition Definition { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistent state, if this NPC is stored in the database.
    /// </summary>
    public CastleSiegeNpcState? PersistedState { get; set; }

    /// <summary>
    /// Gets or sets the spawned map object.
    /// </summary>
    public ILocateable? SpawnedInstance { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the NPC is alive.
    /// </summary>
    public bool IsAlive { get; set; }
}
