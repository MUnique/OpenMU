// <copyright file="MerchantShopExportImportService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

using System.Text.Json;
using System.Threading;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Handles the export and import of a merchant's shop inventory, converting between the shop items and JSON.
/// </summary>
/// <remarks>
/// Like the map editor's spawn export, the file references configuration data by its natural keys
/// (item group and number, option type and number), so that it can be applied to another configuration.
/// The file only contains the items, so it can be imported into any merchant shop.
/// The import replaces all items of the shop; it doesn't touch the money of the storage.
/// <para>
/// The import is all-or-nothing regarding the content of the file: it is validated completely, and all
/// references are resolved, before the shop is touched. If any entry can't be imported, e.g. because its item
/// definition, option or ancient set is unknown, nothing is imported and the problems are reported,
/// so that the file can be adjusted.
/// The items must also fit into the shop (8 columns, 15 rows) without overlapping each other.
/// A file which lacks its format version or its item list is rejected, so that an incomplete file can't
/// be mistaken for an intentionally empty shop. An explicitly empty item list does clear the shop.
/// </para>
/// <para>
/// The persistence context can't undo deletions which were already staged. If the replacement fails
/// after the removal of the old items has started, the exception is propagated after the newly created
/// items have been discarded again, and the caller is expected to discard the unsaved changes of the context.
/// </para>
/// </remarks>
public sealed class MerchantShopExportImportService
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Builds a JSON export string from the shop items of the given merchant.
    /// </summary>
    /// <param name="merchant">The merchant whose shop items are exported.</param>
    /// <returns>A JSON string representing all shop items.</returns>
    public string BuildExport(MonsterDefinition merchant)
    {
        var dto = new MerchantShopExport();

        if (merchant.MerchantStore is { } store)
        {
            foreach (var item in store.Items.OrderBy(item => item.ItemSlot))
            {
                if (CreateItemExport(item) is { } itemExport)
                {
                    dto.Items.Add(itemExport);
                }
            }
        }

        return JsonSerializer.Serialize(dto, SerializerOptions);
    }

    /// <summary>
    /// Replaces the items of the merchant's shop with the items of the given JSON export.
    /// </summary>
    /// <param name="merchant">The merchant whose shop items are replaced.</param>
    /// <param name="json">The JSON string containing the replacement items.</param>
    /// <param name="context">The persistence context used for create and delete operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The outcome of the import, or <see langword="null"/> if the file isn't a valid shop export.
    /// If the file contains entries which can't be imported, the result lists them in
    /// <see cref="MerchantShopImportResult.Problems"/>. In both cases, the shop is left untouched.
    /// </returns>
    public async ValueTask<MerchantShopImportResult?> TryApplyImportAsync(
        MonsterDefinition merchant,
        string json,
        IContext context,
        CancellationToken cancellationToken = default)
    {
        if (ReadExport(json) is not { } dto || merchant.MerchantStore is not { } store)
        {
            return null;
        }

        var definitions = await context.GetAsync<ItemDefinition>(cancellationToken).ConfigureAwait(false);
        var definitionsByKey = definitions
            .GroupBy(definition => (definition.Group, definition.Number))
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(definition => definition.GetId()).First());

        var plannedItems = new List<PlannedItem>();
        var problems = new List<string>();
        var occupiedSlots = new HashSet<int>();
        foreach (var itemExport in dto.Items)
        {
            if (!definitionsByKey.TryGetValue((itemExport.ItemGroup, itemExport.ItemNumber), out var definition))
            {
                problems.Add($"Slot {itemExport.Slot}: unknown item (group {itemExport.ItemGroup}, number {itemExport.ItemNumber}).");
                continue;
            }

            CheckPlacement(itemExport, definition, occupiedSlots, problems);
            plannedItems.Add(PlanItem(itemExport, definition, problems));
        }

        if (problems.Count > 0)
        {
            return new MerchantShopImportResult(0, problems);
        }

        await RemoveItemsAsync(store, context).ConfigureAwait(false);

        var createdItems = new List<Item>();
        try
        {
            foreach (var plannedItem in plannedItems)
            {
                CreateItem(plannedItem, context, createdItems);
            }

            foreach (var item in createdItems)
            {
                store.Items.Add(item);
            }
        }
        catch
        {
            DiscardCreatedItems(createdItems, store, context);
            throw;
        }

        return new MerchantShopImportResult(createdItems.Count, []);
    }

    private static MerchantShopExport? ReadExport(string json)
    {
        try
        {
            // The DTO has defaults for both properties, which is convenient for exporting.
            // For importing, the properties must be explicitly present in the file.
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty(nameof(MerchantShopExport.FormatVersion), out var version)
                || version.ValueKind != JsonValueKind.String
                || version.GetString() != MerchantShopExport.CurrentFormatVersion
                || !root.TryGetProperty(nameof(MerchantShopExport.Items), out var items)
                || items.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var dto = JsonSerializer.Deserialize<MerchantShopExport>(json);
            return dto is not null && IsValid(dto) ? dto : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool IsValid(MerchantShopExport dto)
    {
        if (dto.Items is not { } items)
        {
            return false;
        }

        var usedSlots = new HashSet<byte>();
        foreach (var item in items)
        {
            if (item is null
                || item.Durability < 0
                || item.SocketCount < 0
                || item.PetExperience < 0
                || !usedSlots.Add(item.Slot))
            {
                return false;
            }

            if (item.Options?.Any(option => option is null || option.Level < 0 || option.Index < 0) is true)
            {
                return false;
            }

            if (item.AncientSets?.Any(string.IsNullOrWhiteSpace) is true)
            {
                return false;
            }
        }

        return true;
    }

    private static ShopItemExport? CreateItemExport(Item item)
    {
        if (item.Definition is not { } definition)
        {
            return null;
        }

        var itemExport = new ShopItemExport
        {
            Slot = item.ItemSlot,
            ItemGroup = definition.Group,
            ItemNumber = definition.Number,
            Level = item.Level,
            Durability = item.Durability,
            HasSkill = item.HasSkill,
            SocketCount = item.SocketCount,
            PetExperience = item.PetExperience,
        };

        foreach (var optionLink in item.ItemOptions.OrderBy(link => link.Index))
        {
            if (optionLink.ItemOption is { } option)
            {
                itemExport.Options.Add(new ShopItemOptionExport
                {
                    OptionTypeId = option.OptionType?.Id ?? Guid.Empty,
                    OptionTypeName = option.OptionType?.Name.ValueInNeutralLanguage,
                    Number = option.Number,
                    SubOptionType = option.SubOptionType,
                    Level = optionLink.Level,
                    Index = optionLink.Index,
                });
            }
        }

        foreach (var setLink in item.ItemSetGroups
                     .Where(link => link.AncientSetDiscriminator > 0)
                     .OrderBy(link => link.ItemSetGroup?.Name.ValueInNeutralLanguage))
        {
            if (setLink.ItemSetGroup?.Name.ValueInNeutralLanguage is { } setName)
            {
                itemExport.AncientSets.Add(setName);
            }
        }

        return itemExport;
    }

    private static async ValueTask RemoveItemsAsync(ItemStorage store, IContext context)
    {
        foreach (var item in store.Items.ToList())
        {
            store.Items.Remove(item);
            await context.DeleteAsync(item).ConfigureAwait(false);
        }
    }

    private static List<IncreasableItemOption> GetOptionCandidates(ItemDefinition definition)
    {
        var candidates = definition.PossibleItemOptions
            .SelectMany(optionDefinition => optionDefinition.PossibleOptions)
            .ToList();

        foreach (var itemOfSet in definition.PossibleItemSetGroups.SelectMany(group => group.Items))
        {
            if (itemOfSet.ItemDefinition == definition && itemOfSet.BonusOption is { } bonusOption)
            {
                candidates.Add(bonusOption);
            }
        }

        return candidates;
    }

    private static IncreasableItemOption? FindOption(List<IncreasableItemOption> candidates, ShopItemOptionExport optionExport)
    {
        bool IsSameOption(IncreasableItemOption candidate) =>
            candidate.Number == optionExport.Number
            && candidate.SubOptionType == optionExport.SubOptionType;

        var option = candidates.FirstOrDefault(candidate =>
            IsSameOption(candidate) && candidate.OptionType?.Id == optionExport.OptionTypeId);
        if (option is not null || string.IsNullOrEmpty(optionExport.OptionTypeName))
        {
            return option;
        }

        // The ids of option types differ between configurations, so we fall back to their names.
        return candidates.FirstOrDefault(candidate =>
            IsSameOption(candidate) && candidate.OptionType?.Name.ValueInNeutralLanguage == optionExport.OptionTypeName);
    }

    private static void CheckPlacement(ShopItemExport itemExport, ItemDefinition definition, ISet<int> occupiedSlots, ICollection<string> problems)
    {
        var column = itemExport.Slot % InventoryConstants.RowSize;
        var row = itemExport.Slot / InventoryConstants.RowSize;
        var width = Math.Max((int)definition.Width, 1);
        var height = Math.Max((int)definition.Height, 1);
        if (column + width > InventoryConstants.RowSize || row + height > InventoryConstants.WarehouseRows)
        {
            problems.Add($"Slot {itemExport.Slot}: the item doesn't fit into the shop at this slot.");
            return;
        }

        var overlaps = false;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                overlaps |= !occupiedSlots.Add(((row + y) * InventoryConstants.RowSize) + column + x);
            }
        }

        if (overlaps)
        {
            problems.Add($"Slot {itemExport.Slot}: the item overlaps with another item.");
        }
    }

    private static PlannedItem PlanItem(ShopItemExport itemExport, ItemDefinition definition, ICollection<string> problems)
    {
        var candidates = GetOptionCandidates(definition);
        var options = new List<PlannedOption>();
        foreach (var optionExport in itemExport.Options ?? [])
        {
            if (FindOption(candidates, optionExport) is { } option)
            {
                options.Add(new PlannedOption(option, optionExport));
            }
            else
            {
                var optionTypeName = optionExport.OptionTypeName ?? optionExport.OptionTypeId.ToString();
                problems.Add($"Slot {itemExport.Slot}: option '{optionTypeName}' (number {optionExport.Number}) isn't available for this item.");
            }
        }

        var ancientSets = new List<ItemOfItemSet>();
        foreach (var setName in itemExport.AncientSets ?? [])
        {
            var itemOfSet = definition.PossibleItemSetGroups
                .SelectMany(group => group.Items)
                .FirstOrDefault(candidate => candidate.ItemDefinition == definition
                    && candidate.AncientSetDiscriminator > 0
                    && candidate.ItemSetGroup?.Name.ValueInNeutralLanguage == setName);
            if (itemOfSet is null)
            {
                problems.Add($"Slot {itemExport.Slot}: ancient set '{setName}' isn't available for this item.");
            }
            else
            {
                ancientSets.Add(itemOfSet);
            }
        }

        return new PlannedItem(itemExport, definition, options, ancientSets);
    }

    private static void CreateItem(PlannedItem plannedItem, IContext context, ICollection<Item> createdItems)
    {
        var itemExport = plannedItem.Export;
        var item = context.CreateNew<Item>();
        createdItems.Add(item);
        item.Definition = plannedItem.Definition;
        item.ItemSlot = itemExport.Slot;
        item.Level = itemExport.Level;
        item.Durability = itemExport.Durability;
        item.HasSkill = itemExport.HasSkill;
        item.SocketCount = itemExport.SocketCount;
        item.PetExperience = itemExport.PetExperience;

        foreach (var plannedOption in plannedItem.Options)
        {
            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = plannedOption.Option;
            optionLink.Level = plannedOption.Export.Level;
            optionLink.Index = plannedOption.Export.Index;
            item.ItemOptions.Add(optionLink);
        }

        foreach (var itemOfSet in plannedItem.AncientSets)
        {
            item.ItemSetGroups.Add(itemOfSet);
        }
    }

    private static void DiscardCreatedItems(IEnumerable<Item> createdItems, ItemStorage store, IContext context)
    {
        foreach (var item in createdItems)
        {
            store.Items.Remove(item);
            foreach (var optionLink in item.ItemOptions.ToList())
            {
                context.Detach(optionLink);
            }

            context.Detach(item);
        }
    }

    private sealed record PlannedOption(IncreasableItemOption Option, ShopItemOptionExport Export);

    private sealed record PlannedItem(
        ShopItemExport Export,
        ItemDefinition Definition,
        IReadOnlyList<PlannedOption> Options,
        IReadOnlyList<ItemOfItemSet> AncientSets);
}
