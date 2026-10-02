// <copyright file="ChineseMapNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Verifies the map translations of fresh configurations.</summary>
[TestFixture]
[NonParallelizable]
internal class ChineseMapNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");

    /// <summary>Fresh configurations include Chinese map names.</summary>
    /// <param name="version">The initialization version.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task FreshMapsAreTranslatedAsync(string version)
    {
        var provider = new InMemoryPersistenceContextProvider();
        DataInitializationBase initializer = version switch
        {
            "075" => new Version075.DataInitialization(provider, NullLoggerFactory.Instance),
            "095d" => new Version095d.DataInitialization(provider, NullLoggerFactory.Instance),
            _ => new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance),
        };
        await initializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        Assert.That(configuration.Maps.Single(map => map.Number == 0).Name.GetTranslation(Chinese), Is.EqualTo("勇者大陆"));
        Assert.That(configuration.Maps.Single(map => map.Number == 3).Name.GetTranslation(Chinese), Is.EqualTo("仙踪林"));
        if (version == "Season6")
        {
            Assert.That(configuration.Maps.Single(map => map.Number == 57).Name.GetTranslation(Chinese), Is.EqualTo("冰霜之城"));
            Assert.That(configuration.Maps.Single(map => map.Number == 63).Name.GetTranslation(Chinese), Is.EqualTo("囚禁之岛"));
            Assert.That(configuration.Maps.Single(map => map.Number == 65).Name.GetTranslation(Chinese), Is.EqualTo("生魂广场 1"));
        }
    }
}
