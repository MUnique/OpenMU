// <copyright file="NpcTargetSelection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Shared target-selection helpers for NPC intelligences.
/// </summary>
internal static class NpcTargetSelection
{
    /// <summary>
    /// Returns the nearest candidate with a clear line of sight from the viewer,
    /// falling back to the nearest candidate regardless of sight so the NPC still
    /// approaches (or, for static traps, considers) targets behind walls.
    /// </summary>
    /// <typeparam name="T">The candidate type.</typeparam>
    /// <param name="candidates">The attack candidates.</param>
    /// <param name="viewer">The viewing object (e.g. the attacking monster).</param>
    /// <param name="getDistance">Measures the distance of a candidate; defines what "nearest" means.</param>
    /// <returns>The nearest visible candidate, or the nearest candidate if nobody is visible; <c>null</c> when there are no candidates.</returns>
    internal static T? GetNearestPreferVisible<T>(IEnumerable<T> candidates, ILocateable viewer, Func<T, double> getDistance)
        where T : class, ILocateable
    {
        T? nearest = null;
        var nearestDistance = double.MaxValue;
        T? nearestVisible = null;
        var nearestVisibleDistance = double.MaxValue;
        foreach (var candidate in candidates)
        {
            var distance = getDistance(candidate);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = candidate;
            }

            if (distance < nearestVisibleDistance && viewer.HasLineOfSightTo(candidate))
            {
                nearestVisibleDistance = distance;
                nearestVisible = candidate;
            }
        }

        return nearestVisible ?? nearest;
    }
}
