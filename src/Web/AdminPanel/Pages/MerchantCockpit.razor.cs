// <copyright file="MerchantCockpit.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Practical merchant editor for MU Nueva Era.
/// </summary>
public partial class MerchantCockpit : ComponentBase
{
    private const int MerchantGridWidth = 8;
    private const int MerchantGridHeight = 15;
    private const int MerchantGridSize = MerchantGridWidth * MerchantGridHeight;

    private readonly ItemPriceCalculator _priceCalculator = new();
    private readonly HashSet<Guid> _selectedCatalogIds = [];
    private readonly HashSet<Guid> _cloneTargetIds = [];

    private IContext? _context;
    private GameConfiguration? _gameConfiguration;
    private List<MonsterDefinition> _merchants = [];
    private List<ItemDefinition> _items = [];
    private List<GameMapDefinition> _maps = [];

    private Guid? _selectedMerchantId;
    private Guid? _selectedShopItemId;
    private string _merchantFilter = string.Empty;
    private string _shopFilter = string.Empty;
    private string _catalogFilter = string.Empty;
    private int? _catalogGroup;

    private int _newItemLevel;
    private int _newItemSocketCount;
    private bool _newItemHasSkill;

    private int _editLevel;
    private double _editDurability;
    private int _editSocketCount;
    private bool _editHasSkill;

    private Guid? _optionToAddId;
    private int _optionLevel;

    private string _cloneMode = "replace";
    private bool _skipCloneDuplicates = true;
    private List<string> _clonePreviewLines = [];

    private bool _isLoading = true;
    private bool _isSaving;
    private string _message = string.Empty;
    private string _messageCss = "alert-info";

    /// <summary>
    /// Gets or sets the game configuration data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the loading overlay.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<MerchantCockpit> Logger { get; set; } = null!;

    private bool HasPendingChanges => this._context?.HasChanges is true;

    private MonsterDefinition? SelectedMerchant
        => this._selectedMerchantId.HasValue
            ? this._merchants.FirstOrDefault(m => m.GetId() == this._selectedMerchantId.Value)
            : null;

    private Item? SelectedShopItem
        => this.SelectedMerchant?.MerchantStore?.Items.FirstOrDefault(i => i.GetId() == this._selectedShopItemId);

    private IEnumerable<MonsterDefinition> FilteredMerchants
        => this._merchants
            .Where(m => string.IsNullOrWhiteSpace(this._merchantFilter)
                        || (m.Designation.ToString() ?? string.Empty).Contains(this._merchantFilter, StringComparison.OrdinalIgnoreCase));

    private IEnumerable<Item> FilteredShopItems
        => (this.SelectedMerchant?.MerchantStore?.Items ?? [])
            .Where(i => string.IsNullOrWhiteSpace(this._shopFilter)
                        || (i.Definition?.Name.ToString() ?? string.Empty).Contains(this._shopFilter, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.ItemSlot)
            .ThenBy(i => i.Definition?.Group)
            .ThenBy(i => i.Definition?.Number);

    private IEnumerable<ItemDefinition> FilteredCatalog
        => this._items
            .Where(i => string.IsNullOrWhiteSpace(this._catalogFilter)
                        || (i.Name.ToString() ?? string.Empty).Contains(this._catalogFilter, StringComparison.OrdinalIgnoreCase))
            .Where(i => !this._catalogGroup.HasValue || i.Group == this._catalogGroup.Value)
            .Take(300);

    private IEnumerable<MonsterDefinition> FilteredCloneTargets
        => this._merchants.Where(m => m.GetId() != this._selectedMerchantId);

    private IEnumerable<IncreasableItemOption> AvailableOptionsForSelectedItem
    {
        get
        {
            var item = this.SelectedShopItem;
            if (item?.Definition is null)
            {
                return [];
            }

            var existing = item.ItemOptions
                .Where(o => o.ItemOption is not null)
                .Select(o => o.ItemOption!.GetId())
                .ToHashSet();

            return item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => !existing.Contains(o.GetId()))
                .OrderBy(o => o.OptionType?.ToString())
                .ThenBy(o => o.ToString());
        }
    }

    private bool SelectedItemHasLuck
        => this.SelectedShopItem?.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck) is true;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await this.LoadAsync().ConfigureAwait(true);
    }

    private async Task LoadAsync()
    {
        using var loading = this.LoadingService.ShowLoadingIndicator();
        this._isLoading = true;

        try
        {
            this._context = await this.DataSource.GetContextAsync().ConfigureAwait(true);
            this._gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);

            this._merchants = this.DataSource.GetAll<MonsterDefinition>()
                .Where(m => m is { ObjectKind: NpcObjectKind.PassiveNpc, MerchantStore: not null })
                .OrderBy(m => m.Designation.ToString())
                .ToList();

            this._items = this.DataSource.GetAll<ItemDefinition>()
                .OrderBy(i => i.Group)
                .ThenBy(i => i.Number)
                .ToList();

            this._maps = this.DataSource.GetAll<GameMapDefinition>()
                .OrderBy(m => m.Number)
                .ToList();

            if (!this._selectedMerchantId.HasValue
                || this._merchants.All(m => m.GetId() != this._selectedMerchantId.Value))
            {
                this._selectedMerchantId = this._merchants.FirstOrDefault()?.GetId();
            }

            this._selectedShopItemId = null;
            this._selectedCatalogIds.Clear();
            this._cloneTargetIds.Clear();
            this._clonePreviewLines.Clear();
            this.ClearMessage();
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to load Merchant Cockpit.");
            this.SetMessage($"No se pudo cargar Merchant Cockpit: {ex.Message}", "alert-danger");
        }
        finally
        {
            this._isLoading = false;
        }
    }

    private void SelectMerchant(Guid id)
    {
        this._selectedMerchantId = id;
        this._selectedShopItemId = null;
        this._cloneTargetIds.Remove(id);
        this._clonePreviewLines.Clear();
        this.ClearMessage();
    }

    private void SelectShopItem(Guid id)
    {
        this._selectedShopItemId = id;
        if (this.SelectedShopItem is { } item)
        {
            this._editLevel = item.Level;
            this._editDurability = item.Durability;
            this._editSocketCount = item.SocketCount;
            this._editHasSkill = item.HasSkill;
            this._optionToAddId = null;
            this._optionLevel = 0;
        }
    }

    private void ToggleCatalog(Guid id, bool selected)
    {
        if (selected)
        {
            this._selectedCatalogIds.Add(id);
        }
        else
        {
            this._selectedCatalogIds.Remove(id);
        }
    }

    private void SelectAllVisibleCatalog(bool selected)
    {
        if (!selected)
        {
            this._selectedCatalogIds.Clear();
            return;
        }

        foreach (var definition in this.FilteredCatalog)
        {
            this._selectedCatalogIds.Add(definition.GetId());
        }
    }

    private void ToggleCloneTarget(Guid id, bool selected)
    {
        if (selected)
        {
            this._cloneTargetIds.Add(id);
        }
        else
        {
            this._cloneTargetIds.Remove(id);
        }
    }

    private void AddSelectedCatalogItems()
    {
        var merchant = this.SelectedMerchant;
        if (merchant?.MerchantStore is null || this._context is null)
        {
            return;
        }

        var definitions = this._items
            .Where(i => this._selectedCatalogIds.Contains(i.GetId()))
            .ToList();

        if (definitions.Count == 0)
        {
            return;
        }

        var added = 0;
        foreach (var definition in definitions)
        {
            var freeSlot = FindFirstFreeSlot(merchant.MerchantStore.Items, definition);
            if (!freeSlot.HasValue)
            {
                this.SetMessage(
                    $"La tienda se quedó sin espacio después de agregar {added} item(s). "
                    + $"{definition.Name} y los siguientes no fueron agregados.",
                    "alert-warning");
                break;
            }

            var item = this.CreateMerchantItem(definition, freeSlot.Value);
            merchant.MerchantStore.Items.Add(item);
            added++;
        }

        if (added > 0)
        {
            this.SetMessage(
                $"{added} item(s) agregados a {merchant.Designation}. Revisa la tienda y guarda cuando estés conforme.",
                "alert-success");
        }

        this._selectedCatalogIds.Clear();
    }

    private Item CreateMerchantItem(ItemDefinition definition, byte slot)
    {
        if (this._context is null)
        {
            throw new InvalidOperationException("Persistence context not initialized.");
        }

        var item = this._context.CreateNew<Item>();
        item.Definition = definition;
        item.ItemSlot = slot;
        item.Level = (byte)Math.Clamp(this._newItemLevel, 0, definition.MaximumItemLevel);
        item.Durability = definition.Durability;
        item.HasSkill = this._newItemHasSkill && definition.Skill is not null;
        item.SocketCount = Math.Clamp(this._newItemSocketCount, 0, definition.MaximumSockets);
        return item;
    }

    private async Task RemoveShopItemAsync(Item item)
    {
        var merchant = this.SelectedMerchant;
        if (merchant?.MerchantStore is null || this._context is null)
        {
            return;
        }

        merchant.MerchantStore.Items.Remove(item);
        await this._context.DeleteAsync(item).ConfigureAwait(true);

        if (this._selectedShopItemId == item.GetId())
        {
            this._selectedShopItemId = null;
        }

        this.SetMessage($"{item.Definition?.Name} quitado de {merchant.Designation}. Cambio aún sin guardar.", "alert-warning");
    }

    private async Task ClearCurrentMerchantAsync()
    {
        var merchant = this.SelectedMerchant;
        if (merchant?.MerchantStore is null || this._context is null)
        {
            return;
        }

        var count = merchant.MerchantStore.Items.Count;
        foreach (var item in merchant.MerchantStore.Items.ToList())
        {
            merchant.MerchantStore.Items.Remove(item);
            await this._context.DeleteAsync(item).ConfigureAwait(true);
        }

        this._selectedShopItemId = null;
        this.SetMessage($"{merchant.Designation}: {count} item(s) preparados para eliminación. Pulsa Guardar para confirmar.", "alert-warning");
    }

    private void ApplyQuickItemEdit()
    {
        var item = this.SelectedShopItem;
        if (item?.Definition is null)
        {
            return;
        }

        item.Level = (byte)Math.Clamp(this._editLevel, 0, item.Definition.MaximumItemLevel);
        item.Durability = Math.Max(0, this._editDurability);
        item.SocketCount = Math.Clamp(this._editSocketCount, 0, item.Definition.MaximumSockets);
        item.HasSkill = this._editHasSkill && item.Definition.Skill is not null;

        this._editLevel = item.Level;
        this._editDurability = item.Durability;
        this._editSocketCount = item.SocketCount;
        this._editHasSkill = item.HasSkill;

        this.SetMessage(
            $"{item.Definition.Name} actualizado. Precio calculado actual: {this.FormatZen(this.GetBuyingPrice(item))}.",
            "alert-success");
    }

    private async Task ToggleLuckAsync()
    {
        var item = this.SelectedShopItem;
        if (item?.Definition is null || this._context is null)
        {
            return;
        }

        var existing = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck);
        if (existing is not null)
        {
            item.ItemOptions.Remove(existing);
            await this._context.DeleteAsync(existing).ConfigureAwait(true);
            this.SetMessage($"Luck quitado de {item.Definition.Name}.", "alert-warning");
            return;
        }

        var luck = item.Definition.PossibleItemOptions
            .SelectMany(o => o.PossibleOptions)
            .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck);

        if (luck is null)
        {
            this.SetMessage($"{item.Definition.Name} no tiene una opción Luck válida en su definición.", "alert-warning");
            return;
        }

        var link = this._context.CreateNew<ItemOptionLink>();
        link.ItemOption = luck;
        link.Level = 0;
        link.Index = this.GetNextOptionIndex(item);
        item.ItemOptions.Add(link);
        this.SetMessage($"Luck agregado a {item.Definition.Name}.", "alert-success");
    }

    private async Task RemoveOptionAsync(ItemOptionLink option)
    {
        var item = this.SelectedShopItem;
        if (item is null || this._context is null)
        {
            return;
        }

        item.ItemOptions.Remove(option);
        await this._context.DeleteAsync(option).ConfigureAwait(true);
        this.SetMessage("Opción quitada. Cambio aún sin guardar.", "alert-warning");
    }

    private void AddSelectedOption()
    {
        var item = this.SelectedShopItem;
        if (item?.Definition is null || this._context is null || !this._optionToAddId.HasValue)
        {
            return;
        }

        var option = item.Definition.PossibleItemOptions
            .SelectMany(o => o.PossibleOptions)
            .FirstOrDefault(o => o.GetId() == this._optionToAddId.Value);

        if (option is null)
        {
            this.SetMessage("La opción seleccionada ya no está disponible para este item.", "alert-warning");
            return;
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.GetId() == option.GetId()))
        {
            this.SetMessage("Ese tipo de opción ya está aplicado al item.", "alert-warning");
            return;
        }

        var link = this._context.CreateNew<ItemOptionLink>();
        link.ItemOption = option;
        link.Level = Math.Max(0, this._optionLevel);
        link.Index = this.GetNextOptionIndex(item);
        item.ItemOptions.Add(link);

        this._optionToAddId = null;
        this._optionLevel = 0;
        this.SetMessage($"Opción agregada a {item.Definition.Name}.", "alert-success");
    }

    private int GetNextOptionIndex(Item item)
        => item.ItemOptions.Count == 0 ? 0 : item.ItemOptions.Max(o => o.Index) + 1;

    private void AutoArrangeCurrentMerchant()
    {
        var merchant = this.SelectedMerchant;
        if (merchant?.MerchantStore is null)
        {
            return;
        }

        var items = merchant.MerchantStore.Items
            .OrderBy(i => i.Definition?.Group)
            .ThenBy(i => i.Definition?.Number)
            .ThenBy(i => i.Level)
            .ToList();

        if (!TryPack(items, out var placements))
        {
            this.SetMessage(
                $"No es posible ordenar {merchant.Designation}: los items no caben en la grilla {MerchantGridWidth}×{MerchantGridHeight}.",
                "alert-danger");
            return;
        }

        foreach (var pair in placements)
        {
            pair.Key.ItemSlot = pair.Value;
        }

        this.SetMessage($"{merchant.Designation}: slots reordenados automáticamente.", "alert-success");
    }

    private void PreviewClone()
    {
        var source = this.SelectedMerchant;
        if (source?.MerchantStore is null)
        {
            this._clonePreviewLines = [];
            return;
        }

        var sourceSnapshots = source.MerchantStore.Items.Select(CreateSnapshot).ToList();
        var lines = new List<string>
        {
            $"Origen: {source.Designation} · {sourceSnapshots.Count} items · modo {this._cloneMode}.",
        };

        foreach (var target in this.GetCloneTargets())
        {
            if (this._cloneMode == "replace")
            {
                lines.Add($"{target.Designation}: {target.MerchantStore!.Items.Count} → {sourceSnapshots.Count} items.");
                continue;
            }

            var append = this.GetSnapshotsToAppend(target, sourceSnapshots).ToList();
            var canFit = CanAppend(target.MerchantStore!.Items, append);
            lines.Add(
                $"{target.Designation}: {target.MerchantStore.Items.Count} + {append.Count} "
                + $"→ {(canFit ? target.MerchantStore.Items.Count + append.Count : "SIN ESPACIO")}.");
        }

        this._clonePreviewLines = lines;
    }

    private async Task StageCloneAsync()
    {
        var source = this.SelectedMerchant;
        if (source?.MerchantStore is null || this._context is null)
        {
            return;
        }

        var targets = this.GetCloneTargets().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var sourceSnapshots = source.MerchantStore.Items.Select(CreateSnapshot).ToList();

        if (this._cloneMode == "append")
        {
            foreach (var target in targets)
            {
                var append = this.GetSnapshotsToAppend(target, sourceSnapshots).ToList();
                if (!CanAppend(target.MerchantStore!.Items, append))
                {
                    this.SetMessage(
                        $"No se preparó la copia: {target.Designation} no tiene espacio suficiente. "
                        + "Usa Reemplazar, reduce la plantilla o quita items.",
                        "alert-danger");
                    return;
                }
            }
        }

        foreach (var target in targets)
        {
            if (this._cloneMode == "replace")
            {
                foreach (var oldItem in target.MerchantStore!.Items.ToList())
                {
                    target.MerchantStore.Items.Remove(oldItem);
                    await this._context.DeleteAsync(oldItem).ConfigureAwait(true);
                }

                foreach (var snapshot in sourceSnapshots)
                {
                    target.MerchantStore.Items.Add(this.CreateFromSnapshot(snapshot, snapshot.ItemSlot));
                }

                continue;
            }

            foreach (var snapshot in this.GetSnapshotsToAppend(target, sourceSnapshots))
            {
                var slot = FindFirstFreeSlot(target.MerchantStore!.Items, snapshot.Definition);
                if (!slot.HasValue)
                {
                    throw new InvalidOperationException($"Unexpected shop capacity failure on {target.Designation}.");
                }

                target.MerchantStore.Items.Add(this.CreateFromSnapshot(snapshot, slot.Value));
            }
        }

        this.SetMessage(
            $"Copia preparada para {targets.Count} merchant(s). Revisa los destinos y pulsa Guardar cambios para persistir.",
            "alert-success");
        this.PreviewClone();
    }

    private IEnumerable<MonsterDefinition> GetCloneTargets()
        => this._merchants
            .Where(m => m.GetId() != this._selectedMerchantId && this._cloneTargetIds.Contains(m.GetId()));

    private IEnumerable<ShopItemSnapshot> GetSnapshotsToAppend(
        MonsterDefinition target,
        IReadOnlyList<ShopItemSnapshot> sourceSnapshots)
    {
        if (!this._skipCloneDuplicates)
        {
            return sourceSnapshots;
        }

        return sourceSnapshots.Where(snapshot =>
            !target.MerchantStore!.Items.Any(item =>
                item.Definition?.GetId() == snapshot.Definition.GetId()
                && item.Level == snapshot.Level));
    }

    private async Task SaveAsync()
    {
        if (this._context is null || !this.HasPendingChanges)
        {
            return;
        }

        this._isSaving = true;
        try
        {
            var saved = await this._context.SaveChangesAsync().ConfigureAwait(true);
            if (!saved)
            {
                this.ToastService.ShowError("OpenMU no informó cambios guardados.");
                return;
            }

            this.ToastService.ShowSuccess("Merchant configuration guardada.");
            var selectedMerchantId = this._selectedMerchantId;
            await this.DataSource.ForceDiscardChangesAsync().ConfigureAwait(true);
            this._selectedMerchantId = selectedMerchantId;
            await this.LoadAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to save Merchant Cockpit changes.");
            this.SetMessage($"Error al guardar merchants: {ex.Message}", "alert-danger");
        }
        finally
        {
            this._isSaving = false;
        }
    }

    private async Task DiscardAsync()
    {
        var selectedMerchantId = this._selectedMerchantId;
        await this.DataSource.DiscardChangesAsync().ConfigureAwait(true);
        this._selectedMerchantId = selectedMerchantId;
        await this.LoadAsync().ConfigureAwait(true);
        this.ToastService.ShowSuccess("Cambios de merchants descartados.");
    }

    private string GetMerchantMapsText(MonsterDefinition merchant)
    {
        var names = this._maps
            .Where(map => map.MonsterSpawns.Any(spawn => spawn.MonsterDefinition?.GetId() == merchant.GetId()))
            .Select(map => map.Name.ToString())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        return names.Count == 0 ? "sin mapa detectado" : string.Join(", ", names);
    }

    private long GetBuyingPrice(Item item)
    {
        try
        {
            return this._priceCalculator.CalculateFinalBuyingPrice(item);
        }
        catch
        {
            return 0;
        }
    }

    private string FormatZen(long amount)
        => $"{amount:N0} Zen";

    private static byte? FindFirstFreeSlot(IEnumerable<Item> existingItems, ItemDefinition definition)
    {
        var occupied = BuildOccupancy(existingItems);
        return FindFirstFreeSlot(occupied, definition);
    }

    private static bool[] BuildOccupancy(IEnumerable<Item> items)
    {
        var occupied = new bool[MerchantGridSize];
        foreach (var item in items)
        {
            if (item.Definition is null)
            {
                continue;
            }

            MarkOccupied(occupied, item.ItemSlot, item.Definition, true);
        }

        return occupied;
    }

    private static byte? FindFirstFreeSlot(bool[] occupied, ItemDefinition definition)
    {
        var width = Math.Max(1, (int)definition.Width);
        var height = Math.Max(1, (int)definition.Height);

        if (width > MerchantGridWidth || height > MerchantGridHeight)
        {
            return null;
        }

        for (var row = 0; row <= MerchantGridHeight - height; row++)
        {
            for (var col = 0; col <= MerchantGridWidth - width; col++)
            {
                var fits = true;
                for (var y = 0; y < height && fits; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var slot = ((row + y) * MerchantGridWidth) + col + x;
                        if (occupied[slot])
                        {
                            fits = false;
                            break;
                        }
                    }
                }

                if (!fits)
                {
                    continue;
                }

                var firstSlot = (row * MerchantGridWidth) + col;
                MarkOccupied(occupied, (byte)firstSlot, definition, true);
                return (byte)firstSlot;
            }
        }

        return null;
    }

    private static void MarkOccupied(bool[] occupied, byte itemSlot, ItemDefinition definition, bool value)
    {
        var startRow = itemSlot / MerchantGridWidth;
        var startCol = itemSlot % MerchantGridWidth;
        var width = Math.Max(1, (int)definition.Width);
        var height = Math.Max(1, (int)definition.Height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var row = startRow + y;
                var col = startCol + x;
                if (row >= MerchantGridHeight || col >= MerchantGridWidth)
                {
                    continue;
                }

                occupied[(row * MerchantGridWidth) + col] = value;
            }
        }
    }

    private static bool TryPack(IReadOnlyList<Item> items, out Dictionary<Item, byte> placements)
    {
        placements = [];
        var occupied = new bool[MerchantGridSize];

        foreach (var item in items)
        {
            if (item.Definition is null)
            {
                return false;
            }

            var slot = FindFirstFreeSlot(occupied, item.Definition);
            if (!slot.HasValue)
            {
                return false;
            }

            placements[item] = slot.Value;
        }

        return true;
    }

    private static bool CanAppend(IEnumerable<Item> currentItems, IReadOnlyList<ShopItemSnapshot> snapshots)
    {
        var occupied = BuildOccupancy(currentItems);
        foreach (var snapshot in snapshots)
        {
            if (!FindFirstFreeSlot(occupied, snapshot.Definition).HasValue)
            {
                return false;
            }
        }

        return true;
    }

    private static ShopItemSnapshot CreateSnapshot(Item item)
        => new(
            item.ItemSlot,
            item.Definition ?? throw new InvalidOperationException("Shop item without definition."),
            item.Durability,
            item.Level,
            item.HasSkill,
            item.SocketCount,
            item.StorePrice,
            item.PetExperience,
            item.ItemOptions
                .Where(o => o.ItemOption is not null)
                .Select(o => new ShopOptionSnapshot(o.ItemOption!, o.Level, o.Index))
                .ToList(),
            item.ItemSetGroups.ToList());

    private Item CreateFromSnapshot(ShopItemSnapshot snapshot, byte slot)
    {
        if (this._context is null)
        {
            throw new InvalidOperationException("Persistence context not initialized.");
        }

        var item = this._context.CreateNew<Item>();
        item.ItemSlot = slot;
        item.Definition = snapshot.Definition;
        item.Durability = snapshot.Durability;
        item.Level = snapshot.Level;
        item.HasSkill = snapshot.HasSkill;
        item.SocketCount = snapshot.SocketCount;
        item.StorePrice = snapshot.StorePrice;
        item.PetExperience = snapshot.PetExperience;

        foreach (var optionSnapshot in snapshot.Options)
        {
            var link = this._context.CreateNew<ItemOptionLink>();
            link.ItemOption = optionSnapshot.ItemOption;
            link.Level = optionSnapshot.Level;
            link.Index = optionSnapshot.Index;
            item.ItemOptions.Add(link);
        }

        foreach (var setGroup in snapshot.ItemSetGroups)
        {
            item.ItemSetGroups.Add(setGroup);
        }

        return item;
    }

    private void SetMessage(string text, string css)
    {
        this._message = text;
        this._messageCss = css;
    }

    private void ClearMessage()
    {
        this._message = string.Empty;
        this._messageCss = "alert-info";
    }

    private sealed record ShopOptionSnapshot(IncreasableItemOption ItemOption, int Level, int Index);

    private sealed record ShopItemSnapshot(
        byte ItemSlot,
        ItemDefinition Definition,
        double Durability,
        byte Level,
        bool HasSkill,
        int SocketCount,
        int? StorePrice,
        int PetExperience,
        IReadOnlyList<ShopOptionSnapshot> Options,
        IReadOnlyList<ItemOfItemSet> ItemSetGroups);
}
