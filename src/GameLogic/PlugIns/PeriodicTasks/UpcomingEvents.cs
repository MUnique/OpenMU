// <copyright file="UpcomingEvents.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Globalization;
using System.Reflection;
using System.Resources;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Determines the upcoming events, like mini games and invasions, from the schedules of the active plugins.
/// </summary>
public static class UpcomingEvents
{
    /// <summary>
    /// Gets the events which start within the specified time span, ordered by their start time.
    /// </summary>
    /// <param name="plugInManager">The plugin manager with the active plugins.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <param name="serverTimeZone">The time zone of the server, in which the schedules are defined.</param>
    /// <param name="timeSpan">The time span from now, in which the events have to start.</param>
    /// <param name="culture">The culture of the names.</param>
    /// <returns>The names and start times of the events.</returns>
    public static IReadOnlyList<(string Name, DateTime StartsAtUtc)> Get(
        PlugInManager plugInManager,
        DateTime utcNow,
        TimeZoneInfo serverTimeZone,
        TimeSpan timeSpan,
        CultureInfo culture)
    {
        return plugInManager.GetActivePlugInsOf<IPeriodicTaskPlugIn>()
            .OfType<IScheduledPlugIn>()
            .Select(plugIn => (Name: GetName(plugIn.GetType(), culture), StartsAtUtc: plugIn.GetNextStartUtc(utcNow, serverTimeZone)))
            .Where(e => e.StartsAtUtc is { } start && start - utcNow <= timeSpan)
            .Select(e => (e.Name, e.StartsAtUtc!.Value))
            .OrderBy(e => e.Value)
            .ToList();
    }

    private static string GetName(Type plugInType, CultureInfo culture)
    {
        var display = plugInType.GetCustomAttribute<DisplayAttribute>();
        if (display?.Name is { } name
            && display.ResourceType?.GetProperty(nameof(PlugInResources.ResourceManager), BindingFlags.Public | BindingFlags.Static)?.GetValue(null) is ResourceManager resourceManager
            && resourceManager.GetString(name, culture) is { } localizedName)
        {
            return localizedName;
        }

        return display?.GetName() ?? plugInType.Name;
    }
}
