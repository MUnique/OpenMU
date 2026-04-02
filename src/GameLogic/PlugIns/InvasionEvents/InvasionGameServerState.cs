// <copyright file="InvasionGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Game server state per event.
/// </summary>
public class InvasionGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvasionGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public InvasionGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets or sets the map identifier.
    /// </summary>
    public ushort MapId { get; set; }

    /// <summary>
    /// Gets the map identifiers where monsters will spawn.
    /// </summary>
    public HashSet<ushort> MapIds { get; } = new();

    /// <summary>
    /// Gets the mapping of monster ID to the selected map identifier.
    /// Used when a random map is picked from the configuration's list.
    /// </summary>
    public IDictionary<ushort, ushort> SelectedMaps { get; } = new Dictionary<ushort, ushort>();
}