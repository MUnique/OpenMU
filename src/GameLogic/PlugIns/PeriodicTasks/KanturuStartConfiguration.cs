// <copyright file="KanturuStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// The Kanturu event start configuration.
/// </summary>
public class KanturuStartConfiguration : MiniGameStartConfiguration
{
    /// <summary>
    /// Gets or sets how long the Tower of Refinement stays open after Nightmare is defeated.
    /// Default is 1 hour. Set to zero to skip the tower phase.
    /// </summary>
    public TimeSpan TowerOfRefinementDuration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets the default configuration for the Kanturu event.
    /// The event runs once per day. After Nightmare is defeated the Tower of Refinement
    /// stays open for 1 hour, then the event ends and the next occurrence is the following day.
    /// The preparation window (entry phase) opens 3 minutes before the scheduled start time.
    /// </summary>
    public static KanturuStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "Kanturu Refinery Tower entrance is open and closes in {0} minute(s).",
            EntranceClosedMessage = "Kanturu Refinery Tower entrance closed.",
            TaskDuration = TimeSpan.FromMinutes(135),
            Timetable = [new TimeOnly(20, 0)],   // 20:00 UTC — one occurrence per day
            TowerOfRefinementDuration = TimeSpan.FromHours(1),
        };
}
