// <copyright file="MapObjectKindSelector.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Web.Shared.Properties;

/// <summary>
/// Selects the <see cref="MapObjectKind"/>s which should be exported or imported.
/// </summary>
public partial class MapObjectKindSelector
{
    /// <summary>
    /// Gets or sets the kinds which can be selected. All kinds are always shown, but kinds
    /// which are not available are disabled. When <see langword="null"/>, all kinds are available.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<MapObjectKind>? AvailableKinds { get; set; }

    /// <summary>
    /// Gets or sets the number of objects per kind, which is shown behind the caption of each kind.
    /// When <see langword="null"/>, no counts are shown.
    /// </summary>
    [Parameter]
    public IReadOnlyDictionary<MapObjectKind, int>? Counts { get; set; }

    /// <summary>
    /// Gets or sets the selected kinds.
    /// </summary>
    [Parameter]
    public IReadOnlySet<MapObjectKind> SelectedKinds { get; set; } = new HashSet<MapObjectKind>();

    /// <summary>
    /// Gets or sets the callback which is invoked when the selection changed.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlySet<MapObjectKind>> SelectedKindsChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gates can be selected. Gates reference other maps,
    /// so they are only available for the all-maps flow.
    /// </summary>
    [Parameter]
    public bool AllowGates { get; set; } = true;

    private bool IsEnabled(MapObjectKind kind) =>
        (kind != MapObjectKind.Gates || this.AllowGates)
        && (this.AvailableKinds is null || this.AvailableKinds.Contains(kind));

    private string? GetTooltip(MapObjectKind kind)
    {
        if (this.IsEnabled(kind))
        {
            return null;
        }

        return kind == MapObjectKind.Gates && !this.AllowGates
            ? Resources.GatesRequireAllMaps
            : Resources.KindNotAvailable;
    }

    private string GetLabel(MapObjectKind kind)
    {
        return this.Counts is not null && this.Counts.TryGetValue(kind, out var count)
            ? $"{kind.GetCaption()} ({count})"
            : kind.GetCaption();
    }

    private bool IsSelected(MapObjectKind kind) => this.SelectedKinds.Contains(kind);

    private async Task ToggleAsync(MapObjectKind kind)
    {
        if (!this.IsEnabled(kind))
        {
            return;
        }

        var selected = new HashSet<MapObjectKind>(this.SelectedKinds);
        if (!selected.Remove(kind))
        {
            selected.Add(kind);
        }

        if (selected.Count == 0)
        {
            // At least one kind has to stay selected, otherwise there is nothing to export.
            return;
        }

        await this.SelectedKindsChanged.InvokeAsync(selected).ConfigureAwait(true);
    }
}
