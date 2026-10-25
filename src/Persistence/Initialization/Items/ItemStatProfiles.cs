// <copyright file="ItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using System.Globalization;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;

/// <summary>
/// Creates the item stat profiles of a configuration and gives its items their profile.
/// Used by the initialization data of every version and by the configuration update which adds the profiles.
/// </summary>
internal static class ItemStatProfiles
{
    /// <summary>
    /// The attributes of the item power-ups which get the level bonus table of a stat.
    /// </summary>
    private static readonly IReadOnlyDictionary<ItemStat, AttributeDefinition[]> TableAttributes = new Dictionary<ItemStat, AttributeDefinition[]>
    {
        { ItemStat.Damage, [Stats.MinimumPhysBaseDmgByWeapon, Stats.MaximumPhysBaseDmgByWeapon] },
        { ItemStat.BlockRate, [Stats.DefenseRatePvm] },
        { ItemStat.Defense, [Stats.DefenseBase, Stats.DefenseShield] },
    };

    /// <summary>
    /// Adds the built-in item stat profiles which the configuration doesn't have yet (by name), and gives each item
    /// without a profile the one of its kind (<see cref="BuiltInItemStatProfiles.GetProfileName"/>).
    /// Profiles and items which already have a profile stay as they are.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void Apply(IContext context, GameConfiguration gameConfiguration)
    {
        var addedProfiles = AddMissingProfiles(context, gameConfiguration);
        var assignedItems = AssignProfiles(gameConfiguration);
        TakeTheTablesOfTheItems(context, gameConfiguration, addedProfiles, assignedItems);
    }

    private static List<ItemStatProfile> AddMissingProfiles(IContext context, GameConfiguration gameConfiguration)
    {
        var addedProfiles = new List<ItemStatProfile>();
        foreach (var name in BuiltInItemStatProfiles.Names)
        {
            if (gameConfiguration.ItemStatProfiles.Any(profile => profile.Name == name))
            {
                continue;
            }

            var profile = context.CreateNew<ItemStatProfile>();
            profile.Name = name;
            BuiltInItemStatProfiles.Fill(
                profile,
                () => context.CreateNew<ItemStatTerms>(),
                () => context.CreateNew<ItemRequirementFormula>(),
                table => FindTable(gameConfiguration, table));
            gameConfiguration.ItemStatProfiles.Add(profile);
            addedProfiles.Add(profile);
        }

        return addedProfiles;
    }

    private static HashSet<ItemDefinition> AssignProfiles(GameConfiguration gameConfiguration)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        var assignedItems = gameConfiguration.Items.Where(item => item.StatProfile is null).ToHashSet();
        foreach (var item in assignedItems)
        {
            var name = BuiltInItemStatProfiles.GetProfileName(item);
            item.StatProfile = gameConfiguration.ItemStatProfiles.FirstOrDefault(profile => profile.Name == name) ?? defaultProfile;
        }

        return assignedItems;
    }

    /// <summary>
    /// Gives the stats of the added profiles the level bonus table which their items use. Unlike the name, the items'
    /// reference to the table stays when an owner renames or replaces it. When the items which get a table from a profile
    /// use different tables, the profile takes the one most of them use. Each other item which got its profile in this
    /// run gets a variant of it which names the item's table, e.g. "thirdTierWing2", so that no item's table changes and
    /// each item's profile names its table. A table which no item gets from the profile stays as found by its name,
    /// or empty when the configuration doesn't have it, e.g. the table of the third wings in a version without third wings.
    /// </summary>
    private static void TakeTheTablesOfTheItems(IContext context, GameConfiguration gameConfiguration, List<ItemStatProfile> addedProfiles, HashSet<ItemDefinition> assignedItems)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        var variants = new Dictionary<(ItemStatProfile Profile, ItemStat Stat, ItemLevelBonusTable Table), ItemStatProfile>();
        foreach (var profile in addedProfiles)
        {
            foreach (var terms in profile.StatTerms.Where(t => t.TableState == ItemStatTermState.Values).ToList())
            {
                if (!TableAttributes.TryGetValue(terms.Stat, out var attributes))
                {
                    continue;
                }

                var itemTables = gameConfiguration.Items
                    .Where(item => GetProfileOfTable(item.StatProfile!, terms.Stat, defaultProfile) == profile)
                    .Select(item => (Item: item, Table: GetTableOfItem(item, attributes)))
                    .Where(itemTable => itemTable.Table is not null)
                    .ToList();
                if (itemTables.Count == 0)
                {
                    continue;
                }

                // On a tie, the table with the built-in name stays.
                var table = itemTables
                    .GroupBy(itemTable => itemTable.Table!)
                    .OrderByDescending(group => group.Count())
                    .ThenByDescending(group => group.Key == terms.Table)
                    .ThenBy(group => group.Key.Name.ValueInNeutralLanguage, StringComparer.Ordinal)
                    .First().Key;
                terms.Table = table;
                foreach (var (item, itemTable) in itemTables.Where(itemTable => itemTable.Table != table && assignedItems.Contains(itemTable.Item)))
                {
                    var key = (item.StatProfile!, terms.Stat, itemTable!);
                    if (!variants.TryGetValue(key, out var variant))
                    {
                        variant = CreateVariant(context, gameConfiguration, item.StatProfile!, terms.Stat, itemTable!);
                        variants.Add(key, variant);
                    }

                    item.StatProfile = variant;
                }
            }
        }
    }

    /// <summary>
    /// Gets the level bonus table of the item's power-ups of the given attributes, or <c>null</c> when they have none,
    /// or several, which no profile can name; then the item keeps its profile.
    /// </summary>
    private static ItemLevelBonusTable? GetTableOfItem(ItemDefinition item, AttributeDefinition[] attributes)
    {
        var tables = item.BasePowerUpAttributes
            .Where(powerUp => powerUp.BonusPerLevelTable is not null && attributes.Any(attribute => attribute == powerUp.TargetAttribute))
            .Select(powerUp => powerUp.BonusPerLevelTable!)
            .Distinct()
            .Take(2)
            .ToList();
        return tables.Count == 1 ? tables[0] : null;
    }

    /// <summary>
    /// Creates a copy of the profile with the next free name, e.g. "thirdTierWing2", which names the given table for the stat.
    /// </summary>
    private static ItemStatProfile CreateVariant(IContext context, GameConfiguration gameConfiguration, ItemStatProfile profile, ItemStat stat, ItemLevelBonusTable table)
    {
        var baseName = profile.Name.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        var number = 2;
        while (gameConfiguration.ItemStatProfiles.Any(p => p.Name == baseName + number))
        {
            number++;
        }

        var variant = context.CreateNew<ItemStatProfile>();
        variant.Name = baseName + number;
        variant.LevelPerItemLevel = profile.LevelPerItemLevel;
        variant.ExcellentLevelAdd = profile.ExcellentLevelAdd;
        variant.StrengthPerOptionLevel = profile.StrengthPerOptionLevel;
        foreach (var terms in profile.StatTerms)
        {
            var copy = context.CreateNew<ItemStatTerms>();
            copy.AssignValuesOf(terms, gameConfiguration);
            variant.StatTerms.Add(copy);
        }

        foreach (var formula in profile.RequirementFormulas)
        {
            var copy = context.CreateNew<ItemRequirementFormula>();
            copy.AssignValuesOf(formula, gameConfiguration);
            variant.RequirementFormulas.Add(copy);
        }

        var statTerms = variant.StatTerms.FirstOrDefault(t => t.Stat == stat);
        if (statTerms is null)
        {
            statTerms = context.CreateNew<ItemStatTerms>();
            statTerms.Stat = stat;
            variant.StatTerms.Add(statTerms);
        }

        statTerms.TableState = ItemStatTermState.Values;
        statTerms.Table = table;
        gameConfiguration.ItemStatProfiles.Add(variant);
        return variant;
    }

    /// <summary>
    /// Gets the profile whose level bonus table of the stat items of the given profile get: their own profile when it writes it, or else the default.
    /// </summary>
    private static ItemStatProfile GetProfileOfTable(ItemStatProfile profile, ItemStat stat, ItemStatProfile defaultProfile)
    {
        return profile.StatTerms.Any(t => t.Stat == stat && t.TableState != ItemStatTermState.NotWritten) ? profile : defaultProfile;
    }

    private static ItemLevelBonusTable? FindTable(GameConfiguration gameConfiguration, BuiltInLevelBonusTable table)
    {
        // The members of BuiltInLevelBonusTable are the keys of the level bonus names.
        var neutralName = LevelBonusNames.ResourceManager.GetString(table.ToString(), CultureInfo.InvariantCulture)
                          ?? throw new ArgumentOutOfRangeException(nameof(table), table, "The level bonus names have no such key.");
        return gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name.ValueInNeutralLanguage == neutralName);
    }
}
