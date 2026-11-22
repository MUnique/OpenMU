// <copyright file="MapExportImportPanel.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Exports and imports the objects of the selected map, or of all maps, by kind.
/// </summary>
/// <remarks>
/// There is one kind selection for both scopes. Gates reference other maps, so they are only
/// part of the all-maps export and import.
/// </remarks>
public partial class MapExportImportPanel
{
    private readonly MapExportImportService _exportImportService = new();
    private readonly HashSet<MapObjectKind> _kinds = new(MapObjectKindExtensions.All);

    /// <summary>
    /// Gets or sets all maps which can be exported or imported.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public IReadOnlyCollection<GameMapDefinition> Maps { get; set; } = [];

    /// <summary>
    /// Gets or sets the currently selected map.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public GameMapDefinition SelectedMap { get; set; } = null!;

    /// <summary>
    /// Gets or sets the callback which is invoked after a successful import.
    /// </summary>
    [Parameter]
    public EventCallback OnImported { get; set; }

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    [Inject]
    private IModalService ModalService { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private LoadingOverlayService LoadingOverlay { get; set; } = null!;

    private bool HasSpawnKinds => this.SpawnKinds.Count > 0;

    private HashSet<MapObjectKind> SpawnKinds =>
        this._kinds.Where(kind => kind != MapObjectKind.Gates).ToHashSet();

    private string SingleMapFileName =>
        $"map-{this.SelectedMap.Number}-spawns-{this.SpawnKinds.GetFileSuffix()}.json";

    private string AllMapsFileName => $"maps-spawns-{this._kinds.GetFileSuffix()}.json";

    private string BuildSingleMapExport() => this._exportImportService.BuildExport([this.SelectedMap], this.SpawnKinds);

    private string BuildAllMapsExport() => this._exportImportService.BuildExport(this.Maps, this._kinds);

    private void OnKindsChanged(IReadOnlySet<MapObjectKind> kinds)
    {
        this._kinds.Clear();
        this._kinds.UnionWith(kinds);
    }

    private Task ImportSingleMapAsync(string json)
    {
        return this.ImportAsync(json, [this.SelectedMap], includeGates: false);
    }

    private Task ImportAllMapsAsync(string json)
    {
        return this.ImportAsync(json, this.Maps, includeGates: true);
    }

    private async Task ImportAsync(string json, IReadOnlyCollection<GameMapDefinition> targetMaps, bool includeGates)
    {
        var workflow = new MapImportWorkflow(
            this._exportImportService,
            this.ModalService,
            this.ToastService,
            this.LoadingOverlay);

        if (await workflow.RunAsync(json, targetMaps, includeGates, this.PersistenceContext, this.Maps).ConfigureAwait(true))
        {
            await this.OnImported.InvokeAsync().ConfigureAwait(true);
        }
    }
}
