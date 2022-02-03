// <copyright file="ItemTable.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.Form;

using System.Diagnostics.CodeAnalysis;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
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

    private async Task OnAddClick()
    {
        var modal = this._modal.Show<ModalObjectSelection<TItem>>($"Select {typeof(TItem).Name}");
        var result = await modal.Result;
        if (!result.Cancelled && result.Data is TItem item)
        {
            this.Value ??= new List<TItem>();
            this.Value.Add(item);
            this.StateHasChanged();
        }
    }

    private async Task OnCreateClick()
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
        var result = await modal.Result;
        if (result.Cancelled)
        {
            this.PersistenceContext.Delete(item);
        }
        else
        {
            this.Value ??= new List<TItem>();
            this.Value.Add(item);
            this.PersistenceContext.SaveChanges();
            this.StateHasChanged();
        }
    }

    private void OnRemoveClick(TItem item)
    {
        this.Value?.Remove(item);

        // use the MemberOfAggregateAttribute here!
        if (!this.ValueExpression!.GetAccessedMemberType().IsConfigurationType()
            && !typeof(TItem).IsConfigurationType())
        {
            this.PersistenceContext.Delete(item);
        }

        this.PersistenceContext.SaveChanges();
    }
}