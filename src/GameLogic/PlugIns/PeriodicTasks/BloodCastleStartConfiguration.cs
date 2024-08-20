// <copyright file="BloodCastleStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The blood castle start configuration.
/// </summary>
public class BloodCastleStartConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets the default configuration for blood castle.
    /// </summary>
    public static BloodCastleStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "Blood Castle entrance is open and closes in {0} minute(s).",
            EntranceClosedMessage = "Blood Castle entrance closed.",
            TaskDuration = TimeSpan.FromMinutes(20),
            Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(TimeSpan.FromMinutes(120)).ToList(),
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