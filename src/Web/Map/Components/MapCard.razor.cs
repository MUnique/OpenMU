namespace MUnique.OpenMU.Web.Map.Components;

using Microsoft.AspNetCore.Components;

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

    ///// <inheritdoc />
    //protected override void OnParametersSet()
    //{
    //    base.OnParametersSet();
    //    this.MapController.ObjectsChanged += (_, __) => this.InvokeAsync(this.StateHasChanged);
    //}
}