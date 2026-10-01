// <copyright file="ChineseMonsterNamesTests.cs" company="MUnique">
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
/// Verifies fresh initialization and upgrading stored monster names.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ChineseMonsterNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");

    /// <summary>
    /// New configurations include the names and record the corresponding update as installed.
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
        var bull = AddMonster(4, "Elite Bull Fighter||zh=Elite Bull Fighter||de=Elite Stier");
        var hydra = AddMonster(49, "Hydra");
        var larva = AddMonster(12, "Larva||zh=幼虫");
        var custom = AddMonster(32, "Stone Golem||zh=我的Stone巨人");
        var unknown = AddMonster(30000, "Hydra");
        var renamed = AddMonster(77, "Custom Phoenix");
        var gate = AddMonster(152, "Gate to Kalima 1 of {0}");
        var merchant = AddMonster(251, "Hanzo The Blacksmith||zh=铁匠汉斯");
        var provisional = AddMonster(44, "Red Dragon");
        bull.MoveRange = 9;
        bull.AttackRange = 3;
        bull.RespawnDelay = TimeSpan.FromSeconds(42);
        var dropGroup = context.CreateNew<DropItemGroup>();
        dropGroup.Chance = 0.37;
        bull.DropItemGroups.Add(dropGroup);
        var attribute = context.CreateNew<MonsterAttribute>();
        attribute.Value = 1234;
        bull.Attributes.Add(attribute);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.DiscoverAndRegisterPlugInsOf<IConfigurationUpdatePlugIn>();
        var service = new DataUpdateService(provider, manager);
        var available = (await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false))
            .OfType<AddMissingChineseConfigurationNamesPlugInBase>().ToList();
        Assert.That(available.Select(item => item.Key), Is.EqualTo(new[] { update.Key }));
        Assert.That(available.Single().IsMandatory, Is.False);
        await service.ApplyUpdatesAsync(available, new Progress<(Guid, bool)>()).ConfigureAwait(false);

        Assert.That(bull.Designation.GetTranslation(Chinese, false), Is.EqualTo("蛮牛怪"));
        Assert.That(bull.Designation.ValueInNeutralLanguage, Is.EqualTo("Elite Bull Fighter"));
        Assert.That(bull.Designation.GetTranslation(German, false), Is.EqualTo("Elite Stier"));
        Assert.That(hydra.Designation.GetTranslation(Chinese, false), Is.EqualTo("海魔希特拉"));
        Assert.That(larva.Designation.GetTranslation(Chinese, false), Is.EqualTo("幼虫"));
        Assert.That(custom.Designation.GetTranslation(Chinese, false), Is.EqualTo("我的Stone巨人"));
        Assert.That(unknown.Designation.Value, Is.EqualTo("Hydra"));
        Assert.That(renamed.Designation.Value, Is.EqualTo("Custom Phoenix"));
        Assert.That(merchant.Designation.Value, Is.EqualTo("Hanzo The Blacksmith||zh=铁匠汉斯"));
        Assert.That(gate.Designation.GetTranslation(Chinese, false), Is.EqualTo("{0}的卡利玛1入口"));
        Assert.That(provisional.Designation.Value, Is.EqualTo("Red Dragon"));
        Assert.That(bull.Number, Is.EqualTo(4));
        Assert.That(bull.MoveRange, Is.EqualTo(9));
        Assert.That(bull.AttackRange, Is.EqualTo(3));
        Assert.That(bull.RespawnDelay, Is.EqualTo(TimeSpan.FromSeconds(42)));
        Assert.That(bull.DropItemGroups.Single(), Is.SameAs(dropGroup));
        Assert.That(dropGroup.Chance, Is.EqualTo(0.37));
        Assert.That(bull.Attributes.Single(), Is.SameAs(attribute));
        Assert.That(attribute.Value, Is.EqualTo(1234));
        Assert.That(configuration.Monsters, Has.Count.EqualTo(9));
        Assert.That((await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).OfType<AddMissingChineseConfigurationNamesPlugInBase>(), Is.Empty);

        var names = configuration.Monsters.Select(monster => monster.Designation).ToArray();
        await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        Assert.That(configuration.Monsters.Select(monster => monster.Designation), Is.EqualTo(names));

        MonsterDefinition AddMonster(short number, string name)
        {
            var monster = context.CreateNew<MonsterDefinition>();
            monster.Number = number;
            monster.Designation = new LocalizedString(name);
            configuration.Monsters.Add(monster);
            return monster;
        }
    }

    private static AddMissingChineseConfigurationNamesPlugInBase CreateUpdate(string version) => version switch
    {
        "075" => new AddMissingChineseConfigurationNamesPlugIn075(),
        "095d" => new AddMissingChineseConfigurationNamesPlugIn095D(),
        _ => new AddMissingChineseConfigurationNamesPlugInSeason6(),
    };
}
