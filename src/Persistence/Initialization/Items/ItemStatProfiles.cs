// <copyright file="ItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.ItemFormulas;

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
        // The members of BuiltInLevelBonusTable are the keys of the level bonus names.
        var neutralName = LevelBonusNames.ResourceManager.GetString(table.ToString(), CultureInfo.InvariantCulture)
                          ?? throw new ArgumentOutOfRangeException(nameof(table), table, "The level bonus names have no such key.");
        return gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name.ValueInNeutralLanguage == neutralName);
    }
}
