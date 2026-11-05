// <copyright file="MuItemStorage.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Component for an item box.
/// </summary>
public partial class MuItemStorage
{
    private const int BoxSize = 42;

    private StorageViewModel? _viewModel;
    private (int Column, int Row)? _previewPosition;
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
    /// Gets or sets the column offset between the pointer and the dragged item's top-left corner.
    /// </summary>
    [Parameter]
    public byte DraggedColumnOffset { get; set; }

    /// <summary>
    /// Gets or sets the row offset between the pointer and the dragged item's top-left corner.
    /// </summary>
    [Parameter]
    public byte DraggedRowOffset { get; set; }

    /// <summary>
    /// Gets or sets the type of the storage.
    /// </summary>
    [Parameter]
    public StorageType StorageType { get; set; }

    /// <summary>
    /// Gets or sets the number of extensions.
    /// </summary>
    [Parameter]
    public byte NumberOfExtensions { get; set; }

    /// <summary>
    /// Gets or sets the index of the extension.
    /// </summary>
    [Parameter]
    public byte ExtensionIndex { get; set; }

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
    /// Gets or sets the callback when an item is dropped on this storage.
    /// </summary>
    [Parameter]
    public EventCallback<(byte Slot, int StartIndex, int Rows, bool IsEquipmentSlot)> OnItemDropped { get; set; }

    /// <summary>
    /// Gets or sets the callback when an item should be deleted.
    /// </summary>
    [Parameter]
    public EventCallback OnDelete { get; set; }

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public async Task SetSelectedItemAsync(ItemViewModel viewModel)
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(viewModel.Item).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (this.Value is { } value)
        {
            this._viewModel = value.CreateViewModel(this.StorageType, this.NumberOfExtensions, this.ExtensionIndex);
        }

        if (this.DraggedItem is null)
        {
            this._previewPosition = null;
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out ItemStorage result, out string validationErrorMessage)
    {
        result = null!;
        validationErrorMessage = string.Empty;
        return false;
    }

    private void OnDragOver(byte slot)
    {
        if (this.DraggedItem is not { } draggedItem || this._viewModel is not { } viewModel)
        {
            return;
        }

        var destination = InventoryPlacementService.GetDropPosition(
            slot,
            viewModel.StartIndex,
            this.DraggedColumnOffset,
            this.DraggedRowOffset);
        if (this._previewPosition == destination)
        {
            return;
        }

        this._previewPosition = destination;
        var placementService = new InventoryPlacementService(this.StorageType, this.NumberOfExtensions);
        this._previewIsValid = placementService.CanMoveItem(
            draggedItem,
            destination.Column,
            destination.Row,
            this.Value!.Items,
            viewModel.StartIndex,
            viewModel.Rows);
        this.StateHasChanged();
    }

    private string GetPreviewValidityClass()
    {
        return this._previewIsValid ? "valid" : "invalid";
    }

    private string GetPreviewStyle((int Column, int Row) position, Item item)
    {
        var width = Math.Max(item.Definition?.Width ?? 1, (byte)1);
        var height = Math.Max(item.Definition?.Height ?? 1, (byte)1);
        var maxColumn = Math.Max(0, InventoryConstants.RowSize - width);
        var maxRow = Math.Max(0, this._viewModel!.Rows - height);
        var column = Math.Clamp(position.Column, 0, maxColumn);
        var row = Math.Clamp(position.Row, 0, maxRow);

        return $"left: {column * BoxSize}px; top: {row * BoxSize}px; width: {width * BoxSize}px; height: {height * BoxSize}px";
    }

    private Task DropAsync(byte slot)
    {
        if (this._viewModel is null)
        {
            return Task.CompletedTask;
        }

        return this.OnItemDropped.InvokeAsync((slot, this._viewModel.StartIndex, this._viewModel.Rows, false));
    }

    private async Task OnItemMovedAsync()
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(this.SelectedItem).ConfigureAwait(false);
        }
    }
}
