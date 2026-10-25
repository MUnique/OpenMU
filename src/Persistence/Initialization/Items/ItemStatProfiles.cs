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
    /// <exception cref="InvalidOperationException">
    /// The items which get the level bonus table of a stat from the same added profile use different tables.
    /// </exception>
    public static void Apply(IContext context, GameConfiguration gameConfiguration)
    {
        var addedProfiles = AddMissingProfiles(context, gameConfiguration);
        AssignProfiles(gameConfiguration);
        TakeTheTablesOfTheItems(gameConfiguration, addedProfiles);
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

    private static void AssignProfiles(GameConfiguration gameConfiguration)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        foreach (var item in gameConfiguration.Items.Where(item => item.StatProfile is null))
        {
            var name = BuiltInItemStatProfiles.GetProfileName(item);
            item.StatProfile = gameConfiguration.ItemStatProfiles.FirstOrDefault(profile => profile.Name == name) ?? defaultProfile;
        }
    }

    /// <summary>
    /// Gives the stats of the added profiles the level bonus table which their items use. Unlike the name, the items'
    /// reference to the table stays when an owner renames or replaces it. A table which no item gets from the profile
    /// stays as found by its name, or empty when the configuration doesn't have it, e.g. the table of the third wings
    /// in a version without third wings.
    /// </summary>
    private static void TakeTheTablesOfTheItems(GameConfiguration gameConfiguration, List<ItemStatProfile> addedProfiles)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        foreach (var profile in addedProfiles)
        {
            foreach (var terms in profile.StatTerms.Where(t => t.TableState == ItemStatTermState.Values))
            {
                if (!TableAttributes.TryGetValue(terms.Stat, out var attributes))
                {
                    continue;
                }

                var tables = gameConfiguration.Items
                    .Where(item => GetProfileOfTable(item.StatProfile!, terms.Stat, defaultProfile) == profile)
                    .SelectMany(item => item.BasePowerUpAttributes)
                    .Where(powerUp => powerUp.BonusPerLevelTable is not null && attributes.Any(attribute => attribute == powerUp.TargetAttribute))
                    .Select(powerUp => powerUp.BonusPerLevelTable!)
                    .Distinct()
                    .ToList();
                if (tables.Count > 1)
                {
                    var tableNames = string.Join(", ", tables.Select(table => $"'{table.Name.ValueInNeutralLanguage}'"));
                    throw new InvalidOperationException(
                        $"The items of the item stat profile '{profile.Name}' use different level bonus tables for the {terms.Stat}: {tableNames}. "
                        + "Give them the same table, so that the profile can name it.");
                }

                if (tables.Count == 1)
                {
                    terms.Table = tables[0];
                }
            }
        }
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
