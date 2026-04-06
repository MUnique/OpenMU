// <copyright file="MapEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    private string? _terrainImageDataUrl;
    private ElementReference _mapHostRef;
    private ElementReference _objectSelectRef;
    private IJSObjectReference? _jsModule;

    private GameMapDefinition? _selectedMap;

    private bool _showGrid;
    private bool _isInitialized;
    private bool _createMode;
    private bool _isDragging;
    private bool _isPanning;
    private bool _isRendering;
    private bool _undoStepRecorded;

    private object? _focusedObject;
    private string _searchFilter = string.Empty;
    private ObjectTypeFilter _activeFilter = ObjectTypeFilter.All;
    private Resizers.ResizerPosition? _resizerPosition;

    private int _mouseMapX;
    private int _mouseMapY;

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

    /// <summary>
    /// Gets the current zoom level expressed as a rounded percentage.
    /// </summary>
    private int ZoomPercentage => this._zoomManager?.ZoomPercentage ?? 100;

    /// <summary>
    /// Gets the effective pixel scale for the current zoom level.
    /// </summary>
    private float EffectiveScale => this._coordinateService.GetEffectiveScale(this._zoomManager?.ZoomLevel ?? 1.0f);

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
                this._mapHostRef,
                zoomLevel).ConfigureAwait(true);

            this._isInitialized = true;
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

    private void SetActiveFilter(ObjectTypeFilter filter)
    {
        this._activeFilter = filter;
        if (this._focusedObject is not null &&
            !MapObjectSelector.MatchesFilters(this._focusedObject, this._activeFilter, this._searchFilter))
        {
            this._focusedObject = null;
        }
    }

    private int GetObjectListSize()
    {
        var count = 1
            + this.SelectedMap.EnterGates
                .Count(g => MapObjectSelector.MatchesFilters(g, this._activeFilter, this._searchFilter))
            + this.SelectedMap.ExitGates
                .Count(g => MapObjectSelector.MatchesFilters(g, this._activeFilter, this._searchFilter))
            + this.SelectedMap.MonsterSpawns
                .Count(s => MapObjectSelector.MatchesFilters(s, this._activeFilter, this._searchFilter));

        return Math.Min(count, MaxObjectSelectSize);
    }

    private IEnumerable<object> GetMapObjects()
    {
        return this._activeFilter switch
        {
            ObjectTypeFilter.All =>
                this.SelectedMap.EnterGates.Cast<object>()
                    .Concat(this.SelectedMap.ExitGates)
                    .Concat(this.SelectedMap.MonsterSpawns)
                    .Where(o => MapObjectSelector.MatchesFilters(o, this._activeFilter, this._searchFilter)),
            ObjectTypeFilter.Gates =>
                this.SelectedMap.EnterGates.Cast<object>()
                    .Concat(this.SelectedMap.ExitGates)
                    .Where(o => MapObjectSelector.MatchesFilters(o, this._activeFilter, this._searchFilter)),
            _ =>
                this.SelectedMap.MonsterSpawns
                    .Where(s => MapObjectSelector.MatchesFilters(s, this._activeFilter, this._searchFilter)),
        };
    }

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

        if (obj is not null && !MapObjectSelector.MatchesFilters(obj, this._activeFilter, this._searchFilter))
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
        var objectAtPosition = this._objectSelector.GetObjectAtPosition(
            this.SelectedMap, x, y, this._activeFilter, this._searchFilter);

        if (objectAtPosition is not null)
        {
            this._focusedObject = objectAtPosition;
            this._isDragging = true;
            this._undoStepRecorded = false;
            this._dragStartX = x;
            this._dragStartY = y;

            if (objectAtPosition is MonsterSpawnArea spawn)
            {
                this._dragObjX1 = spawn.X1;
                this._dragObjY1 = spawn.Y1;
                this._dragObjX2 = spawn.X2;
                this._dragObjY2 = spawn.Y2;
            }
            else if (objectAtPosition is Gate gate)
            {
                this._dragObjX1 = gate.X1;
                this._dragObjY1 = gate.Y1;
                this._dragObjX2 = gate.X2;
                this._dragObjY2 = gate.Y2;
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
            if (this._focusedObject is MonsterSpawnArea spawnArea)
            {
                this.OnSpawnAreaResizing(spawnArea, x, y);
            }
            else if (this._focusedObject is Gate gate)
            {
                this.OnGateResizing(gate, x, y);
            }
            else
            {
                // Other object types don't support corner-resizing they are moved via dragging handled separately.
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

    private void OnMapMouseUp(MouseEventArgs _)
    {
        this._resizerPosition = null;
        this._isDragging = false;
        this._isPanning = false;
    }

    private void OnMapMouseLeave(MouseEventArgs _)
    {
        this._isPanning = false;
    }

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
                if (spawn.X1 != newX1 || spawn.Y1 != newY1 || spawn.X2 != newX2 || spawn.Y2 != newY2)
                {
                    if (!this._undoStepRecorded)
                    {
                        this._history.RecordSnapshot(spawn);
                        this._undoStepRecorded = true;
                    }


                    spawn.X1 = (byte)newX1;
                    spawn.Y1 = (byte)newY1;
                    spawn.X2 = (byte)newX2;
                    spawn.Y2 = (byte)newY2;
                }

                break;
            case Gate gate:
                if (gate.X1 != newX1 || gate.Y1 != newY1 || gate.X2 != newX2 || gate.Y2 != newY2)
                {
                    if (!this._undoStepRecorded)
                    {
                        this._history.RecordSnapshot(gate);
                        this._undoStepRecorded = true;
                    }

                    gate.X1 = (byte)newX1;
                    gate.Y1 = (byte)newY1;
                    gate.X2 = (byte)newX2;
                    gate.Y2 = (byte)newY2;
                }

                break;
            default:
                throw new InvalidOperationException($"Dragging is not supported for {this._focusedObject!.GetType().Name}.");
        }
    }

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
            default:
                // Not supported.
                break;
        }
    }

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
            default:
                // Not supported.
                break;
        }
    }

    private string? GetFocusedRotation()
    {
        if (this._focusedObject is MonsterSpawnArea spawn)
        {
            return DirectionNames[(int)spawn.Direction];
        }

        return null;
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

    private void CreateNewSpawnArea()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateSpawnArea(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
    }

    private void CreateNewEnterGate()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateEnterGate(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
    }

    private void CreateNewExitGate()
    {
        this._createMode = true;
        this._focusedObject = this._objectFactory!.CreateExitGate(this.SelectedMap);
        this._history.RecordCreation(this.SelectedMap, this._focusedObject);
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
                // Not supported.
                return;
        }

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
            return;
        }

        this.SelectedMap = this.Maps.First(m => m.GetId() == mapId);
        this._focusedObject = null;
        this._isInitialized = false;
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

        var duplicate = this._objectFactory!.Duplicate(this._focusedObject, this.SelectedMap);
        if (duplicate is not null)
        {
            this._history.RecordCreation(this.SelectedMap, duplicate);
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

    /// <summary>
    /// Represents the bounding client rectangle of an HTML element.
    /// </summary>
    public sealed class BoundingClientRect
    {
        /// <summary>Gets or sets the left edge position relative to the viewport.</summary>
        public double Left { get; set; }

        /// <summary>Gets or sets the top edge position relative to the viewport.</summary>
        public double Top { get; set; }
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