// <copyright file="CashShopCatalog.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// The page which edits the packages and products of the cash shop.
/// </summary>
/// <remarks>
/// The game client shows the catalog of its own script files, so this page only edits the server side
/// of it: what is for sale, which items are delivered and the prices which are charged.
/// </remarks>
public partial class CashShopCatalog : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// The text of a grid cell without value. A grid row with an empty cell is hidden by the shared grid style.
    /// </summary>
    private const string NoValue = "–";

    private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };

    private CancellationTokenSource? _disposeCts;
    private IDisposable? _navigationLockDisposable;
    private IContext? _persistenceContext;
    private CashShopConfiguration? _configuration;
    private CashShopPackage? _selectedPackage;
    private string _searchText = string.Empty;
    private bool _onlyForSale;
    private bool _isLoading = true;

    [Inject]
    private IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    [Inject]
    private ILookupController LookupController { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private IJSRuntime JavaScript { get; set; } = null!;

    [Inject]
    private ILogger<CashShopCatalog> Logger { get; set; } = null!;

    private IQueryable<CashShopPackage>? FilteredPackages => this._configuration?.Packages
        .Where(p => !this._onlyForSale || p.IsForSale)
        .Where(p => string.IsNullOrWhiteSpace(this._searchText)
                    || p.Name.Contains(this._searchText, StringComparison.OrdinalIgnoreCase)
                    || p.PackageSequence.ToString() == this._searchText.Trim())
        .OrderBy(p => p.PackageSequence)
        .AsQueryable();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._navigationLockDisposable?.Dispose();
        this._navigationLockDisposable = null;
        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        // The data source is shared, so unsaved changes would be saved by the next page which saves.
        // The navigation handler only catches navigation within the panel, not a closed tab or a reload.
        if (this._persistenceContext?.HasChanges is true)
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        this._navigationLockDisposable = this.NavigationManager.RegisterLocationChangingHandler(this.OnBeforeInternalNavigationAsync);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        this._disposeCts = new CancellationTokenSource();
        await this.LoadDataAsync(this._disposeCts.Token).ConfigureAwait(true);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private static string GetItemCaption(ItemDefinition? item)
    {
        return item is null ? string.Empty : $"{(string)item.Name} ({item.Group}, {item.Number})";
    }

    private static string GetFlagText(bool value) => value ? "✓" : NoValue;

    private static string GetPriceText(CashShopPackage package)
    {
        if (package.IsBundle)
        {
            return package.Price.ToString("N0");
        }

        var prices = package.Products.Select(p => p.Price).Distinct().Order().ToList();
        return prices.Count switch
        {
            0 => NoValue,
            1 => prices[0].ToString("N0"),
            _ => $"{prices[0]:N0} – {prices[^1]:N0}",
        };
    }

    private static void SetDurationHours(CashShopProduct product, object? value)
    {
        if (int.TryParse(value?.ToString(), out var hours))
        {
            product.Duration = TimeSpan.FromHours(Math.Max(hours, 0));
        }
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        this._isLoading = true;
        try
        {
            this._persistenceContext = await this.DataSource.GetContextAsync(cancellationToken).ConfigureAwait(true);
            var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
            this._configuration = gameConfiguration.CashShopConfiguration;
        }
        catch (OperationCanceledException)
        {
            // Expected when navigating away.
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the cash shop catalog.");
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private async ValueTask OnBeforeInternalNavigationAsync(LocationChangingContext context)
    {
        if (this._persistenceContext?.HasChanges is not true)
        {
            return;
        }

        if (!await this.JavaScript.InvokeAsync<bool>("window.confirm", Resources.UnsavedChangesQuestion).ConfigureAwait(true))
        {
            context.PreventNavigation();
        }
        else
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        }
    }

    private void OnEditPackage(CashShopPackage package)
    {
        this._selectedPackage = package;
    }

    private void OnBackToList()
    {
        this._selectedPackage = null;
    }

    private void OnAddPackage()
    {
        if (this._persistenceContext is not { } context || this._configuration is not { } configuration)
        {
            return;
        }

        var package = context.CreateNew<CashShopPackage>();
        package.PackageSequence = configuration.Packages.Select(p => p.PackageSequence).DefaultIfEmpty().Max() + 1;
        configuration.Packages.Add(package);
        this._selectedPackage = package;
    }

    private void OnAddProduct(CashShopPackage package)
    {
        if (this._persistenceContext is not { } context || this._configuration is not { } configuration)
        {
            return;
        }

        var product = context.CreateNew<CashShopProduct>();
        product.PriceSequence = configuration.Packages.SelectMany(p => p.Products).Select(p => p.PriceSequence).DefaultIfEmpty().Max() + 1;
        product.Quantity = 1;
        package.Products.Add(product);
    }

    private async Task OnRemoveProductAsync(CashShopPackage package, CashShopProduct product)
    {
        package.Products.Remove(product);
        if (this._persistenceContext is { } context)
        {
            await context.DeleteAsync(product).ConfigureAwait(true);
        }
    }

    private async Task OnDeletePackageAsync(CashShopPackage package)
    {
        if (this._persistenceContext is not { } context
            || this._configuration is not { } configuration
            || !await this.JavaScript.InvokeAsync<bool>("window.confirm", Resources.CashShopDeletePackageQuestion).ConfigureAwait(true))
        {
            return;
        }

        configuration.Packages.Remove(package);
        foreach (var product in package.Products.ToList())
        {
            await context.DeleteAsync(product).ConfigureAwait(true);
        }

        await context.DeleteAsync(package).ConfigureAwait(true);
        this._selectedPackage = null;
    }

    private async Task OnSaveAsync()
    {
        try
        {
            if (this._persistenceContext is not { } context)
            {
                this.ToastService.ShowError(Resources.FailedByUninitializedContext);
                return;
            }

            var success = await context.SaveChangesAsync().ConfigureAwait(true);
            this.ToastService.ShowSuccess(success ? Resources.SavedChanges : Resources.NoChangesToSave);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred on saving the cash shop catalog.");
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
    }

    private async Task OnDiscardAsync()
    {
        var selectedSequence = this._selectedPackage?.PackageSequence;
        await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        await this.LoadDataAsync(this._disposeCts?.Token ?? default).ConfigureAwait(true);
        this._selectedPackage = selectedSequence is { } sequence
            ? this._configuration?.Packages.FirstOrDefault(p => p.PackageSequence == sequence)
            : null;
    }
}
