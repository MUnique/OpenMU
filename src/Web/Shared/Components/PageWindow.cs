// <copyright file="PageWindow.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components;

/// <summary>
/// Calculates which page numbers a <see cref="Pager"/> shows.
/// The amount of shown entries stays constant while navigating, so that the pager doesn't change its width
/// and the buttons for the previous and next page always stay at the same position.
/// </summary>
public static class PageWindow
{
    private const int MinimumSlotCount = 5;

    /// <summary>
    /// Gets the default amount of entries (page numbers and gap markers).
    /// </summary>
    public static int DefaultSlotCount => 7;

    /// <summary>
    /// Gets the entries to show. The first and the last page are always included, except when the last page is unknown.
    /// </summary>
    /// <param name="currentPage">The current, one-based page number.</param>
    /// <param name="totalPages">The total number of pages, if known.</param>
    /// <param name="hasNextPage">A value indicating whether there is a page after the current one. Only used when <paramref name="totalPages"/> is unknown.</param>
    /// <param name="slotCount">The maximum amount of entries, including gap markers. Must be at least 5. If <c>null</c>, <see cref="DefaultSlotCount"/> is used.</param>
    /// <returns>The entries in ascending order. A <c>null</c> entry marks a gap between two page numbers.</returns>
    public static IReadOnlyList<int?> Create(int currentPage, int? totalPages, bool hasNextPage, int? slotCount = null)
    {
        var slots = slotCount ?? DefaultSlotCount;
        ArgumentOutOfRangeException.ThrowIfLessThan(slots, MinimumSlotCount);

        var current = Math.Max(1, currentPage);
        var last = Math.Max(1, totalPages ?? (hasNextPage ? current + 1 : current));
        current = Math.Min(current, last);

        if (last <= slots)
        {
            return Range(1, last);
        }

        // Layouts for the case that there are more pages than slots. They always use all slots:
        // "1 2 3 4 5 … 20" (near the start), "1 … 8 9 10 … 20" (in the middle), "1 … 16 17 18 19 20" (near the end).
        var edgeCount = slots - 2;
        var nearStart = current <= slots - 3;
        var nearEnd = current >= last - (slots - 4);
        var result = new List<int?>(slots);

        if (totalPages is null)
        {
            // The last page is unknown, so the known pages are shown at the end, up to the last known one.
            result.Add(1);
            result.Add(null);
            result.AddRange(Range(last - edgeCount + 1, last));
        }
        else if (nearStart)
        {
            result.AddRange(Range(1, edgeCount));
            result.Add(null);
            result.Add(last);
        }
        else if (nearEnd)
        {
            result.Add(1);
            result.Add(null);
            result.AddRange(Range(last - edgeCount + 1, last));
        }
        else
        {
            var siblings = (slots - 4) / 2;
            result.Add(1);
            result.Add(null);
            result.AddRange(Range(current - siblings, current + siblings));
            result.Add(null);
            result.Add(last);
        }

        return result;
    }

    private static List<int?> Range(int first, int last)
    {
        return Enumerable.Range(first, last - first + 1).Select(page => (int?)page).ToList();
    }
}
