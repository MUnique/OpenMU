// <copyright file="AutoForm.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A generic auto-generated form for a model of type <typeparamref name="T"/>.
/// Renders all public properties via <see cref="AutoFields"/> and optionally shows
/// a search box to filter fields when the model exposes many properties.
/// </summary>
/// <typeparam name="T">The model type to edit.</typeparam>
public partial class AutoForm<T>
{
    private string? _currentSearchTerm;
    private string? _lastIncomingSearchTerm;

    /// <summary>
    /// Gets or sets the model of the form, <see cref="EditForm.Model"/>.
    /// </summary>
    [Parameter]
    public T Model { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether collection properties should be hidden.
    /// </summary>
    [Parameter]
    public bool HideCollections { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the form is submitted with valid data.
    /// </summary>
    [Parameter]
    public EventCallback OnValidSubmit { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the cancel button is clicked.
    /// When <see langword="null"/>, the cancel button is not rendered.
    /// </summary>
    [Parameter]
    public EventCallback? OnCancel { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the refresh button is clicked.
    /// When not set, the refresh button is not rendered.
    /// </summary>
    [Parameter]
    public EventCallback? OnRefresh { get; set; }

    /// <summary>
    /// Gets or sets an optional search term used to pre-filter the form fields.
    /// The value is treated as an initial suggestion — the user can still type freely.
    /// </summary>
    [Parameter]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets a value indicating whether the search box should be displayed.
    /// The box appears automatically when the model has many properties,
    /// or when the user has already started typing a search term.
    /// </summary>
    private bool ShowSearch =>
        !string.IsNullOrWhiteSpace(this._currentSearchTerm)
        || (this.Model?.GetType().GetProperties().Length ?? 0) > 15;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Only sync the externally-supplied SearchTerm on actual changes to avoid
        // clobbering what the user has already typed in the input box.
        if (string.Equals(this.SearchTerm, this._lastIncomingSearchTerm, StringComparison.Ordinal))
        {
            return;
        }

        this._lastIncomingSearchTerm = this.SearchTerm;
        this._currentSearchTerm = this.SearchTerm;
    }
}
