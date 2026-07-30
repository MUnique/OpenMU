// <copyright file="MapEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// A graphical editor for a <see cref="GameMapDefinition"/>.
/// </summary>
public partial class MapEditor : IAsyncDisposable
{
    private static readonly string JsModulePath =
        $"./_content/{typeof(MapEditor).Assembly.GetName().Name}/Components/MapEditor/{nameof(MapEditor)}.razor.js";

    private static readonly string[] DirectionNames = Enum.GetNames<Direction>()
        .Select(d => d.ToLowerInvariant())
        .ToArray();

    private readonly MapCoordinateService _coordinateService = new();
    private readonly MapObjectStyleService _styleService = new();
    private readonly MapObjectSelector _objectSelector = new();
    private readonly MapEditorHistory _history = new();
    private readonly List<object> _pendingDeletions = new();
    private readonly MapExportImportService _exportImportService = new();
    private readonly MapFilterService _filterService = new();

    private MapCrudOperationsService? _crudService;
    private MapZoomManager? _zoomManager;
    private MapObjectFactory? _objectFactory;
    private Image<Rgba32>? _terrainImage;
    private string? _terrainImageDataUrl;
    private ElementReference _mapHostRef;
    private ElementReference _mapSelectRef;
    private ElementReference _objectSelectRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<MapEditor>? _dotNetRef;

    private GameMapDefinition? _selectedMap;
    private object? _focusedObject;

    private bool _isInitialized;
    private bool _createMode;
    private bool _isRendering;
    private bool _showGrid;
    private bool _isExporting;
    private bool _isImporting;

    private int _mouseMapX;
    private int _mouseMapY;

    private Resizers.ResizerPosition? _resizerPosition;
    private MapDragState _dragState;
    private bool _hasDragSnapshot;

    /// <summary>
    /// Gets or sets the callback invoked before the selected map changes,
    /// allowing the caller to cancel the change.
    /// </summary>
    [Parameter]
    public EventCallback<MapChangingArgs> SelectedMapChanging { get; set; }

    /// <summary>
    /// Gets or sets the full list of available maps to display in the map selector.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public required List<GameMapDefinition> Maps { get; set; }

    /// <summary>
    /// Gets or sets the ID of the initially selected map.
    /// </summary>
    [Parameter]
    public Guid SelectedMapId { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked after a successful save operation.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    /// <summary>
    /// Gets or sets the persistence context used for creating and deleting map objects.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JS runtime used to load and invoke the map editor JS module.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the notification service used to propagate object property changes.
    /// </summary>
    [Inject]
    private IChangeNotificationService NotificationService { get; set; } = null!;

    [Inject]
    private LoadingOverlayService LoadingOverlay { get; set; } = null!;

    /// <summary>
    /// Gets the current zoom level expressed as a rounded percentage.
    /// </summary>
    private int ZoomPercentage => this._zoomManager?.ZoomPercentage ?? 100;

    /// <summary>
    /// Gets the effective pixel scale for the current zoom level.
    /// </summary>
    private float EffectiveScale => this._coordinateService.GetEffectiveScale(this._zoomManager?.ZoomLevel ?? MapZoomManager.DefaultZoom);

    /// <summary>
    /// Gets or sets the currently selected map, rebuilding the terrain image when changed.
    /// </summary>
    private GameMapDefinition SelectedMap
    {
        get => this._selectedMap ?? throw new InvalidOperationException($"{nameof(this.SelectedMap)} is not initialized.");
        set
        {
            this._terrainImage?.Dispose();
            this._selectedMap = value;
            this._terrainImage = new GameMapTerrain(this.SelectedMap).ToImage();
            this._terrainImageDataUrl = this._terrainImage.ToBase64String(PngFormat.Instance);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._dotNetRef?.Dispose();
        this._terrainImage?.Dispose();
        this.NotificationService.PropertyChanged -= this.OnPropertyChanged;

        if (this._jsModule is not null)
        {
            try
            {
                await this._jsModule.InvokeVoidAsync("dispose").ConfigureAwait(false);
                await this._jsModule.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // Ignore
            }
        }
    }

    /// <summary>
    /// Called from JavaScript when the pointer goes down on the map.
    /// </summary>
    /// <param name="x">Map X coordinate.</param>
    /// <param name="y">Map Y coordinate.</param>
    /// <param name="shiftKey">Whether the shift key was held during the click.</param>
    [JSInvokable]
    public void OnPointerDown(byte x, byte y, bool shiftKey = false)
    {
        if (this._resizerPosition is not null)
        {
            return;
        }

        if (shiftKey)
        {
            this.HandleShiftClick(x, y);
            return;
        }

        var objectAtPosition = this._objectSelector.GetObjectAtPosition(
            this.SelectedMap, x, y, this._filterService.ActiveFilter, this._filterService.SearchFilter);

        if (objectAtPosition is IMapArea mapArea)
        {
            this._focusedObject = objectAtPosition;
            this._dragState.StartX = x;
            this._dragState.StartY = y;
            this._dragState.Capture(mapArea);
        }
        else if (objectAtPosition is not null)
        {
            this._focusedObject = objectAtPosition;
        }
        else
        {
            this._focusedObject = null;
            _ = this._jsModule?.InvokeVoidAsync("setPanning", true);
        }

        _ = this.UpdateSelectValueAsync();
        this.StateHasChanged();
    }

    /// <summary>
    /// Called from JavaScript when the pointer moves while on the map.
    /// </summary>
    /// <param name="x">Map X coordinate.</param>
    /// <param name="y">Map Y coordinate.</param>
    [JSInvokable]
    public void OnPointerMove(byte x, byte y)
    {
        this._mouseMapX = x;
        this._mouseMapY = y;

        if (this._resizerPosition is { } pos && this._focusedObject is IMapArea resizeTarget)
        {
            MapObjectResizer.Resize(resizeTarget, pos, x, y);
            this.NotificationService.NotifyChange(this._focusedObject, null);
            return;
        }

        if (this._focusedObject is IMapArea)
        {
            this.OnObjectDragging(x, y);
            this.NotificationService.NotifyChange(this._focusedObject, null);
        }
    }

    /// <summary>
    /// Called from JavaScript when the pointer goes up after a drag.
    /// </summary>
    [JSInvokable]
    public void OnPointerUp()
    {
        this._hasDragSnapshot = false;
    }

    /// <summary>
    /// Called from JavaScript when clicking empty space without dragging.
    /// </summary>
    [JSInvokable]
    public async Task OnPointerClickAsync()
    {
        if (this._createMode || this._jsModule is null)
        {
            return;
        }

        this._focusedObject = null;
        await this.UpdateSelectValueAsync().ConfigureAwait(true);
        this.StateHasChanged();
    }

    /// <summary>
    /// Called from JavaScript when the zoom level changes.
    /// </summary>
    /// <param name="zoomLevel">The new zoom level.</param>
    [JSInvokable]
    public void OnZoomChanged(float zoomLevel)
    {
        this._zoomManager?.SyncZoomLevel(zoomLevel);
    }

    /// <summary>
    /// Called from JavaScript when a resize operation ends.
    /// </summary>
    [JSInvokable]
    public void OnResizingEnd()
    {
        this._resizerPosition = null;
        if (this._focusedObject is not null)
        {
            this.NotificationService.NotifyChange(this._focusedObject, null);
        }
    }

    /// <summary>
    /// Called from JavaScript when the DELETE key is pressed.
    /// Removes the currently focused object from the map.
    /// </summary>
    [JSInvokable]
    public void OnDeleteKeyPressed()
    {
        if (this._focusedObject is null || this._createMode)
        {
            return;
        }

        this.RemoveFocusedObject();
        _ = this.UpdateSelectValueAsync();
        this.StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.NotificationService.PropertyChanged += this.OnPropertyChanged;
        await base.OnInitializedAsync().ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this._jsModule = await this.JsRuntime
                .InvokeAsync<IJSObjectReference>("import", JsModulePath)
                .ConfigureAwait(true);
        }

        if (this._jsModule is not { } jsModule)
        {
            return;
        }

        if (!this._isInitialized)
        {
            var zoomLevel = this._zoomManager?.ZoomLevel ?? MapZoomManager.DefaultZoom;
            this._zoomManager = new MapZoomManager(jsModule, this._mapHostRef);
            this._objectFactory = new MapObjectFactory(this.PersistenceContext);
            this._crudService = new MapCrudOperationsService(this._objectFactory, this._history);
            this._dotNetRef = DotNetObjectReference.Create(this);

            await jsModule.InvokeVoidAsync(
                "initialize",
                this._mapHostRef,
                this._dotNetRef,
                zoomLevel,
                MapCoordinateService.BaseScale).ConfigureAwait(true);

            await jsModule.InvokeVoidAsync(
                "setScroll", this._mapHostRef, 0, 0).ConfigureAwait(true);

            this._isInitialized = true;
        }
        else if (!firstRender)
        {
            // Re-register document listeners after each render,
            // keeping them resilient to DOM replacement by Blazor.
            var zoomLevel = this._zoomManager?.ZoomLevel ?? MapZoomManager.DefaultZoom;
            await jsModule.InvokeVoidAsync(
                "initialize",
                this._mapHostRef,
                this._dotNetRef,
                zoomLevel,
                MapCoordinateService.BaseScale).ConfigureAwait(true);
        }
        else
        {
            // no initialization required.
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (this.Maps is not { Count: > 0 })
        {
            throw new InvalidOperationException("The Maps parameter is required and must not be empty.");
        }

        if (this._selectedMap is null)
        {
            this.SelectedMap = this.SelectedMapId == Guid.Empty
                ? this.Maps.First()
                : this.Maps.First(m => m.GetId() == this.SelectedMapId);
        }
    }

    private static int IndexOf(IReadOnlyList<object> items, object target)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (ReferenceEquals(items[i], target))
            {
                return i;
            }
        }

        return -1;
    }

    [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            if (sender == this._focusedObject && !this._isRendering)
            {
                this._isRendering = true;
                await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
                this._isRendering = false;
            }
        }
        catch (ObjectDisposedException)
        {
            // Ignored, component was disposed before callback completed.
        }
        catch (JSDisconnectedException)
        {
            // Ignored, browser tab was closed or navigated away.
        }
    }

    private void ToggleFilter(ObjectTypeFilter filter)
    {
        this._filterService.ToggleFilter(filter);
        if (this._focusedObject is not null && this._filterService.IsObjectFilteredOut(this._focusedObject))
        {
            this._focusedObject = null;
        }
    }

    private void SetAllFilter()
    {
        this._filterService.SetAllFilter();
        this._focusedObject = null;
    }

    private bool IsFilterActive(ObjectTypeFilter filter) =>
        this._filterService.IsFilterActive(filter);

    private int GetObjectListSize() =>
        this._filterService.GetObjectListSize(this.SelectedMap);

    private IEnumerable<object> GetMapObjects() =>
        this._filterService.GetMapObjects(this.SelectedMap);

    private string? GetObjectSize(object obj) =>
        MapObjectSelector.GetObjectSize(obj, this._focusedObject);

    private bool ShowResizers(object obj)
    {
        if (obj is MonsterSpawnArea spawn && spawn.IsPoint())
        {
            return false;
        }

        return obj == this._focusedObject;
    }

    private bool ShowArrow(object obj, [MaybeNullWhen(false)] out MonsterSpawnArea spawn)
    {
        spawn = obj as MonsterSpawnArea;
        return spawn is not null
               && this._focusedObject == spawn
               && spawn.IsPoint();
    }

    private string GetCssClass(object obj) =>
        this._styleService.GetCssClass(obj, this._focusedObject);

    private string GetGridStyle() =>
        this._coordinateService.GetGridStyle(this.EffectiveScale);

    private Dictionary<string, object> GetDataAttributes(object obj)
    {
        if (obj is IMapArea area)
        {
            return new Dictionary<string, object>
            {
                ["data-x1"] = area.X1,
                ["data-y1"] = area.Y1,
                ["data-x2"] = area.X2,
                ["data-y2"] = area.Y2,
            };
        }

        return [];
    }

    private string GetSizeAndPositionStyle(object obj) =>
        this._styleService.GetSizeAndPositionStyle(obj, this.EffectiveScale);

    private async Task OnObjectSelectedAsync(ChangeEventArgs args)
    {
        if (!Guid.TryParse(args.Value?.ToString(), out var selectedId))
        {
            return;
        }

        var obj = this.SelectedMap.EnterGates
                      .FirstOrDefault<object>(g => g.GetId() == selectedId)
                  ?? this.SelectedMap.ExitGates
                      .FirstOrDefault<object>(g => g.GetId() == selectedId)
                  ?? this.SelectedMap.MonsterSpawns
                      .FirstOrDefault(g => g.GetId() == selectedId);

        if (obj is not null && !MapObjectSelector.MatchesFilters(obj, this._filterService.ActiveFilter, this._filterService.SearchFilter))
        {
            return;
        }

        this._focusedObject = obj;

        if (obj is not null && this._jsModule is not null)
        {
            var scrollTarget = MapObjectSelector.GetScrollTarget(obj);
            if (scrollTarget is { } st)
            {
                await this._jsModule.InvokeVoidAsync(
                    "centerOn",
                    this._mapHostRef,
                    st.X,
                    st.Y,
                    MapCoordinateService.BaseScale).ConfigureAwait(true);
            }
        }
    }

    private async Task OnWheelAsync(WheelEventArgs args)
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.HandleWheelAsync(args.DeltaY, args.ClientX, args.ClientY).ConfigureAwait(true);
    }

    private async Task ZoomInAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ZoomInAsync().ConfigureAwait(true);
    }

    private async Task ZoomOutAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ZoomOutAsync().ConfigureAwait(true);
    }

    private async Task ResetZoomAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ResetZoomAsync().ConfigureAwait(true);
    }

    private async Task UpdateSelectValueAsync()
    {
        if (this._jsModule is null)
        {
            return;
        }

        var id = this._focusedObject switch
        {
            EnterGate gate => gate.GetId(),
            ExitGate gate => gate.GetId(),
            MonsterSpawnArea spawn => spawn.GetId(),
            _ => Guid.Empty,
        };

        await this._jsModule.InvokeVoidAsync(
            "setSelectValue", this._objectSelectRef, id.ToString()).ConfigureAwait(true);
    }

    private void OnObjectDragging(byte x, byte y)
    {
        if (!this._dragState.ApplyDrag(x, y, out var newX1, out var newY1, out var newX2, out var newY2))
        {
            return;
        }

        if (!this._hasDragSnapshot)
        {
            this._hasDragSnapshot = true;
            this.RecordDragSnapshot();
        }

        this.ApplyNewBounds(newX1, newY1, newX2, newY2);
    }

    private void RecordDragSnapshot()
    {
        // MonsterSpawnArea snapshot also captures Direction, so the two overloads are intentionally distinct.
        switch (this._focusedObject)
        {
            case MonsterSpawnArea spawn:
                this._history.RecordSnapshot(spawn);
                break;
            case Gate gate:
                this._history.RecordSnapshot(gate);
                break;
            default:
                // Not supported.
                break;
        }
    }

    private void ApplyNewBounds(byte x1, byte y1, byte x2, byte y2)
    {
        if (this._focusedObject is not IMapArea area)
        {
            return;
        }

        area.X1 = x1;
        area.Y1 = y1;
        area.X2 = x2;
        area.Y2 = y2;
    }

    private string? GetFocusedRotation()
    {
        if (this._focusedObject is MonsterSpawnArea spawn)
        {
            return DirectionNames[(int)spawn.Direction];
        }

        return null;
    }

    private void CreateNewSpawnArea()
    {
        this._createMode = true;
        this._focusedObject = this._crudService!.CreateSpawnArea(this.SelectedMap);
    }

    private void CreateNewEnterGate()
    {
        this._createMode = true;
        this._focusedObject = this._crudService!.CreateEnterGate(this.SelectedMap);
    }

    private void CreateNewExitGate()
    {
        this._createMode = true;
        this._focusedObject = this._crudService!.CreateExitGate(this.SelectedMap);
    }

    private void CancelCreation()
    {
        if (!this._createMode)
        {
            return;
        }

        this._createMode = false;
        this.RemoveFocusedObject();
    }

    private void RemoveFocusedObject()
    {
        if (this._focusedObject is null)
        {
            return;
        }

        this._crudService!.RemoveFromMap(this.SelectedMap, this._focusedObject, this._pendingDeletions);
        this._focusedObject = null;
    }

    private async Task OnMapSelectedAsync(ChangeEventArgs args)
    {
        if (args.Value is not string idString || !Guid.TryParse(idString, out var mapId))
        {
            return;
        }

        var cancelEventArgs = new MapChangingArgs(mapId);
        if (this.SelectedMapChanging.HasDelegate)
        {
            await this.SelectedMapChanging.InvokeAsync(cancelEventArgs).ConfigureAwait(true);
        }

        if (cancelEventArgs.Cancel)
        {
            if (this._jsModule is not null)
            {
                await this._jsModule.InvokeVoidAsync(
                    "setSelectValue", this._mapSelectRef, this.SelectedMap.GetId().ToString()).ConfigureAwait(true);
            }

            return;
        }

        this.SelectedMap = this.Maps.First(m => m.GetId() == mapId);
        this._focusedObject = null;
        this._isInitialized = false;
        this._createMode = false;
        this._zoomManager = null;
        this._history.Clear();
        this._pendingDeletions.Clear();
    }

    private void DuplicateFocusedObject()
    {
        if (this._focusedObject is null)
        {
            return;
        }

        var duplicate = this._crudService!.DuplicateObject(this._focusedObject, this.SelectedMap);
        if (duplicate is not null)
        {
            this._focusedObject = duplicate;
        }
    }

    private void UndoLastAction()
    {
        var affected = this._history.Undo();
        if (affected is not null)
        {
            var inMap = affected switch
            {
                MonsterSpawnArea spawn => this.SelectedMap.MonsterSpawns.Contains(spawn),
                EnterGate enterGate => this.SelectedMap.EnterGates.Contains(enterGate),
                ExitGate exitGate => this.SelectedMap.ExitGates.Contains(exitGate),
                _ => false,
            };

            if (inMap)
            {
                this._pendingDeletions.Remove(affected);
            }
            else
            {
                this.PersistenceContext.Detach(affected);
            }
        }

        this._focusedObject = affected;
    }

    private async Task SaveAsync()
    {
        foreach (var obj in this._pendingDeletions)
        {
            await this.PersistenceContext.DeleteAsync(obj).ConfigureAwait(true);
        }

        this._pendingDeletions.Clear();
        this._history.Clear();

        if (this.OnValidSubmit.HasDelegate)
        {
            await this.OnValidSubmit.InvokeAsync().ConfigureAwait(true);
        }
    }

    private void OnStartResizing(Resizers.ResizerPosition? position)
    {
        if (position is not null && this._focusedObject is not null)
        {
            switch (this._focusedObject)
            {
                case MonsterSpawnArea spawn:
                    this._history.RecordSnapshot(spawn);
                    break;
                case Gate gate:
                    this._history.RecordSnapshot(gate);
                    break;
                default:
                    // Not supported.
                    break;
            }
        }

        this._resizerPosition = position;
    }

    private void HandleShiftClick(byte x, byte y)
    {
        if (this.SelectedMap is not { } map)
        {
            return;
        }

        var objects = this._objectSelector.GetAllObjectsAtPosition(
            map, x, y, this._filterService.ActiveFilter, this._filterService.SearchFilter);

        if (objects.Count > 0)
        {
            var idx = this._focusedObject is not null
                ? IndexOf(objects, this._focusedObject)
                : -1;

            this._focusedObject = objects[idx >= 0 ? (idx + 1) % objects.Count : 0];
        }

        _ = this.UpdateSelectValueAsync();
        this.StateHasChanged();
    }

    private async Task ExportMapAsync()
    {
        if (this._selectedMap is null)
        {
            return;
        }

        this._isExporting = true;
        using var loading = this.LoadingOverlay.ShowLoadingIndicator();
        try
        {
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

            var json = this._exportImportService.BuildExport(this._selectedMap);
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            var fileName = $"map-{this._selectedMap.Number}-spawns.json";

            if (this._jsModule is not null)
            {
                await this._jsModule.InvokeVoidAsync("downloadFile", fileName, base64).ConfigureAwait(true);
            }
        }
        finally
        {
            this._isExporting = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task TriggerFileImportAsync()
    {
        if (this._jsModule is not null)
        {
            await this._jsModule.InvokeVoidAsync("triggerFileInput").ConfigureAwait(true);
        }
    }

    private async Task ImportFileSelected(IFileListEntry[] files)
    {
        if (this._selectedMap is null || this.PersistenceContext is null)
        {
            return;
        }

        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        this._isImporting = true;
        using var loading = this.LoadingOverlay.ShowLoadingIndicator();
        try
        {
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);

            await using var memoryStream = new MemoryStream();
            await file.Data.CopyToAsync(memoryStream).ConfigureAwait(true);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            var json = await reader.ReadToEndAsync().ConfigureAwait(true);

            await this._exportImportService.ApplyImportAsync(this._selectedMap, json, this.PersistenceContext).ConfigureAwait(true);

            this._history.Clear();
            this._pendingDeletions.Clear();
            this._focusedObject = null;
        }
        finally
        {
            this._isImporting = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }


}
