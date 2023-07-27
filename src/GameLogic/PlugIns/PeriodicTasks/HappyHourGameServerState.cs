// <copyright file="HappyHourGameServerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Game server state per event.
/// </summary>
public class HappyHourGameServerState : PeriodicTaskGameServerState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HappyHourGameServerState"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public HappyHourGameServerState(IGameContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets or sets the new experience multiplier.
    /// </summary>
    public float NewExperienceRate { get; set; }

    /// <summary>
    /// Gets or sets the old experience multiplier.
    /// </summary>
    public float OldExperienceRate { get; set; }
}
