// <copyright file="KanturuNightmareHpPhase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// One health based phase of the Nightmare boss fight. When the boss' health drops below
/// <see cref="HealthPercentage"/>, it's teleported to the configured position and its health
/// is restored.
/// </summary>
public class KanturuNightmareHpPhase
{
    /// <summary>
    /// Gets or sets the health percentage below which this phase starts.
    /// </summary>
    public float HealthPercentage { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate to which the boss is teleported.
    /// </summary>
    public byte TeleportTargetX { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate to which the boss is teleported.
    /// </summary>
    public byte TeleportTargetY { get; set; }

    /// <summary>
    /// Gets or sets the key of the localized message which is shown when this phase starts.
    /// </summary>
    public string? MessageKey { get; set; }
}
