// <copyright file="MapImportPreview.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// The validated content of an import file, before anything is changed.
/// </summary>
public sealed class MapImportPreview
{
    private readonly IReadOnlyCollection<MapImportTarget> _targets;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapImportPreview"/> class.
    /// </summary>
    public MapImportPreview()
        : this([])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapImportPreview"/> class.
    /// </summary>
    /// <param name="targets">The validated import targets.</param>
    internal MapImportPreview(IReadOnlyCollection<MapImportTarget> targets)
    {
        this._targets = targets;
    }

    /// <summary>
    /// Gets or initializes the error which prevents the import, if any.
    /// </summary>
    public MapImportError? Error { get; init; }

    /// <summary>
    /// Gets or initializes the kinds which are declared by the file and can be imported.
    /// </summary>
    public IReadOnlyCollection<MapObjectKind> DeclaredKinds { get; init; } = [];

    /// <summary>
    /// Gets or initializes the per-map summaries.
    /// </summary>
    public IReadOnlyCollection<MapImportMapPreview> Maps { get; init; } = [];

    /// <summary>
    /// Gets or initializes the records which will be skipped.
    /// </summary>
    public IReadOnlyCollection<MapImportWarning> Warnings { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether the file contains anything to import.
    /// </summary>
    public bool HasChanges => this.Maps.Any(map => map.HasChanges);

    /// <summary>
    /// Counts the records which would be imported for the specified kinds.
    /// </summary>
    /// <param name="kinds">The selected kinds.</param>
    /// <returns>The number of records.</returns>
    public int CountImported(IReadOnlySet<MapObjectKind> kinds)
    {
        return kinds
            .Where(kind => this.DeclaredKinds.Contains(kind))
            .Sum(kind => this.GetCount(kind));
    }

    /// <summary>
    /// Counts the records of the specified kind which are contained in the file and can be imported.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <returns>The number of records.</returns>
    public int GetCount(MapObjectKind kind)
    {
        return this.Maps.Sum(map => map.GetObjectCount(kind));
    }

    /// <summary>
    /// Gets the validated import targets.
    /// </summary>
    /// <returns>The validated import targets.</returns>
    internal IReadOnlyCollection<MapImportTarget> GetTargets() => this._targets;
}
