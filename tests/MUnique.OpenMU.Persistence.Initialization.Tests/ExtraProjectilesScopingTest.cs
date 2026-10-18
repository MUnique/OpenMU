// <copyright file="ExtraProjectilesScopingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the extra projectiles are scoped per skill: the generic pool of the (cross)bows
/// applies to both arrow skills, while the triple shot mastery only boosts the triple shot.
/// </summary>
[TestFixture]
internal class ExtraProjectilesScopingTest
{
    /// <summary>
    /// Tests that a freshly initialized season 6 database scopes the extra projectiles correctly.
    /// </summary>
    [Test]
    public async Task FreshDatabaseScopesExtraProjectilesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertScoped(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update scopes the extra projectiles on a database which still leaks the mastery
    /// bonus into the generic pool, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateScopesExtraProjectilesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        SimulateLegacyDatabase(gameConfiguration);

        var update = new ScopeExtraProjectilesPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertScoped(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update depends on the updates which provide the target attributes it needs.
    /// </summary>
    [Test]
    public void UpdateDependsOnElfUpdates()
    {
        var update = new ScopeExtraProjectilesPlugIn();

        Assert.Multiple(() =>
        {
            Assert.That(update.DependsOn.Select(d => d.Key), Does.Contain(typeof(FinishElfMasterTreePlugIn).GUID));
            Assert.That(update.DependsOn.Select(d => d.Key), Does.Contain(typeof(AddMasterSkillPassivePowerUpsPlugIn).GUID));
            Assert.That(update.IsMandatory, Is.True);
        });
    }

    private static void AssertScoped(GameConfiguration gameConfiguration)
    {
        Assert.That(gameConfiguration.Attributes.Any(a => a.Id == Stats.SkillExtraProjectiles.Id), Is.True);
        Assert.That(gameConfiguration.Attributes.Any(a => a.Id == Stats.TripleShotExtraProjectiles.Id), Is.True);

        var tripleShot = GetSkill(gameConfiguration, SkillNumber.TripleShot);
        Assert.Multiple(() =>
        {
            Assert.That(HasRelationship(tripleShot, Stats.SkillExtraProjectiles, Stats.ExtraProjectiles), Is.True);
            Assert.That(HasRelationship(tripleShot, Stats.SkillExtraProjectiles, Stats.TripleShotExtraProjectiles), Is.True);
        });

        var multiShot = GetSkill(gameConfiguration, SkillNumber.MultiShot);
        Assert.Multiple(() =>
        {
            Assert.That(HasRelationship(multiShot, Stats.SkillExtraProjectiles, Stats.ExtraProjectiles), Is.True);
            Assert.That(HasRelationship(multiShot, Stats.SkillExtraProjectiles, Stats.TripleShotExtraProjectiles), Is.False, "The mastery bonus is exclusive to the triple shot.");
        });

        var mastery = GetMasterDefinition(gameConfiguration, SkillNumber.TripleShotMastery);
        Assert.Multiple(() =>
        {
            Assert.That(mastery.TargetAttribute!.Id, Is.EqualTo(Stats.TripleShotExtraProjectiles.Id));
            Assert.That(mastery.PassivePowerUps, Has.Count.EqualTo(1));
            Assert.That(mastery.PassivePowerUps.Single().TargetAttribute!.Id, Is.EqualTo(Stats.TripleShotExtraProjectiles.Id));
        });
    }

    private static void SimulateLegacyDatabase(GameConfiguration gameConfiguration)
    {
        // The legacy database knew neither the skill-scoped attribute nor the exclusive mastery pool,
        // so the mastery bonus was applied to the generic pool which every projectile skill uses.
        foreach (var skillNumber in new[] { SkillNumber.TripleShot, SkillNumber.MultiShot })
        {
            var skill = GetSkill(gameConfiguration, skillNumber);
            while (skill.AttributeRelationships.FirstOrDefault(r => r.TargetAttribute?.Id == Stats.SkillExtraProjectiles.Id) is { } relationship)
            {
                skill.AttributeRelationships.Remove(relationship);
            }
        }

        var extraProjectiles = gameConfiguration.Attributes.Single(a => a.Id == Stats.ExtraProjectiles.Id);
        var mastery = GetMasterDefinition(gameConfiguration, SkillNumber.TripleShotMastery);
        mastery.TargetAttribute = extraProjectiles;
        foreach (var powerUp in mastery.PassivePowerUps)
        {
            powerUp.TargetAttribute = extraProjectiles;
        }

        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.SkillExtraProjectiles.Id));
        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.TripleShotExtraProjectiles.Id));
    }

    private static bool HasRelationship(Skill skill, AttributeDefinition target, AttributeDefinition source)
        => skill.AttributeRelationships.Any(r => r.TargetAttribute?.Id == target.Id && r.InputAttribute?.Id == source.Id);

    private static Skill GetSkill(GameConfiguration gameConfiguration, SkillNumber skillNumber)
        => gameConfiguration.Skills.Single(s => s.Number == (short)skillNumber);

    private static MasterSkillDefinition GetMasterDefinition(GameConfiguration gameConfiguration, SkillNumber skillNumber)
        => GetSkill(gameConfiguration, skillNumber).MasterDefinition!;

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
