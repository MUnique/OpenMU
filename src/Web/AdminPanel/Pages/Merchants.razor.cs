// <copyright file="Merchants.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Threading;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Components.Form;

/// <summary>
/// Razor page which shows objects of the specified type in a grid.
/// </summary>
public partial class Merchants : ComponentBase, IAsyncDisposable
{
    private readonly PaginationState _merchantPagination = new() { ItemsPerPage = 20 };
    private readonly PaginationState _itemPagination = new() { ItemsPerPage = 20 };

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private List<MerchantStorageViewModel>? _viewModels;
    private MerchantStorageViewModel? _selectedMerchant;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the context provider.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider ContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    private IQueryable<MerchantStorageViewModel>? ViewModels => this._viewModels?.AsQueryable();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        try
        {
            await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // we can ignore that ...
        }
        catch
        {
            // and we should not throw exceptions in the dispose method ...
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await this.DataSource.GetOwnerAsync().ConfigureAwait(true);
        cancellationToken.ThrowIfCancellationRequested();
        var data = this.DataSource.GetAll<MonsterDefinition>()
            .Where(m => m is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore: { } });
        this._viewModels = data
            .Select(o => new MerchantStorageViewModel(o))
            .OrderBy(o => o.Merchant.Designation)
            .ToList();

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async Task OnMerchantEditClickAsync(MerchantStorageViewModel context)
    {
        this._selectedMerchant = context;
        await this.InvokeAsync(async () => await this._itemPagination.SetCurrentPageIndexAsync(0).ConfigureAwait(true)).ConfigureAwait(true);
    }

    private async Task OnRemoveItemClickAsync(Item item)
    {
        var context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
        if (await context.DeleteAsync(item).ConfigureAwait(true))
        {
            this._selectedMerchant?.Items.Remove(item);
        }

        await context.SaveChangesAsync().ConfigureAwait(true);
    }

    private async Task OnCreateItemClickAsync()
    {
        using var tempContext = this.ContextProvider.CreateNewContext();
        var tempItem = tempContext.CreateNew<Item>();
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<Item>.Item), tempItem);
        parameters.Add(nameof(ModalCreateNew<Item>.PersistenceContext), tempContext);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
            Class = "blazored-modal modal-edit-item",
        };

        var modal = this.ModalService.Show<ModalCreateNew<Item>>($"Create item for {this._selectedMerchant!.Merchant.Designation}", parameters, options);
        var result = await modal.Result.ConfigureAwait(true);

        if (result.Cancelled)
        {
            return;
        }

        var context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
        var createdItem = context.CreateNew<Item>();
        createdItem.AssignValues(tempItem);
        this._selectedMerchant.Items.Add(createdItem);
        await context.SaveChangesAsync().ConfigureAwait(true);
    }

    private async Task OnEditItemClickAsync(Item item)
    {
        var context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalCreateNew<Item>.Item), item);
        parameters.Add(nameof(ModalCreateNew<Item>.PersistenceContext), context);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
            Class = "blazored-modal modal-edit-item",
        };

        var modal = this.ModalService.Show<ModalCreateNew<Item>>($"Edit item of {this._selectedMerchant!.Merchant.Designation}", parameters, options);
        var result = await modal.Result.ConfigureAwait(true);

        if (result.Cancelled)
        {
            if (context.HasChanges)
            {
                await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
                await this.LoadDataAsync(default).ConfigureAwait(true);
            }

            return;
        }

        await context.SaveChangesAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// The view model for a merchant store.
    /// </summary>
    public class MerchantStorageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantStorageViewModel"/> class.
        /// </summary>
        /// <param name="merchant">The merchant.</param>
        public MerchantStorageViewModel(MonsterDefinition merchant)
        {
            this.Merchant = merchant;
            this.Id = merchant.GetId();
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [Browsable(false)]
        public Guid Id { get; }

        /// <summary>
        /// Gets the merchant definition.
        /// </summary>
        [Browsable(false)]
        public MonsterDefinition Merchant { get; }

        /// <summary>
        /// Gets the name of the merchant.
        /// </summary>
        [Browsable(false)]
        public string Name => this.Merchant.Designation;

        /// <summary>
        /// Gets the items of the merchant.
        /// </summary>
        public ICollection<Item> Items => this.Merchant.MerchantStore!.Items;
    }
}