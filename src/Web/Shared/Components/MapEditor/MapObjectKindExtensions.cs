// <copyright file="MapObjectKindExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.Web.Shared.Properties;

/// <summary>
/// Provides helpers for <see cref="MapObjectKind"/>.
/// </summary>
public static class MapObjectKindExtensions
{
    /// <summary>
    /// Gets all object kinds.
    /// </summary>
    public static IReadOnlyCollection<MapObjectKind> All { get; } = Enum.GetValues<MapObjectKind>();

    /// <summary>
    /// Builds the lower-case file name suffix of the given kinds, e.g. <c>monsters-npcs</c>.
    /// </summary>
    /// <param name="kinds">The kinds.</param>
    /// <returns>The file name suffix.</returns>
    public static string GetFileSuffix(this IEnumerable<MapObjectKind> kinds)
    {
        return string.Join('-', kinds.OrderBy(kind => kind).Select(kind => kind.ToString().ToLowerInvariant()));
    }

    /// <summary>
    /// Gets the localized caption of the given kind.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <returns>The localized caption.</returns>
    public static string GetCaption(this MapObjectKind kind)
    {
        return kind switch
        {
            MapObjectKind.Monsters => Resources.Monsters,
            MapObjectKind.Npcs => Resources.Npcs,
            MapObjectKind.Others => Resources.Others,
            MapObjectKind.Gates => Resources.Gates,
            _ => kind.ToString(),
        };
    }
}
