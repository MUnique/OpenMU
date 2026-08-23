// <copyright file="IllusionTempleGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The state of a game server for an illusion temple event.
/// </summary>
public class IllusionTempleGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public IllusionTempleGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override string Description => "Illusion Temple";
}