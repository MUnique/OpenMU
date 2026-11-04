// <copyright file="EquippedItems.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Component for a item box.
/// </summary>
public partial class EquippedItems
{
    private static readonly string[] ClassNames =
    {
        "slot-left",
        "slot-right",
        "slot-helm",
        "slot-armor",
        "slot-pants",
        "slot-gloves",
        "slot-boots",
        "slot-wings",
        "slot-pet",
        "slot-pendant",
        "slot-ring1",
        "slot-ring2",
    };

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    [Parameter]
    public Item? SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets the callback when the selected item changed.
    /// </summary>
    [Parameter]
    public EventCallback<Item?> SelectedItemChanged { get; set; }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public async Task SetSelectedItemAsync(ItemViewModel viewModel)
    {
        this.SelectedItem = viewModel.Item;
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(viewModel.Item).ConfigureAwait(true);
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out ItemStorage result, out string validationErrorMessage)
    {
        result = null!;
        validationErrorMessage = string.Empty;
        return false;
    }

    private ItemViewModel? GetViewItemOfSlot(byte slot)
    {
        return this.Value?.Items.FirstOrDefault(item => item.ItemSlot == slot)?.AsViewModel();
    }

    private async Task OnItemMovedAsync()
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(this.SelectedItem).ConfigureAwait(true);
        }
    }
}