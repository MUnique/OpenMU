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
    /// Gives the terms of the added profiles the level bonus tables which their items use. Unlike the name, the items'
    /// reference to a table stays when an owner renames or replaces it. First, each term takes the table which most of
    /// the items use that get it from the profile. Then each item which got its profile in this run and uses other
    /// tables than its profile names gets one variant of its profile which names all of the item's tables, e.g.
    /// "shield2", so that no item's table changes and each item's profile names its tables. A table which no item gets
    /// from the profile stays as found by its name, or empty when the configuration doesn't have it, e.g. the table of
    /// the third wings in a version without third wings.
    /// </summary>
    /// <remarks>
    /// The table of an attribute is the one of the item's power-ups of that attribute. So for a weapon whose maximum
    /// physical damage uses another table than its minimum physical damage, the profile names the table of the minimum
    /// damage, which the item formulas read, and the maximum damage keeps its own table, which no profile names.
    /// </remarks>
    private static void TakeTheTablesOfTheItems(IContext context, GameConfiguration gameConfiguration, List<ItemStatProfile> addedProfiles, HashSet<ItemDefinition> assignedItems)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        var tableTerms = addedProfiles
            .SelectMany(profile => profile.StatTerms
                .Where(terms => terms.TableState == ItemStatTermState.Values && terms.Attribute is not null)
                .Select(terms => (Profile: profile, Terms: terms)))
            .ToList();
        foreach (var (profile, terms) in tableTerms)
        {
            var tables = gameConfiguration.Items
                .Where(item => GetProfileOfTable(item.StatProfile!, terms.Attribute!, defaultProfile) == profile)
                .Select(item => GetTableOfItem(item, terms.Attribute!))
                .OfType<ItemLevelBonusTable>()
                .ToList();
            if (tables.Count > 0)
            {
                // On a tie, the table with the built-in name stays.
                terms.Table = tables
                    .GroupBy(table => table)
                    .OrderByDescending(group => group.Count())
                    .ThenByDescending(group => group.Key == terms.Table)
                    .ThenBy(group => group.Key.Name.ValueInNeutralLanguage, StringComparer.Ordinal)
                    .First().Key;
            }
        }

        var attributes = tableTerms.Select(tableTerm => tableTerm.Terms.Attribute!).Distinct().ToList();
        var variants = new List<(ItemStatProfile Profile, List<(AttributeDefinition Attribute, ItemLevelBonusTable Table)> Tables, ItemStatProfile Variant)>();
        foreach (var item in assignedItems.OrderBy(item => item.Group).ThenBy(item => item.Number))
        {
            var profile = item.StatProfile!;
            var ownTables = attributes
                .Select(attribute => (Attribute: attribute, Table: GetTableOfItem(item, attribute)))
                .Where(own => own.Table is not null && own.Table != GetTableOfProfile(profile, own.Attribute, defaultProfile))
                .Select(own => (own.Attribute, Table: own.Table!))
                .ToList();
            if (ownTables.Count == 0)
            {
                continue;
            }

            var variant = variants.FirstOrDefault(v => v.Profile == profile && v.Tables.SequenceEqual(ownTables)).Variant;
            if (variant is null)
            {
                variant = CreateVariant(context, gameConfiguration, profile, ownTables);
                variants.Add((profile, ownTables, variant));
            }

            item.StatProfile = variant;
        }
    }

    /// <summary>
    /// Gets the level bonus table of the item's power-ups of the attribute, or <c>null</c> when they have none,
    /// or several, which no profile can name; then the item keeps its profile for it.
    /// </summary>
    private static ItemLevelBonusTable? GetTableOfItem(ItemDefinition item, AttributeDefinition attribute)
    {
        var tables = item.BasePowerUpAttributes
            .Where(powerUp => powerUp.TargetAttribute == attribute && powerUp.BonusPerLevelTable is not null)
            .Select(powerUp => powerUp.BonusPerLevelTable!)
            .Distinct()
            .Take(2)
            .ToList();
        return tables.Count == 1 ? tables[0] : null;
    }

    /// <summary>
    /// Gets the level bonus table which the profile names for the attribute, or the default when the profile doesn't write it.
    /// </summary>
    private static ItemLevelBonusTable? GetTableOfProfile(ItemStatProfile profile, AttributeDefinition attribute, ItemStatProfile defaultProfile)
    {
        var terms = GetProfileOfTable(profile, attribute, defaultProfile).StatTerms.FirstOrDefault(t => t.Attribute == attribute);
        return terms?.TableState == ItemStatTermState.Values ? terms.Table : null;
    }

    /// <summary>
    /// Creates a copy of the profile with the next free name, e.g. "shield2", which names the given tables for their attributes.
    /// </summary>
    private static ItemStatProfile CreateVariant(IContext context, GameConfiguration gameConfiguration, ItemStatProfile profile, List<(AttributeDefinition Attribute, ItemLevelBonusTable Table)> tables)
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

        foreach (var (attribute, table) in tables)
        {
            var terms = variant.StatTerms.FirstOrDefault(t => t.Attribute == attribute);
            if (terms is null)
            {
                terms = context.CreateNew<ItemStatTerms>();
                terms.Attribute = attribute;
                variant.StatTerms.Add(terms);
            }

            terms.TableState = ItemStatTermState.Values;
            terms.Table = table;
        }

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
