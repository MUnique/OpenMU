// <copyright file="DevilSquareGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The state of a game server state for a devil square event.
/// </summary>
public class DevilSquareGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevilSquareGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public DevilSquareGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override string Description => "Devil Square";
}