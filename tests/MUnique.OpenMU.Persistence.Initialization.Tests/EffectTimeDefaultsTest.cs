// <copyright file="EffectTimeDefaultsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the <see cref="EffectTimeDefaults"/>, which know the effects with a time in the data of a data initialization.
/// </summary>
[TestFixture]
internal class EffectTimeDefaultsTest
{
    /// <summary>
    /// In the Season 6 data, the effects of consumed items send their time, the skill effects don't, as in the original game:
    /// e.g. not Infinite Arrow or the defense reductions of Fire Slash and Beast Uppercut.
    /// </summary>
    [Test]
    public async Task SeasonSixSendsTheTimeOfItemsAndSomeSkillsAsync()
    {
        var defaults = await CreateDefaults().GetAsync(VersionSeasonSix.DataInitialization.Id).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.GreaterDamage).WithValue(false));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.ElfSoldierBuff).WithValue(false));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.InfiniteArrow).WithValue(false));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.DefenseReduction).WithValue(false));
            Assert.That(defaults.Values, Has.Some.True);
        });
    }

    /// <summary>
    /// The defaults of the installed data are the ones of the data initialization which created it, with its caption.
    /// </summary>
    [Test]
    public async Task InstalledDataGivesItsDefaultsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        using (var context = contextProvider.CreateNewContext())
        {
            var updateState = context.CreateNew<ConfigurationUpdateState>();
            updateState.InitializationKey = VersionSeasonSix.DataInitialization.Id;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var (key, caption, defaults) = await CreateDefaults(contextProvider).GetForInstalledDataAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(key, Is.EqualTo(VersionSeasonSix.DataInitialization.Id));
            Assert.That(caption, Is.EqualTo("1.04d - Season 6 Episode 3"));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.InfiniteArrow).WithValue(false));
        });
    }

    /// <summary>
    /// An unknown data initialization can't give defaults.
    /// </summary>
    [Test]
    public void UnknownInitializationThrows()
    {
        Assert.ThrowsAsync<ReferenceConfigurationFailedException>(async () => await CreateDefaults().GetAsync("unknown").ConfigureAwait(false));
    }

    private static EffectTimeDefaults CreateDefaults(IPersistenceContextProvider? installedData = null)
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint<IDataInitializationPlugIn>(new VersionSeasonSix.DataInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        return new EffectTimeDefaults(plugInManager, NullLoggerFactory.Instance, installedData ?? new InMemoryPersistenceContextProvider());
    }
}
