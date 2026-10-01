// <copyright file="ChineseMerchantNamesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Verifies fresh initialization and upgrading stored merchant names.
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
    /// New configurations include the names and record the corresponding update as installed.
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

        var updates = await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false);
        Assert.That(updates.Any(update => update.Key == CreateUpdate(version).Key && update.InstalledAt is not null), Is.True);
    }

    /// <summary>
    /// Each version discovers its update once and preserves unrelated values and translations.
    /// </summary>
    /// <param name="version">The supported initialization version.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task ExistingDatabaseUpdateIsDiscoverableAndSafeAsync(string version)
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var update = CreateUpdate(version);
        context.CreateNew<ConfigurationUpdateState>().InitializationKey = update.DataInitializationKey;
        var smith = AddMerchant(251, "Hanzo The Blacksmith||de=Schmied");
        var silvia = AddMerchant(415, "Silvia||zh=西尔维娅");
        var oracle = AddMerchant(259, "Oracle Layla||zh=Oracle Layla");
        var custom = AddMerchant(253, "Potion Girl Amy||zh=我的药水商人");
        var unknown = AddMerchant(999, "Alex");
        var renamed = AddMerchant(230, "Custom Alex");
        var nonMerchant = AddMerchant(254, "Pasi The Mage");
        nonMerchant.MerchantStore = null;
        var provisional = AddMerchant(545, "Christine the General Goods Merchant");
        var store = smith.MerchantStore!;
        var item = context.CreateNew<MUnique.OpenMU.DataModel.Entities.Item>();
        item.Level = 7;
        store.Items.Add(item);
        smith.MoveRange = 9;
        await context.SaveChangesAsync().ConfigureAwait(false);

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.DiscoverAndRegisterPlugInsOf<IConfigurationUpdatePlugIn>();
        var service = new DataUpdateService(provider, manager);
        var available = (await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false))
            .OfType<AddConfigurationNameTranslationsPlugInBase>().ToList();
        Assert.That(available.Select(item => item.Key), Is.EqualTo(new[] { update.Key }));
        Assert.That(available.Single().IsMandatory, Is.False);
        await service.ApplyUpdatesAsync(available, new Progress<(Guid, bool)>()).ConfigureAwait(false);

        Assert.That(smith.Designation.GetTranslation(Chinese, false), Is.EqualTo("铁匠汉斯"));
        Assert.That(smith.Designation.ValueInNeutralLanguage, Is.EqualTo("Hanzo The Blacksmith"));
        Assert.That(smith.Designation.GetTranslation(German, false), Is.EqualTo("Schmied"));
        Assert.That(silvia.Designation.GetTranslation(Chinese, false), Is.EqualTo("西尔维娅"));
        Assert.That(oracle.Designation.GetTranslation(Chinese, false), Is.EqualTo("雷拉"));
        Assert.That(custom.Designation.GetTranslation(Chinese, false), Is.EqualTo("我的药水商人"));
        Assert.That(unknown.Designation.Value, Is.EqualTo("Alex"));
        Assert.That(renamed.Designation.Value, Is.EqualTo("Custom Alex"));
        Assert.That(nonMerchant.Designation.Value, Is.EqualTo("Pasi The Mage"));
        Assert.That(provisional.Designation.Value, Is.EqualTo("Christine the General Goods Merchant"));
        Assert.That(smith.Number, Is.EqualTo(251));
        Assert.That(smith.MoveRange, Is.EqualTo(9));
        Assert.That(smith.MerchantStore, Is.SameAs(store));
        Assert.That(store.Items.Single(), Is.SameAs(item));
        Assert.That(item.Level, Is.EqualTo(7));
        Assert.That(configuration.Monsters, Has.Count.EqualTo(8));
        Assert.That((await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).OfType<AddConfigurationNameTranslationsPlugInBase>(), Is.Empty);

        var names = configuration.Monsters.Select(monster => monster.Designation).ToArray();
        await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        Assert.That(configuration.Monsters.Select(monster => monster.Designation), Is.EqualTo(names));

        MonsterDefinition AddMerchant(short number, string name)
        {
            var merchant = context.CreateNew<MonsterDefinition>();
            merchant.Number = number;
            merchant.Designation = new LocalizedString(name);
            merchant.MerchantStore = context.CreateNew<MUnique.OpenMU.DataModel.Entities.ItemStorage>();
            configuration.Monsters.Add(merchant);
            return merchant;
        }
    }

    private static AddConfigurationNameTranslationsPlugInBase CreateUpdate(string version) => version switch
    {
        "075" => new AddConfigurationNameTranslationsPlugIn075(),
        "095d" => new AddConfigurationNameTranslationsPlugIn095D(),
        _ => new AddConfigurationNameTranslationsPlugInSeason6(),
    };
}
