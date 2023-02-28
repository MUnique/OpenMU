// <copyright file="PeriodicTaskGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Game server state per event.
/// </summary>
public class PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicTaskGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public PeriodicTaskGameServerState(IGameContext context)
    {
        this.Context = context;
    }

    /// <summary>
    /// Gets the context to which this state belongs.
    /// </summary>
    public IGameContext Context { get; }

    /// <summary>
    /// Gets or sets the next run in UTC.
    /// </summary>
    public DateTime NextRunUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public PeriodicTaskState State { get; set; } = PeriodicTaskState.NotStarted;

    /// <summary>
    /// Gets the description about the state.
    /// </summary>
    public virtual string Description => string.Empty;
}