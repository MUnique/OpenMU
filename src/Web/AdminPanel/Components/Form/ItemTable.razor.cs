// <copyright file="ItemTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Persistence;

/// <summary>
/// A component which shows a collection of <typeparamref name="TItem"/> in a table.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public partial class ItemTable<TItem>
    where TItem : class
{
    private bool _isEditable;

    private bool _isAddingSupported;

    private bool _isCreatingSupported;

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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this._isEditable = typeof(TItem).Namespace
                              ?.StartsWith(nameof(MUnique), StringComparison.InvariantCulture)
                          ?? false;
        var isMemberOfAggregate = this.ValueExpression!.IsAccessToMemberOfAggregate();
        this._isAddingSupported = !isMemberOfAggregate;
        this._isCreatingSupported = isMemberOfAggregate;
        this._isStartingCollapsed = this.Value is not null && this.Value.Count > 10;
        this._isCollapsed = this._isStartingCollapsed;
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out ICollection<TItem> result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }

    private void OnToggleCollapseClick()
    {
        this._isCollapsed = !this._isCollapsed;
    }

    private async Task OnAddClickAsync()
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<TItem>.PersistenceContext), this.PersistenceContext);
        var modal = this._modal.Show<ModalObjectSelection<TItem>>($"Select {typeof(TItem).Name}", parameters);
        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled && result.Data is TItem item)
        {
            this.Value ??= new List<TItem>();
            this.Value.Add(item);
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task OnCreateClickAsync()
    {
        var item = this.PersistenceContext.CreateNew<TItem>();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<TItem>.Item), item);
        parameters.Add(nameof(ModalCreateNew<TItem>.PersistenceContext), this.PersistenceContext);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this._modal.Show<ModalCreateNew<TItem>>($"Create {typeof(TItem).Name}", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);
        if (result.Cancelled)
        {
            await this.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        }
        else
        {
            this.Value ??= new List<TItem>();
            this.Value.Add(item);
            await this.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task OnRemoveClickAsync(TItem item)
    {
        this.Value?.Remove(item);

        if (this.ValueExpression?.Body is MemberExpression { Member: PropertyInfo propertyInfo }
            && propertyInfo.GetCustomAttribute<MemberOfAggregateAttribute>() is not null)
        {
            await this.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        }

        await this.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
    }
}