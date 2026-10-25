// <copyright file="ItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using System.Globalization;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Creates the item stat profiles of a configuration and gives its items their profile.
/// Used by the initialization data of every version and by the configuration update which adds the profiles.
/// </summary>
internal static class ItemStatProfiles
{
    private static readonly HashSet<AttributeDefinition> StatRequirements =
    [
        Stats.TotalStrength,
        Stats.TotalAgility,
        Stats.TotalEnergy,
        Stats.TotalVitality,
        Stats.TotalLeadership,
        Stats.TotalStrengthRequirementValue,
        Stats.TotalAgilityRequirementValue,
        Stats.TotalEnergyRequirementValue,
        Stats.TotalVitalityRequirementValue,
        Stats.TotalLeadershipRequirementValue,
    ];

    /// <summary>
    /// Adds the built-in item stat profiles which the configuration doesn't have yet (by name), and gives each item
    /// without a profile the one of its kind (<see cref="GetProfileName"/>).
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

    /// <summary>
    /// Gets the name of the built-in profile of the item's kind, following the profile rules of the game client's
    /// item stat file with what the item data of the server tells.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The name of the item's profile.</returns>
    internal static string GetProfileName(ItemDefinition item)
    {
        var slots = item.ItemSlot?.ItemSlots ?? [];
        if (item.IsAmmunition)
        {
            return ItemStatProfile.DefaultName;
        }

        if (HasPowerUp(item, Stats.ScepterRise))
        {
            return BuiltInItemStatProfiles.Sceptre;
        }

        if (HasPowerUp(item, Stats.BookRise))
        {
            return BuiltInItemStatProfiles.Book;
        }

        if (item.Group == (byte)ItemGroups.Shields)
        {
            return BuiltInItemStatProfiles.Shield;
        }

        if (item.Group <= (byte)ItemGroups.Staff && slots.Count > 0)
        {
            return BuiltInItemStatProfiles.Weapon;
        }

        if (slots.Any(slot => slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot))
        {
            return BuiltInItemStatProfiles.Armor;
        }

        if (slots.Contains(InventoryConstants.WingsSlot))
        {
            return GetWingProfileName(item);
        }

        if (item.Group == (byte)ItemGroups.Orbs && HasBaseRequirement(item, StatRequirements.Contains))
        {
            return BuiltInItemStatProfiles.Orb;
        }

        if (item.Group == (byte)ItemGroups.Scrolls && HasBaseRequirement(item, attribute => attribute == Stats.Level || StatRequirements.Contains(attribute)))
        {
            return BuiltInItemStatProfiles.Scroll;
        }

        if (item.Group == ItemConstants.Fenrir.Group && item.Number == ItemConstants.Fenrir.Number)
        {
            // The game client's item data gives the Horn of Fenrir the default, although the helper rule matches it.
            return ItemStatProfile.DefaultName;
        }

        if (item.Group is (byte)ItemGroups.Misc1 or (byte)ItemGroups.Misc2 && HasBaseRequirement(item, attribute => attribute == Stats.Level))
        {
            return BuiltInItemStatProfiles.Helper;
        }

        return ItemStatProfile.DefaultName;
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
                table => FindTable(gameConfiguration, table),
                attribute => attribute.GetPersistent(gameConfiguration));
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
            var name = GetProfileName(item);
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
        var variants = new Dictionary<(ItemStatProfile Profile, AttributeDefinition Attribute, ItemLevelBonusTable Table), ItemStatProfile>();
        foreach (var profile in addedProfiles)
        {
            foreach (var terms in profile.StatTerms.Where(t => t.TableState == ItemStatTermState.Values).ToList())
            {
                if (terms.Attribute is not { } attribute)
                {
                    continue;
                }

                var attributes = GetPowerUpAttributes(attribute);
                var itemTables = gameConfiguration.Items
                    .Where(item => GetProfileOfTable(item.StatProfile!, attribute, defaultProfile) == profile)
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
                    var key = (item.StatProfile!, attribute, itemTable!);
                    if (!variants.TryGetValue(key, out var variant))
                    {
                        variant = CreateVariant(context, gameConfiguration, item.StatProfile!, attribute, itemTable!);
                        variants.Add(key, variant);
                    }

                    item.StatProfile = variant;
                }
            }
        }
    }

    /// <summary>
    /// Gets the attributes of the item power-ups which get the level bonus table of the terms of the given attribute:
    /// the attribute itself, and the maximum physical damage for the minimum physical damage.
    /// </summary>
    private static AttributeDefinition[] GetPowerUpAttributes(AttributeDefinition attribute)
    {
        return attribute == Stats.MinimumPhysBaseDmgByWeapon
            ? [Stats.MinimumPhysBaseDmgByWeapon, Stats.MaximumPhysBaseDmgByWeapon]
            : [attribute];
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
    /// Creates a copy of the profile with the next free name, e.g. "thirdTierWing2", which names the given table for the attribute.
    /// </summary>
    private static ItemStatProfile CreateVariant(IContext context, GameConfiguration gameConfiguration, ItemStatProfile profile, AttributeDefinition attribute, ItemLevelBonusTable table)
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

        var statTerms = variant.StatTerms.FirstOrDefault(t => t.Attribute == attribute);
        if (statTerms is null)
        {
            statTerms = context.CreateNew<ItemStatTerms>();
            statTerms.Attribute = attribute;
            variant.StatTerms.Add(statTerms);
        }

        statTerms.TableState = ItemStatTermState.Values;
        statTerms.Table = table;
        gameConfiguration.ItemStatProfiles.Add(variant);
        return variant;
    }

    /// <summary>
    /// Gets the profile whose level bonus table of the attribute items of the given profile get: their own profile when it writes it, or else the default.
    /// </summary>
    private static ItemStatProfile GetProfileOfTable(ItemStatProfile profile, AttributeDefinition attribute, ItemStatProfile defaultProfile)
    {
        return profile.StatTerms.Any(t => t.Attribute == attribute && t.TableState != ItemStatTermState.NotWritten) ? profile : defaultProfile;
    }

    private static ItemLevelBonusTable? FindTable(GameConfiguration gameConfiguration, string tableKey)
    {
        var neutralName = LevelBonusNames.ResourceManager.GetString(tableKey, CultureInfo.InvariantCulture)
                          ?? throw new ArgumentOutOfRangeException(nameof(tableKey), tableKey, "The level bonus names have no such key.");
        return gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name.ValueInNeutralLanguage == neutralName);
    }

    private static string GetWingProfileName(ItemDefinition item)
    {
        // The wings are in group 12, the Cape of Lord in group 13; their tier is the one of the game client's item data.
        return (item.Group, item.Number) switch
        {
            ((byte)ItemGroups.Orbs, >= 130 and <= 135) => BuiltInItemStatProfiles.SmallWing,
            ((byte)ItemGroups.Orbs, >= 0 and <= 2 or 41) => BuiltInItemStatProfiles.FirstTierWing,
            ((byte)ItemGroups.Orbs, 49) or ((byte)ItemGroups.Misc1, 30) => BuiltInItemStatProfiles.SecondTierCape,
            ((byte)ItemGroups.Orbs, >= 3 and <= 6 or 42) => BuiltInItemStatProfiles.SecondTierWing,
            ((byte)ItemGroups.Orbs, >= 36 and <= 40 or 43 or 50) => BuiltInItemStatProfiles.ThirdTierWing,
            _ => ItemStatProfile.DefaultName,
        };
    }

    private static bool HasPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute == attribute);
    }

    /// <summary>
    /// Determines whether the item has a base requirement above 0 of one of the wanted attributes.
    /// A requirement which depends on the item level (one of its rows applies from an item level on) doesn't count,
    /// as in the game client, where such a requirement is given by level.
    /// </summary>
    private static bool HasBaseRequirement(ItemDefinition item, Func<AttributeDefinition, bool> isWanted)
    {
        return item.Requirements.Any(requirement => requirement.Attribute is { } attribute
                                                    && isWanted(attribute)
                                                    && requirement.MinimumValue > 0
                                                    && requirement.MinimumItemLevel is null
                                                    && !item.Requirements.Any(other => other.Attribute == attribute && other.MinimumItemLevel is not null));
    }
}
