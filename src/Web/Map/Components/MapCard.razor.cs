// <copyright file="MapCard.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A bootstrap card for a game map.
/// </summary>
public partial class MapCard
{
    /// <summary>
    /// Gets or sets the map information.
    /// </summary>
    [Parameter]
    public IGameMapInfo MapInfo { get; set; } = null!;

    /// <summary>
    /// Gets or sets the route to the live map, where the map id just has to be appended.
    /// </summary>
    [CascadingParameter(Name = nameof(LiveMapRoute))]
    public string LiveMapRoute { get; set; } = string.Empty;
}