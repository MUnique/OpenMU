// <copyright file="DropItemGroupTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.ItemDrops;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// A compact, inline editable table of <see cref="DropItemGroup"/>s.
/// </summary>
public partial class DropItemGroupTable
{
    private const int MaximumItemNamesInSummary = 3;

    private static readonly CultureInfo NeutralCulture = CultureInfo.GetCultureInfo(LocalizedString.NeutralLanguageCode);

    private readonly HashSet<DropItemGroup> _expandedGroups = [];
    private readonly Dictionary<DropItemGroup, EditContext> _editContexts = [];
    private readonly Dictionary<DropItemGroup, IList<ItemDefinition>> _possibleItems = [];
    private readonly Dictionary<DropItemGroup, IList<GameMapDefinition>> _assignedMaps = [];

    /// <summary>
    /// Gets or sets the drop item groups which should be shown.
    /// </summary>
    [Parameter]
    public IReadOnlyList<DropItemGroup> Groups { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the groups are <see cref="ItemDropItemGroup"/>s
    /// which are dropped by items, e.g. boxes.
    /// </summary>
    [Parameter]
    public bool IsItemBoxDrop { get; set; }

    /// <summary>
    /// Gets or sets all available maps. If set, the maps to which a group is assigned can be edited.
    /// </summary>
    [Parameter]
    public IEnumerable<GameMapDefinition>? Maps { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="DropItemGroup.Monster"/> restriction can be edited.
    /// </summary>
    [Parameter]
    public bool ShowMonsterRestriction { get; set; }

    /// <summary>
    /// Gets or sets the function which provides a text which describes where a group is used.
    /// If it's <see langword="null"/>, the column is not shown.
    /// </summary>
    [Parameter]
    public Func<DropItemGroup, string?>? UsageProvider { get; set; }

    /// <summary>
    /// Gets or sets the callback which is called when the remove button of a group has been clicked.
    /// If it has no delegate, no remove button is shown.
    /// </summary>
    [Parameter]
    public EventCallback<DropItemGroup> OnRemove { get; set; }

    /// <summary>
    /// Gets or sets the title of the remove button.
    /// </summary>
    [Parameter]
    public string? RemoveTitle { get; set; }

    /// <summary>
    /// Gets or sets the callback which is called when the map assignment of a group changed.
    /// </summary>
    [Parameter]
    public EventCallback OnMapsChanged { get; set; }

    private int ColumnCount => (this.IsItemBoxDrop ? 11 : 8) + (this.UsageProvider is null ? 0 : 1);

    private static double ToPercent(double chance) => Math.Round(chance * 100.0, 8);

    private static double FromPercent(double percent) => Math.Clamp(percent / 100.0, 0.0, 1.0);

    private static void SetDescription(DropItemGroup group, string? text)
    {
        group.Description = group.Description.WithTranslation(NeutralCulture, text);
    }

    private static string GetItemsSummary(DropItemGroup group)
    {
        if (group.PossibleItems.Count == 0)
        {
            return group.ItemType switch
            {
                SpecialItemType.Money => Resources.Money,
                SpecialItemType.RandomItem or SpecialItemType.Excellent or SpecialItemType.SocketItem => Resources.RandomItemsByMonsterLevel,
                SpecialItemType.Ancient => Resources.RandomAncientItems,
                _ => Resources.NoItems,
            };
        }

        var names = group.PossibleItems
            .Take(MaximumItemNamesInSummary)
            .Select(item => item.GetNameForLevel(group.ItemLevel ?? 0));
        var summary = string.Join(", ", names);
        if (group.PossibleItems.Count > MaximumItemNamesInSummary)
        {
            summary += string.Format(CultureInfo.InvariantCulture, " (+{0})", group.PossibleItems.Count - MaximumItemNamesInSummary);
        }

        return summary;
    }

    private static string? GetItemsTooltip(DropItemGroup group)
    {
        if (group.PossibleItems.Count <= MaximumItemNamesInSummary)
        {
            return null;
        }

        return string.Join(Environment.NewLine, group.PossibleItems.Select(item => item.Name.ToString()));
    }

    private void ToggleExpanded(DropItemGroup group)
    {
        if (!this._expandedGroups.Remove(group))
        {
            this._expandedGroups.Add(group);
        }
    }

    private EditContext GetEditContext(DropItemGroup group)
    {
        if (!this._editContexts.TryGetValue(group, out var editContext))
        {
            editContext = new EditContext(group);
            this._editContexts.Add(group, editContext);
        }

        return editContext;
    }

    private IList<ItemDefinition> GetPossibleItems(DropItemGroup group)
    {
        if (!this._possibleItems.TryGetValue(group, out var list))
        {
            list = new CollectionListAdapter<ItemDefinition>(group.PossibleItems);
            this._possibleItems.Add(group, list);
        }

        return list;
    }

    private IList<GameMapDefinition> GetAssignedMaps(DropItemGroup group)
    {
        if (!this._assignedMaps.TryGetValue(group, out var list))
        {
            list = new DropGroupAssignmentList<GameMapDefinition>(group, this.Maps ?? [], map => map.DropItemGroups);
            this._assignedMaps.Add(group, list);
        }

        return list;
    }

    private async Task AssignToAllMapsAsync(DropItemGroup group)
    {
        var list = this.GetAssignedMaps(group);
        foreach (var map in this.Maps ?? [])
        {
            list.Add(map);
        }

        await this.OnMapsChanged.InvokeAsync().ConfigureAwait(true);
    }

    private async Task RemoveFromAllMapsAsync(DropItemGroup group)
    {
        this.GetAssignedMaps(group).Clear();
        await this.OnMapsChanged.InvokeAsync().ConfigureAwait(true);
    }
}
