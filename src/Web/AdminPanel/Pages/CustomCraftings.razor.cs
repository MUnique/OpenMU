// <copyright file="CustomCraftings.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>Creates configurable item crafting recipes and attaches them to an NPC.</summary>
public partial class CustomCraftings : ComponentBase, IAsyncDisposable
{
    private readonly RecipeForm _form = new();
    private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };
    private readonly PaginationState _allRecipesPagination = new() { ItemsPerPage = 20 };
    private RequirementForm _requirementDraft = new();
    private ResultForm _resultDraft = new();
    private GameConfiguration? _gameConfiguration;
    private IContext? _context;
    private List<MonsterDefinition> _npcs = [];
    private List<ItemDefinition> _items = [];
    private List<ItemOptionType> _optionTypes = [];
    private string? _selectedNpcId;
    private string _allRecipesSearch = string.Empty;
    private bool _loading = true;
    private bool _saving;
    private ActiveTab _activeTab = ActiveTab.AvailableRecipes;

    private enum ActiveTab
    {
        AvailableRecipes,
        CreateRecipe,
        AllRecipes,
    }

    [Inject]
    private IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;

    [Inject]
    private ILogger<CustomCraftings> Logger { get; set; } = null!;

    [Inject]
    private IJSRuntime JavaScript { get; set; } = null!;

    private MonsterDefinition? SelectedNpc => Guid.TryParse(this._selectedNpcId, out var id)
        ? this._npcs.FirstOrDefault(npc => npc.GetId() == id)
        : null;

    private ItemDefinition? SelectedResultItem => this.FindItem(this._resultDraft.ItemId);

    private ItemDefinition? SelectedRequiredItem => this.FindItem(this._requirementDraft.ItemId);

    private bool SelectedResultIsStackable => this.SelectedResultItem is { } item && this.IsStackable(item);

    private bool SelectedResultHasDurability => this.SelectedResultItem?.ItemSlot is not null;

    private bool SelectedRequiredItemIsStackable => this.SelectedRequiredItem is { } item && this.IsStackable(item);

    private string RequiredMinimumAmountLabel => this.Text(this.SelectedRequiredItemIsStackable ? "CustomCraftingsMinimumStackQuantityShort" : "CustomCraftingsMinimumItemCountShort");

    private string RequiredMaximumAmountLabel => this.Text(this.SelectedRequiredItemIsStackable ? "CustomCraftingsMaximumStackQuantityShort" : "CustomCraftingsMaximumItemCountShort");

    private string RequiredAmountHint => this.Text(this.SelectedRequiredItemIsStackable ? "CustomCraftingsRequiredStackAmountHint" : "CustomCraftingsRequiredItemAmountHint");

    private string ResultQuantityOrDurabilityLabel => this.SelectedResultIsStackable
        ? this.Text("CustomCraftingsStackQuantity")
        : this.SelectedResultHasDurability ? this.Text("CustomCraftingsDurability") : this.Text("CustomCraftingsSingleItem");

    private string ResultQuantityOrDurabilityHint => this.SelectedResultIsStackable
        ? string.Format(CultureInfo.CurrentCulture, this.Text("CustomCraftingsStackQuantityHint"), this.SelectedResultItem!.Durability)
        : this.SelectedResultHasDurability ? this.Text("CustomCraftingsDurabilityHint") : this.Text("CustomCraftingsSingleItemHint");

    private int ResultQuantityOrDurabilityMinimum => this.SelectedResultIsStackable ? 1 : 0;

    private int ResultQuantityOrDurabilityMaximum => this.SelectedResultIsStackable ? this.SelectedResultItem!.Durability : byte.MaxValue;

    private IQueryable<RecipeListEntry> FilteredAllRecipes
    {
        get
        {
            var search = this._allRecipesSearch.Trim();
            var recipes = this._npcs.SelectMany(npc => npc.ItemCraftings.Select(crafting => new RecipeListEntry(npc, crafting)));
            return (string.IsNullOrEmpty(search)
                ? recipes
                : recipes.Where(entry => this.LocalizedName(entry.Npc.Designation).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || entry.Npc.Number.ToString(CultureInfo.CurrentCulture).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || this.LocalizedName(entry.Crafting.Name).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || entry.Crafting.Number.ToString(CultureInfo.CurrentCulture).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || this.CraftingRequirements(entry.Crafting).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || this.CraftingResults(entry.Crafting).Contains(search, StringComparison.CurrentCultureIgnoreCase)))
                .AsQueryable();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._context is { HasChanges: true })
        {
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        try
        {
            this._context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
            this._gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);
            this._npcs = this._gameConfiguration.Monsters
                .Where(npc => npc.NpcWindow == NpcWindow.ChaosMachine || npc.ItemCraftings.Count > 0)
                .OrderBy(npc => this.LocalizedName(npc.Designation))
                .ThenBy(npc => npc.Number)
                .ToList();
            this._items = this._gameConfiguration.Items
                .OrderBy(item => this.LocalizedName(item.Name))
                .ThenBy(item => item.Group)
                .ThenBy(item => item.Number)
                .ToList();
            this._optionTypes = this._gameConfiguration.ItemOptionTypes.OrderBy(option => this.LocalizedName(option.Name)).ToList();
            this._requirementDraft = new RequirementForm();
            this._resultDraft = new ResultForm();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, "Couldn't load game configuration for custom crafting editor.");
        }
        finally
        {
            this._loading = false;
        }
    }

    private bool IsStackable(ItemDefinition item) => item.ItemSlot is null && item.Durability > 1;

    private string ItemCaption(ItemDefinition item) => $"{this.LocalizedName(item.Name)} ({item.Group}, {item.Number})";

    private string CraftingSuccessRate(ItemCrafting crafting)
        => crafting.SimpleCraftingSettings is { } settings
            ? string.Format(CultureInfo.CurrentCulture, "{0}% (max. {1}%)", settings.SuccessPercent, settings.MaximumSuccessPercent)
            : "—";

    private bool CanDeleteRecipe(ItemCrafting crafting)
        => crafting.ItemCraftingHandlerClassName == typeof(SimpleItemCraftingHandler).FullName;

    private string CraftingRequirements(ItemCrafting crafting)
        => crafting.SimpleCraftingSettings is { } settings
            ? string.Join(", ", settings.RequiredItems.Select(requirement =>
                $"{this.RequiredAmountSummary(requirement)} × {string.Join(" / ", requirement.PossibleItems.Select(item => this.LocalizedName(item.Name)).DefaultIfEmpty(this.Text("CustomCraftingsRandomItem")))}"))
            : "—";

    private string CraftingResults(ItemCrafting crafting)
        => crafting.SimpleCraftingSettings is { } settings
            ? string.Join(", ", settings.ResultItems.Select(result => result.ItemDefinition is { } item ? this.LocalizedName(item.Name) : string.Format(CultureInfo.CurrentCulture, this.Text("CustomCraftingsReferencedResult"), result.Reference)))
            : "—";

    private string RequirementItemName(RequirementForm requirement)
        => Guid.TryParse(requirement.ItemId, out var itemId) && this._items.FirstOrDefault(item => item.GetId() == itemId) is { } item
            ? this.LocalizedName(item.Name)
            : this.Text("CustomCraftingsSelectItem");

    private string RequirementOptions(RequirementForm requirement)
        => string.Join(", ", requirement.OptionTypeIds
            .Select(id => Guid.TryParse(id, out var optionId) ? this._optionTypes.FirstOrDefault(option => option.GetId() == optionId) : null)
            .Where(option => option is not null)
            .Select(option => this.LocalizedName(option!.Name))
            .DefaultIfEmpty("—"));

    private string ResultItemName(ResultForm result)
        => this.FindItem(result.ItemId) is { } item
            ? this.LocalizedName(item.Name)
            : this.Text("CustomCraftingsSelectItem");

    private string ResultQuantityOrDurabilitySummary(ResultForm result)
    {
        if (result.Durability is not { } value)
        {
            var item = this.FindItem(result.ItemId);
            return item is not null && this.IsStackable(item)
                ? this.Text("CustomCraftingsDefaultStackQuantity")
                : item?.ItemSlot is not null ? this.Text("CustomCraftingsDefaultDurability") : this.Text("CustomCraftingsSingleItem");
        }

        var resultItem = this.FindItem(result.ItemId);
        return resultItem is not null && this.IsStackable(resultItem)
            ? string.Format(CultureInfo.CurrentCulture, this.Text("CustomCraftingsStackQuantitySummary"), value)
            : resultItem?.ItemSlot is not null
                ? string.Format(CultureInfo.CurrentCulture, this.Text("CustomCraftingsDurabilitySummary"), value)
                : this.Text("CustomCraftingsSingleItem");
    }

    private string RequiredAmountSummary(ItemCraftingRequiredItem requirement)
        => this.RequiredAmountSummary(requirement.MinimumAmount, requirement.MaximumAmount, requirement.PossibleItems.FirstOrDefault());

    private string RequiredAmountSummary(RequirementForm requirement)
        => this.RequiredAmountSummary(requirement.MinimumAmount, requirement.MaximumAmount, this.FindItem(requirement.ItemId));

    private string RequiredAmountSummary(int minimumAmount, int maximumAmount, ItemDefinition? item)
    {
        var summaryKey = item is not null && this.IsStackable(item)
            ? "CustomCraftingsStackAmountSummary"
            : "CustomCraftingsItemCountSummary";
        var summary = string.Format(CultureInfo.CurrentCulture, this.Text(summaryKey), minimumAmount, maximumAmount);
        return summary;
    }

    private ItemDefinition? FindItem(string? itemId)
        => Guid.TryParse(itemId, out var id) ? this._items.FirstOrDefault(item => item.GetId() == id) : null;

    private string LocalizedName(LocalizedString name)
        => name.GetTranslation(CultureInfo.CurrentUICulture) ?? name.ValueInNeutralLanguage;

    private string Text(string resourceKey) => Resources.ResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture) ?? resourceKey;

    private async Task ResetRecipePageAsync()
    {
        await this._pagination.SetCurrentPageIndexAsync(0).ConfigureAwait(true);
    }

    private async Task ResetAllRecipesPageAsync()
    {
        await this._allRecipesPagination.SetCurrentPageIndexAsync(0).ConfigureAwait(true);
    }

    private async Task DeleteRecipeAsync(MonsterDefinition npc, ItemCrafting crafting)
    {
        if (this._context is not { } context || !this.CanDeleteRecipe(crafting))
        {
            return;
        }

        var confirmation = string.Format(
            CultureInfo.CurrentCulture,
            this.Text("CustomCraftingsDeleteConfirmation"),
            this.LocalizedName(crafting.Name),
            this.LocalizedName(npc.Designation));
        if (!await this.JavaScript.InvokeAsync<bool>("window.confirm", confirmation).ConfigureAwait(true))
        {
            return;
        }

        npc.ItemCraftings.Remove(crafting);
        try
        {
            await context.DeleteAsync(crafting).ConfigureAwait(true);
            await context.SaveChangesAsync().ConfigureAwait(true);
            this.ToastService.ShowSuccess(this.Text("CustomCraftingsDeleted"));
            await this.ResetRecipePageAsync().ConfigureAwait(true);
            await this.ResetAllRecipesPageAsync().ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, "Couldn't delete crafting recipe {RecipeName} from NPC {NpcName}.", crafting.Name, npc.Designation);
            this.ToastService.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.UnexpectedErrorOccurred, exception.Message));
            await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
            this._context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
            this._gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);
            this._npcs = this._gameConfiguration.Monsters
                .Where(candidate => candidate.NpcWindow == NpcWindow.ChaosMachine || candidate.ItemCraftings.Count > 0)
                .OrderBy(candidate => this.LocalizedName(candidate.Designation))
                .ThenBy(candidate => candidate.Number)
                .ToList();
        }
    }

    private void AddRequirement()
    {
        if (!Guid.TryParse(this._requirementDraft.ItemId, out _)
            || this._requirementDraft.MinimumAmount is < 1 or > byte.MaxValue
            || this._requirementDraft.MaximumAmount is < 1 or > byte.MaxValue
            || this._requirementDraft.MinimumAmount > this._requirementDraft.MaximumAmount
            || this._requirementDraft.MinimumLevel is < 0 or > 15
            || this._requirementDraft.MaximumLevel is < 0 or > 15
            || this._requirementDraft.MinimumLevel > this._requirementDraft.MaximumLevel)
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsRequirementInvalid"));
            return;
        }

        this._form.Requirements.Add(this._requirementDraft);
        this._requirementDraft = new RequirementForm();
    }

    private void RemoveRequirement(RequirementForm requirement) => this._form.Requirements.Remove(requirement);

    private void AddResult()
    {
        if (this.FindItem(this._resultDraft.ItemId) is not { } item
            || this._resultDraft.MinimumLevel is < 0 or > 15
            || this._resultDraft.MaximumLevel is < 0 or > 15
            || this._resultDraft.MinimumLevel > this._resultDraft.MaximumLevel
            || !this.IsResultQuantityOrDurabilityValid(this._resultDraft, item))
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsResultInvalid"));
            return;
        }

        this._form.Results.Add(this._resultDraft);
        this._resultDraft = new ResultForm();
    }

    private void RemoveResult(ResultForm result) => this._form.Results.Remove(result);

    private async Task OnSubmitAsync(EditContext editContext)
    {
        if (!editContext.Validate())
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsFormInvalid"));
            return;
        }

        await this.SaveAsync().ConfigureAwait(true);
    }

    private bool IsResultQuantityOrDurabilityValid(ResultForm result, ItemDefinition item)
    {
        if (result.Durability is not { } value)
        {
            return true;
        }

        return value >= (this.IsStackable(item) ? 1 : 0)
            && value <= (this.IsStackable(item) ? item.Durability : byte.MaxValue)
            && (this.IsStackable(item) || item.ItemSlot is not null);
    }

    private void OnResultItemChanged()
    {
        if (this.SelectedResultItem is { } item && !this.IsStackable(item) && item.ItemSlot is null)
        {
            this._resultDraft.Durability = null;
        }
    }

    private async Task SaveAsync()
    {
        if (this._context is not { } context || this._gameConfiguration is not { } gameConfiguration)
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsLoadFailed"));
            return;
        }

        if (!Guid.TryParse(this._selectedNpcId, out var npcId)
            || gameConfiguration.Monsters.FirstOrDefault(npc => npc.GetId() == npcId) is not { } npc)
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsSelectNpc"));
            return;
        }

        if (string.IsNullOrWhiteSpace(this._form.Name)
            || this._form.SuccessPercent is < 0 or > 100
            || this._form.MaximumSuccessPercent is < 0 or > 100
            || this._form.Money < 0
            || this._form.MoneyPerFinalSuccessPercentage < 0
            || this._form.Requirements.Count == 0
            || this._form.Results.Count == 0
            || this._form.Requirements.Any(requirement => !Guid.TryParse(requirement.ItemId, out _) || requirement.MinimumAmount is < 1 or > byte.MaxValue || requirement.MaximumAmount is < 1 or > byte.MaxValue || requirement.MinimumAmount > requirement.MaximumAmount || requirement.MinimumLevel is < 0 or > 15 || requirement.MaximumLevel is < 0 or > 15 || requirement.MinimumLevel > requirement.MaximumLevel || requirement.OptionTypeIds.Any(id => !Guid.TryParse(id, out _)))
            || this._form.Results.Any(result => this.FindItem(result.ItemId) is not { } item || result.MinimumLevel is < 0 or > 15 || result.MaximumLevel is < 0 or > 15 || result.MinimumLevel > result.MaximumLevel || !this.IsResultQuantityOrDurabilityValid(result, item)))
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsValidationFailed"));
            return;
        }

        var usedNumbers = npc.ItemCraftings.Select(crafting => crafting.Number).ToHashSet();
        var number = Enumerable.Range(1, byte.MaxValue).Select(value => (byte)value).FirstOrDefault(candidate => !usedNumbers.Contains(candidate));
        if (number == 0)
        {
            this.ToastService.ShowError(this.Text("CustomCraftingsNoAvailableNumber"));
            return;
        }

        this._saving = true;
        try
        {
            var crafting = context.CreateNew<ItemCrafting>();
            crafting.Number = number;
            crafting.Name = new LocalizedString(this._form.Name.Trim());
            crafting.ItemCraftingHandlerClassName = typeof(SimpleItemCraftingHandler).FullName!;
            crafting.SimpleCraftingSettings = context.CreateNew<SimpleCraftingSettings>();
            var settings = crafting.SimpleCraftingSettings;
            settings.SuccessPercent = (byte)this._form.SuccessPercent;
            settings.MaximumSuccessPercent = (byte)this._form.MaximumSuccessPercent;
            settings.Money = this._form.Money;
            settings.MoneyPerFinalSuccessPercentage = this._form.MoneyPerFinalSuccessPercentage;
            settings.NpcPriceDivisor = 0;
            settings.ResultItemSelect = this._form.ResultItemSelection;

            foreach (var requirementForm in this._form.Requirements)
            {
                var item = gameConfiguration.Items.First(definition => definition.GetId() == Guid.Parse(requirementForm.ItemId!));
                var requirement = context.CreateNew<ItemCraftingRequiredItem>();
                requirement.PossibleItems.Add(item);
                requirement.MinimumAmount = (byte)requirementForm.MinimumAmount;
                requirement.MaximumAmount = (byte)requirementForm.MaximumAmount;
                requirement.MinimumItemLevel = (byte)requirementForm.MinimumLevel;
                requirement.MaximumItemLevel = (byte)requirementForm.MaximumLevel;
                requirement.SuccessResult = requirementForm.SuccessResult;
                requirement.FailResult = requirementForm.FailResult;
                foreach (var optionTypeId in requirementForm.OptionTypeIds)
                {
                    requirement.RequiredItemOptions.Add(gameConfiguration.ItemOptionTypes.First(option => option.GetId() == Guid.Parse(optionTypeId)));
                }

                settings.RequiredItems.Add(requirement);
            }

            foreach (var resultForm in this._form.Results)
            {
                var item = gameConfiguration.Items.First(definition => definition.GetId() == Guid.Parse(resultForm.ItemId!));
                var result = context.CreateNew<ItemCraftingResultItem>();
                result.ItemDefinition = item;
                result.RandomMinimumLevel = (byte)resultForm.MinimumLevel;
                result.RandomMaximumLevel = (byte)resultForm.MaximumLevel;
                result.Durability = (this.IsStackable(item) || item.ItemSlot is not null) && resultForm.Durability is { } durability ? (byte)durability : null;
                settings.ResultItems.Add(result);
            }

            npc.ItemCraftings.Add(crafting);
            await context.SaveChangesAsync().ConfigureAwait(true);
            this.ToastService.ShowSuccess(string.Format(CultureInfo.CurrentCulture, this.Text("CustomCraftingsCreated"), this._form.Name, number));
            this.ResetForm();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, "Couldn't create custom crafting recipe.");
            this.ToastService.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.UnexpectedErrorOccurred, exception.Message));
        }
        finally
        {
            this._saving = false;
        }
    }

    private async Task DiscardAsync()
    {
        await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        this._context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
        this._gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);
        this.ResetForm();
    }

    private void ResetForm()
    {
        this._form.Name = string.Empty;
        this._form.SuccessPercent = 100;
        this._form.MaximumSuccessPercent = 100;
        this._form.ResultItemSelection = ResultItemSelection.Any;
        this._form.Money = 0;
        this._form.MoneyPerFinalSuccessPercentage = 0;
        this._form.Requirements.Clear();
        this._requirementDraft = new RequirementForm();
        this._form.Results.Clear();
        this._resultDraft = new ResultForm();
    }

    private sealed class RecipeForm
    {
        public string Name { get; set; } = string.Empty;

        public int SuccessPercent { get; set; } = 100;

        public int MaximumSuccessPercent { get; set; } = 100;

        public int Money { get; set; }

        public int MoneyPerFinalSuccessPercentage { get; set; }

        public ResultItemSelection ResultItemSelection { get; set; } = ResultItemSelection.Any;

        public List<RequirementForm> Requirements { get; } = [];

        public List<ResultForm> Results { get; } = [];
    }

    private sealed record RecipeListEntry(MonsterDefinition Npc, ItemCrafting Crafting);

    private sealed class RequirementForm
    {
        public string? ItemId { get; set; }

        public int MinimumAmount { get; set; } = 1;

        public int MaximumAmount { get; set; } = 1;

        public int MinimumLevel { get; set; }

        public int MaximumLevel { get; set; }

        public MixResult SuccessResult { get; set; } = MixResult.Disappear;

        public MixResult FailResult { get; set; } = MixResult.Disappear;

        public string[] OptionTypeIds { get; set; } = [];
    }

    private sealed class ResultForm
    {
        public string? ItemId { get; set; }

        public int MinimumLevel { get; set; }

        public int MaximumLevel { get; set; }

        public int? Durability { get; set; }
    }
}
