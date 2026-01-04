// <copyright file="Merchants.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Threading;
using BlazorInputFile;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Razor page which shows objects of the specified type in a grid.
/// </summary>
public partial class Merchants : ComponentBase, IAsyncDisposable
{
    private const string MerchantStoreExportVersion = "openmu-merchant-store-v1";
    private static readonly JsonSerializerOptions MerchantStoreJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private static readonly IItemSerializer StoreItemSerializer = new ItemSerializer();
    private readonly PaginationState _merchantPagination = new() { ItemsPerPage = 20 };
    private readonly PaginationState _itemPagination = new() { ItemsPerPage = 20 };

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private List<MerchantStorageViewModel>? _viewModels;
    private MerchantStorageViewModel? _selectedMerchant;
    private IContext? _persistenceContext;
    private GameConfiguration? _gameConfiguration;
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
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

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

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<Merchants> Logger { get; set; } = null!;

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
        this._gameConfiguration = await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
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
        try
        {
            if (this._persistenceContext is { } context)
            {
                var success = await context.SaveChangesAsync().ConfigureAwait(true);
                var text = success ? "The changes have been saved." : "There were no changes to save.";
                this.ToastService.ShowSuccess(text);
            }
            else
            {
                this.ToastService.ShowError("Failed, context not initialized.");
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"An unexpected error occurred on save: {ex.Message}");
            this.ToastService.ShowError($"An unexpected error occurred: {ex.Message}");
        }
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

    private string GetExportDownloadUri()
    {
        var export = this.BuildExport();
        var json = JsonSerializer.Serialize(export, MerchantStoreJsonOptions);
        var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return $"data:application/json;base64,{payload}";
    }

    private string GetExportFileName()
    {
        var merchantName = this._selectedMerchant?.Merchant.Designation ?? "merchant";
        var safeName = string.Concat(merchantName.Select(ch => ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' ? ch : '_'));
        return $"merchant_store_{safeName}.json";
    }

    private MerchantStoreExport BuildExport()
    {
        var store = this._selectedMerchant?.Merchant.MerchantStore;
        var result = new MerchantStoreExport
        {
            FormatVersion = MerchantStoreExportVersion,
            Serializer = nameof(ItemSerializer),
            MerchantName = this._selectedMerchant?.Merchant.Designation ?? string.Empty,
            Money = store?.Money ?? 0,
        };

        if (store is null)
        {
            return result;
        }

        foreach (var item in store.Items.OrderBy(entry => entry.ItemSlot))
        {
            var buffer = new byte[StoreItemSerializer.NeededSpace];
            var size = StoreItemSerializer.SerializeItem(buffer, item);
            result.Items.Add(new MerchantStoreItemExport
            {
                ItemSlot = item.ItemSlot,
                ItemData = Convert.ToHexString(buffer.AsSpan(0, size)),
            });
        }

        return result;
    }

    private async Task OnImportFileAsync(IFileListEntry[] files)
    {
        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        if (this._persistenceContext is null || this._gameConfiguration is null)
        {
            this.ToastService.ShowError("Failed to import: context not initialized.");
            return;
        }

        try
        {
            await using var stream = file.Data;
            var import = await JsonSerializer.DeserializeAsync<MerchantStoreExport>(stream, MerchantStoreJsonOptions).ConfigureAwait(true);
            if (import is null)
            {
                this.ToastService.ShowError("Failed to import: file is empty or invalid.");
                return;
            }

            await this.ApplyImportAsync(import).ConfigureAwait(true);
            this.ToastService.ShowSuccess($"Imported {import.Items.Count} items.");
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred while importing merchant store.");
            this.ToastService.ShowError($"Failed to import: {ex.Message}");
        }
    }

    private async Task ApplyImportAsync(MerchantStoreExport import)
    {
        var store = this._selectedMerchant?.Merchant.MerchantStore;
        if (store is null)
        {
            this.ToastService.ShowError("Failed to import: merchant store not loaded.");
            return;
        }

        if (!string.Equals(import.FormatVersion, MerchantStoreExportVersion, StringComparison.OrdinalIgnoreCase))
        {
            this.ToastService.ShowWarning("Import format does not match the expected version.");
        }

        if (!string.IsNullOrWhiteSpace(import.Serializer) && !string.Equals(import.Serializer, nameof(ItemSerializer), StringComparison.OrdinalIgnoreCase))
        {
            this.ToastService.ShowWarning($"Import uses serializer '{import.Serializer}', but this server expects '{nameof(ItemSerializer)}'.");
        }

        var itemsToRemove = store.Items.ToList();
        foreach (var item in itemsToRemove)
        {
            store.Items.Remove(item);
            await this._persistenceContext!.DeleteAsync(item).ConfigureAwait(true);
        }

        store.Money = import.Money;

        foreach (var importedItem in import.Items)
        {
            if (string.IsNullOrWhiteSpace(importedItem.ItemData))
            {
                continue;
            }

            try
            {
                var bytes = Convert.FromHexString(importedItem.ItemData);
                if (bytes.Length < StoreItemSerializer.NeededSpace)
                {
                    this.Logger.LogWarning("Skipped item import because data is too short (slot {Slot}).", importedItem.ItemSlot);
                    continue;
                }

                var item = StoreItemSerializer.DeserializeItem(bytes.AsSpan(0, StoreItemSerializer.NeededSpace), this._gameConfiguration!, this._persistenceContext!);
                item.ItemSlot = importedItem.ItemSlot;
                store.Items.Add(item);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "Skipped item import at slot {Slot}.", importedItem.ItemSlot);
            }
        }

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
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

    public sealed class MerchantStoreExport
    {
        public string FormatVersion { get; set; } = MerchantStoreExportVersion;

        public string Serializer { get; set; } = nameof(ItemSerializer);

        public string MerchantName { get; set; } = string.Empty;

        public int Money { get; set; }

        public List<MerchantStoreItemExport> Items { get; set; } = new();
    }

    public sealed class MerchantStoreItemExport
    {
        public byte ItemSlot { get; set; }

        public string ItemData { get; set; } = string.Empty;
    }
}
