// <copyright file="MapExportImportService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Text.Json;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Exports map objects to JSON and imports them back, without changing anything before
/// the import was validated and confirmed.
/// </summary>
public sealed class MapExportImportService
{
    private readonly MapExportBuilder _builder = new();
    private readonly MapImportPlanner _planner = new(new MapExportFileReader());
    private readonly MapImportApplier _applier = new();

    /// <summary>
    /// Builds a JSON export of the given maps and kinds.
    /// </summary>
    /// <param name="maps">The maps to export.</param>
    /// <param name="kinds">The kinds to export.</param>
    /// <returns>The JSON content.</returns>
    public string BuildExport(IReadOnlyCollection<GameMapDefinition> maps, IReadOnlySet<MapObjectKind> kinds)
    {
        var file = this._builder.Build(maps, kinds);
        return JsonSerializer.Serialize(file, MapExportJson.Options);
    }

    /// <summary>
    /// Validates the given JSON content and builds a preview without changing anything.
    /// </summary>
    /// <param name="json">The JSON content.</param>
    /// <param name="targetMaps">The maps which are available as import target.</param>
    /// <param name="includeGates">Whether gates may be imported. They are only supported for all maps at once.</param>
    /// <param name="context">The persistence context used to resolve monster definitions.</param>
    /// <param name="allMaps">All maps in their regular order, to tell apart maps which share a number.</param>
    /// <returns>The preview of the importable changes.</returns>
    public ValueTask<MapImportPreview> PreviewAsync(
        string json,
        IReadOnlyCollection<GameMapDefinition> targetMaps,
        bool includeGates,
        IContext context,
        IReadOnlyCollection<GameMapDefinition>? allMaps = null)
    {
        return this._planner.CreatePreviewAsync(json, targetMaps, includeGates, context, allMaps);
    }

    /// <summary>
    /// Applies the selected kinds of a validated preview.
    /// </summary>
    /// <param name="preview">The validated preview.</param>
    /// <param name="selectedKinds">The kinds which the user selected for import.</param>
    /// <param name="context">The persistence context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask ApplyAsync(
        MapImportPreview preview,
        IReadOnlySet<MapObjectKind> selectedKinds,
        IContext context)
    {
        return this._applier.ApplyAsync(preview, selectedKinds, context);
    }
}
