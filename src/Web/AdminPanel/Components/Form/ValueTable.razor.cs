// <copyright file="ValueTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A component which shows a collection of <typeparamref name="TValue"/> in a table.
/// </summary>
/// <typeparam name="TValue">The type of the item.</typeparam>
public partial class ValueTable<TValue>
    where TValue : struct, IParsable<TValue>
{
    private bool _isStartingCollapsed;

    private bool _isCollapsed;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    [Parameter]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the persistence context.
    /// </summary>
    [CascadingParameter]
    public IContext PersistenceContext { get; set; } = null!;

    private ValueListWrapper<TValue>? WrappedList { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this._isStartingCollapsed = this.Value is not null && this.Value.Count > 10;
        this._isCollapsed = this._isStartingCollapsed;
        if (this.Value is { })
        {
            this.WrappedList = new ValueListWrapper<TValue>(this.Value);
        }
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out IList<TValue> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }

    private void OnToggleCollapseClick()
    {
        this._isCollapsed = !this._isCollapsed;
    }

    private async Task OnAddClickAsync()
    {
        if (this.Value is null)
        {
            this.Value = new List<TValue>();
        }

        this.WrappedList ??= new ValueListWrapper<TValue>(this.Value);
        this.WrappedList!.Add(new TValue());
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async Task OnRemoveClickAsync(int index)
    {
        this.WrappedList?.RemoveAt(index);
    }
}