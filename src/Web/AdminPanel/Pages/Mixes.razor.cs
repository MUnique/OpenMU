// <copyright file="Mixes.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A page which shows the crafting definitions (mixes) of the chaos machine and the other NPCs which offer them.
/// Each mix is edited and saved on its own.
/// </summary>
public partial class Mixes : ComponentBase, IAsyncDisposable
{
    private readonly List<Category> _categories = [];

    private GameConfiguration? _gameConfiguration;

    private CancellationTokenSource? _disposeCts;

    private Task? _loadTask;

    private string? _filter;

    /// <summary>
    /// Gets or sets the data source of the game configuration.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the persistence context provider.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<Mixes> Logger { get; set; } = null!;

    /// <summary>
    /// Gets the monsters with their crafting definitions, grouped and filtered by the search term.
    /// </summary>
    private IEnumerable<(Category Category, IReadOnlyList<ItemCrafting> Craftings)> CraftingGroups
    {
        get
        {
            foreach (var category in this._categories)
            {
                var craftings = category.Craftings
                    .Where(crafting => this.MatchesFilter(category.Title, crafting))
                    .OrderBy(crafting => crafting.Number)
                    .ToList();
                if (craftings.Count > 0)
                {
                    yield return (category, craftings);
                }
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        try
        {
            await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // expected when navigating away
        }
        catch
        {
            // and we should not throw exceptions in the dispose method
        }

        foreach (var category in this._categories)
        {
            category.Dispose();
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
        using var loading = this.LoadingService.ShowLoadingIndicator();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
            var categories = new List<Category>();
            foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.ItemCraftings.Count > 0).OrderBy(monster => monster.Number))
            {
                var category = new Category(monster.GetId(), $"{monster.Number} - {monster.Designation}");
                await this.LoadCategoryAsync(category, monster.ItemCraftings.Select(crafting => crafting.GetId()), gameConfiguration, cancellationToken).ConfigureAwait(true);
                categories.Add(category);
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var category in this._categories)
            {
                category.Dispose();
            }

            this._categories.Clear();
            this._categories.AddRange(categories);
            this._gameConfiguration = gameConfiguration;

            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // expected when navigating away
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the mixes");
        }
    }

    private async Task LoadCategoryAsync(Category category, IEnumerable<Guid> craftingIds, GameConfiguration gameConfiguration, CancellationToken cancellationToken)
    {
        var context = this.PersistenceContextProvider.CreateNewTypedContext(typeof(ItemCrafting), true, gameConfiguration);
        var craftings = new List<ItemCrafting>();
        try
        {
            foreach (var craftingId in craftingIds)
            {
                if (await context.GetByIdAsync(craftingId, typeof(ItemCrafting), cancellationToken).ConfigureAwait(true) is ItemCrafting crafting)
                {
                    craftings.Add(crafting);
                }
            }
        }
        catch
        {
            context.Dispose();
            throw;
        }

        category.Context = context;
        category.Craftings = craftings.OrderBy(crafting => crafting.Number).ToList();
    }

    private EventCallback GetDiscardCallback(Category category) => EventCallback.Factory.Create(this, () => this.OnDiscardAsync(category));

    private async Task OnSaveAsync(Category category)
    {
        if (category.Context is not { } context)
        {
            this.ToastService.ShowError(Resources.FailedByUninitializedContext);
            return;
        }

        category.IsSaving = true;
        try
        {
            var success = await context.SaveChangesAsync().ConfigureAwait(true);
            this.ToastService.ShowSuccess(success ? Resources.SavedChanges : Resources.NoChangesToSave);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred on save: {Message}", ex.Message);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            category.IsSaving = false;
        }
    }

    private async Task OnDiscardAsync(Category category)
    {
        if (this._gameConfiguration is not { } gameConfiguration || category.Context?.HasChanges is not true)
        {
            return;
        }

        var craftingIds = this._gameConfiguration.Monsters
            .Where(monster => monster.GetId() == category.MonsterId)
            .SelectMany(monster => monster.ItemCraftings)
            .Select(crafting => crafting.GetId())
            .ToList();

        var reloaded = new Category(category.MonsterId, category.Title);
        try
        {
            await this.LoadCategoryAsync(reloaded, craftingIds, gameConfiguration, this._disposeCts?.Token ?? default).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error reloading the mixes of {Category}", category.Title);
            return;
        }

        category.Dispose();
        category.Context = reloaded.Context;
        category.Craftings = reloaded.Craftings;
    }

    private bool MatchesFilter(string categoryTitle, ItemCrafting crafting)
    {
        if (string.IsNullOrWhiteSpace(this._filter))
        {
            return true;
        }

        var filter = this._filter.Trim();
        return Matches(categoryTitle)
               || Matches(crafting.Name.ToString()) || Matches(crafting.Name.ValueInNeutralLanguage)
               || Matches(crafting.ItemCraftingHandlerClassName)
               || crafting.Number.ToString(System.Globalization.CultureInfo.InvariantCulture).Contains(filter, StringComparison.OrdinalIgnoreCase);

        bool Matches(string? text) => text?.Contains(filter, StringComparison.OrdinalIgnoreCase) is true;
    }

    /// <summary>
    /// A category of mixes, i.e. the mixes of one NPC, with the editing context of that NPC.
    /// </summary>
    private sealed class Category : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        /// <param name="monsterId">The identifier of the NPC which offers the mixes.</param>
        /// <param name="title">The caption of the category.</param>
        public Category(Guid monsterId, string title)
        {
            this.MonsterId = monsterId;
            this.Title = title;
        }

        /// <summary>
        /// Gets the identifier of the NPC which offers the mixes.
        /// </summary>
        public Guid MonsterId { get; }

        /// <summary>
        /// Gets the caption of the category.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets or sets the editing context which contains the mixes of this category.
        /// </summary>
        public IContext? Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the category is being saved.
        /// </summary>
        public bool IsSaving { get; set; }

        /// <summary>
        /// Gets or sets the mixes which are shown.
        /// </summary>
        public IReadOnlyList<ItemCrafting> Craftings { get; set; } = [];

        /// <inheritdoc />
        public void Dispose() => this.Context?.Dispose();
    }
}
