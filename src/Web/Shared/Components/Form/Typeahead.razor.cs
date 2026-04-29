// <copyright file="Typeahead.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.Form;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

/// <summary>
/// A custom typeahead component that supports single and multiple selections.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public partial class Typeahead<TItem, TValue> : ComponentBase, IDisposable
{
    private readonly Debouncer _searchDebouncer = new(300);

    private bool _isOpen;
    private bool _isLoading;
    private int _focusedIndex = -1;
    private string _searchText = string.Empty;
    private ElementReference _searchInput;
    private CancellationTokenSource? _searchCts;
    private List<TItem> _suggestions = new();

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds.
    /// </summary>
    [Parameter]
    public int DebounceTime { get; set; } = 300;

    /// <summary>
    /// Gets or sets the element id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the placeholder.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the search method.
    /// </summary>
    [Parameter]
    public Func<string, Task<IEnumerable<TItem>>> SearchMethod { get; set; } = null!;

    /// <summary>
    /// Gets or sets the single value.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the value changed callback.
    /// </summary>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the value expression.
    /// </summary>
    [Parameter]
    public System.Linq.Expressions.Expression<Func<TValue>>? ValueExpression { get; set; }

    /// <summary>
    /// Gets or sets the multiple values.
    /// </summary>
    [Parameter]
    public IList<TValue>? Values { get; set; }

    /// <summary>
    /// Gets or sets the values changed callback.
    /// </summary>
    [Parameter]
    public EventCallback<IList<TValue>?> ValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the values expression.
    /// </summary>
    [Parameter]
    public System.Linq.Expressions.Expression<Func<IList<TValue>>>? ValuesExpression { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown is enabled.
    /// </summary>
    [Parameter]
    public bool EnableDropDown { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the dropdown on focus.
    /// </summary>
    [Parameter]
    public bool ShowDropDownOnFocus { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of suggestions.
    /// </summary>
    [Parameter]
    public int MaximumSuggestions { get; set; } = 10;

    /// <summary>
    /// Gets or sets the selected item template.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? SelectedTemplate { get; set; }

    /// <summary>
    /// Gets or sets the result item template.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ResultTemplate { get; set; }

    private bool IsMultiple => this.Values != null || this.ValuesChanged.HasDelegate;

    /// <inheritdoc />
    public void Dispose()
    {
#pragma warning disable VSTHRD103
        this._searchCts?.Cancel();
#pragma warning restore VSTHRD103
        this._searchCts?.Dispose();
        this._searchDebouncer.Dispose();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!this.IsMultiple && this.Value != null)
        {
            var text = this.GetItemText(this.GetItemFromValue(this.Value!));
            if (this._searchText != text && !this._isOpen)
            {
                this._searchText = text;
            }
        }
        else if (!this.IsMultiple && this.Value == null)
        {
            if (!this._isOpen)
            {
                this._searchText = string.Empty;
            }
        }
    }

    private async Task OnSearchInputAsync()
    {
        if (!this.IsMultiple)
        {
            if (string.IsNullOrEmpty(this._searchText))
            {
                this._searchDebouncer.Cancel();
                this._isOpen = false;
                await this.SelectResultAsync(default).ConfigureAwait(false);
                return;
            }
        }

        this._isOpen = true;
        this._isLoading = true;
        this.StateHasChanged();

        _ = this._searchDebouncer.DebounceAsync(async () =>
        {
            await this.InvokeAsync(async () =>
            {
                await this.PerformSearchAsync().ConfigureAwait(false);
            }).ConfigureAwait(true);
        });
    }

    private async Task OnFocusAsync()
    {
        if (this.ShowDropDownOnFocus)
        {
            this._searchDebouncer.Cancel();
            await this.PerformSearchAsync().ConfigureAwait(false);
        }
    }

    private async Task OnFocusOutAsync(FocusEventArgs args)
    {
        // Allow time for click events on results to process
        await Task.Delay(150).ConfigureAwait(true);
        this._isOpen = false;
        if (!this.IsMultiple)
        {
            this._searchText = this.Value == null ? string.Empty : this.GetItemText(this.GetItemFromValue(this.Value));
        }
        else
        {
            this._searchText = string.Empty;
        }

        this.StateHasChanged();
    }

    private async Task PerformSearchAsync()
    {
        if (this._searchCts != null)
        {
#pragma warning disable VSTHRD103
            this._searchCts.Cancel();
#pragma warning restore VSTHRD103
        }

        this._searchCts = new CancellationTokenSource();
        var token = this._searchCts.Token;

        this._isLoading = true;
        this._isOpen = true;
        this._focusedIndex = -1;
        this.StateHasChanged();

        try
        {
            var results = await this.SearchMethod(this._searchText).ConfigureAwait(true);
            if (!token.IsCancellationRequested)
            {
                this._suggestions = results?.Take(this.MaximumSuggestions).ToList() ?? new List<TItem>();

                if (this.IsMultiple && this.Values != null)
                {
                    this._suggestions = this._suggestions.Where(s => !this.Values.Contains((TValue)(object)s!)).ToList();
                }
            }
        }
        catch (TaskCanceledException)
        {
            // Ignore
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                this._isLoading = false;
                this.StateHasChanged();
            }
        }
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        if (!this._isOpen && args.Key == "ArrowDown")
        {
            this._searchDebouncer.Cancel();
            await this.PerformSearchAsync().ConfigureAwait(false);
            return;
        }

        if (this._isOpen && this._suggestions.Any())
        {
            if (args.Key == "ArrowDown")
            {
                this._focusedIndex = Math.Min(this._focusedIndex + 1, this._suggestions.Count - 1);
            }
            else if (args.Key == "ArrowUp")
            {
                this._focusedIndex = Math.Max(this._focusedIndex - 1, 0);
            }
            else if (args.Key == "Enter")
            {
                if (this._focusedIndex >= 0 && this._focusedIndex < this._suggestions.Count)
                {
                    await this.SelectResultAsync(this._suggestions[this._focusedIndex]).ConfigureAwait(false);
                }
            }
            else if (args.Key == "Escape")
            {
                this._isOpen = false;
            }
        }
    }

    private async Task SelectResultAsync(TItem? item)
    {
        if (this.IsMultiple)
        {
            if (item != null)
            {
                var val = (TValue)(object)item;
                var newValues = this.Values == null ? new List<TValue>() : new List<TValue>(this.Values);
                if (!newValues.Contains(val))
                {
                    newValues.Add(val);
                    await this.ValuesChanged.InvokeAsync(newValues).ConfigureAwait(false);
                }
            }

            this._searchText = string.Empty;
            this._isOpen = false;
        }
        else
        {
            if (item == null)
            {
                await this.ValueChanged.InvokeAsync(default).ConfigureAwait(false);
                this._searchText = string.Empty;
            }
            else
            {
                await this.ValueChanged.InvokeAsync((TValue)(object)item).ConfigureAwait(false);
                this._searchText = this.GetItemText(item);
            }

            this._isOpen = false;
        }

        this.StateHasChanged();
    }

    private async Task RemoveValueAsync(TValue value)
    {
        if (this.Values != null)
        {
            var newValues = new List<TValue>(this.Values);
            if (newValues.Remove(value))
            {
                await this.ValuesChanged.InvokeAsync(newValues).ConfigureAwait(false);
            }
        }
    }

    private async Task ClearValueAsync()
    {
        await this.ValueChanged.InvokeAsync(default).ConfigureAwait(false);
        this._searchText = string.Empty;
        this._isOpen = true;
        this.StateHasChanged();
        await this.PerformSearchAsync().ConfigureAwait(false);
    }

    private TItem? GetItemFromValue(TValue value)
    {
        if (value == null)
        {
            return default;
        }

        return (TItem)(object)value;
    }

    private string GetItemText(TItem? item)
    {
        return item?.ToString() ?? string.Empty;
    }
}
