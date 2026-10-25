// <copyright file="ItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Creates the item stat profiles of a configuration and gives its items their profile.
/// Used by the initialization data of every version and by the configuration update which adds the profiles.
/// </summary>
internal static class ItemStatProfiles
{
    /// <summary>
    /// Adds the built-in item stat profiles which the configuration doesn't have yet (by name), and gives each item
    /// without a profile the one of its kind (<see cref="BuiltInItemStatProfiles.GetProfileName"/>).
    /// Profiles and items which already have a profile stay as they are.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void Apply(IContext context, GameConfiguration gameConfiguration)
    {
        AddMissingProfiles(context, gameConfiguration);
        AssignProfiles(gameConfiguration);
    }

    private static void AddMissingProfiles(IContext context, GameConfiguration gameConfiguration)
    {
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
        }
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

    private static ItemLevelBonusTable? FindTable(GameConfiguration gameConfiguration, BuiltInLevelBonusTable table)
    {
        var name = table switch
        {
            BuiltInLevelBonusTable.DamageIncreaseWeapons => LocalizedString.FromResource(() => LevelBonusNames.DamageIncreaseWeapons),
            BuiltInLevelBonusTable.DefenseRateIncreaseShields => LocalizedString.FromResource(() => LevelBonusNames.DefenseRateIncreaseShields),
            BuiltInLevelBonusTable.DefenseIncreaseArmors => LocalizedString.FromResource(() => LevelBonusNames.DefenseIncreaseArmors),
            BuiltInLevelBonusTable.DefenseIncreaseShields => LocalizedString.FromResource(() => LevelBonusNames.DefenseIncreaseShields),
            BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings => LocalizedString.FromResource(() => LevelBonusNames.DefenseBonus1stAnd2ndWings),
            BuiltInLevelBonusTable.DefenseBonus3rdWings => LocalizedString.FromResource(() => LevelBonusNames.DefenseBonus3rdWings),
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, null),
        };

        var neutralName = name.ValueInNeutralLanguage;
        return gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name.ValueInNeutralLanguage == neutralName);
    }
}
