// <copyright file="TestItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// A game configuration with the built-in item stat profiles, for tests which compute item values: the item formulas
/// read them from the configuration.
/// </summary>
internal static class TestItemStatProfiles
{
    /// <summary>
    /// Gets a game configuration with the built-in item stat profiles. It has no level bonus tables, so the profiles
    /// name none; the level bonus comes from the items' power-ups anyway.
    /// </summary>
    public static GameConfiguration Configuration { get; } = Create();

    /// <summary>
    /// Gives the item definition the built-in profile of its kind, as the initialization data does.
    /// </summary>
    /// <param name="definition">The item definition, with the data which decides its kind; <c>null</c> leaves it out.</param>
    /// <returns>The configuration of the profiles.</returns>
    public static GameConfiguration ForItem(ItemDefinition? definition) => ForItem(definition, Configuration);

    /// <summary>
    /// Gives the item definition the built-in profile of its kind of the given configuration, as the initialization data does.
    /// </summary>
    /// <param name="definition">The item definition, with the data which decides its kind; <c>null</c> leaves it out.</param>
    /// <param name="configuration">The configuration with the built-in profiles.</param>
    /// <returns>The configuration.</returns>
    public static GameConfiguration ForItem(ItemDefinition? definition, GameConfiguration configuration)
    {
        if (definition is not null)
        {
            var name = ItemStatProfiles.GetProfileName(definition);
            definition.StatProfile = configuration.ItemStatProfiles.First(profile => profile.Name == name);
        }

        return configuration;
    }

    /// <summary>
    /// Creates a game configuration with the built-in item stat profiles, which a test may change.
    /// </summary>
    /// <returns>The game configuration.</returns>
    public static GameConfiguration Create()
    {
        var configuration = new Persistence.BasicModel.GameConfiguration();
        foreach (var name in BuiltInItemStatProfiles.Names)
        {
            var profile = new Persistence.BasicModel.ItemStatProfile { Name = name };
            BuiltInItemStatProfiles.Fill(
                profile,
                () => new Persistence.BasicModel.ItemStatTerms(),
                () => new Persistence.BasicModel.ItemRequirementFormula(),
                _ => null,
                attribute => attribute);
            configuration.ItemStatProfiles.Add(profile);
        }

        return configuration;
    }
}
