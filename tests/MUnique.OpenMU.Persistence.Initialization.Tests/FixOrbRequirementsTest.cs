// <copyright file="FixOrbRequirementsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the stat requirements of the items which are used to learn skills
/// are calculated like the client does it, and that the update fixes existing databases.
/// </summary>
[TestFixture]
internal class FixOrbRequirementsTest
{
    /// <summary>
    /// The expected requirements of season 6, as the client calculates them
    /// from the requirement factor and the drop level of the item file.
    /// </summary>
    private static readonly (byte Number, AttributeDefinition Stat, int Expected, int OldFactor)[] Season6Requirements =
    [
        (8, Stats.TotalEnergy, 52, 100), // Orb of Healing
        (9, Stats.TotalEnergy, 72, 100), // Orb of Greater Defense
        (10, Stats.TotalEnergy, 92, 100), // Orb of Greater Damage
        (16, Stats.TotalStrength, 596, 320), // Orb of Fire Slash
        (18, Stats.TotalAgility, 646, 258), // Orb of Ice Arrow
        (21, Stats.TotalEnergy, 79, 20), // Scroll of Fire Burst
        (22, Stats.TotalEnergy, 153, 34), // Scroll of Summon
        (23, Stats.TotalEnergy, 102, 25), // Scroll of Critical Damage
        (24, Stats.TotalEnergy, 126, 29), // Scroll of Electric Spark
        (35, Stats.TotalEnergy, 150, 32), // Scroll of Fire Scream
        (46, Stats.TotalEnergy, 168, 37), // Crystal of Recovery
        (48, Stats.TotalEnergy, 84, 16), // Scroll of Chaotic Diseier
    ];

    /// <summary>
    /// Tests that a new season 6 database has the calculated requirements.
    /// </summary>
    [Test]
    public async Task NewSeason6DatabaseHasCalculatedRequirementsAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        Assert.Multiple(() =>
        {
            foreach (var (number, stat, expected, _) in Season6Requirements)
            {
                Assert.That(GetRequirement(gameConfiguration, number, stat), Is.EqualTo(expected), $"Item 12/{number}, {stat.Designation}");
            }
        });
    }

    /// <summary>
    /// Tests that a new 0.75 database has the calculated energy requirements of the elf orbs.
    /// </summary>
    [Test]
    public async Task NewVersion075DatabaseHasCalculatedRequirementsAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new Version075.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;

        Assert.Multiple(() =>
        {
            Assert.That(GetRequirement(gameConfiguration, 8, Stats.TotalEnergy), Is.EqualTo(52));
            Assert.That(GetRequirement(gameConfiguration, 9, Stats.TotalEnergy), Is.EqualTo(72));
            Assert.That(GetRequirement(gameConfiguration, 10, Stats.TotalEnergy), Is.EqualTo(92));
        });
    }

    /// <summary>
    /// Tests that the update fixes the requirements of a season 6 database which was
    /// created with the requirement factors, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateFixesSeason6RequirementsOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;
        foreach (var (number, stat, _, oldFactor) in Season6Requirements)
        {
            GetItem(gameConfiguration, number).Requirements.First(r => r.Attribute == stat).MinimumValue = oldFactor;
        }

        var update = new FixOrbRequirementsPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var (number, stat, expected, _) in Season6Requirements)
            {
                Assert.That(GetRequirement(gameConfiguration, number, stat), Is.EqualTo(expected), $"Item 12/{number}, {stat.Designation}");
                Assert.That(GetItem(gameConfiguration, number).Requirements.Count(r => r.Attribute == stat), Is.EqualTo(1), $"Item 12/{number}, {stat.Designation}");
            }
        });
    }

    private static ItemDefinition GetItem(GameConfiguration gameConfiguration, byte number)
    {
        return gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == number);
    }

    private static double GetRequirement(GameConfiguration gameConfiguration, byte number, AttributeDefinition attribute)
    {
        return GetItem(gameConfiguration, number).Requirements.First(r => r.Attribute == attribute).MinimumValue;
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(Func<IPersistenceContextProvider, DataInitializationBase> createDataInitialization)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await createDataInitialization(contextProvider).CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
