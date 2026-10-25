// <copyright file="AreaSkillSettingsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the versioned area skill settings update repairs the Hellfire, Decay
/// and Ice Storm skills on an existing database, guarantees the first hit of the
/// delayed Flame, Twister and Evil Spirit skills, without duplicating settings.
/// </summary>
[TestFixture]
internal class AreaSkillSettingsUpdateTest
{
    /// <summary>
    /// Tests that a freshly initialized database already guarantees the first hit
    /// of the delayed skills and spans the full Twister reach.
    /// </summary>
    [Test]
    public async Task FreshDatabaseGuaranteesFirstHitOfDelayedSkillsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        Assert.Multiple(() =>
        {
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).FrustumDistance, Is.EqualTo(6f));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Tests that a freshly initialized database already applies the base skill's area
    /// settings to its mastery version, which is what players actually cast.
    /// </summary>
    [Test]
    public async Task FreshDatabaseSyncsMasteryAreaSkillSettingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var tripleShot = GetSettings(gameConfiguration, SkillNumber.TripleShot);
        var mastery = GetSettings(gameConfiguration, SkillNumber.TripleShotMastery);

        Assert.Multiple(() =>
        {
            Assert.That(mastery.UseFrustumFilter, Is.EqualTo(tripleShot.UseFrustumFilter));
            Assert.That(mastery.ProjectileCount, Is.EqualTo(tripleShot.ProjectileCount));
            Assert.That(mastery.DelayBetweenHits, Is.EqualTo(tripleShot.DelayBetweenHits));
        });
    }

    /// <summary>
    /// Tests that the update repairs missing area skill settings on an existing
    /// database.
    /// </summary>
    [Test]
    public async Task UpdateRepairsMissingAreaSkillSettingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.Decay).AreaSkillSettings = null;

        await new AddAreaSkillSettingsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.Decay).AreaSkillSettings?.EffectRange, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that the update repairs the data of databases which were seeded before
    /// delayed projectiles guaranteed their first hit.
    /// </summary>
    [Test]
    public async Task UpdateRepairsLegacyDelayedSkillsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.Twister).FrustumDistance = 4f;
        GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget = 0;
        var iceStorm = GetSettings(gameConfiguration, SkillNumber.IceStorm);
        iceStorm.DelayBetweenHits = TimeSpan.FromMilliseconds(200);
        var tripleShotMastery = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.TripleShotMastery);
        tripleShotMastery.AreaSkillSettings = context.CreateNew<AreaSkillSettings>();
        tripleShotMastery.AreaSkillSettings.DelayBetweenHits = TimeSpan.FromMilliseconds(999);

        await new AddAreaSkillSettingsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).FrustumDistance, Is.EqualTo(6f));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.IceStorm).DelayBetweenHits, Is.EqualTo(TimeSpan.Zero));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.TripleShot).ProjectileCount, Is.EqualTo(3));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.TripleShot).DelayBetweenHits, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.MultiShot).ProjectileCount, Is.EqualTo(5));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.MultiShot).DelayBetweenHits, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.TripleShotMastery).DelayBetweenHits, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.TripleShotMastery).ProjectileCount, Is.EqualTo(3));
        });
    }

    /// <summary>
    /// Tests that the update keeps Force Wave targeted while preserving its
    /// frustum settings.
    /// </summary>
    [Test]
    public async Task UpdateKeepsForceWaveTargetedAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var forceWave = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.ForceWave);
        forceWave.SkillType = SkillType.AreaSkillAutomaticHits;

        await new AddAreaSkillSettingsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(forceWave.SkillType, Is.EqualTo(SkillType.DirectHit));
            Assert.That(forceWave.AreaSkillSettings?.UseFrustumFilter, Is.True);
        });
    }

    /// <summary>
    /// Tests that applying the update twice doesn't duplicate any settings
    /// and keeps the repaired values.
    /// </summary>
    [Test]
    public async Task ApplyingUpdateTwiceChangesNothingAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var update = new AddAreaSkillSettingsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var settingsCount = (await context.GetAsync<AreaSkillSettings>().ConfigureAwait(false)).Count();

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var settingsCountAfterSecondRun = (await context.GetAsync<AreaSkillSettings>().ConfigureAwait(false)).Count();

        Assert.Multiple(() =>
        {
            Assert.That(settingsCountAfterSecondRun, Is.EqualTo(settingsCount));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).FrustumDistance, Is.EqualTo(6f));
            Assert.That(gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.ForceWave).SkillType, Is.EqualTo(SkillType.DirectHit));
        });
    }

    /// <summary>
    /// Tests that the 0.75 update repairs the delayed skills and skips the
    /// skills which don't exist in that game version.
    /// </summary>
    [Test]
    public async Task Update075RepairsDelayedSkillsAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new Version075.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;
        GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget = 0;

        await new AddAreaSkillSettingsUpdatePlugIn075().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Tests that the 0.95d update repairs the delayed skills and skips the
    /// skills which don't exist in that game version.
    /// </summary>
    [Test]
    public async Task Update095dRepairsDelayedSkillsAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(provider => new Version095d.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
        using var _ = context;
        GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget = 0;
        GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget = 0;

        await new AddAreaSkillSettingsUpdatePlugIn095d().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Twister).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.EvilSpirit).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
            Assert.That(GetSettings(gameConfiguration, SkillNumber.Flame).MinimumNumberOfHitsPerTarget, Is.EqualTo(1));
        });
    }

    private static AreaSkillSettings GetSettings(GameConfiguration gameConfiguration, SkillNumber skillNumber)
    {
        return gameConfiguration.Skills.First(s => s.Number == (short)skillNumber).AreaSkillSettings!;
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        return await CreateConfigurationAsync(provider => new VersionSeasonSix.DataInitialization(provider, new NullLoggerFactory())).ConfigureAwait(false);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(Func<InMemoryPersistenceContextProvider, DataInitializationBase> createInitialization)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await createInitialization(contextProvider).CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
