// <copyright file="PeriodicInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Abstract configuration for periodic invasions.
/// </summary>
public class PeriodicInvasionConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static PeriodicInvasionConfiguration DefaultGoldenInvasion => new()
    {
        TaskDuration = TimeSpan.FromMinutes(5),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        Message = "[{mapName}] Golden Invasion!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(4)).ToList(), // Every 4 hours
    };

    /// <summary>
    /// Gets the default configuration for the red dragon invasion.
    /// </summary>
    public static PeriodicInvasionConfiguration DefaultRedDragonInvasion => new()
    {
        TaskDuration = TimeSpan.FromMinutes(10),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        Message = "[{mapName}] Red Dragon Invasion!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(6), new TimeOnly(2, 0)).ToList(), // Every 6 hours, starting from 02:00
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicInvasionConfiguration"/> class.
    /// </summary>
    public PeriodicInvasionConfiguration()
    {
        this.Message = "Invasion's been started!";
    }
}
