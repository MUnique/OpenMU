// <copyright file="ChineseMapNamesTests.cs" company="MUnique">
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

/// <summary>Verifies map translations, update discovery, and preservation of map configuration.</summary>
[TestFixture]
[NonParallelizable]
internal class ChineseMapNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");

    /// <summary>Fresh configurations include Chinese map names and mark the update installed.</summary>
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

        var updates = await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false);
        Assert.That(updates.Any(update => update.Key == CreateUpdate(version).Key && update.InstalledAt is not null), Is.True);
    }

    /// <summary>Updates distinguish shared map numbers, preserve custom names, and are idempotent.</summary>
    /// <param name="version">The initialization version.</param>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task StoredMapsAreUpdatedSafelyAsync(string version)
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var update = CreateUpdate(version);
        context.CreateNew<ConfigurationUpdateState>().InitializationKey = update.DataInitializationKey;
        var lorencia = AddMap(0, "Lorencia||de=Stadt||zh=Lorencia");
        var missing = AddMap(3, "Noria");
        var ice = AddMap(57, "LaCleon||zh=狼魂要塞");
        var wolf = AddMap(34, "Crywolf Fortress||zh=狼魂要塞");
        var devil1 = AddMap(9, "Devil Square 1");
        var devil2 = AddMap(9, "Devil Square 2");
        var custom = AddMap(63, "Vulcanus||zh=自定义 PK 地图");
        var renamed = AddMap(65, "Custom Map||zh=幽灵神殿 1");
        var unknown = AddMap(200, "Noria");
        var unverified = AddMap(40, "Silent Map?");
        var gate = context.CreateNew<ExitGate>();
        gate.X1 = 45;
        gate.Y1 = 67;
        lorencia.ExitGates.Add(gate);
        var spawn = context.CreateNew<MonsterSpawnArea>();
        spawn.Quantity = 7;
        lorencia.MonsterSpawns.Add(spawn);
        await context.SaveChangesAsync().ConfigureAwait(false);

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.DiscoverAndRegisterPlugInsOf<IConfigurationUpdatePlugIn>();
        var service = new DataUpdateService(provider, manager);
        var available = (await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).OfType<AddConfigurationNameTranslationsPlugInBase>().ToList();
        Assert.That(available.Select(item => item.Key), Is.EqualTo(new[] { update.Key }));
        Assert.That(available.Single().IsMandatory, Is.False);
        await service.ApplyUpdatesAsync(available, new Progress<(Guid, bool)>()).ConfigureAwait(false);

        Assert.That(lorencia.Name.ValueInNeutralLanguage, Is.EqualTo("Lorencia"));
        Assert.That(lorencia.Name.GetTranslation(Chinese), Is.EqualTo("勇者大陆"));
        Assert.That(lorencia.Name.GetTranslation(CultureInfo.GetCultureInfo("de")), Is.EqualTo("Stadt"));
        Assert.That(missing.Name.GetTranslation(Chinese), Is.EqualTo("仙踪林"));
        Assert.That(ice.Name.GetTranslation(Chinese), Is.EqualTo("狼魂要塞"));
        Assert.That(wolf.Name.GetTranslation(Chinese), Is.EqualTo("狼魂要塞"));
        Assert.That(devil1.Name.GetTranslation(Chinese), Is.EqualTo("恶魔广场 1"));
        Assert.That(devil2.Name.GetTranslation(Chinese), Is.EqualTo("恶魔广场 2"));
        Assert.That(custom.Name.GetTranslation(Chinese), Is.EqualTo("自定义 PK 地图"));
        Assert.That(renamed.Name.Value, Is.EqualTo("Custom Map||zh=幽灵神殿 1"));
        Assert.That(unknown.Name.Value, Is.EqualTo("Noria"));
        Assert.That(unverified.Name.Value, Is.EqualTo("Silent Map?"));
        Assert.That(lorencia.Number, Is.Zero);
        Assert.That(lorencia.ExitGates.Single(), Is.SameAs(gate));
        Assert.That(gate.X1, Is.EqualTo(45));
        Assert.That(gate.Y1, Is.EqualTo(67));
        Assert.That(lorencia.MonsterSpawns.Single(), Is.SameAs(spawn));
        Assert.That(spawn.Quantity, Is.EqualTo(7));
        Assert.That(configuration.Maps, Has.Count.EqualTo(10));
        Assert.That((await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).OfType<AddConfigurationNameTranslationsPlugInBase>(), Is.Empty);
        var names = configuration.Maps.Select(map => map.Name).ToArray();
        await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        Assert.That(configuration.Maps.Select(map => map.Name), Is.EqualTo(names));

        GameMapDefinition AddMap(short number, string name)
        {
            var map = context.CreateNew<GameMapDefinition>();
            map.Number = number;
            map.Name = new LocalizedString(name);
            configuration.Maps.Add(map);
            return map;
        }
    }

    private static AddConfigurationNameTranslationsPlugInBase CreateUpdate(string version) => version switch
    {
        "075" => new AddConfigurationNameTranslationsPlugIn075(),
        "095d" => new AddConfigurationNameTranslationsPlugIn095D(),
        _ => new AddConfigurationNameTranslationsPlugInSeason6(),
    };
}
