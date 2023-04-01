// <copyright file="InvasionGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

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
    /// Gets the map.
    /// </summary>
    public GameMapDefinition Map => this.Context.Configuration.Maps.First(m => m.Number == this.MapId);

    /// <summary>
    /// Gets the name of the map.
    /// </summary>
    public string MapName => this.Map.Name;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.MapName;
    }
}