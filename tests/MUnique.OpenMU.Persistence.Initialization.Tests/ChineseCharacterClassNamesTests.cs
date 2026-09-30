// <copyright file="ChineseCharacterClassNamesTests.cs" company="MUnique">
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
/// Verifies fresh initialization and upgrading stored character class names.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ChineseCharacterClassNamesTests
{
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");
    private static readonly IReadOnlyDictionary<byte, string> OfficialNames = new Dictionary<byte, string>
    {
        [0] = "魔法师",
        [2] = "魔导师",
        [3] = "神导师",
        [4] = "剑士",
        [6] = "骑士",
        [7] = "神骑士",
        [8] = "弓箭手",
        [10] = "圣射手",
        [11] = "神射手",
        [12] = "魔剑士",
        [13] = "剑圣",
        [16] = "圣导师",
        [17] = "祭祀",
        [20] = "召唤术师",
        [22] = "召唤导师",
        [23] = "召唤巫师",
        [24] = "格斗家",
        [25] = "格斗大师",
    };

    /// <summary>
    /// New configurations include the names and record the corresponding update as installed.
    /// </summary>
    /// <param name="version">The supported initialization version.</param>
    /// <param name="classCount">The number of classes in that version.</param>
    [TestCase("075", 3)]
    [TestCase("095d", 4)]
    [TestCase("Season6", 18)]
    public async Task InitializationIncludesOfficialNamesAsync(string version, int classCount)
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
        Assert.That(configuration.CharacterClasses, Has.Count.EqualTo(classCount));
        foreach (var characterClass in configuration.CharacterClasses)
        {
            Assert.That(characterClass.Name.GetTranslation(Chinese, false), Is.EqualTo(OfficialNames[characterClass.Number]));
            Assert.That(characterClass.Name.ValueInNeutralLanguage, Does.Not.Contain("||"));
        }

        var updates = await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false);
        Assert.That(updates.Any(update => update.Version == (int)CreateUpdate(version).Version && update.InstalledAt is not null), Is.True);
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
        var wizard = AddClass(0, "Dark Wizard||zh=Dark Wizard||de=Zauberer");
        var knight = AddClass(6, "Blade Knight||zh=剑圣");
        var fighter = AddClass(24, "Rage Fighter||zh=圣导师");
        var summoner = AddClass(22, "Bloody Summoner");
        var custom = AddClass(4, "Dark Knight||zh=自定义剑士");
        var unknown = AddClass(255, "Dark Wizard||zh=自定义职业");
        var renamed = AddClass(3, "Custom Master||zh=自定义大师");
        wizard.NextGenerationClass = knight;
        wizard.LevelRequirementByCreation = 123;
        await context.SaveChangesAsync().ConfigureAwait(false);

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        manager.DiscoverAndRegisterPlugInsOf<IConfigurationUpdatePlugIn>();
        var service = new DataUpdateService(provider, manager);
        var available = (await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false))
            .OfType<AlignChineseConfigurationNamesPlugInBase>().ToList();
        Assert.That(available.Select(item => item.Version), Is.EqualTo(new[] { update.Version }));
        Assert.That(available.Single().IsMandatory, Is.False);
        await service.ApplyUpdatesAsync(available, new Progress<(UpdateVersion, bool)>()).ConfigureAwait(false);

        Assert.That(wizard.Name.GetTranslation(Chinese, false), Is.EqualTo("魔法师"));
        Assert.That(wizard.Name.ValueInNeutralLanguage, Is.EqualTo("Dark Wizard"));
        Assert.That(wizard.Name.GetTranslation(German, false), Is.EqualTo("Zauberer"));
        Assert.That(knight.Name.GetTranslation(Chinese, false), Is.EqualTo("剑圣"));
        Assert.That(fighter.Name.GetTranslation(Chinese, false), Is.EqualTo("圣导师"));
        Assert.That(summoner.Name.GetTranslation(Chinese, false), Is.EqualTo("召唤导师"));
        Assert.That(custom.Name.GetTranslation(Chinese, false), Is.EqualTo("自定义剑士"));
        Assert.That(unknown.Name.GetTranslation(Chinese, false), Is.EqualTo("自定义职业"));
        Assert.That(renamed.Name.ValueInNeutralLanguage, Is.EqualTo("Custom Master"));
        Assert.That(renamed.Name.GetTranslation(Chinese, false), Is.EqualTo("自定义大师"));
        Assert.That(wizard.Number, Is.Zero);
        Assert.That(wizard.NextGenerationClass, Is.SameAs(knight));
        Assert.That(wizard.LevelRequirementByCreation, Is.EqualTo(123));
        Assert.That(configuration.CharacterClasses, Has.Count.EqualTo(7));
        Assert.That((await service.DetermineAvailableUpdatesAsync().ConfigureAwait(false)).OfType<AlignChineseConfigurationNamesPlugInBase>(), Is.Empty);

        var names = configuration.CharacterClasses.Select(item => item.Name).ToArray();
        await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        Assert.That(configuration.CharacterClasses.Select(item => item.Name), Is.EqualTo(names));

        CharacterClass AddClass(byte number, string name)
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Number = number;
            characterClass.Name = new LocalizedString(name);
            configuration.CharacterClasses.Add(characterClass);
            return characterClass;
        }
    }

    private static AlignChineseConfigurationNamesPlugInBase CreateUpdate(string version) => version switch
    {
        "075" => new AlignChineseConfigurationNamesPlugIn075(),
        "095d" => new AlignChineseConfigurationNamesPlugIn095D(),
        _ => new AlignChineseConfigurationNamesPlugInSeason6(),
    };
}
