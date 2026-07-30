// <copyright file="MapFilterService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Manages the active object type filter and search string,
/// and provides methods for querying the filtered object list.
/// </summary>
public sealed class MapFilterService
{
    private const int MaxObjectSelectSize = 20;

    /// <summary>
    /// Gets or sets the active object type filter.
    /// </summary>
    public ObjectTypeFilter ActiveFilter { get; set; }

    /// <summary>
    /// Gets or sets the search string used to further filter map objects by name.
    /// </summary>
    public string SearchFilter { get; set; } = string.Empty;

    /// <summary>
    /// Toggles the specified filter flag.
    /// </summary>
    /// <param name="filter">The filter flag to toggle.</param>
    public void ToggleFilter(ObjectTypeFilter filter)
    {
        this.ActiveFilter ^= filter;
    }

    /// <summary>
    /// Resets the filter so that all object types are shown.
    /// </summary>
    public void SetAllFilter()
    {
        this.ActiveFilter = ObjectTypeFilter.None;
    }

    /// <summary>
    /// Returns a value indicating whether the specified filter flag is currently active.
    /// </summary>
    /// <param name="filter">The filter flag to check.</param>
    /// <returns><see langword="true"/> if the filter flag is active; otherwise, <see langword="false"/>.</returns>
    public bool IsFilterActive(ObjectTypeFilter filter) => this.ActiveFilter.HasFlag(filter);

    /// <summary>
    /// Determines whether the given object is filtered out by the current filter and search settings.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns><see langword="true"/> if the object is filtered out; otherwise, <see langword="false"/>.</returns>
    public bool IsObjectFilteredOut(object obj) =>
        !MapObjectSelector.MatchesFilters(obj, this.ActiveFilter, this.SearchFilter);

    /// <summary>
    /// Gets the number of visible objects in the map, capped at <see cref="MaxObjectSelectSize"/>.
    /// </summary>
    /// <param name="map">The map whose objects are to be counted.</param>
    /// <returns>The count of visible objects, capped at the maximum select size.</returns>
    public int GetObjectListSize(GameMapDefinition map)
    {
        var count = 1
            + map.EnterGates.Count(g => MapObjectSelector.MatchesFilters(g, this.ActiveFilter, this.SearchFilter))
            + map.ExitGates.Count(g => MapObjectSelector.MatchesFilters(g, this.ActiveFilter, this.SearchFilter))
            + map.MonsterSpawns.Count(s => MapObjectSelector.MatchesFilters(s, this.ActiveFilter, this.SearchFilter));

        return Math.Min(count, MaxObjectSelectSize);
    }

    /// <summary>
    /// Enumerates the objects in the map that match the current filter and search settings.
    /// </summary>
    /// <param name="map">The map whose objects are to be enumerated.</param>
    /// <returns>The matching map objects.</returns>
    public IEnumerable<object> GetMapObjects(GameMapDefinition map)
    {
        var objects = Enumerable.Empty<object>();

        if (this.ActiveFilter == ObjectTypeFilter.None || this.ActiveFilter.HasFlag(ObjectTypeFilter.Gates))
        {
            objects = objects.Concat(map.EnterGates)
                             .Concat(map.ExitGates);
        }

        if (this.ActiveFilter == ObjectTypeFilter.None
            || this.ActiveFilter.HasFlag(ObjectTypeFilter.Monsters)
            || this.ActiveFilter.HasFlag(ObjectTypeFilter.Npcs)
            || this.ActiveFilter.HasFlag(ObjectTypeFilter.Others))
        {
            objects = objects.Concat(map.MonsterSpawns);
        }

        return objects.Where(o => MapObjectSelector.MatchesFilters(o, this.ActiveFilter, this.SearchFilter));
    }
}
