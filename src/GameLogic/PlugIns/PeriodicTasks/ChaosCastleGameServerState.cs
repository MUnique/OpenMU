// <copyright file="ChaosCastleGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The state of a game server state for a chaos castle event.
/// </summary>
public class ChaosCastleGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastleGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public ChaosCastleGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override string Description => "Chaos Castle";
}