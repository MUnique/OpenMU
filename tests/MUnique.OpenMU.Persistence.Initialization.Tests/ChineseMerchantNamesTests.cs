// <copyright file="ChineseMerchantNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Verifies the merchant names of fresh configurations.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ChineseMerchantNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");
    private static readonly IReadOnlyDictionary<short, string> ExpectedNames = new Dictionary<short, string>
    {
        [230] = "流浪商人阿莱斯",
        [231] = "武器商人托姆绅",
        [242] = "精灵安吉拉",
        [243] = "工匠尤达",
        [244] = "老板娘莉娜",
        [245] = "魔导师露茜",
        [246] = "武器商人苏菲",
        [248] = "流浪商人马丁",
        [250] = "流浪商人海罗德",
        [251] = "铁匠汉斯",
        [253] = "少女安娜",
        [254] = "魔导师帕希",
        [255] = "老板娘莉雅",
        [259] = "雷拉",
        [415] = "塞尔维亚",
        [416] = "雷亚",
        [417] = "摩尔塞",
        [577] = "蕾娜",
        [578] = "贝莱",
    };

    /// <summary>
    /// New configurations include the names, linked to their resource source.
    /// </summary>
    /// <param name="version">The supported initialization version.</param>
    /// <param name="merchantCount">The number of merchants in that version.</param>
    [TestCase("075", 11)]
    [TestCase("095d", 11)]
    [TestCase("Season6", 22)]
    public async Task InitializationIncludesChineseNamesAsync(string version, int merchantCount)
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
        var merchants = configuration.Monsters.Where(monster => monster.MerchantStore is not null).ToList();
        Assert.That(merchants, Has.Count.EqualTo(merchantCount));
        foreach (var merchant in merchants.Where(merchant => ExpectedNames.ContainsKey(merchant.Number)))
        {
            Assert.That(merchant.Designation.GetTranslation(Chinese, false), Is.EqualTo(ExpectedNames[merchant.Number]));
            Assert.That(merchant.Designation.ValueInNeutralLanguage, Does.Not.Contain("||"));
        }
    }
}
