// <copyright file="InvasionGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Invasion state that is created for every periodic invasion run.
/// </summary>
public class InvasionGameServerState : PeriodicTaskGameServerState
{
    private readonly HashSet<ushort> _mapIds = [];
    private readonly Dictionary<ushort, ushort> _selectedMaps = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InvasionGameServerState"/> class.
    /// </summary>
    /// <param name="context">The game context.</param>
    public InvasionGameServerState(IGameContext context)
        : base(context)
    {
    }


    /// <summary>
    /// Gets or sets the map identifier used for UI display / map-event state broadcasts.
    /// <c>null</c> means no event is active or the display map has not been selected yet.
    /// </summary>
    public ushort? MapId { get; set; }

    /// <summary>
    /// Gets the set of map identifiers on which monsters will spawn this run.
    /// </summary>
    public IReadOnlySet<ushort> MapIds => this._mapIds;

    /// <summary>
    /// Gets the read-only mapping of monster ID to selected map identifier.
    /// Populated for spawns whose <see cref="SpawnMapStrategy"/> is <see cref="SpawnMapStrategy.RandomMap"/>.
    /// </summary>
    public IReadOnlyDictionary<ushort, ushort> SelectedMaps => this._selectedMaps;

    /// <summary>
    /// Registers a map as active for this run and optionally records which map was
    /// randomly chosen for a particular monster type.
    /// </summary>
    /// <param name="mapId">The map identifier to register.</param>
    /// <param name="monsterId">
    /// When provided, records the <paramref name="mapId"/> as the chosen map for this monster.
    /// Pass <c>null</c> for "spawn-on-all-maps" entries.
    /// </param>
    internal void RegisterMap(ushort mapId, ushort? monsterId = null)
    {
        this._mapIds.Add(mapId);
        if (monsterId.HasValue)
        {
            this._selectedMaps[monsterId.Value] = mapId;
        }
    }

    /// <summary>
    /// Clears all state accumulated from a previous run so the object can be reused.
    /// </summary>
    internal void Reset()
    {
        this.MapId = null;
        this._mapIds.Clear();
        this._selectedMaps.Clear();
    }
}