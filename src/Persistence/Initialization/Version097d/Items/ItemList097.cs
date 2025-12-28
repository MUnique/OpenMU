// <copyright file="ItemList097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d.Items;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;

internal static class ItemList097
{
    private const string ItemListRelativePath = "Version097d/Items/ItemList097.txt";

    public static HashSet<(byte Group, byte Number)> LoadAllowedItems()
    {
        var itemListPath = Path.Combine(
            AppContext.BaseDirectory,
            ItemListRelativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(itemListPath))
        {
            return [];
        }

        var allowedItems = new HashSet<(byte Group, byte Number)>();
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
            if (byte.TryParse(firstToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            {
                allowedItems.Add((currentGroup.Value, number));
            }
        }

        return allowedItems;
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
}

internal sealed class ItemList097Filter
{
    private readonly GameConfiguration _gameConfiguration;
    private readonly HashSet<(byte Group, byte Number)> _allowedItems;

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
