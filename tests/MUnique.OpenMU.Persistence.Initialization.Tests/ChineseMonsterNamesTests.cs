// <copyright file="ChineseMonsterNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Verifies the monster names of fresh configurations.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ChineseMonsterNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");

    /// <summary>
    /// New configurations include the names, linked to their resource source.
    /// </summary>
    /// <param name="version">The supported initialization version.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task InitializationIncludesChineseNamesAsync(string version)
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
        Assert.That(configuration.Monsters.Single(monster => monster.Number == 4).Designation.GetTranslation(Chinese, false), Is.EqualTo("蛮牛怪"));
        Assert.That(configuration.Monsters.Single(monster => monster.Number == 32).Designation.GetTranslation(Chinese, false), Is.EqualTo("石巨人"));
        if (version == "Season6")
        {
            Assert.That(configuration.Monsters.Single(monster => monster.Number == 49).Designation.GetTranslation(Chinese, false), Is.EqualTo("海魔希特拉"));
            Assert.That(configuration.Monsters.Single(monster => monster.Number == 535).Designation.GetTranslation(Chinese, false), Is.EqualTo("生魂剑士"));
            var gate = configuration.Monsters.Single(monster => monster.Number == 152).Designation.GetTranslation(Chinese, false)!;
            Assert.That(string.Format(gate, "测试角色"), Is.EqualTo("测试角色的卡利玛1入口"));
        }
    }
}
