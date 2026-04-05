// <copyright file="MultiLookupField.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Lookup field which allows to select multiple objects which will be stored in a bound <see cref="IList{TObject}"/>.
/// </summary>
/// <typeparam name="TObject">The type of the object.</typeparam>
public partial class MultiLookupField<TObject> : IDisposable
    where TObject : class
{
    /// <summary>
    /// The delay in milliseconds before the search is triggered after the last keystroke.
    /// </summary>
    private const int SearchDebounceDelayMs = 300;

    /// <summary>
    /// The delay in milliseconds to wait on blur before closing the dropdown,
    /// allowing click events on dropdown items to fire first.
    /// </summary>
    private const int BlurDelayMs = 150;

    private readonly Debouncer _debouncer = new(SearchDebounceDelayMs);

    private bool _isOpen;
    private bool _hasSearched;
    private bool _isLoading;
    private bool _mouseDownOnDropdown;
    private bool _disposed;
    private string _searchText = string.Empty;
#pragma warning disable CS0649 // Field is never assigned to (assigned by Blazor @ref)
    private ElementReference _searchInput;
#pragma warning restore CS0649
    private IEnumerable<TObject> _filteredItems = Enumerable.Empty<TObject>();

    /// <summary>
    /// Gets or sets the label which should be displayed.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the lookup controller.
    /// </summary>
    [Inject]
    public ILookupController LookupController { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<MultiLookupField<TObject>> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the explicit lookup controller which should be used instead
    /// of the injected <see cref="LookupController"/>.
    /// </summary>
    [Parameter]
    public ILookupController? ExplicitLookupController { get; set; }

    /// <summary>
    /// Gets or sets the caption factory.
    /// </summary>
    [Parameter]
    public Func<TObject, string> CaptionFactory { get; set; } = obj => obj.GetName();

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter(Name = "PersistenceContext")]
    protected IContext PersistenceContext { get; set; } = null!;

    private ILookupController EffectiveLookupController => this.ExplicitLookupController ?? this.LookupController;

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        this._debouncer.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out IList<TObject> result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = default;
        validationErrorMessage = $"{nameof(MultiLookupField<TObject>)} does not support string-based value parsing.";
        return false;
    }

    /// <summary>
    /// Focuses the search input when clicking on the container.
    /// </summary>
    private async Task FocusSearchInputAsync()
    {
        await this._searchInput.FocusAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Handles input focus.
    /// </summary>
    private void OnFocus(FocusEventArgs e)
    {
        this.Logger.LogDebug("Focus event fired: Type={Type}", e.Type);
        this._isOpen = true;
    }

    /// <summary>
    /// Handles input blur (focus loss).
    /// </summary>
    private void OnBlur(FocusEventArgs e)
    {
        this.Logger.LogDebug("Blur event fired: Type={Type}", e.Type);

        _ = Task.Run(async () =>
        {
            await Task.Delay(BlurDelayMs).ConfigureAwait(true);
            if (!this._mouseDownOnDropdown)
            {
                this._isOpen = false;
                this._hasSearched = false;
                this._searchText = string.Empty;
                this.CurrentValue = this.Value;
                await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
            }

            this._mouseDownOnDropdown = false;
        });
    }

    /// <summary>
    /// Closes the dropdown and resets state.
    /// </summary>
    private void CloseDropdown()
    {
        this._isOpen = false;
        this._hasSearched = false;
        this._searchText = string.Empty;
    }

    /// <summary>
    /// Handles the search input event with debouncing.
    /// </summary>
    private async Task OnSearchInputAsync(ChangeEventArgs e)
    {
        this._searchText = e.Value?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(this._searchText))
        {
            this._hasSearched = false;
            this._filteredItems = Enumerable.Empty<TObject>();
            this._debouncer.Cancel();
            return;
        }

        this._hasSearched = true;
        this._isOpen = true;

        await this._debouncer.DebounceAsync(this.SearchAsync).ConfigureAwait(true);
    }

    /// <summary>
    /// Performs the search.
    /// </summary>
    private async Task SearchAsync()
    {
        this._isLoading = true;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);

        try
        {
            var results = await this.EffectiveLookupController
                .GetSuggestionsAsync<TObject>(this._searchText, null)
                .ConfigureAwait(true);

            this._filteredItems = results;
        }
        finally
        {
            this._isLoading = false;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Toggles the selection of an item. Keeps the dropdown open.
    /// </summary>
    private void ToggleItem(TObject item)
    {
        this.Value ??= new List<TObject>();

        if (this.Value.Contains(item))
        {
            this.Value.Remove(item);
        }
        else
        {
            this.Value.Add(item);
        }

        this.StateHasChanged();
    }

    /// <summary>
    /// Removes an item from the selected list.
    /// </summary>
    private void RemoveItem(TObject item)
    {
        if (this.Value is null)
        {
            return;
        }

        this.Value.Remove(item);
        this.CurrentValue = this.Value;
    }

    /// <summary>
    /// Handles keyboard events.
    /// </summary>
    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (string.Equals(e.Key, "Escape", StringComparison.Ordinal))
        {
            this.CloseDropdown();
        }
    }
}