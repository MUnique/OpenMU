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
/// and Ice Storm skills on an existing database, without duplicating settings.
/// </summary>
[TestFixture]
internal class AreaSkillSettingsUpdateTest
{
    /// <summary>
    /// Tests that the update restores missing area skill settings and the Ice Storm
    /// delay on an existing database, and that applying it twice doesn't
    /// duplicate any settings.
    /// </summary>
    [Test]
    public async Task UpdateRepairsAreaSkillSettingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var decay = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.Decay);
        decay.AreaSkillSettings = null;
        var iceStorm = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.IceStorm);
        iceStorm.AreaSkillSettings!.DelayBetweenHits = TimeSpan.FromMilliseconds(200);

        var update = new AddAreaSkillSettingsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var settingsCount = (await context.GetAsync<AreaSkillSettings>().ConfigureAwait(false)).Count();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(decay.AreaSkillSettings?.EffectRange, Is.EqualTo(2));
        Assert.That(iceStorm.AreaSkillSettings?.DelayBetweenHits, Is.EqualTo(TimeSpan.Zero));
        Assert.That(gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.TripleShot).AreaSkillSettings?.ProjectileCount, Is.EqualTo(3));
        Assert.That((await context.GetAsync<AreaSkillSettings>().ConfigureAwait(false)).Count(), Is.EqualTo(settingsCount));
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
