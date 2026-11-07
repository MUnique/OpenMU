// <copyright file="MuItemStorage.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Component for a item box.
/// </summary>
public partial class MuItemStorage
{
    private StorageViewModel? _viewModel;

    /// <summary>
    /// The dragged item, if the drag state isn't shared through <see cref="DraggedItemChanged"/>.
    /// </summary>
    private Item? _localDraggedItem;

    /// <summary>
    /// The cell (column, row) of this box which the dragged item is currently over.
    /// </summary>
    private (int Column, int Row)? _dropTarget;

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    [Parameter]
    public Item? SelectedItem { get; set; }

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
    /// Gets or sets the item which is currently dragged.
    /// It's shared between the boxes of the same storage (e.g. the inventory and its extensions),
    /// so that an item can be dragged from one box into another.
    /// </summary>
    [Parameter]
    public Item? DraggedItem { get; set; }

    /// <summary>
    /// Gets or sets the callback when the dragged item changed.
    /// </summary>
    [Parameter]
    public EventCallback<Item?> DraggedItemChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback when an item should be deleted.
    /// </summary>
    [Parameter]
    public EventCallback OnDelete { get; set; }

    private Item? CurrentDraggedItem => this.DraggedItemChanged.HasDelegate ? this.DraggedItem : this._localDraggedItem;

    /// <summary>
    /// Sets the selected item.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public async Task SetSelectedItemAsync(ItemViewModel viewModel)
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(viewModel.Item).ConfigureAwait(true);
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

        if (this.CurrentDraggedItem is null)
        {
            // The drag ended in another box of the same storage, so a preview of this box must not stay.
            this._dropTarget = null;
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out ItemStorage result, out string validationErrorMessage)
    {
        result = null!;
        validationErrorMessage = string.Empty;
        return false;
    }

    private async Task OnDragStartAsync(Item item)
    {
        this._dropTarget = null;
        await this.SetDraggedItemAsync(item).ConfigureAwait(true);
    }

    private async Task OnDragEndAsync()
    {
        this._dropTarget = null;
        await this.SetDraggedItemAsync(null).ConfigureAwait(true);
    }

    private void OnDragEnter(int column, int row)
    {
        this._dropTarget = (column, row);
    }

    private void OnDragLeave(int column, int row)
    {
        // The browser raises dragenter on the next cell before dragleave on the previous one,
        // so the target is only cleared when the pointer left the box.
        if (this._dropTarget == (column, row))
        {
            this._dropTarget = null;
        }
    }

    private async Task OnDropAsync(int column, int row)
    {
        this._dropTarget = null;
        var item = this.CurrentDraggedItem;
        await this.SetDraggedItemAsync(null).ConfigureAwait(true);
        if (item is null || this._viewModel?.TryPlace(item, column, row) is not true)
        {
            return;
        }

        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(item).ConfigureAwait(true);
        }
    }

    private async ValueTask SetDraggedItemAsync(Item? item)
    {
        if (this.DraggedItemChanged.HasDelegate)
        {
            await this.DraggedItemChanged.InvokeAsync(item).ConfigureAwait(false);
        }
        else
        {
            this._localDraggedItem = item;
        }
    }

    private async Task OnItemMovedAsync()
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(this.SelectedItem).ConfigureAwait(true);
        }
    }
}