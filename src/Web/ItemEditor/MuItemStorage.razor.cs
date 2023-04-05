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
    /// Gets the selected item.
    /// </summary>
    public ItemViewModel? SelectedItem { get; private set; }

    /// <summary>
    /// Gets or sets the type of the storage.
    /// </summary>
    [Parameter]
    public StorageType StorageType { get; set; }

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
        this.SelectedItem = viewModel;
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
            this._viewModel = value.CreateViewModel(this.StorageType);
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, out ItemStorage result, out string validationErrorMessage)
    {
        result = null!;
        validationErrorMessage = string.Empty;
        return false;
    }

    private async Task OnItemMovedAsync()
    {
        if (this.SelectedItemChanged.HasDelegate)
        {
            await this.SelectedItemChanged.InvokeAsync(this.SelectedItem?.Item).ConfigureAwait(true);
        }
    }
}