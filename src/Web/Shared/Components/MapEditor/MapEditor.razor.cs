// <copyright file="MapEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// A graphical editor for a <see cref="GameMapDefinition"/>.
/// </summary>
public partial class MapEditor : IAsyncDisposable
{
    private const int MapSize = 256;
    private const int MaxObjectSelectSize = 30;

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

    private MapZoomManager? _zoomManager;
    private MapObjectFactory? _objectFactory;

    private Image<Rgba32>? _terrainImage;
    private ElementReference _mapHostRef;
    private ElementReference _objectSelectRef;
    private IJSObjectReference? _jsModule;
    private bool _showGrid;
    private bool _isInitialized;

    private bool _createMode;
    private object? _focusedObject;
    private string _searchFilter = string.Empty;
    private ObjectTypeFilter _activeFilter = ObjectTypeFilter.All;
    private GameMapDefinition? _selectedMap;
    private Resizers.ResizerPosition? _resizerPosition;

    private int _mouseMapX;
    private int _mouseMapY;

    private bool _isDragging;
    private bool _isPanning;
    private bool _isRendering;
    private byte _dragStartX;
    private byte _dragStartY;
    private byte _dragObjX1;
    private byte _dragObjY1;
    private byte _dragObjX2;
    private byte _dragObjY2;

    /// <summary>
    /// Gets or sets the callback invoked before the selected map changes,
    /// allowing the caller to cancel the change.
    /// </summary>
    [Parameter]
    public EventCallback<MapChangingArgs>? SelectedMapChanging { get; set; }

    /// <summary>
    /// Gets or sets the full list of available maps to display in the map selector.
    /// </summary>
    [Parameter]
    public List<GameMapDefinition> Maps { get; set; } = null!;

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

    /// <summary>
    /// Gets the current zoom level expressed as a rounded percentage.
    /// </summary>
    private int ZoomPercentage => this._zoomManager?.ZoomPercentage ?? 100;

    /// <summary>
    /// Gets the effective pixel scale for the current zoom level.
    /// </summary>
    private float EffectiveScale => this._coordinateService.GetEffectiveScale(
        this._zoomManager?.ZoomLevel ?? 1.0f);

    /// <summary>
    /// Gets or sets the currently selected map, rebuilding the terrain image when changed.
    /// </summary>
    private GameMapDefinition SelectedMap
    {
        get => this._selectedMap ?? throw new InvalidOperationException($"{nameof(this.SelectedMap)} is not initialized.");
        set
        {
            this._selectedMap = value;
            this._terrainImage = new GameMapTerrain(this.SelectedMap).ToImage();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
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

        if (this._jsModule is not null && !this._isInitialized)
        {
            var zoomLevel = this._zoomManager?.ZoomLevel ?? 1.0f;
            this._zoomManager = new MapZoomManager(this._jsModule, this._mapHostRef);
            this._objectFactory = new MapObjectFactory(this.PersistenceContext);

            await this._jsModule.InvokeVoidAsync(
                "initialize",
                DotNetObjectReference.Create(this),
                this._mapHostRef,
                zoomLevel).ConfigureAwait(true);

            this._isInitialized = true;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (this._selectedMap is null)
        {
            this.SelectedMap = this.SelectedMapId == Guid.Empty
                ? this.Maps.First()
                : this.Maps.First(m => m.GetId() == this.SelectedMapId);
        }
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
        catch
        {
            // Ignore
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> if the given object matches the current search filter.
    /// </summary>
    /// <param name="obj">The object to test against the search filter.</param>
    /// <returns><see langword="true"/> if the object matches or the filter is empty.</returns>
    private bool MatchesSearch(object? obj)
    {
        if (string.IsNullOrWhiteSpace(this._searchFilter))
        {
            return true;
        }

        return obj?.ToString()?.Contains(
            this._searchFilter, StringComparison.InvariantCultureIgnoreCase) ?? false;
    }

    /// <summary>
    /// Returns the number of objects in the object list, capped at <see cref="MaxObjectSelectSize"/>.
    /// </summary>
    /// <returns>The capped object count for the select element size attribute.</returns>
    private int GetObjectListSize()
    {
        var count = 1
            + this.SelectedMap.EnterGates.Count(this.MatchesSearch)
            + this.SelectedMap.ExitGates.Count(this.MatchesSearch)
            + this.SelectedMap.MonsterSpawns.Count(this.MatchesSearch);

        return Math.Min(count, MaxObjectSelectSize);
    }

    /// <summary>
    /// Returns all map objects as a flat sequence for rendering and selection.
    /// </summary>
    /// <returns>A sequence of enter gates, exit gates, and monster spawns.</returns>
    private IEnumerable<object> GetMapObjects()
    {
        return this.SelectedMap.EnterGates.OfType<object>()
            .Concat(this.SelectedMap.ExitGates)
            .Concat(this.SelectedMap.MonsterSpawns);
    }

    /// <summary>
    /// Returns a display string for the size of the focused map object,
    /// or <see langword="null"/> if the object does not have a displayable size.
    /// </summary>
    /// <param name="obj">The map object to compute the size for.</param>
    /// <returns>A formatted size string, or <see langword="null"/>.</returns>
    private string? GetObjectSize(object obj) =>
        MapObjectSelector.GetObjectSize(obj, this._focusedObject);

    /// <summary>
    /// Returns <see langword="true"/> if resize handles should be shown for the given object.
    /// </summary>
    /// <param name="obj">The map object to check.</param>
    /// <returns>
    /// <see langword="true"/> if the object is focused and is not a point spawn.
    /// </returns>
    private bool ShowResizers(object obj)
    {
        if (obj is MonsterSpawnArea spawn && spawn.IsPoint())
        {
            return false;
        }

        return obj == this._focusedObject;
    }

    /// <summary>
    /// Returns <see langword="true"/> if a direction arrow should be shown for the given object.
    /// </summary>
    /// <param name="obj">The map object to check.</param>
    /// <param name="spawn">
    /// When this method returns <see langword="true"/>, contains the focused point spawn;
    /// otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the object is a focused point spawn; otherwise <see langword="false"/>.
    /// </returns>
    private bool ShowArrow(object obj, [MaybeNullWhen(false)] out MonsterSpawnArea spawn)
    {
        spawn = obj as MonsterSpawnArea;
        return spawn is not null
               && this._focusedObject == spawn
               && spawn.IsPoint();
    }

    /// <summary>
    /// Returns the CSS class string for the given map object.
    /// </summary>
    /// <param name="obj">The map object to style.</param>
    /// <returns>A CSS class string appropriate for the object type and focus state.</returns>
    private string GetCssClass(object obj)
    {
        var css = this._styleService.GetCssClass(obj, this._focusedObject);
        if (!this.MatchesFilters(obj))
        {
            css += " filtered-out";
        }

        return css;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the given object passes the active type filter and search filter.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns><see langword="true"/> if the object should be shown.</returns>
    private bool MatchesFilters(object? obj)
    {
        if (!this.MatchesSearch(obj))
        {
            return false;
        }

        var isSpawn = obj is MonsterSpawnArea;
        var objectKind = (obj as MonsterSpawnArea)?.MonsterDefinition?.ObjectKind;

        return this._activeFilter switch
        {
            ObjectTypeFilter.Monsters => isSpawn && objectKind == NpcObjectKind.Monster,
            ObjectTypeFilter.Npcs => isSpawn && (objectKind == NpcObjectKind.PassiveNpc || objectKind == NpcObjectKind.Guard),
            ObjectTypeFilter.Gates => obj is EnterGate or ExitGate,
            ObjectTypeFilter.Others => isSpawn && (objectKind == NpcObjectKind.Trap
                                           || objectKind == NpcObjectKind.Statue
                                           || objectKind == NpcObjectKind.SoccerBall
                                           || objectKind == NpcObjectKind.Destructible),
            _ => true,
        };
    }

    /// <summary>
    /// Returns the CSS background-size style for the map grid overlay.
    /// </summary>
    /// <returns>An inline CSS style string for the grid background size.</returns>
    private string GetGridStyle() =>
        this._coordinateService.GetGridStyle(this.EffectiveScale);

    /// <summary>
    /// Returns the inline CSS size and position style for the given map object.
    /// </summary>
    /// <param name="obj">The map object to compute styles for.</param>
    /// <returns>An inline CSS style string defining width, height, top, and left.</returns>
    private string GetSizeAndPositionStyle(object obj) =>
        this._styleService.GetSizeAndPositionStyle(obj, this.EffectiveScale);

    /// <summary>
    /// Handles selection of a map object from the object list dropdown.
    /// </summary>
    /// <param name="args">The change event args containing the selected object ID.</param>
    private async Task OnObjectSelectedAsync(ChangeEventArgs args)
    {
        var obj = this.SelectedMap.EnterGates
                      .FirstOrDefault<object>(g => g.GetId().ToString() == args.Value?.ToString())
                  ?? this.SelectedMap.ExitGates
                      .FirstOrDefault<object>(g => g.GetId().ToString() == args.Value?.ToString())
                  ?? this.SelectedMap.MonsterSpawns
                      .FirstOrDefault(g => g.GetId().ToString() == args.Value?.ToString());

        this._focusedObject = obj;

        if (obj is not null && this._jsModule is not null)
        {
            var center = MapObjectSelector.GetObjectCenter(obj);
            if (center is { } c)
            {
                await this._jsModule.InvokeVoidAsync(
                    "centerOn",
                    this._mapHostRef,
                    c.X,
                    c.Y,
                    MapCoordinateService.BaseScale).ConfigureAwait(true);
            }
        }
    }

    /// <summary>
    /// Handles a mouse down event on the map, initiating dragging or panning.
    /// </summary>
    /// <param name="args">The mouse event args.</param>
    private async Task OnMapMouseDownAsync(MouseEventArgs args)
    {
        if (args.Button != 0 || this._createMode || this._jsModule is null || this._zoomManager is null)
        {
            return;
        }

        if (this._resizerPosition is not null)
        {
            return;
        }

        var state = await this._jsModule
            .InvokeAsync<MapHostState>("getState", this._mapHostRef)
            .ConfigureAwait(true);

        var coords = this._coordinateService.GetMapCoordinates(
            args.ClientX, args.ClientY, state.Rect, state.Scroll, state.ZoomLevel);

        if (!coords.HasValue)
        {
            return;
        }

        var (x, y) = coords.Value;
        var objectAtPosition = this._objectSelector.GetObjectAtPosition(this.SelectedMap, x, y);

        if (objectAtPosition is not null)
        {
            this._focusedObject = objectAtPosition;
            this._isDragging = true;
            this._dragStartX = x;
            this._dragStartY = y;

            switch (objectAtPosition)
            {
                case MonsterSpawnArea spawn:
                    this._history.RecordSnapshot(spawn);
                    this._dragObjX1 = spawn.X1;
                    this._dragObjY1 = spawn.Y1;
                    this._dragObjX2 = spawn.X2;
                    this._dragObjY2 = spawn.Y2;
                    break;
                case Gate gate:
                    this._history.RecordSnapshot(gate);
                    this._dragObjX1 = gate.X1;
                    this._dragObjY1 = gate.Y1;
                    this._dragObjX2 = gate.X2;
                    this._dragObjY2 = gate.Y2;
                    break;
            }

            await this.UpdateSelectValueAsync().ConfigureAwait(true);
        }
        else
        {
            this._focusedObject = null;
            await this.UpdateSelectValueAsync().ConfigureAwait(true);
            this._isPanning = true;
        }
    }

    /// <summary>
    /// Handles mouse move events on the map, updating coordinates and applying
    /// dragging, resizing, or panning as appropriate.
    /// </summary>
    /// <param name="args">The mouse event args.</param>
    private async Task OnMouseMoveAsync(MouseEventArgs args)
    {
        if (this._jsModule is null || this._zoomManager is null)
        {
            return;
        }

        var state = await this._jsModule
            .InvokeAsync<MapHostState>("getState", this._mapHostRef)
            .ConfigureAwait(true);

        var coords = this._coordinateService.GetMapCoordinates(
            args.ClientX, args.ClientY, state.Rect, state.Scroll, state.ZoomLevel);

        if (coords.HasValue)
        {
            this._mouseMapX = coords.Value.X;
            this._mouseMapY = coords.Value.Y;
        }

        if (this._resizerPosition is not null && this._focusedObject is not null && coords.HasValue)
        {
            var (x, y) = coords.Value;
            switch (this._focusedObject)
            {
                case MonsterSpawnArea spawnArea:
                    this.OnSpawnAreaResizing(spawnArea, x, y);
                    break;
                case Gate gate:
                    this.OnGateResizing(gate, x, y);
                    break;
            }

            this.NotificationService.NotifyChange(this._focusedObject, null);
            return;
        }

        if (this._isDragging && this._focusedObject is not null && coords.HasValue)
        {
            this.OnObjectDragging(coords.Value.X, coords.Value.Y);
            this.NotificationService.NotifyChange(this._focusedObject, null);
            return;
        }

        if (this._isPanning && args.Buttons == 1)
        {
            var newScrollLeft = state.Scroll.ScrollLeft - args.MovementX;
            var newScrollTop = state.Scroll.ScrollTop - args.MovementY;

            await this._jsModule.InvokeVoidAsync(
                "setScroll", this._mapHostRef, newScrollLeft, newScrollTop).ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Handles the mouse up event on the map, ending any drag, resize, or pan operation.
    /// </summary>
    /// <param name="args">The mouse event args.</param>
    private void OnMapMouseUp(MouseEventArgs args)
    {
        this._resizerPosition = null;
        this._isDragging = false;
        this._isPanning = false;
    }

    /// <summary>
    /// Handles the mouse leave event on the map, ending any active panning.
    /// </summary>
    /// <param name="args">The mouse event args.</param>
    private void OnMapMouseLeave(MouseEventArgs args)
    {
        this._isPanning = false;
    }

    /// <summary>
    /// Applies drag movement to the focused object, keeping it within map bounds.
    /// </summary>
    /// <param name="x">The current map X coordinate of the mouse.</param>
    /// <param name="y">The current map Y coordinate of the mouse.</param>
    private void OnObjectDragging(byte x, byte y)
    {
        int dx = x - this._dragStartX;
        int dy = y - this._dragStartY;

        var width = this._dragObjX2 - this._dragObjX1;
        var height = this._dragObjY2 - this._dragObjY1;

        var newX1 = Math.Clamp(this._dragObjX1 + dx, 0, MapSize - 1 - width);
        var newY1 = Math.Clamp(this._dragObjY1 + dy, 0, MapSize - 1 - height);
        var newX2 = newX1 + width;
        var newY2 = newY1 + height;

        switch (this._focusedObject)
        {
            case MonsterSpawnArea spawn:
                spawn.X1 = (byte)newX1;
                spawn.Y1 = (byte)newY1;
                spawn.X2 = (byte)newX2;
                spawn.Y2 = (byte)newY2;
                break;
            case Gate gate:
                gate.X1 = (byte)newX1;
                gate.Y1 = (byte)newY1;
                gate.X2 = (byte)newX2;
                gate.Y2 = (byte)newY2;
                break;
        }
    }

    /// <summary>
    /// Applies resize logic to a <see cref="MonsterSpawnArea"/> based on the active resizer handle.
    /// </summary>
    /// <param name="spawnArea">The spawn area being resized.</param>
    /// <param name="x">The current map X coordinate of the mouse.</param>
    /// <param name="y">The current map Y coordinate of the mouse.</param>
    private void OnSpawnAreaResizing(MonsterSpawnArea spawnArea, byte x, byte y)
    {
        switch (this._resizerPosition)
        {
            case Resizers.ResizerPosition.TopLeft:
                spawnArea.X1 = Math.Min(x, spawnArea.X2);
                spawnArea.Y1 = Math.Min(y, spawnArea.Y2);
                break;
            case Resizers.ResizerPosition.TopRight:
                spawnArea.X1 = Math.Min(x, spawnArea.X2);
                spawnArea.Y2 = Math.Max(y, spawnArea.Y1);
                break;
            case Resizers.ResizerPosition.BottomRight:
                spawnArea.X2 = Math.Max(x, spawnArea.X1);
                spawnArea.Y2 = Math.Max(y, spawnArea.Y1);
                break;
            case Resizers.ResizerPosition.BottomLeft:
                spawnArea.X2 = Math.Max(x, spawnArea.X1);
                spawnArea.Y1 = Math.Min(y, spawnArea.Y2);
                break;
        }
    }

    /// <summary>
    /// Applies resize logic to a <see cref="Gate"/> based on the active resizer handle.
    /// </summary>
    /// <param name="gate">The gate being resized.</param>
    /// <param name="x">The current map X coordinate of the mouse.</param>
    /// <param name="y">The current map Y coordinate of the mouse.</param>
    private void OnGateResizing(Gate gate, byte x, byte y)
    {
        switch (this._resizerPosition)
        {
            case Resizers.ResizerPosition.TopLeft:
                gate.X1 = Math.Min(x, gate.X2);
                gate.Y1 = Math.Min(y, gate.Y2);
                break;
            case Resizers.ResizerPosition.TopRight:
                gate.X1 = Math.Min(x, gate.X2);
                gate.Y2 = Math.Max(y, gate.Y1);
                break;
            case Resizers.ResizerPosition.BottomRight:
                gate.X2 = Math.Max(x, gate.X1);
                gate.Y2 = Math.Max(y, gate.Y1);
                break;
            case Resizers.ResizerPosition.BottomLeft:
                gate.X2 = Math.Max(x, gate.X1);
                gate.Y1 = Math.Min(y, gate.Y2);
                break;
        }
    }

    /// <summary>
    /// Returns the direction name of the focused spawn, if applicable.
    /// </summary>
    /// <returns>A lowercase direction name, or <see langword="null"/> if the focused object is not a spawn.</returns>
    private string? GetFocusedRotation()
    {
        if (this._focusedObject is MonsterSpawnArea spawn)
        {
            return DirectionNames[(int)spawn.Direction];
        }

        return null;
    }

    /// <summary>
    /// Handles a mouse wheel event to zoom the map in or out.
    /// </summary>
    /// <param name="args">The wheel event args.</param>
    private async Task OnWheelAsync(WheelEventArgs args)
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.HandleWheelAsync(args.DeltaY, args.ClientX, args.ClientY).ConfigureAwait(true);
    }

    /// <summary>
    /// Zooms the map in by one step.
    /// </summary>
    private async Task ZoomInAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ZoomInAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Zooms the map out by one step.
    /// </summary>
    private async Task ZoomOutAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ZoomOutAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Resets the zoom level to fit the map within the viewport.
    /// </summary>
    private async Task ResetZoomAsync()
    {
        if (this._zoomManager is null)
        {
            return;
        }

        await this._zoomManager.ResetZoomAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Synchronizes the object select element's value to match the focused object.
    /// </summary>
    private async Task UpdateSelectValueAsync()
    {
        if (this._jsModule is not null && this._focusedObject is not null)
        {
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
        else if (this._jsModule is not null)
        {
            await this._jsModule.InvokeVoidAsync(
                "setSelectValue", this._objectSelectRef, Guid.Empty.ToString()).ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Creates a new <see cref="MonsterSpawnArea"/> and enters creation mode.
    /// </summary>
    private void CreateNewSpawnArea()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateSpawnArea(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
    }

    /// <summary>
    /// Creates a new <see cref="EnterGate"/> and enters creation mode.
    /// </summary>
    private void CreateNewEnterGate()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateEnterGate(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
    }

    /// <summary>
    /// Creates a new <see cref="ExitGate"/> and enters creation mode.
    /// </summary>
    private void CreateNewExitGate()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateExitGate(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
    }

    /// <summary>
    /// Cancels the current creation operation, removing the newly created object.
    /// </summary>
    private void CancelCreation()
    {
        if (!this._createMode)
        {
            return;
        }

        this._createMode = false;
        this.RemoveFocusedObject();
    }

    /// <summary>
    /// Removes the currently focused object from the map.
    /// The deletion is not persisted until the user explicitly saves.
    /// </summary>
    private void RemoveFocusedObject()
    {
        if (this._focusedObject is null)
        {
            return;
        }

        this._history.RecordDeletion(this.SelectedMap, this._focusedObject);

        switch (this._focusedObject)
        {
            case MonsterSpawnArea spawnArea:
                this.SelectedMap.MonsterSpawns.Remove(spawnArea);
                this._pendingDeletions.Add(spawnArea);
                break;
            case EnterGate enterGate:
                this.SelectedMap.EnterGates.Remove(enterGate);
                this._pendingDeletions.Add(enterGate);
                break;
            case ExitGate exitGate:
                this.SelectedMap.ExitGates.Remove(exitGate);
                this._pendingDeletions.Add(exitGate);
                break;
            default:
                return;
        }

        this._focusedObject = null;
    }

    /// <summary>
    /// Handles the map selection dropdown change, loading the newly selected map.
    /// </summary>
    /// <param name="args">The change event args containing the selected map ID.</param>
    private async Task OnMapSelectedAsync(ChangeEventArgs args)
    {
        if (args.Value is string idString && Guid.TryParse(idString, out var mapId))
        {
            var cancelEventArgs = new MapChangingArgs(mapId);
            if (this.SelectedMapChanging is { } callback)
            {
                await callback.InvokeAsync(cancelEventArgs).ConfigureAwait(true);
            }

            if (cancelEventArgs.Cancel)
            {
                return;
            }

            this.SelectedMap = this.Maps.First(m => m.GetId() == mapId);
            this._focusedObject = null;
            this._isInitialized = false;
            this._zoomManager = null;
            this._history.Clear();
            this._pendingDeletions.Clear();
        }
    }

    /// <summary>
    /// Duplicates the currently focused object with a positional offset.
    /// </summary>
    private void DuplicateFocusedObject()
    {
        if (this._focusedObject is null)
        {
            return;
        }

        var duplicate = this._objectFactory!.Duplicate(this._focusedObject, this.SelectedMap);
        if (duplicate is not null)
        {
            this._history.RecordCreation(this.SelectedMap, duplicate);
            this._focusedObject = duplicate;
        }
    }

    /// <summary>
    /// Undoes the most recent map edit operation.
    /// </summary>
    private void UndoLastAction()
    {
        var affected = this._history.Undo();
        if (affected is not null)
        {
            this._pendingDeletions.Remove(affected);
        }

        this._focusedObject = affected;
    }

    /// <summary>
    /// Saves all pending changes to the persistence context, including deferred deletions.
    /// </summary>
    private async Task SaveAsync()
    {
        foreach (var obj in this._pendingDeletions)
        {
            await this.PersistenceContext.DeleteAsync(obj).ConfigureAwait(true);
        }

        this._pendingDeletions.Clear();

        if (this.OnValidSubmit.HasDelegate)
        {
            await this.OnValidSubmit.InvokeAsync().ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Handles the resizer position being set when a resize handle is activated.
    /// </summary>
    /// <param name="position">The resizer handle position that was activated, or <see langword="null"/> to stop resizing.</param>
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
            }
        }

        this._resizerPosition = position;
    }

    /// <summary>
    /// Represents the bounding client rectangle of an HTML element.
    /// </summary>
    public sealed class BoundingClientRect
    {
        /// <summary>Gets or sets the left edge position relative to the viewport.</summary>
        public double Left { get; set; }

        /// <summary>Gets or sets the top edge position relative to the viewport.</summary>
        public double Top { get; set; }

        /// <summary>Gets or sets the width of the element.</summary>
        public double Width { get; set; }

        /// <summary>Gets or sets the height of the element.</summary>
        public double Height { get; set; }
    }

    /// <summary>
    /// Represents the scroll position of an HTML element.
    /// </summary>
    public sealed class ScrollInfo
    {
        /// <summary>Gets or sets the horizontal scroll offset in pixels.</summary>
        public double ScrollLeft { get; set; }

        /// <summary>Gets or sets the vertical scroll offset in pixels.</summary>
        public double ScrollTop { get; set; }
    }

    /// <summary>
    /// Provides event data for the map changing event, supporting cancellation.
    /// </summary>
    public sealed class MapChangingArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapChangingArgs"/> class.
        /// </summary>
        /// <param name="nextMap">The ID of the map being switched to.</param>
        public MapChangingArgs(Guid nextMap)
        {
            this.NextMap = nextMap;
        }

        /// <summary>Gets the ID of the map that is about to be selected.</summary>
        public Guid NextMap { get; }
    }

    /// <summary>
    /// Represents the full state of the map host element retrieved from JavaScript.
    /// </summary>
    private sealed class MapHostState
    {
        /// <summary>Gets or sets the bounding client rect of the map host element.</summary>
        public BoundingClientRect Rect { get; set; } = new();

        /// <summary>Gets or sets the current scroll position of the map host element.</summary>
        public ScrollInfo Scroll { get; set; } = new();

        /// <summary>Gets or sets the current zoom level as maintained by the JS module.</summary>
        public float ZoomLevel { get; set; } = 1.0f;
    }
}