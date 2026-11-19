// <copyright file="MasterSkillResistanceIncreaseTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

/// <summary>
/// Tests that the resistance increases of the master trees are whole numbers, like the elemental resistances of the
/// characters, in the initialization data and after <see cref="FixJeweleryPetsDamageCalcsPlugInSeason6"/>.
/// </summary>
[TestFixture]
internal class MasterSkillResistanceIncreaseTest
{
    private static readonly SkillNumber[] ResistanceIncreases =
    [
        SkillNumber.PoisonResistanceInc,
        SkillNumber.LightningResistanceInc,
        SkillNumber.IceResistanceIncrease,
        SkillNumber.IncreasePoisonResistance,
        SkillNumber.IncreaseLightningResistance,
        SkillNumber.IncreaseIceResistance,
    ];

    private static readonly SkillNumber[] RageFighterResistanceIncreases =
    [
        SkillNumber.IncreasePoisonResistance,
        SkillNumber.IncreaseLightningResistance,
        SkillNumber.IncreaseIceResistance,
    ];

    /// <summary>
    /// Tests that a new Season 6 database has the resistance increases of all master trees as whole numbers.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasWholeResistanceIncreasesAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        AssertWholeResistanceIncreases(gameConfiguration);
    }

    /// <summary>
    /// Tests that version 2 of the update converts the resistance increases of the Rage Fighter on a database on which
    /// version 1 is installed, and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateFromVersion1ConvertsRageFighterResistanceIncreasesAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        foreach (var skillNumber in RageFighterResistanceIncreases)
        {
            GetMasterDefinition(gameConfiguration, skillNumber).ValueFormula = SkillsInitializer.Formula120Value;
        }

        var update = new FixJeweleryPetsDamageCalcsPlugInSeason6();
        var entry = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).FirstOrDefault(e => e.Key == update.Key)
                    ?? context.CreateNew<ConfigurationUpdate>();
        entry.Key = update.Key;
        entry.Version = 1;

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertWholeResistanceIncreases(gameConfiguration);
        Assert.That(entry.Version, Is.EqualTo(2));
    }

    private static void AssertWholeResistanceIncreases(GameConfiguration gameConfiguration)
    {
        Assert.Multiple(() =>
        {
            foreach (var skillNumber in ResistanceIncreases)
            {
                Assert.That(GetMasterDefinition(gameConfiguration, skillNumber).ValueFormula, Is.EqualTo(SkillsInitializer.Formula120), skillNumber.ToString());
            }
        });
    }

    private static MasterSkillDefinition GetMasterDefinition(GameConfiguration gameConfiguration, SkillNumber skillNumber)
    {
        return gameConfiguration.Skills.Single(s => s.Number == (short)skillNumber).MasterDefinition
               ?? throw new InvalidOperationException($"{skillNumber} has no master definition");
    }
}
