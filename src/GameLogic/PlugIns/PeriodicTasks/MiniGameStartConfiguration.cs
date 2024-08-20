// <copyright file="MiniGameStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The minimal minigame start configuration.
/// </summary>
public abstract class MiniGameStartConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets or sets the entrance opened message.
    /// </summary>
    public string? EntranceOpenedMessage { get; set; }

    /// <summary>
    /// Gets or sets the entrance closed message.
    /// </summary>
    public string? EntranceClosedMessage { get; set; }
}