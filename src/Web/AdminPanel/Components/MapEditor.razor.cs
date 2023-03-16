// <copyright file="MapEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Interop;
using MUnique.OpenMU.Web.AdminPanel.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// An graphical editor for a <see cref="GameMapDefinition"/>.
/// </summary>
public partial class MapEditor : IDisposable
{
    private static readonly string[] DirectionNames = Enum.GetNames<Direction>().Select(d => d.ToLowerInvariant()).ToArray();

    private readonly float _scale = 3;

    private Image<Rgba32> _terrainImage = null!;

    private object? _focusedObject;
    private Resizers.ResizerPosition? _resizerPosition;
    private bool _createMode;
    private GameMapDefinition? _selectedMap;

    /// <summary>
    /// Occurs before the selected map changes by user input.
    /// </summary>
    [Parameter]
    public EventCallback<MapChangingArgs>? SelectedMapChanging { get; set; }

    /// <summary>
    /// Gets or sets the maps which can be edited.
    /// </summary>
    [Parameter]
    public List<GameMapDefinition> Maps { get; set; } = null!;

    /// <summary>
    /// Gets or sets the selected map identifier.
    /// </summary>
    [Parameter]
    public Guid SelectedMapId { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="EditForm.OnValidSubmit"/> event callback.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the javascript runtime.
    /// </summary>
    /// <remarks>
    /// It's used for the resizing stuff. There is room for improvement, though.
    /// Currently, every mouse move in the resize process causes additional network roundtrips.
    /// This means, the resizing might be laggy and a bit buggy. It may make sense to do more stuff in javascript instead.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the change notification service.
    /// </summary>
    [Inject]
    private IChangeNotificationService NotificationService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the map which is edited in this component.
    /// </summary>
    // [Parameter]
    private GameMapDefinition SelectedMap
    {
        get => this._selectedMap ?? throw Error.NotInitializedProperty(this);
        set
        {
            this._selectedMap = value;
            this._terrainImage = new GameMapTerrain(this.SelectedMap).ToImage();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.NotificationService.PropertyChanged -= this.OnPropertyChanged;
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.NotificationService.PropertyChanged += this.OnPropertyChanged;
        await base.OnInitializedAsync().ConfigureAwait(false);
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        try
        {
            if (sender == this._focusedObject)
            {
                await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    private Task? OnCancelAsync()
    {
        this._focusedObject = null;
        return null;
    }

    private IEnumerable<object> GetMapObjects()
    {
        return this.SelectedMap.EnterGates.OfType<object>()
            .Concat(this.SelectedMap.ExitGates)
            .Concat(this.SelectedMap.MonsterSpawns);
    }

    private bool ShowResizers(object obj)
    {
        if (obj is MonsterSpawnArea spawn
            && spawn.IsPoint())
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

    private string GetCssClass(object obj)
    {
        var result = string.Empty;
        switch (obj)
        {
            case EnterGate:
                result = "gate-enter";
                break;
            case ExitGate:
                result = "gate-exit";
                break;
            case MonsterSpawnArea spawn:
                result = spawn.IsPoint() ? "spawn-single" : "spawn-area";
                break;
            default:
                // we have no specific css class for others
                break;
        }

        if (this._focusedObject == obj)
        {
            result += " focused-object";
        }

        return result;
    }

    private string GetSizeAndPositionStyle(object obj)
    {
        if (obj is Gate gate)
        {
            return this.GetSizeAndPositionStyle(gate);
        }

        if (obj is MonsterSpawnArea spawn)
        {
            return this.GetSizeAndPositionStyle(spawn);
        }

        return string.Empty;
    }

    private string GetSizeAndPositionStyle(Gate gate)
    {
        return string.Format(
            "width: {0}px; height: {1}px; top: {2}px; left:{3}px;",
            (this._scale * (1 + gate.Y2 - gate.Y1)).ToString(CultureInfo.InvariantCulture),
            (this._scale * (1 + gate.X2 - gate.X1)).ToString(CultureInfo.InvariantCulture),
            (this._scale * gate.X1).ToString(CultureInfo.InvariantCulture),
            (this._scale * gate.Y1).ToString(CultureInfo.InvariantCulture));
    }

    private string GetSizeAndPositionStyle(MonsterSpawnArea spawn)
    {
        var objScale = 1.0f;

        var result = new StringBuilder();

        if (spawn.IsPoint())
        {
            // We want the small point to be more visible, so it's bigger and has a higher opacity.
            objScale = 1.75f;
        }

        // X and Y are twisted, it's not an error!
        var width = objScale * this._scale * (1 + spawn.Y2 - spawn.Y1);
        var height = objScale * this._scale * (1 + spawn.X2 - spawn.X1);
        var xyOffset = (objScale - 1.0f) * this._scale;
        var top = (this._scale * spawn.X1) - xyOffset;
        var left = (this._scale * spawn.Y1) - xyOffset;
        result.Append($"width: {width.ToString(CultureInfo.InvariantCulture)}px;")
            .Append($"height: {height.ToString(CultureInfo.InvariantCulture)}px;")
            .Append($"top: {top.ToString(CultureInfo.InvariantCulture)}px;")
            .Append($"left: {left.ToString(CultureInfo.InvariantCulture)}px;");

        return result.ToString();
    }

    private void OnObjectSelected(ChangeEventArgs args)
    {
        var obj = this.SelectedMap.EnterGates.FirstOrDefault<object>(g => g.GetId().ToString() == args.Value?.ToString())
                  ?? this.SelectedMap.ExitGates.FirstOrDefault<object>(g => g.GetId().ToString() == args.Value?.ToString())
                  ?? this.SelectedMap.MonsterSpawns.FirstOrDefault(g => g.GetId().ToString() == args.Value?.ToString());
        
        this._focusedObject = obj;
    }

    private async Task<(byte X, byte Y)?> OnGetObjectCoordinatesAsync(MouseEventArgs args)
    {
        if (args.Buttons == 1)
        {
            // For GetMapHostBoundingClientRect(), see map.js
            // Warning: it's NOT working with the Edge Browser! BoundingClientRect gets all values with 0.
            var mapClientRect = await this.JsRuntime.InvokeAsync<BoundingClientRect>("GetMapHostBoundingClientRect").ConfigureAwait(false);
            var x = (args.ClientY - mapClientRect.Top) / this._scale;
            var y = (args.ClientX - mapClientRect.Left) / this._scale;
            return ((byte)x, (byte)y);
        }

        return null;
    }

    private void OnStartResizing(Resizers.ResizerPosition? position)
    {
        this._resizerPosition = position;
    }

    private async Task OnMouseMoveAsync(MouseEventArgs args)
    {
        if (this._resizerPosition is null)
        {
            return;
        }

        if (await this.OnGetObjectCoordinatesAsync(args).ConfigureAwait(false) is { } coordinates)
        {
            var (x, y) = coordinates;

            switch (this._focusedObject)
            {
                case MonsterSpawnArea spawnArea:
                    this.OnSpawnAreaResizing(spawnArea, x, y);
                    break;
                case Gate gate:
                    this.OnGateResizing(gate, x, y);
                    break;
                default:
                    // do nothing
                    return;
            }

            this.NotificationService.NotifyChange(this._focusedObject, null);
        }
    }

    private void OnSpawnAreaResizing(MonsterSpawnArea spawnArea, byte x, byte y)
    {
        switch (this._resizerPosition)
        {
            case Resizers.ResizerPosition.TopLeft:
                spawnArea.X1 = x;
                spawnArea.Y1 = y;
                break;
            case Resizers.ResizerPosition.TopRight:
                spawnArea.X1 = x;
                spawnArea.Y2 = y;
                break;
            case Resizers.ResizerPosition.BottomRight:
                spawnArea.X2 = x;
                spawnArea.Y2 = y;
                break;
            case Resizers.ResizerPosition.BottomLeft:
                spawnArea.X2 = x;
                spawnArea.Y1 = y;
                break;
            default:
                // do nothing.
                break;
        }
    }

    private void OnGateResizing(Gate gate, byte x, byte y)
    {
        switch (this._resizerPosition)
        {
            case Resizers.ResizerPosition.TopLeft:
                gate.X1 = x;
                gate.Y1 = y;
                break;
            case Resizers.ResizerPosition.TopRight:
                gate.X1 = x;
                gate.Y2 = y;
                break;
            case Resizers.ResizerPosition.BottomRight:
                gate.X2 = x;
                gate.Y2 = y;
                break;
            case Resizers.ResizerPosition.BottomLeft:
                gate.X2 = x;
                gate.Y1 = y;
                break;
            default:
                // do nothing
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

    private void CreateNewSpawnArea()
    {
        this._createMode = true;

        var area = this.PersistenceContext.CreateNew<MonsterSpawnArea>();
        area.GameMap = this.SelectedMap;
        this.SelectedMap.MonsterSpawns.Add(area);
        area.X1 = 100;
        area.Y1 = 100;
        area.X2 = 200;
        area.Y2 = 200;
        area.Quantity = 1;

        this._focusedObject = area;
    }

    private void CreateNewEnterGate()
    {
        this._createMode = true;

        var enterGate = this.PersistenceContext.CreateNew<EnterGate>();
        this.SelectedMap.EnterGates.Add(enterGate);
        enterGate.X1 = 120;
        enterGate.Y1 = 120;
        enterGate.X2 = 140;
        enterGate.Y2 = 140;

        this._focusedObject = enterGate;
    }

    private void CreateNewExitGate()
    {
        this._createMode = true;

        var exitGate = this.PersistenceContext.CreateNew<ExitGate>();
        exitGate.Map = this.SelectedMap;
        this.SelectedMap.ExitGates.Add(exitGate);
        exitGate.X1 = 120;
        exitGate.Y1 = 120;
        exitGate.X2 = 140;
        exitGate.Y2 = 140;

        this._focusedObject = exitGate;
    }

    private async Task CancelCreationAsync()
    {
        if (!this._createMode)
        {
            return;
        }

        this._createMode = false;
        await this.RemoveFocusedObjectAsync().ConfigureAwait(false);
    }

    private async Task RemoveFocusedObjectAsync()
    {
        switch (this._focusedObject)
        {
            case MonsterSpawnArea spawnArea:
                this.SelectedMap.MonsterSpawns.Remove(spawnArea);
                break;
            case EnterGate enterGate:
                this.SelectedMap.EnterGates.Remove(enterGate);
                break;
            case ExitGate exitGate:
                this.SelectedMap.ExitGates.Remove(exitGate);
                break;
            default:
                return;
        }

        await this.PersistenceContext.DeleteAsync(this._focusedObject).ConfigureAwait(false);
        this._focusedObject = null;
    }

    private async Task OnMapSelectedAsync(ChangeEventArgs args)
    {
        if (args.Value is string idString
            && Guid.TryParse(idString, out var mapId))
        {
            var cancelEventArgs = new MapChangingArgs(mapId);
            if (this.SelectedMapChanging is { } callback)
            {
                await callback.InvokeAsync(cancelEventArgs);
            }

            if (cancelEventArgs.Cancel)
            {
                return;
            }

            this.SelectedMap = this.Maps.First(m => m.GetId() == mapId);
        }
    }

    /// <summary>
    /// Arguments of <see cref="MapEditor.SelectedMapChanging"/>.
    /// </summary>
    public class MapChangingArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapChangingArgs"/> class.
        /// </summary>
        /// <param name="nextMap">The next map.</param>
        public MapChangingArgs(Guid nextMap)
        {
            NextMap = nextMap;
        }

        /// <summary>
        /// Gets the id of the next map.
        /// </summary>
        public Guid NextMap { get; }
    }
}