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
    /// Gets or sets the type of the storage.
    /// </summary>
    [Parameter]
    public StorageType StorageType { get; set; }

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
}