// <copyright file="EffectTimeDefaultsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
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
    /// In the Season 6 data, the effects of consumed items and a few skills send their time, the other skills don't.
    /// </summary>
    [Test]
    public async Task SeasonSixSendsTheTimeOfItemsAndSomeSkillsAsync()
    {
        var defaults = await CreateDefaults().GetAsync(VersionSeasonSix.DataInitialization.Id).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.GreaterDamage).WithValue(false));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.ElfSoldierBuff).WithValue(false));
            Assert.That(defaults, Contains.Key((short)MagicEffectNumber.InfiniteArrow).WithValue(true));
            Assert.That(defaults.Values, Has.Some.True);
        });
    }

    /// <summary>
    /// An unknown data initialization can't give defaults.
    /// </summary>
    [Test]
    public void UnknownInitializationThrows()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () => await CreateDefaults().GetAsync("unknown").ConfigureAwait(false));
    }

    private static EffectTimeDefaults CreateDefaults()
    {
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        plugInManager.RegisterPlugInAtPlugInPoint<IDataInitializationPlugIn>(new VersionSeasonSix.DataInitialization(new InMemoryPersistenceContextProvider(), NullLoggerFactory.Instance));
        return new EffectTimeDefaults(plugInManager, NullLoggerFactory.Instance);
    }
}
