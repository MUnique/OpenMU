// <copyright file="ChaosCastleStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The chaos castle start configuration.
/// </summary>
public class ChaosCastleStartConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets the default configuration for chaos castle.
    /// </summary>
    public static ChaosCastleStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "Chaos Castle entrance is open and closes in {0} minute(s).",
            EntranceClosedMessage = "Chaos Castle entrance closed.",
            TaskDuration = TimeSpan.FromMinutes(15),
            Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromMinutes(20)).ToList(),
        };

    /// <summary>
    /// Gets or sets the entrance opened message.
    /// </summary>
    public string? EntranceOpenedMessage { get; set; }

    /// <summary>
    /// Gets or sets the entrance closed message.
    /// </summary>
    public string? EntranceClosedMessage { get; set; }
}