// <copyright file="LiveMap.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Web.Map.Map;
using Nito.AsyncEx;

/// <summary>
/// The map component which shows the terrain with the players, monsters etc. on top.
/// </summary>
public partial class LiveMap
{
    /// <summary>
    /// Gets or sets the id of the maps DOM-Element.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the server which hosts the map.
    /// </summary>
    [Parameter]
    public int ServerId { get; set; }

    /// <summary>
    /// Gets or sets the id of the map.
    /// </summary>
    [Parameter]
    public Guid MapId { get; set; }

    /// <summary>
    /// Gets or sets the lazy map controller. It'll be initialized after the first render.
    /// </summary>
    [Parameter]
    public AsyncLazy<IMapController> MapController { get; set; } = null!;

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    private ILogger<LiveMap> Logger { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);

        if (firstRender)
        {
            await this.MapController;
            this.StateHasChanged();
        }
    }

    private async Task ResetViewAsync()
    {
        try
        {
            await this.JsRuntime.InvokeVoidAsync("ResetMapView", this.ServerId, this.MapId).ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            // The circuit was torn down while the call was in flight; ignore.
        }
        catch (JSException ex)
        {
            this.Logger.LogDebug(ex, "Unexpected JS interop error while resetting the view of map {MapId}.", this.MapId);
        }
    }
}
