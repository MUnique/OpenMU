// <copyright file="KanturuTransitionDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// The definition of the transition from the Maya battlefield into the Nightmare zone.
/// </summary>
public class KanturuTransitionDefinition
{
    /// <summary>
    /// Gets or sets how long the context waits for the client cinematic (camera pan, Maya
    /// explosion and the fall of the players) before it moves the players.
    /// </summary>
    public TimeSpan CinematicDuration { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the x coordinate to which the players are moved.
    /// </summary>
    public byte EntryPointX { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate to which the players are moved.
    /// </summary>
    public byte EntryPointY { get; set; }

    /// <summary>
    /// Gets or sets the delay between moving the players and playing the warp animation at
    /// the entry point, so the clients can process the new position first.
    /// </summary>
    public TimeSpan WarpAnimationDelay { get; set; } = TimeSpan.FromMilliseconds(200);
}
