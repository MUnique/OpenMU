// <copyright file="EditItemDrops.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Page which allows to configure all item drops of the game configuration on one page.
/// </summary>
public partial class EditItemDrops : ComponentBase, IAsyncDisposable
{
    private readonly AdditionSelection _addition = new();
    private readonly EditContext _additionEditContext;

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;
    private IContext? _persistenceContext;
    private IDisposable? _navigationLockDisposable;

    private GameConfiguration? _gameConfiguration;
    private List<GameMapDefinition> _maps = [];
    private List<MonsterDefinition> _monsters = [];
    private List<ItemDefinition> _items = [];
    private List<ItemOptionDefinition> _options = [];
    private Dictionary<DropItemGroup, string> _usages = [];

    private string _filter = string.Empty;
    private bool _showOnlyDroppableItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditItemDrops"/> class.
    /// </summary>
    public EditItemDrops()
    {
        this._additionEditContext = new EditContext(this._addition);
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

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<EditItemDrops> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    private bool HasFilter => !string.IsNullOrWhiteSpace(this._filter);

    /// <summary>
    /// Gets the groups which are assigned to maps (or not assigned at all), but not to monsters, quests or mini games.
    /// </summary>
    private IReadOnlyList<DropItemGroup> MapGroups
    {
        get
        {
            var monsterGroups = this._monsters.SelectMany(m => m.DropItemGroups).ToHashSet();
            return this._gameConfiguration!.DropItemGroups
                .Where(g => g is not ItemDropItemGroup && !monsterGroups.Contains(g) && !this._usages.ContainsKey(g))
                .Where(this.IsMatchingFilter)
                .OrderByDescending(g => g.PossibleItems.Count == 0)
                .ThenByDescending(g => g.Chance)
                .ThenBy(g => g.Description.ValueInNeutralLanguage)
                .ToList();
        }
    }

    /// <summary>
    /// Gets the groups which are used by quests or mini games, but are not assigned to monsters.
    /// </summary>
    private IReadOnlyList<DropItemGroup> QuestAndEventGroups
    {
        get
        {
            var monsterGroups = this._monsters.SelectMany(m => m.DropItemGroups).ToHashSet();
            return this._usages.Keys
                .Where(g => !monsterGroups.Contains(g))
                .Where(this.IsMatchingFilter)
                .OrderBy(g => this._usages[g])
                .ThenBy(g => g.Description.ValueInNeutralLanguage)
                .ToList();
        }
    }

    /// <summary>
    /// Gets the drop overviews of the monsters which are spawned on each map, filtered by the current filter.
    /// </summary>
    private IEnumerable<(GameMapDefinition Map, IReadOnlyList<MonsterDropOverview> Monsters)> MonsterDropOverviewsPerMap
    {
        get
        {
            foreach (var map in this._maps)
            {
                var mapMatches = !this.HasFilter || this.Matches(map.Name);
                var overviews = map.MonsterSpawns
                    .Select(spawn => spawn.MonsterDefinition)
                    .OfType<MonsterDefinition>()
                    .Where(monster => monster.ObjectKind is NpcObjectKind.Monster or NpcObjectKind.Destructible)
                    .Distinct()
                    .Select(monster => new MonsterDropOverview(monster, map))
                    .Where(overview => mapMatches
                                       || this.Matches(overview.Monster.Designation)
                                       || overview.Groups.Any(this.IsMatchingFilter))
                    .OrderBy(overview => overview.Level)
                    .ThenBy(overview => overview.Monster.Number)
                    .ToList();
                if (overviews.Count > 0)
                {
                    yield return (map, overviews);
                }
            }
        }
    }

    private IEnumerable<MonsterDefinition> FilteredMonsters =>
        this._monsters
            .Where(m => m.DropItemGroups.Count > 0)
            .Where(m => !this.HasFilter
                        || this.Matches(m.Designation)
                        || m.DropItemGroups.Any(this.IsMatchingFilter));

    private IEnumerable<ItemDefinition> FilteredBoxItems =>
        this._items
            .Where(i => i.DropItems.Count > 0)
            .Where(i => !this.HasFilter
                        || this.Matches(i.Name)
                        || i.DropItems.Any(this.IsMatchingFilter));

    private IEnumerable<ItemDefinition> FilteredItems =>
        this._items
            .Where(i => !this._showOnlyDroppableItems || i.DropsFromMonsters)
            .Where(i => !this.HasFilter || this.Matches(i.Name));

    private IEnumerable<ItemOptionDefinition> FilteredOptions =>
        this._options.Where(o => !this.HasFilter || this.Matches(o.Name));

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

    private static double ToPercent(float chance) => Math.Round(chance * 100.0, 6);

    private static float FromPercent(double percent) => (float)Math.Clamp(percent / 100.0, 0.0, 1.0);

    private static bool IsRelevantOption(ItemOptionDefinition option)
    {
        return option.AddsRandomly
               || option.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck
                                                  || o.OptionType == ItemOptionTypes.Option
                                                  || o.OptionType == ItemOptionTypes.Excellent);
    }

    private bool Matches(string? text)
    {
        return text?.Contains(this._filter.Trim(), StringComparison.OrdinalIgnoreCase) is true;
    }

    private bool IsMatchingFilter(DropItemGroup group)
    {
        return !this.HasFilter
               || this.Matches(group.Description.ToString())
               || this.Matches(group.ItemType.ToString())
               || this.Matches(group.Monster?.Designation)
               || (this._usages.TryGetValue(group, out var usage) && this.Matches(usage))
               || group.PossibleItems.Any(i => this.Matches(i.Name));
    }

    private IReadOnlyList<DropItemGroup> GetMonsterGroups(MonsterDefinition monster)
    {
        return monster.DropItemGroups
            .OrderByDescending(g => g.Chance)
            .ThenBy(g => g.Description.ValueInNeutralLanguage)
            .ToList();
    }

    private IReadOnlyList<DropItemGroup> GetBoxGroups(ItemDefinition item)
    {
        return item.DropItems
            .OrderBy(g => g.SourceItemLevel)
            .ThenByDescending(g => g.Chance)
            .ToList<DropItemGroup>();
    }

    private string? GetUsage(DropItemGroup group)
    {
        return this._usages.GetValueOrDefault(group);
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

            this._maps = gameConfiguration.Maps.OrderBy(m => m.Number).ToList();
            this._monsters = gameConfiguration.Monsters.OrderBy(m => m.Number).ToList();
            this._items = gameConfiguration.Items.OrderBy(i => i.Group).ThenBy(i => i.Number).ToList();
            this._options = gameConfiguration.ItemOptions
                .Where(IsRelevantOption)
                .OrderBy(o => o.Name.ValueInNeutralLanguage)
                .ToList();
            this._usages = this.DetermineUsages(gameConfiguration);
            this._gameConfiguration = gameConfiguration;

            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Expected when navigating away - ignore
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error loading item drop data");
        }
    }

    /// <summary>
    /// Determines where drop item groups are used, apart from maps, monsters and items.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>A text for each group which describes the usage.</returns>
    private Dictionary<DropItemGroup, string> DetermineUsages(GameConfiguration gameConfiguration)
    {
        var usages = new Dictionary<DropItemGroup, List<string>>();
        void AddUsage(DropItemGroup? group, string usage)
        {
            if (group is null)
            {
                return;
            }

            if (!usages.TryGetValue(group, out var list))
            {
                list = [];
                usages.Add(group, list);
            }

            if (!list.Contains(usage))
            {
                list.Add(usage);
            }
        }

        foreach (var quest in gameConfiguration.Monsters.SelectMany(m => m.Quests))
        {
            foreach (var requirement in quest.RequiredItems)
            {
                AddUsage(requirement.DropItemGroup, string.Format(Resources.QuestUsage, quest.Name));
            }
        }

        foreach (var miniGame in gameConfiguration.MiniGameDefinitions)
        {
            foreach (var reward in miniGame.Rewards)
            {
                AddUsage(reward.ItemReward, string.Format(Resources.MiniGameUsage, miniGame.Name));
            }
        }

        return usages.ToDictionary(pair => pair.Key, pair => string.Join(", ", pair.Value));
    }

    private DropItemGroup CreateGroup()
    {
        var group = this._persistenceContext!.CreateNew<DropItemGroup>();
        group.Description = Resources.NewDropItemGroup;
        group.Chance = 0.01;
        group.ItemType = SpecialItemType.None;
        this._gameConfiguration!.DropItemGroups.Add(group);
        return group;
    }

    private void OnAddMapGroup()
    {
        this.CreateGroup();
    }

    private void OnAddMonsterGroup(MonsterDefinition monster)
    {
        var group = this.CreateGroup();
        group.Description = string.Format(Resources.NewDropItemGroupOf, monster.Designation);
        monster.DropItemGroups.Add(group);
    }

    private void OnAddGroupForSelectedMonster()
    {
        if (this._addition.Monster is { } monster)
        {
            this.OnAddMonsterGroup(monster);
            this._addition.Monster = null;
        }
    }

    private void OnAddBoxGroup(ItemDefinition item)
    {
        var group = this._persistenceContext!.CreateNew<ItemDropItemGroup>();
        group.Description = string.Format(Resources.NewDropItemGroupOf, item.Name);
        group.Chance = 0.1;
        group.ItemType = SpecialItemType.None;
        item.DropItems.Add(group);
    }

    private void OnAddGroupForSelectedItem()
    {
        if (this._addition.Item is { } item)
        {
            this.OnAddBoxGroup(item);
            this._addition.Item = null;
        }
    }

    private async Task OnDeleteGroupAsync(DropItemGroup group)
    {
        if (this._usages.TryGetValue(group, out var usage))
        {
            this.ToastService.ShowError(string.Format(Resources.DropItemGroupStillUsed, group.Description, usage));
            return;
        }

        var isConfirmed = await this.ModalService.ShowQuestionAsync(
                Resources.AreYouSure,
                string.Format(Resources.DeleteDropItemGroupQuestion, group.Description))
            .ConfigureAwait(true);
        if (!isConfirmed)
        {
            return;
        }

        await this.DeleteGroupAsync(group).ConfigureAwait(true);
    }

    private async Task OnRemoveGroupFromMonsterAsync(MonsterDefinition monster, DropItemGroup group)
    {
        monster.DropItemGroups.Remove(group);

        var isStillUsed = this._usages.ContainsKey(group)
                          || this._monsters.Any(m => m.DropItemGroups.Contains(group))
                          || this._maps.Any(m => m.DropItemGroups.Contains(group));
        if (!isStillUsed)
        {
            await this.DeleteGroupAsync(group).ConfigureAwait(true);
        }
    }

    private async Task OnRemoveBoxGroupAsync(ItemDefinition item, DropItemGroup group)
    {
        if (group is not ItemDropItemGroup boxGroup)
        {
            return;
        }

        item.DropItems.Remove(boxGroup);
        await this._persistenceContext!.DeleteAsync(boxGroup).ConfigureAwait(true);
    }

    private async Task DeleteGroupAsync(DropItemGroup group)
    {
        foreach (var map in this._maps)
        {
            map.DropItemGroups.Remove(group);
        }

        foreach (var monster in this._monsters)
        {
            monster.DropItemGroups.Remove(group);
        }

        this._gameConfiguration!.DropItemGroups.Remove(group);
        await this._persistenceContext!.DeleteAsync(group).ConfigureAwait(true);
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
            this._gameConfiguration = null;
            await this.LoadDataAsync(this._disposeCts?.Token ?? default).ConfigureAwait(true);
        }
    }

    /// <summary>
    /// The drop overview of a monster on a map. It considers the drop item groups in the same way
    /// as the <see cref="MUnique.OpenMU.GameLogic.DefaultDropGenerator"/>, except the quest and character specific groups.
    /// </summary>
    private sealed class MonsterDropOverview
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterDropOverview"/> class.
        /// </summary>
        /// <param name="monster">The monster.</param>
        /// <param name="map">The map on which the monster is spawned.</param>
        public MonsterDropOverview(MonsterDefinition monster, GameMapDefinition map)
        {
            this.Monster = monster;
            this.Level = (int)monster[Stats.Level];
            var groups = monster.DropItemGroups.ToList();
            if (monster.ObjectKind != NpcObjectKind.Destructible)
            {
                groups.AddRange(map.DropItemGroups.Where(this.IsRelevant).Except(groups));
            }

            this.Groups = groups.OrderByDescending(g => g.Chance).ToList();
            this.GuaranteedDrops = groups.Count(g => g.Chance >= 1.0);
            this.TotalChance = groups.Where(g => g.Chance < 1.0).Sum(g => g.Chance);
        }

        /// <summary>
        /// Gets the monster.
        /// </summary>
        public MonsterDefinition Monster { get; }

        /// <summary>
        /// Gets the level of the monster.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the drop item groups which apply to the monster.
        /// </summary>
        public IReadOnlyList<DropItemGroup> Groups { get; }

        /// <summary>
        /// Gets the number of groups which drop always.
        /// </summary>
        public int GuaranteedDrops { get; }

        /// <summary>
        /// Gets the sum of the chances of the chance based groups, which applies per drop roll.
        /// </summary>
        public double TotalChance { get; }

        /// <summary>
        /// Gets the chance that nothing drops in a drop roll.
        /// </summary>
        public double NoDropChance => Math.Max(0, 1.0 - this.TotalChance);

        /// <summary>
        /// Gets the tooltip text which lists the groups with their chance.
        /// </summary>
        public string GroupsTooltip => string.Join(Environment.NewLine, this.Groups.Select(g => $"{g.Chance:P2} {g.Description}"));

        private bool IsRelevant(DropItemGroup group)
        {
            return (group.MinimumMonsterLevel is not { } minimumLevel || this.Level >= minimumLevel)
                   && (group.MaximumMonsterLevel is not { } maximumLevel || this.Level <= maximumLevel)
                   && (group.Monster is null || group.Monster.Equals(this.Monster));
        }
    }

    /// <summary>
    /// Holds the selected objects for which new drop item groups should be added.
    /// </summary>
    private sealed class AdditionSelection
    {
        /// <summary>
        /// Gets or sets the monster.
        /// </summary>
        public MonsterDefinition? Monster { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public ItemDefinition? Item { get; set; }
    }
}
