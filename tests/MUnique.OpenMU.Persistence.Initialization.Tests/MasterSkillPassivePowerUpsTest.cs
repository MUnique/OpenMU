// <copyright file="MasterSkillPassivePowerUpsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the <see cref="MasterSkillDefinition.PassivePowerUps"/> in the season 6 data.
/// </summary>
[TestFixture]
internal class MasterSkillPassivePowerUpsTest
{
    private const short TwistingSlashMasteryNumber = 332;
    private const short DurabilityReduction1Number = 300;
    private const short DurabilityReduction1FistMasterNumber = 578;

    /// <summary>
    /// Tests that a new season 6 database has the passive power-ups of the master skills.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasPassivePowerUpsAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        AssertPassivePowerUps(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the passive power-ups to a database which was created before they existed,
    /// and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsPassivePowerUpsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var masterDefinition in gameConfiguration.Skills.Select(s => s.MasterDefinition).OfType<MasterSkillDefinition>())
        {
            masterDefinition.PassivePowerUps.Clear();
        }

        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.MasterSkillValue.Id));

        var update = new AddMasterSkillPassivePowerUpsPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertPassivePowerUps(gameConfiguration);
    }

    private static void AssertPassivePowerUps(GameConfiguration gameConfiguration)
    {
        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.MasterSkillValue.Id), Is.EqualTo(1));

        var passiveSkills = gameConfiguration.Skills
            .Where(s => s.SkillType == SkillType.PassiveBoost && s.MasterDefinition?.TargetAttribute is not null)
            .ToList();
        Assert.That(passiveSkills, Is.Not.Empty);
        foreach (var skill in passiveSkills)
        {
            var masterDefinition = skill.MasterDefinition!;
            Assert.That(masterDefinition.PassivePowerUps.Count(p => p.TargetAttribute == masterDefinition.TargetAttribute), Is.EqualTo(1), skill.Name);
        }

        var twistingSlashMastery = GetMasterDefinition(gameConfiguration, TwistingSlashMasteryNumber);
        Assert.That(twistingSlashMastery.PassivePowerUps, Has.Count.EqualTo(1));
        Assert.That(twistingSlashMastery.PassivePowerUps.Single().TargetAttribute, Is.EqualTo(twistingSlashMastery.TargetAttribute));

        foreach (var number in new[] { DurabilityReduction1Number, DurabilityReduction1FistMasterNumber })
        {
            var durabilityReduction = GetMasterDefinition(gameConfiguration, number);
            Assert.That(durabilityReduction.PassivePowerUps, Has.Count.EqualTo(2));
            var factorPowerUp = durabilityReduction.PassivePowerUps.Single(p => p.TargetAttribute == Stats.DurabilityReductionFactor);
            Assert.That(factorPowerUp.Boost!.RelatedValues.Single().InputAttribute, Is.EqualTo(Stats.SkillLevel));
        }
    }

    private static MasterSkillDefinition GetMasterDefinition(GameConfiguration gameConfiguration, short skillNumber)
    {
        return gameConfiguration.Skills.Single(s => s.Number == skillNumber).MasterDefinition!;
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
