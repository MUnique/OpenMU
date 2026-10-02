// <copyright file="ContentActivation.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which allows to activate and deactivate the content of the game configuration in batches,
/// e.g. to restrict the game to the content of a certain season.
/// </summary>
public partial class ContentActivation : ComponentBase, IAsyncDisposable
{
    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private IContext? _persistenceContext;
    private IDisposable? _navigationLockDisposable;

    private Dictionary<ContentType, IReadOnlyList<ContentEntry>>? _entries;
    private ContentType _selectedContentType = ContentType.Maps;
    private StateFilter _stateFilter = StateFilter.All;
    private string _filter = string.Empty;

    /// <summary>
    /// The type of content which can be activated and deactivated.
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// The maps (<see cref="GameMapDefinition"/>).
        /// </summary>
        Maps,

        /// <summary>
        /// The character classes (<see cref="CharacterClass"/>).
        /// </summary>
        CharacterClasses,

        /// <summary>
        /// The monsters and NPCs (<see cref="MonsterDefinition"/>).
        /// </summary>
        Monsters,

        /// <summary>
        /// The items (<see cref="DataModel.Configuration.Items.ItemDefinition"/>).
        /// </summary>
        Items,

        /// <summary>
        /// The mini games (<see cref="MiniGameDefinition"/>).
        /// </summary>
        MiniGames,
    }

    /// <summary>
    /// The filter for the active state of the shown entries.
    /// </summary>
    public enum StateFilter
    {
        /// <summary>
        /// All entries are shown.
        /// </summary>
        All,

        /// <summary>
        /// Only active entries are shown.
        /// </summary>
        OnlyActive,

        /// <summary>
        /// Only inactive entries are shown.
        /// </summary>
        OnlyInactive,
    }

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

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
    public ILogger<ContentActivation> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    private ContentType SelectedContentType
    {
        get => this._selectedContentType;
        set
        {
            this._selectedContentType = value;
            this._filter = string.Empty;
        }
    }

    private IReadOnlyList<ContentEntry> SelectedEntries =>
        this._entries?.GetValueOrDefault(this._selectedContentType) ?? [];

    private IEnumerable<ContentEntry> ShownEntries =>
        this.SelectedEntries
            .Where(entry => this._stateFilter switch
            {
                StateFilter.OnlyActive => entry.IsActive,
                StateFilter.OnlyInactive => !entry.IsActive,
                _ => true,
            })
            .Where(entry => string.IsNullOrWhiteSpace(this._filter)
                            || entry.Name.Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase)
                            || entry.Number.Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase));

    private int ActiveCount => this.SelectedEntries.Count(entry => entry.IsActive);

    private int TotalCount => this.SelectedEntries.Count;

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
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token));
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private static string GetCaption(ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Maps => Resources.GameMaps,
            ContentType.CharacterClasses => Resources.CharacterClasses,
            ContentType.Monsters => Resources.Monsters,
            ContentType.Items => Resources.Items,
            ContentType.MiniGames => Resources.MiniGames,
            _ => contentType.ToString(),
        };
    }

    private static Dictionary<ContentType, IReadOnlyList<ContentEntry>> CreateEntries(GameConfiguration configuration)
    {
        return new Dictionary<ContentType, IReadOnlyList<ContentEntry>>
        {
            [ContentType.Maps] = configuration.Maps
                .OrderBy(map => map.Number)
                .ThenBy(map => map.Discriminator)
                .Select(map => new ContentEntry(map, map.Number.ToString(), map.Name.ToString() ?? string.Empty, () => map.IsActive, value => map.IsActive = value))
                .ToList(),
            [ContentType.CharacterClasses] = configuration.CharacterClasses
                .OrderBy(characterClass => characterClass.Number)
                .Select(characterClass => new ContentEntry(characterClass, characterClass.Number.ToString(), characterClass.Name.ToString() ?? string.Empty, () => characterClass.IsActive, value => characterClass.IsActive = value))
                .ToList(),
            [ContentType.Monsters] = configuration.Monsters
                .OrderBy(monster => monster.Number)
                .Select(monster => new ContentEntry(monster, monster.Number.ToString(), monster.Designation.ToString() ?? string.Empty, () => monster.IsActive, value => monster.IsActive = value))
                .ToList(),
            [ContentType.Items] = configuration.Items
                .OrderBy(item => item.Group)
                .ThenBy(item => item.Number)
                .Select(item => new ContentEntry(item, $"{item.Group}/{item.Number}", item.Name.ToString() ?? string.Empty, () => item.IsActive, value => item.IsActive = value))
                .ToList(),
            [ContentType.MiniGames] = configuration.MiniGameDefinitions
                .OrderBy(miniGame => miniGame.Type)
                .ThenBy(miniGame => miniGame.GameLevel)
                .Select(miniGame => new ContentEntry(miniGame, $"{miniGame.Type} {miniGame.GameLevel}", miniGame.Name.ToString() ?? string.Empty, () => miniGame.IsActive, value => miniGame.IsActive = value))
                .ToList(),
        };
    }

    private void SetActiveOfShownEntries(bool isActive)
    {
        foreach (var entry in this.ShownEntries.ToList())
        {
            entry.IsActive = isActive;
        }
    }

    private async ValueTask OnBeforeInternalNavigationAsync(LocationChangingContext context)
    {
        if (this._persistenceContext?.HasChanges is not true)
        {
            return;
        }

        var isConfirmed = await this.JavaScript.InvokeAsync<bool>(
                "window.confirm",
                Resources.UnsavedChangesQuestion)
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
        using var loading = this.LoadingService.ShowLoadingIndicator();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            this._persistenceContext = await this.DataSource.GetContextAsync(cancellationToken).ConfigureAwait(true);
            var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();

            this._entries = CreateEntries(gameConfiguration);
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Expected when navigating away - ignore
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading the content activation data");
        }
    }

    private async Task OnSaveButtonClickAsync()
    {
        try
        {
            if (this._persistenceContext is { } context)
            {
                var success = await context.SaveChangesAsync().ConfigureAwait(true);
                var text = success ? Resources.SavedChanges : Resources.NoChangesToSave;
                this.ToastService.ShowSuccess(text);
            }
            else
            {
                this.ToastService.ShowError(Resources.FailedByUninitializedContext);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An unexpected error occurred on save: {Message}", ex.Message);
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
    }

    private async Task OnCancelButtonClickAsync()
    {
        if (this._persistenceContext?.HasChanges is true)
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
            this._entries = null;
            await this.LoadDataAsync(this._disposeCts?.Token ?? default).ConfigureAwait(true);
        }
    }

    /// <summary>
    /// An entry of the configuration which can be activated and deactivated.
    /// </summary>
    private sealed class ContentEntry
    {
        private readonly Func<bool> _getIsActive;
        private readonly Action<bool> _setIsActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentEntry"/> class.
        /// </summary>
        /// <param name="entity">The entity of the configuration.</param>
        /// <param name="number">The number which identifies the entity.</param>
        /// <param name="name">The name of the entity.</param>
        /// <param name="getIsActive">The function which gets the active state of the entity.</param>
        /// <param name="setIsActive">The action which sets the active state of the entity.</param>
        public ContentEntry(object entity, string number, string name, Func<bool> getIsActive, Action<bool> setIsActive)
        {
            this.Entity = entity;
            this.Number = number;
            this.Name = name;
            this._getIsActive = getIsActive;
            this._setIsActive = setIsActive;
        }

        /// <summary>
        /// Gets the entity of the configuration.
        /// </summary>
        public object Entity { get; }

        /// <summary>
        /// Gets the number which identifies the entity.
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active.
        /// </summary>
        public bool IsActive
        {
            get => this._getIsActive();
            set => this._setIsActive(value);
        }
    }
}
