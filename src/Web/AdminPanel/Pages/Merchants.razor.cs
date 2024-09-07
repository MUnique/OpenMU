// <copyright file="Merchants.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Threading;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

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
    private IContext? _persistenceContext;
    private IDisposable? _navigationLockDisposable;

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

    /// <summary>
    /// Gets or sets the java script runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JavaScript { get; set; } = null!;

    private IQueryable<MerchantStorageViewModel>? ViewModels => this._viewModels?.AsQueryable();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._navigationLockDisposable?.Dispose();
        this._navigationLockDisposable = null;

        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
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
    protected override Task OnInitializedAsync()
    {
        this._navigationLockDisposable = this.NavigationManager.RegisterLocationChangingHandler(this.OnBeforeInternalNavigationAsync);
        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private async ValueTask OnBeforeInternalNavigationAsync(LocationChangingContext context)
    {
        if (this._persistenceContext?.HasChanges is not true)
        {
            return;
        }

        var isConfirmed = await this.JavaScript.InvokeAsync<bool>(
                "window.confirm",
                "There are unsaved changes. Are you sure you want to discard them?")
            .ConfigureAwait(true);

        if (!isConfirmed)
        {
            context.PreventNavigation();
        }
        else
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        }
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        this._persistenceContext = await this.DataSource.GetContextAsync(cancellationToken).ConfigureAwait(true);
        await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
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

    private async Task OnSaveButtonClickAsync()
    {
        string text;
        try
        {
            if (this._persistenceContext is { } context)
            {
                var success = await context.SaveChangesAsync().ConfigureAwait(true);
                text = success ? "The changes have been saved." : "There were no changes to save.";
            }
            else
            {
                text = "Failed, context not initialized";
            }
        }
        catch (Exception ex)
        {
            text = $"An unexpected error occurred: {ex.Message}.";
        }

        await this.ModalService.ShowMessageAsync("Save", text).ConfigureAwait(true);
    }

    private async Task OnCancelButtonClickAsync()
    {
        if (this._persistenceContext?.HasChanges is true)
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
            await this.LoadDataAsync(this._disposeCts?.Token ?? default).ConfigureAwait(true);
        }
    }

    private async Task OnBackButtonClickAsync()
    {
        if (this._persistenceContext?.HasChanges is true)
        {
            var isConfirmed = await this.JavaScript.InvokeAsync<bool>(
                    "window.confirm",
                    "There are unsaved changes. Are you sure you want to discard them?")
                .ConfigureAwait(true);

            if (!isConfirmed)
            {
                return;
            }

            await this.OnCancelButtonClickAsync().ConfigureAwait(true);
        }

        this._selectedMerchant = null;
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