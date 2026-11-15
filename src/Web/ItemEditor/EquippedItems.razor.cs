// <copyright file="EquippedItems.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Component for equipped items.
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

    private readonly InventoryPlacementService _placementService = new(StorageType.Inventory, 0);
    private byte? _previewSlot;
    private bool _previewIsValid;

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    [Parameter]
    public Item? SelectedItem { get; set; }

    /// <summary>
    /// Gets or sets the item which is currently being dragged.
    /// </summary>
    [Parameter]
    public Item? DraggedItem { get; set; }

    /// <summary>
    /// Gets or sets the callback when the selected item changed.
    /// </summary>
    [Parameter]
    public EventCallback<Item?> SelectedItemChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback when an item starts being dragged.
    /// </summary>
    [Parameter]
    public EventCallback<(Item Item, byte ColumnOffset, byte RowOffset)> OnDragStarted { get; set; }

    /// <summary>
    /// Gets or sets the callback when dragging ends.
    /// </summary>
    [Parameter]
    public EventCallback OnDragEnded { get; set; }

    /// <summary>
    /// Gets or sets the callback when an item is dropped on an equipment slot.
    /// </summary>
    [Parameter]
    public EventCallback<(byte Slot, int StartIndex, int Rows, bool IsEquipmentSlot)> OnItemDropped { get; set; }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public async Task SetSelectedItemAsync(ItemViewModel viewModel)
    {
        this.SelectedItem = viewModel.Item;
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(viewModel.Item).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (this.DraggedItem is null)
        {
            this._previewSlot = null;
        }
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

    private void OnDragOver(byte slot)
    {
        if (this.DraggedItem is not { } draggedItem || this.Value is not { } storage || this._previewSlot == slot)
        {
            return;
        }

        this._previewSlot = slot;
        this._previewIsValid = this._placementService.CanEquipItem(draggedItem, slot, storage.Items);
        this.StateHasChanged();
    }

    private string GetDropPreviewClass(byte slot)
    {
        if (this._previewSlot != slot)
        {
            return string.Empty;
        }

        return this._previewIsValid ? "drop-preview-valid" : "drop-preview-invalid";
    }

    private Task DropAsync(byte slot)
    {
        return this.OnItemDropped.InvokeAsync((slot, 0, 0, true));
    }

    private async Task OnItemMovedAsync()
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(this.SelectedItem).ConfigureAwait(false);
        }
    }
}
