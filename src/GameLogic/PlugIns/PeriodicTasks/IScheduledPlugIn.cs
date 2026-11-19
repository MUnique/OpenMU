// <copyright file="IScheduledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// A plugin which runs according to a schedule, e.g. an event like Blood Castle.
/// </summary>
public interface IScheduledPlugIn
{
    /// <summary>
    /// Gets the next start time.
    /// </summary>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <param name="serverTimeZone">The time zone of the server, in which the schedule is defined.</param>
    /// <returns>The next start time, in UTC; or <see langword="null"/>, if there is none.</returns>
    DateTime? GetNextStartUtc(DateTime utcNow, TimeZoneInfo serverTimeZone);

    /// <summary>
    /// Determines whether the schedule applies to the game server which owns the specified context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the plugin runs on this game server according to its schedule.</returns>
    bool IsScheduledOn(IGameContext gameContext) => true;
}
