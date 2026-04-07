// <copyright file="MapZoomManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// Manages zoom state and delegates zoom operations to the map editor's
/// JavaScript module, keeping zoom logic isolated from the Blazor component.
/// </summary>
public sealed class MapZoomManager
{
    // Values match MIN_ZOOM / MAX_ZOOM in MapEditor.razor.js.
    private const float MinZoom = 1.0f;
    private const float MaxZoom = 4.0f;
    private const float ZoomButtonFactor = 1.25f;

    private readonly IJSObjectReference _jsModule;
    private readonly ElementReference _mapHostRef;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapZoomManager"/> class.
    /// </summary>
    /// <param name="jsModule">The JS module reference for the map editor.</param>
    /// <param name="mapHostRef">The element reference for the map host container.</param>
    public MapZoomManager(IJSObjectReference jsModule, ElementReference mapHostRef)
    {
        this._jsModule = jsModule;
        this._mapHostRef = mapHostRef;
    }

    /// <summary>
    /// Gets the current zoom level, where 1.0 represents 100%.
    /// </summary>
    public float ZoomLevel { get; private set; } = 1.0f;

    /// <summary>
    /// Gets the current zoom level expressed as a rounded percentage.
    /// </summary>
    public int ZoomPercentage => (int)Math.Round(this.ZoomLevel * 100);

    /// <summary>
    /// Gets a value indicating whether the zoom level is at its minimum.
    /// </summary>
    public bool IsAtMinZoom => this.ZoomLevel <= MinZoom;

    /// <summary>
    /// Gets a value indicating whether the zoom level is at its maximum.
    /// </summary>
    public bool IsAtMaxZoom => this.ZoomLevel >= MaxZoom;

    /// <summary>
    /// Increases the zoom level by the zoom button factor, up to <see cref="MaxZoom"/>.
    /// </summary>
    public async Task ZoomInAsync()
    {
        var newZoom = Math.Min(this.ZoomLevel * ZoomButtonFactor, MaxZoom);
        this.ZoomLevel = await this._jsModule
            .InvokeAsync<float>("zoomTo", this._mapHostRef, newZoom)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Decreases the zoom level by the zoom button factor, down to <see cref="MinZoom"/>.
    /// </summary>
    public async Task ZoomOutAsync()
    {
        var newZoom = Math.Max(this.ZoomLevel / ZoomButtonFactor, MinZoom);
        this.ZoomLevel = await this._jsModule
            .InvokeAsync<float>("zoomTo", this._mapHostRef, newZoom)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Resets the zoom level to fit the map within the viewport.
    /// </summary>
    public async Task ResetZoomAsync()
    {
        this.ZoomLevel = await this._jsModule
            .InvokeAsync<float>("resetZoom", this._mapHostRef)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Handles a mouse wheel event to zoom in or out centered on the cursor position.
    /// </summary>
    /// <param name="deltaY">The vertical scroll delta from the wheel event.</param>
    /// <param name="clientX">The client X position of the cursor.</param>
    /// <param name="clientY">The client Y position of the cursor.</param>
    public async Task HandleWheelAsync(double deltaY, double clientX, double clientY)
    {
        var result = await this._jsModule
            .InvokeAsync<ZoomResult>("handleWheel", this._mapHostRef, deltaY, clientX, clientY)
            .ConfigureAwait(false);

        if (result.Handled)
        {
            this.ZoomLevel = result.ZoomLevel;
        }
    }

    /// <summary>
    /// Synchronizes the zoom level from a value retrieved from the JavaScript module state.
    /// </summary>
    /// <param name="zoomLevel">The zoom level as reported by the JS module.</param>
    public void SyncZoomLevel(float zoomLevel)
    {
        this.ZoomLevel = zoomLevel;
    }

    /// <summary>
    /// Represents the result returned from the JavaScript zoom handler.
    /// </summary>
    private sealed class ZoomResult
    {
        /// <summary>Gets or sets the new zoom level after the wheel event.</summary>
        public float ZoomLevel { get; set; }

        /// <summary>Gets or sets a value indicating whether the wheel event was consumed.</summary>
        public bool Handled { get; set; }
    }
}