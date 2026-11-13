// <copyright file="MerchantShopExportImportService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

using System.Text.Json;
using System.Threading;
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
    /// In that case, the shop is left untouched.
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

        var newItems = new List<Item>();
        var skippedItems = 0;
        var skippedOptions = 0;
        foreach (var itemExport in dto.Items ?? [])
        {
            if (!definitionsByKey.TryGetValue((itemExport.ItemGroup, itemExport.ItemNumber), out var definition))
            {
                skippedItems++;
                continue;
            }

            var item = CreateItem(itemExport, definition, context, out var skippedOptionsOfItem);
            newItems.Add(item);
            skippedOptions += skippedOptionsOfItem;
        }

        await RemoveItemsAsync(store, context).ConfigureAwait(false);
        foreach (var item in newItems)
        {
            store.Items.Add(item);
        }

        return new MerchantShopImportResult(newItems.Count, skippedItems, skippedOptions);
    }

    private static MerchantShopExport? ReadExport(string json)
    {
        MerchantShopExport? dto;
        try
        {
            dto = JsonSerializer.Deserialize<MerchantShopExport>(json);
        }
        catch (JsonException)
        {
            return null;
        }

        return dto?.FormatVersion == MerchantShopExport.CurrentFormatVersion ? dto : null;
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

    private static Item CreateItem(
        ShopItemExport itemExport,
        ItemDefinition definition,
        IContext context,
        out int skippedOptionCount)
    {
        var item = context.CreateNew<Item>();
        item.Definition = definition;
        item.ItemSlot = itemExport.Slot;
        item.Level = itemExport.Level;
        item.Durability = itemExport.Durability;
        item.HasSkill = itemExport.HasSkill;
        item.SocketCount = itemExport.SocketCount;
        item.PetExperience = itemExport.PetExperience;

        skippedOptionCount = AddOptions(item, definition, itemExport, context);
        skippedOptionCount += AddAncientSets(item, definition, itemExport);

        return item;
    }

    private static int AddOptions(Item item, ItemDefinition definition, ShopItemExport itemExport, IContext context)
    {
        var candidates = GetOptionCandidates(definition);
        var skipped = 0;
        foreach (var optionExport in itemExport.Options ?? [])
        {
            var option = candidates.FirstOrDefault(candidate =>
                candidate.OptionType?.Id == optionExport.OptionTypeId
                && candidate.Number == optionExport.Number
                && candidate.SubOptionType == optionExport.SubOptionType);
            if (option is null)
            {
                skipped++;
                continue;
            }

            var optionLink = context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            optionLink.Level = optionExport.Level;
            optionLink.Index = optionExport.Index;
            item.ItemOptions.Add(optionLink);
        }

        return skipped;
    }

    private static int AddAncientSets(Item item, ItemDefinition definition, ShopItemExport itemExport)
    {
        var skipped = 0;
        foreach (var setName in itemExport.AncientSets ?? [])
        {
            var itemOfSet = definition.PossibleItemSetGroups
                .SelectMany(group => group.Items)
                .FirstOrDefault(candidate => candidate.ItemDefinition == definition
                    && candidate.AncientSetDiscriminator > 0
                    && candidate.ItemSetGroup?.Name.ValueInNeutralLanguage == setName);
            if (itemOfSet is null)
            {
                skipped++;
                continue;
            }

            item.ItemSetGroups.Add(itemOfSet);
        }

        return skipped;
    }
}
