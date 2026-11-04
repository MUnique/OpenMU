// <copyright file="SeedSphereMaximumItemLevelTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the maximum item level of the seed spheres, which has to allow every option of their element,
/// because the item level of a seed sphere defines its option.
/// </summary>
[TestFixture]
internal class SeedSphereMaximumItemLevelTest
{
    /// <summary>
    /// Tests that a new season 6 database allows every option of the element of a seed sphere.
    /// </summary>
    [Test]
    public async Task NewDatabaseAllowsEveryOptionAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertMaximumItemLevelsAllowEveryOption(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update repairs the maximum item levels of an existing database, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateRepairsMaximumItemLevelsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var seedSphere in GetSeedSpheres(gameConfiguration))
        {
            seedSphere.MaximumItemLevel = 0;
        }

        var update = new FixSeedSphereMaximumItemLevelUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertMaximumItemLevelsAllowEveryOption(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update keeps a maximum item level which was changed on purpose.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomMaximumItemLevelAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var seedSphere = GetSeedSpheres(gameConfiguration).First();
        seedSphere.MaximumItemLevel = 2;

        await new FixSeedSphereMaximumItemLevelUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(seedSphere.MaximumItemLevel, Is.EqualTo(2));
    }

    private static void AssertMaximumItemLevelsAllowEveryOption(GameConfiguration gameConfiguration)
    {
        var seedSpheres = GetSeedSpheres(gameConfiguration);
        Assert.That(seedSpheres, Has.Count.EqualTo(30));
        Assert.Multiple(() =>
        {
            foreach (var seedSphere in seedSpheres)
            {
                var optionNumbers = seedSphere.PossibleItemOptions.Single().PossibleOptions
                    .Where(o => o.OptionType == ItemOptionTypes.SocketOption)
                    .Select(o => o.Number)
                    .ToList();
                Assert.That(optionNumbers.Min(), Is.EqualTo(0), seedSphere.Name.ValueInNeutralLanguage);
                Assert.That(seedSphere.MaximumItemLevel, Is.EqualTo(optionNumbers.Max()), seedSphere.Name.ValueInNeutralLanguage);
            }
        });
    }

    private static List<ItemDefinition> GetSeedSpheres(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Where(item => item.Group == 12 && item.Number is >= 100 and <= 129).ToList();
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
