// <copyright file="ItemList097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d.Items;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Initialization.Items;
using Version095dItems = MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

internal static class ItemList097
{
    private const string ItemListRelativePath = "Version097d/Items/ItemList097.dat";

    public static HashSet<(byte Group, short Number)> LoadAllowedItems()
    {
        var itemListPath = Path.Combine(
            AppContext.BaseDirectory,
            ItemListRelativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(itemListPath))
        {
            return [];
        }

        var allowedItems = new HashSet<(byte Group, short Number)>();
        byte? currentGroup = null;

        foreach (var rawLine in File.ReadLines(itemListPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.Equals("end", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (IsGroupHeader(line, out var group))
            {
                currentGroup = group;
                continue;
            }

            if (currentGroup is null)
            {
                continue;
            }

            var firstToken = GetFirstToken(line);
            if (short.TryParse(firstToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            {
                allowedItems.Add((currentGroup.Value, number));
            }
        }

        return allowedItems;
    }

    public static IReadOnlyList<ItemListEntry> LoadItems()
    {
        var itemListPath = Path.Combine(
            AppContext.BaseDirectory,
            ItemListRelativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(itemListPath))
        {
            return Array.Empty<ItemListEntry>();
        }

        var items = new List<ItemListEntry>();
        byte? currentGroup = null;

        foreach (var rawLine in File.ReadLines(itemListPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.Equals("end", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (IsGroupHeader(line, out var group))
            {
                currentGroup = group;
                continue;
            }

            if (currentGroup is null)
            {
                continue;
            }

            if (TryParseItemLine(line, currentGroup.Value, out var entry))
            {
                items.Add(entry);
            }
        }

        return items;
    }

    private static bool IsGroupHeader(string line, out byte group)
    {
        group = 0;
        if (!byte.TryParse(line, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedGroup))
        {
            return false;
        }

        group = parsedGroup;
        return true;
    }

    private static string GetFirstToken(string line)
    {
        var index = line.IndexOfAny(new[] { ' ', '\t' });
        return index < 0 ? line : line[..index];
    }

    private static bool TryParseItemLine(string line, byte group, out ItemListEntry entry)
    {
        entry = default;
        var columns = line.Split('\t');
        if (columns.Length < 9)
        {
            return false;
        }

        if (!short.TryParse(columns[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
        {
            return false;
        }

        var slot = GetByte(columns, 1);
        var width = GetByte(columns, 3);
        var height = GetByte(columns, 4);
        var dropsFromMonsters = GetByte(columns, 7) != 0;
        var name = columns[8].Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var dropLevel = GetByte(columns, 13);
        var durability = GetInt(columns, 17);
        var isAmmunition = durability < 0
            || name.Contains("arrow", StringComparison.OrdinalIgnoreCase)
            || name.Contains("bolt", StringComparison.OrdinalIgnoreCase);

        entry = new ItemListEntry(
            group,
            number,
            slot,
            width,
            height,
            dropsFromMonsters,
            name,
            dropLevel,
            durability,
            isAmmunition);

        return true;
    }

    private static byte GetByte(string[] columns, int index)
    {
        var value = GetInt(columns, index);
        if (value <= 0)
        {
            return 0;
        }

        return value >= byte.MaxValue ? byte.MaxValue : (byte)value;
    }

    private static int GetInt(string[] columns, int index)
    {
        if (index < 0 || index >= columns.Length)
        {
            return 0;
        }

        return int.TryParse(columns[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : 0;
    }
}

internal sealed class ItemList097Filter
{
    private readonly GameConfiguration _gameConfiguration;
    private readonly HashSet<(byte Group, short Number)> _allowedItems;

    public ItemList097Filter(GameConfiguration gameConfiguration)
    {
        this._gameConfiguration = gameConfiguration;
        this._allowedItems = ItemList097.LoadAllowedItems();
    }

    public void Apply()
    {
        if (this._allowedItems.Count == 0)
        {
            return;
        }

        this.FilterDropItemGroups(this._gameConfiguration.DropItemGroups);
        this.FilterMerchantStores(this._gameConfiguration.Monsters);
        this.FilterItemSets(this._gameConfiguration.ItemSetGroups);
    }

    private void FilterDropItemGroups(IEnumerable<DropItemGroup> dropItemGroups)
    {
        foreach (var dropItemGroup in dropItemGroups)
        {
            var itemsToRemove = dropItemGroup.PossibleItems
                .Where(itemDefinition => !this.IsAllowed(itemDefinition))
                .ToList();

            foreach (var itemDefinition in itemsToRemove)
            {
                dropItemGroup.PossibleItems.Remove(itemDefinition);
            }
        }
    }

    private void FilterMerchantStores(IEnumerable<MonsterDefinition> monsters)
    {
        foreach (var monster in monsters)
        {
            var merchantStore = monster.MerchantStore;
            if (merchantStore is null)
            {
                continue;
            }

            var itemsToRemove = merchantStore.Items
                .Where(item => !this.IsAllowed(item.Definition))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                merchantStore.Items.Remove(item);
            }
        }
    }

    private void FilterItemSets(IEnumerable<ItemSetGroup> itemSetGroups)
    {
        foreach (var itemSetGroup in itemSetGroups)
        {
            var itemsToRemove = itemSetGroup.Items
                .Where(itemOfSet => !this.IsAllowed(itemOfSet.ItemDefinition))
                .ToList();

            foreach (var itemOfSet in itemsToRemove)
            {
                itemSetGroup.Items.Remove(itemOfSet);
            }
        }
    }

    private bool IsAllowed(ItemDefinition? itemDefinition)
    {
        return itemDefinition is not null
            && this._allowedItems.Contains((itemDefinition.Group, itemDefinition.Number));
    }
}

internal readonly record struct ItemListEntry(
    byte Group,
    short Number,
    byte Slot,
    byte Width,
    byte Height,
    bool DropsFromMonsters,
    string Name,
    byte DropLevel,
    int Durability,
    bool IsAmmunition);

internal sealed class ItemList097Importer : InitializerBase
{
    public ItemList097Importer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    public override void Initialize()
    {
        var entries = ItemList097.LoadItems();
        if (entries.Count == 0)
        {
            return;
        }

        var existingItems = new HashSet<(byte Group, short Number)>(
            this.GameConfiguration.Items.Select(item => (item.Group, item.Number)));

        foreach (var entry in entries)
        {
            if (existingItems.Contains((entry.Group, entry.Number)))
            {
                continue;
            }

            var item = this.Context.CreateNew<ItemDefinition>();
            item.Name = entry.Name;
            item.Group = entry.Group;
            item.Number = entry.Number;
            item.Width = entry.Width;
            item.Height = entry.Height;
            item.DropsFromMonsters = entry.DropsFromMonsters;
            item.DropLevel = entry.DropLevel;
            item.Durability = entry.Durability < 0 ? (byte)255 : (byte)Math.Min(byte.MaxValue, entry.Durability);
            item.IsAmmunition = entry.IsAmmunition;
            item.MaximumItemLevel = GetMaximumItemLevel(entry);
            item.ItemSlot = this.GetSlotType(entry);
            item.SetGuid(item.Group, item.Number);
            this.ApplyTemplateOptions(item);

            this.GameConfiguration.Items.Add(item);
            existingItems.Add((entry.Group, entry.Number));
        }
    }

    private ItemSlotType? GetSlotType(ItemListEntry entry)
    {
        if (entry.Slot <= 1 && entry.Group <= (byte)ItemGroups.Staff)
        {
            var dualSlot = this.GameConfiguration.ItemSlotTypes
                .FirstOrDefault(type => type.ItemSlots.Contains(0) && type.ItemSlots.Contains(1));
            if (dualSlot is not null)
            {
                return dualSlot;
            }
        }

        return this.GameConfiguration.ItemSlotTypes
            .FirstOrDefault(type => type.ItemSlots.Contains(entry.Slot));
    }

    private void ApplyTemplateOptions(ItemDefinition item)
    {
        if (item.PossibleItemOptions.Count > 0)
        {
            return;
        }

        var template = this.GameConfiguration.Items
            .FirstOrDefault(existing => existing.Group == item.Group && existing.PossibleItemOptions.Count > 0);
        if (template is null)
        {
            return;
        }

        foreach (var option in template.PossibleItemOptions)
        {
            item.PossibleItemOptions.Add(option);
        }
    }

    private static byte GetMaximumItemLevel(ItemListEntry entry)
    {
        if (entry.IsAmmunition)
        {
            return 0;
        }

        if (IsNonUpgradeableConsumable(entry.Name))
        {
            return 0;
        }

        return Version095dItems.Constants.MaximumItemLevel;
    }

    private static bool IsNonUpgradeableConsumable(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return name.Contains("Orb of", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Jewel of", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Scroll of", StringComparison.OrdinalIgnoreCase);
    }
}
