// <copyright file="BloodCastleGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The state of a game server state for a blood castle event.
/// </summary>
public class BloodCastleGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastleGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public BloodCastleGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override string Description => "Blood Castle";
}