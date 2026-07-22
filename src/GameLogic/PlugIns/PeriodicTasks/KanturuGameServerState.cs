// <copyright file="KanturuGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The state of a game server state for the Kanturu event.
/// </summary>
public class KanturuGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public KanturuGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override string Description => "Kanturu Event";
}
