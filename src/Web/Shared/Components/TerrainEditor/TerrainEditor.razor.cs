// <copyright file="TerrainEditor.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.TerrainEditor;

using System.IO;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.MapEditor;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Properties;

/// <summary>
/// A graphical editor for the terrains of a <see cref="GameMapDefinition"/>, which are its
/// <see cref="GameMapDefinition.TerrainData"/> and the <see cref="GameMapDefinition.TerrainVariants"/>.
/// </summary>
/// <remarks>
/// The tiles are edited in the browser. After each edit, the whole terrain is transferred back
/// and assigned to the edited terrain, so that the persistence context tracks the change.
/// </remarks>
public partial class TerrainEditor : IAsyncDisposable
{
    private const string ImportInputId = "terrain-import-file-input";

    private const int MaximumBrushSize = 25;

    /// <summary>
    /// The default zoom, which shows the whole terrain with 3 pixels per tile.
    /// </summary>
    private const int DefaultZoom = 3;

    /// <summary>
    /// The minimum zoom, which shows one pixel per tile.
    /// </summary>
    private const int MinimumZoom = 1;

    /// <summary>
    /// The maximum zoom, which shows 16 pixels per tile.
    /// </summary>
    private const int MaximumZoom = 16;

    /// <summary>
    /// The maximum size of an imported file, which is the size of a client terrain file with two bytes per tile.
    /// </summary>
    private const int MaximumImportFileSize = 4 + (TerrainFileConverter.TileCount * sizeof(ushort));

    private const string WalkableColor = "#50c850";

    /// <summary>
    /// The attributes which can be edited. <see cref="TerrainAttributeType.Character"/> is
    /// just set at runtime by the game server, so it doesn't make sense to edit it.
    /// </summary>
    private static readonly TerrainAttributeType[] EditableAttributes =
    [
        TerrainAttributeType.Safezone,
        TerrainAttributeType.Blocked,
        TerrainAttributeType.NoGround,
        TerrainAttributeType.Water,
    ];

    /// <summary>
    /// The colors of the tiles, by priority. The first entry whose mask matches the attributes of a tile defines its color.
    /// Tiles without attributes are drawn with the <see cref="WalkableColor"/>.
    /// </summary>
    private static readonly (int Mask, string Color)[] Palette =
    [
        ((int)TerrainAttributeType.Blocked, "#7a3b3b"),
        ((int)TerrainAttributeType.NoGround, "#000000"),
        ((int)TerrainAttributeType.Water, "#2f6fd6"),
        (OtherAttributesMask, "#b08a3c"),
        ((int)TerrainAttributeType.Safezone, "#9a9a9a"),
    ];

    private static readonly string JsModulePath =
        $"./_content/{typeof(TerrainEditor).Assembly.GetName().Name}/Components/TerrainEditor/{nameof(TerrainEditor)}.razor.js";

    private ElementReference _mapSelectRef;
    private ElementReference _hostRef;
    private ElementReference _canvasRef;
    private ElementReference _selectionRef;
    private ElementReference _statusRef;

    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _editor;
    private DotNetObjectReference<TerrainEditor>? _dotNetRef;

    private GameMapDefinition? _selectedMap;
    private GameMapTerrainVariant? _selectedVariant;

    /// <summary>
    /// The map or variant of which the terrain is currently loaded in the browser.
    /// </summary>
    private object? _loadedTerrainOwner;

    private TerrainTool _tool = TerrainTool.Brush;
    private TerrainEditMode _mode = TerrainEditMode.Add;
    private TerrainAttributeType _attributes = TerrainAttributeType.Blocked;
    private int _brushSize = 1;
    private int _zoom = DefaultZoom;
    private bool _showGrid;
    private bool _canUndo;
    private bool _canRedo;

    /// <summary>
    /// Gets or sets the callback invoked before the selected map changes,
    /// allowing the caller to cancel the change.
    /// </summary>
    [Parameter]
    public EventCallback<MapChangingArgs> SelectedMapChanging { get; set; }

    /// <summary>
    /// Gets or sets the maps which can be selected.
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
    /// Gets or sets the callback which saves the changes.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    private static int OtherAttributesMask => byte.MaxValue & ~EditableAttributes.Aggregate(0, (mask, attribute) => mask | (int)attribute);

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<TerrainEditor> Logger { get; set; } = null!;

    /// <summary>
    /// Gets the map or variant which owns the selected terrain.
    /// </summary>
    private object? SelectedTerrainOwner => (object?)this._selectedVariant ?? this._selectedMap;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (this._editor is not null)
            {
                await this._editor.InvokeVoidAsync("dispose").ConfigureAwait(false);
                await this._editor.DisposeAsync().ConfigureAwait(false);
            }

            if (this._jsModule is not null)
            {
                await this._jsModule.DisposeAsync().ConfigureAwait(false);
            }
        }
        catch (JSDisconnectedException)
        {
            // The circuit is already gone, so there is nothing to clean up in the browser.
        }

        this._dotNetRef?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called by the browser after the terrain was edited.
    /// Transfers the edited terrain and assigns it to the edited map or variant.
    /// </summary>
    /// <param name="canUndo">If set to <c>true</c>, an edit can be undone.</param>
    /// <param name="canRedo">If set to <c>true</c>, an undone edit can be redone.</param>
    [JSInvokable]
    public async Task OnTerrainChangedAsync(bool canUndo, bool canRedo)
    {
        this._canUndo = canUndo;
        this._canRedo = canRedo;
        if (this._editor is not { } editor || this._loadedTerrainOwner is not { } owner)
        {
            return;
        }

        try
        {
            var streamReference = await editor.InvokeAsync<IJSStreamReference>("getData").ConfigureAwait(true);
            await using (streamReference.ConfigureAwait(true))
            {
                var tileAttributes = new byte[TerrainFileConverter.TileCount];
                var stream = await streamReference.OpenReadStreamAsync(TerrainFileConverter.TileCount).ConfigureAwait(true);
                await using (stream.ConfigureAwait(true))
                {
                    await stream.ReadExactlyAsync(tileAttributes).ConfigureAwait(true);
                }

                SetTerrainData(owner, TerrainFileConverter.ToServerFile(tileAttributes));
            }
        }
        catch (Exception ex) when (ex is not JSDisconnectedException)
        {
            this.Logger.LogError(ex, "Could not transfer the edited terrain.");
            this.ToastService.ShowError(string.Format(Resources.TerrainTransferFailed, ex.Message));
        }

        this.StateHasChanged();
    }

    /// <summary>
    /// Called by the browser when the zoom was changed by the mouse wheel.
    /// </summary>
    /// <param name="zoom">The zoom.</param>
    [JSInvokable]
    public void OnZoomChanged(int zoom)
    {
        this._zoom = zoom;
        this.StateHasChanged();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (this.Maps is not { Count: > 0 })
        {
            throw new InvalidOperationException("The Maps parameter is required and must not be empty.");
        }

        // The maps are loaded again when changes were discarded, so we select the new instances.
        var selectedMapId = this._selectedMap?.GetId() ?? this.SelectedMapId;
        var selectedVariantId = this._selectedVariant?.GetId();
        this._selectedMap = this.Maps.FirstOrDefault(m => m.GetId() == selectedMapId) ?? this.Maps.First();
        this._selectedVariant = this._selectedMap.TerrainVariants.FirstOrDefault(v => v.GetId() == selectedVariantId);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        if (firstRender)
        {
            this._jsModule = await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath).ConfigureAwait(true);
            this._dotNetRef = DotNetObjectReference.Create(this);
            var options = new
            {
                Palette = Palette.Select(p => new { p.Mask, p.Color }).ToArray(),
                WalkableColor,
                FlagNames = Enum.GetValues<TerrainAttributeType>().Select(a => new { Mask = (int)a, Name = a.GetEnumCaption() }).ToArray(),
                Zoom = this._zoom,
                MinimumZoom,
                MaximumZoom,
            };

            this._editor = await this._jsModule.InvokeAsync<IJSObjectReference>(
                "createEditor",
                this._hostRef,
                this._canvasRef,
                this._selectionRef,
                this._statusRef,
                this._dotNetRef,
                options).ConfigureAwait(true);
            await this.UpdateToolAsync().ConfigureAwait(true);
        }

        if (this._editor is not null && !ReferenceEquals(this._loadedTerrainOwner, this.SelectedTerrainOwner))
        {
            await this.LoadSelectedTerrainAsync().ConfigureAwait(true);
        }
    }

    private static byte[]? GetTerrainData(object owner)
    {
        return owner switch
        {
            GameMapTerrainVariant variant => variant.TerrainData,
            GameMapDefinition map => map.TerrainData,
            _ => null,
        };
    }

    private static void SetTerrainData(object owner, byte[] terrainData)
    {
        switch (owner)
        {
            case GameMapTerrainVariant variant:
                variant.TerrainData = terrainData;
                break;
            case GameMapDefinition map:
                map.TerrainData = terrainData;
                break;
            default:
                throw new ArgumentException($"Unknown terrain owner {owner}.", nameof(owner));
        }
    }

    private static string GetToolCaption(TerrainTool tool) => tool switch
    {
        TerrainTool.Rectangle => Resources.TerrainToolRectangle,
        TerrainTool.Fill => Resources.TerrainToolFill,
        _ => Resources.TerrainToolBrush,
    };

    private static string GetToolIcon(TerrainTool tool) => tool switch
    {
        TerrainTool.Rectangle => "oi-crop",
        TerrainTool.Fill => "oi-droplet",
        _ => "oi-brush",
    };

    private static string GetModeCaption(TerrainEditMode mode) => mode switch
    {
        TerrainEditMode.Remove => Resources.TerrainEditModeRemove,
        TerrainEditMode.Replace => Resources.TerrainEditModeReplace,
        _ => Resources.TerrainEditModeAdd,
    };

    private static string GetColor(TerrainAttributeType attribute)
    {
        return Palette.First(p => p.Mask == (int)attribute).Color;
    }

    private static string GetLegendCaption(int mask)
    {
        return mask == OtherAttributesMask
            ? Resources.TerrainOtherAttributes
            : ((TerrainAttributeType)mask).GetEnumCaption();
    }

    private async Task LoadSelectedTerrainAsync()
    {
        if (this._editor is not { } editor || this.SelectedTerrainOwner is not { } owner)
        {
            return;
        }

        this._loadedTerrainOwner = owner;
        this._canUndo = false;
        this._canRedo = false;
        var tileAttributes = TerrainFileConverter.GetTileAttributes(GetTerrainData(owner));
        await editor.InvokeVoidAsync("setData", Convert.ToBase64String(tileAttributes), false).ConfigureAwait(true);
        this.StateHasChanged();
    }

    private async Task OnMapSelectedAsync(ChangeEventArgs args)
    {
        if (args.Value is not string idString || !Guid.TryParse(idString, out var mapId) || this._selectedMap is null)
        {
            return;
        }

        var changingArgs = new MapChangingArgs(mapId);
        if (this.SelectedMapChanging.HasDelegate)
        {
            await this.SelectedMapChanging.InvokeAsync(changingArgs).ConfigureAwait(true);
        }

        if (changingArgs.Cancel)
        {
            if (this._jsModule is not null)
            {
                await this._jsModule.InvokeVoidAsync("setSelectValue", this._mapSelectRef, this._selectedMap.GetId().ToString()).ConfigureAwait(true);
            }

            return;
        }

        // The maps may have been reloaded by the callback, so we look it up afterward.
        this._selectedMap = this.Maps.First(m => m.GetId() == mapId);
        this._selectedVariant = null;
    }

    private void OnTerrainSelected(ChangeEventArgs args)
    {
        if (args.Value is not string idString || !Guid.TryParse(idString, out var variantId))
        {
            return;
        }

        this._selectedVariant = this._selectedMap?.TerrainVariants.FirstOrDefault(v => v.GetId() == variantId);
    }

    private async Task SetToolAsync(TerrainTool tool)
    {
        this._tool = tool;
        await this.UpdateToolAsync().ConfigureAwait(true);
    }

    private async Task SetModeAsync(TerrainEditMode mode)
    {
        this._mode = mode;
        await this.UpdateToolAsync().ConfigureAwait(true);
    }

    private async Task ToggleAttributeAsync(TerrainAttributeType attribute)
    {
        this._attributes ^= attribute;
        await this.UpdateToolAsync().ConfigureAwait(true);
    }

    private async Task OnBrushSizeChangedAsync(ChangeEventArgs args)
    {
        if (int.TryParse(args.Value?.ToString(), out var size))
        {
            this._brushSize = Math.Clamp(size, 1, MaximumBrushSize);
            await this.UpdateToolAsync().ConfigureAwait(true);
        }
    }

    private async Task UpdateToolAsync()
    {
        if (this._editor is { } editor)
        {
            await editor.InvokeVoidAsync(
                "setTool",
                this._tool.ToString().ToLowerInvariant(),
                this._brushSize,
                this._mode.ToString().ToLowerInvariant(),
                (int)this._attributes).ConfigureAwait(true);
        }
    }

    private async Task ZoomByAsync(int steps)
    {
        if (this._editor is { } editor)
        {
            this._zoom = await editor.InvokeAsync<int>("zoomBy", steps).ConfigureAwait(true);
        }
    }

    private async Task ResetZoomAsync()
    {
        if (this._editor is { } editor)
        {
            this._zoom = await editor.InvokeAsync<int>("zoomTo", DefaultZoom).ConfigureAwait(true);
        }
    }

    private async Task ToggleGridAsync()
    {
        this._showGrid = !this._showGrid;
        if (this._editor is { } editor)
        {
            await editor.InvokeVoidAsync("setGrid", this._showGrid).ConfigureAwait(true);
        }
    }

    private async Task UndoAsync()
    {
        if (this._editor is { } editor)
        {
            await editor.InvokeVoidAsync("undo").ConfigureAwait(true);
        }
    }

    private async Task RedoAsync()
    {
        if (this._editor is { } editor)
        {
            await editor.InvokeVoidAsync("redo").ConfigureAwait(true);
        }
    }

    private async Task SaveAsync()
    {
        await this.OnValidSubmit.InvokeAsync().ConfigureAwait(true);
    }

    private async Task TriggerFileImportAsync()
    {
        if (this._jsModule is { } jsModule)
        {
            await jsModule.InvokeVoidAsync("triggerFileInput", ImportInputId).ConfigureAwait(true);
        }
    }

    private async Task ImportFileSelectedAsync(IFileListEntry[] files)
    {
        if (files.FirstOrDefault() is not { } file || this._editor is not { } editor)
        {
            return;
        }

        if (file.Size > MaximumImportFileSize)
        {
            this.ToastService.ShowError(Resources.InvalidTerrainFile);
            return;
        }

        using var memoryStream = new MemoryStream();
        await file.Data.CopyToAsync(memoryStream).ConfigureAwait(true);
        if (!TerrainFileConverter.TryReadFile(memoryStream.ToArray(), out var tileAttributes))
        {
            this.ToastService.ShowError(Resources.InvalidTerrainFile);
            return;
        }

        // The editor takes it as an edit which can be undone, and transfers it back to us.
        await editor.InvokeVoidAsync("setData", Convert.ToBase64String(tileAttributes), true).ConfigureAwait(true);
        this.ToastService.ShowSuccess(Resources.TerrainImported);
    }

    private async Task DownloadServerFileAsync()
    {
        if (this._selectedMap is not { } map || this.SelectedTerrainOwner is not { } owner)
        {
            return;
        }

        var tileAttributes = TerrainFileConverter.GetTileAttributes(GetTerrainData(owner));
        await this.DownloadAsync($"Terrain{this.GetFileNameSuffix(map)}.att", TerrainFileConverter.ToServerFile(tileAttributes)).ConfigureAwait(true);
    }

    private async Task DownloadClientFileAsync()
    {
        if (this._selectedMap is not { } map || this.SelectedTerrainOwner is not { } owner)
        {
            return;
        }

        var tileAttributes = TerrainFileConverter.GetTileAttributes(GetTerrainData(owner));
        await this.DownloadAsync($"EncTerrain{this.GetFileNameSuffix(map)}.att", TerrainFileConverter.ToClientFile(tileAttributes, map.Number)).ConfigureAwait(true);
    }

    /// <summary>
    /// Gets the suffix of the file names, which is the number of the map as it's used by the file names of the client and the original server.
    /// The number of a variant is appended, because there is no common naming scheme for them.
    /// </summary>
    private string GetFileNameSuffix(GameMapDefinition map)
    {
        var suffix = (map.Number + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return this._selectedVariant is { } variant ? $"{suffix}_{variant.Number}" : suffix;
    }

    private async Task DownloadAsync(string fileName, byte[] content)
    {
        if (this._jsModule is { } jsModule)
        {
            await jsModule.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(content)).ConfigureAwait(true);
        }
    }
}
