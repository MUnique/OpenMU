// <copyright file="HappyHourConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System;

/// <summary>
/// The Happy Hour event configuration.
/// </summary>
public class HappyHourConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static HappyHourConfiguration Default => new()
    {
        TaskDuration = TimeSpan.FromHours(1),
        PreStartMessageDelay = TimeSpan.FromSeconds(0),
        Message = "Happy Hour event has been started!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(6), new TimeOnly(0, 5)).ToList(), // Every 6 hours,
        ExperienceMultiplier = 1.5f,
    };

    /// <summary>
    /// Gets or sets the experience multiplier.
    /// </summary>
    public float ExperienceMultiplier { get; set; } = 1.5f;
}