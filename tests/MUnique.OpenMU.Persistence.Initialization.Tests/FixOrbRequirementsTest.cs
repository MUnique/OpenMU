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
    /// from the requirement factor and the drop level (or level requirement) of the item file,
    /// and the values which were stored before the update.
    /// </summary>
    private static readonly (byte Group, byte Number, AttributeDefinition Stat, int Expected, int OldValue)[] Season6Requirements =
    [
        (12, 8, Stats.TotalEnergy, 52, 100), // Orb of Healing
        (12, 9, Stats.TotalEnergy, 72, 100), // Orb of Greater Defense
        (12, 10, Stats.TotalEnergy, 92, 100), // Orb of Greater Damage
        (12, 16, Stats.TotalStrength, 596, 320), // Orb of Fire Slash
        (12, 18, Stats.TotalAgility, 646, 258), // Orb of Ice Arrow
        (12, 21, Stats.TotalEnergy, 79, 20), // Scroll of Fire Burst
        (12, 22, Stats.TotalEnergy, 153, 34), // Scroll of Summon
        (12, 23, Stats.TotalEnergy, 102, 25), // Scroll of Critical Damage
        (12, 24, Stats.TotalEnergy, 126, 29), // Scroll of Electric Spark
        (12, 35, Stats.TotalEnergy, 150, 32), // Scroll of Fire Scream
        (12, 46, Stats.TotalEnergy, 168, 37), // Crystal of Recovery
        (12, 48, Stats.TotalEnergy, 84, 16), // Scroll of Chaotic Diseier
        (15, 12, Stats.TotalEnergyRequirementValue, 500, 436), // Scroll of Cometfall
        (15, 13, Stats.TotalEnergyRequirementValue, 724, 578), // Scroll of Inferno
        (15, 28, Stats.TotalEnergyRequirementValue, 1058, 118), // Scroll of Wizardry Enhance
        (15, 29, Stats.TotalEnergyRequirementValue, 1058, 118), // Scroll of Gigantic Storm
        (15, 34, Stats.TotalEnergyRequirementValue, 404, 404), // Ignore Defense Parchment, was already correct
        (15, 35, Stats.TotalEnergyRequirementValue, 132, 132), // Increase Health Parchment, was already correct
        (15, 36, Stats.TotalEnergyRequirementValue, 80, 80), // Increase Block Parchment, was already correct
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
            foreach (var (group, number, stat, expected, _) in Season6Requirements)
            {
                Assert.That(GetRequirement(gameConfiguration, group, number, stat), Is.EqualTo(expected), $"Item {group}/{number}, {stat.Designation}");
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
            Assert.That(GetRequirement(gameConfiguration, 12, 8, Stats.TotalEnergy), Is.EqualTo(52));
            Assert.That(GetRequirement(gameConfiguration, 12, 9, Stats.TotalEnergy), Is.EqualTo(72));
            Assert.That(GetRequirement(gameConfiguration, 12, 10, Stats.TotalEnergy), Is.EqualTo(92));
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
        foreach (var (group, number, stat, _, oldValue) in Season6Requirements)
        {
            GetItem(gameConfiguration, group, number).Requirements.First(r => r.Attribute == stat).MinimumValue = oldValue;
        }

        var update = new FixOrbRequirementsPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            foreach (var (group, number, stat, expected, _) in Season6Requirements)
            {
                Assert.That(GetRequirement(gameConfiguration, group, number, stat), Is.EqualTo(expected), $"Item {group}/{number}, {stat.Designation}");
                Assert.That(GetItem(gameConfiguration, group, number).Requirements.Count(r => IsRequirementOf(r, stat)), Is.EqualTo(1), $"Item {group}/{number}, {stat.Designation}");
            }
        });
    }

    /// <summary>
    /// The scrolls of group 15 store their energy requirement as <see cref="Stats.TotalEnergyRequirementValue"/>;
    /// the update must keep that one instead of adding a second one with <see cref="Stats.TotalEnergy"/>.
    /// </summary>
    private static bool IsRequirementOf(AttributeRequirement requirement, AttributeDefinition stat)
    {
        return requirement.Attribute == stat
               || (stat == Stats.TotalEnergyRequirementValue && requirement.Attribute == Stats.TotalEnergy);
    }

    private static ItemDefinition GetItem(GameConfiguration gameConfiguration, byte group, byte number)
    {
        return gameConfiguration.Items.Single(i => i.Group == group && i.Number == number);
    }

    private static double GetRequirement(GameConfiguration gameConfiguration, byte group, byte number, AttributeDefinition attribute)
    {
        return GetItem(gameConfiguration, group, number).Requirements.First(r => r.Attribute == attribute).MinimumValue;
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
