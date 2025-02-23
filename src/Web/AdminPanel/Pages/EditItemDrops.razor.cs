// <copyright file="EditItemDrops.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Implements a simplified page for editing item drops.
/// </summary>
public partial class EditItemDrops : IAsyncDisposable
{
    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private IContext? _persistenceContext;
    private IDisposable? _navigationLockDisposable;

    private DropItemGroup? _randomDropGroup;

    private DropItemGroup? _excellentDropGroup;

    private DropItemGroup? _moneyDropGroup;

    private List<ItemOptionDefinition> _normalOptions = [];

    private List<ItemOptionDefinition> _excellentOptions = [];

    private ItemOptionDefinition? _luckOption;

    private double CommonItemPercentage
    {
        get => this._randomDropGroup!.Chance * 100;
        set => this._randomDropGroup!.Chance = value / 100;
    }

    private double ExcellentItemPercentage
    {
        get => this._excellentDropGroup!.Chance * 100;
        set => this._excellentDropGroup!.Chance = value / 100;
    }

    private double MoneyPercentage
    {
        get => this._moneyDropGroup!.Chance * 100;
        set => this._moneyDropGroup!.Chance = value / 100;
    }

    private double NoDropPercentage
    {
        get => Math.Max(0, 1 - (this._randomDropGroup!.Chance + this._moneyDropGroup!.Chance + this._excellentDropGroup!.Chance));
    }

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
        var data = this.DataSource.GetAll<DropItemGroup>()
            .Where(m => m is { Monster: null, })
            .ToList();
        this._excellentDropGroup = data.FirstOrDefault(d => d is { ItemType: SpecialItemType.Excellent, PossibleItems.Count: 0 });
        this._randomDropGroup = data.FirstOrDefault(d => d is { ItemType: SpecialItemType.RandomItem, PossibleItems.Count: 0 });
        this._moneyDropGroup = data.FirstOrDefault(d => d.ItemType == SpecialItemType.Money);

        var options = this.DataSource.GetAll<ItemOptionDefinition>();
        this._normalOptions = options.Where(d => d.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option)).ToList();
        this._excellentOptions = options.Where(d => d.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Excellent)).ToList();
        this._luckOption = options.FirstOrDefault(d => d.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck));

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
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
}
