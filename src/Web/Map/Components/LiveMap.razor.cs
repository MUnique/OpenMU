// <copyright file="LiveMap.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Components;

using Microsoft.AspNetCore.Components;
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
    /// Gets or sets the lazy map controller. It'll be initialized after the first render.
    /// </summary>
    [Parameter]
    public AsyncLazy<IMapController> MapController { get; set; } = null!;

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
}