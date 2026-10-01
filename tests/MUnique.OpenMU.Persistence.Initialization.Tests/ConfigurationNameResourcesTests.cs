// <copyright file="ConfigurationNameResourcesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections;
using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Checks resource coverage and direct initialization for every available language.</summary>
[TestFixture]
[NonParallelizable]
internal class ConfigurationNameResourcesTests
{
    /// <summary>Satellite keys map to neutral resources, and every mapping is used by a supported initializer.</summary>
    [Test]
    public async Task ResourceKeysMatchInitializedEntitiesAsync()
    {
        var categories = new[]
        {
            (CharacterClassNames.ResourceManager, ConfigurationNameTranslations.CharacterClassMappings),
            (MapNames.ResourceManager, ConfigurationNameTranslations.MapMappings),
            (MerchantNames.ResourceManager, ConfigurationNameTranslations.MerchantMappings),
            (MonsterNames.ResourceManager, ConfigurationNameTranslations.MonsterMappings),
        };
        var used = categories.Select(_ => new HashSet<string>(StringComparer.Ordinal)).ToArray();
        foreach (var (resources, mappings) in categories)
        {
            var neutralKeys = resources.GetResourceSet(CultureInfo.InvariantCulture, true, false)!
                .Cast<DictionaryEntry>().Select(entry => (string)entry.Key).ToArray();
            Assert.That(mappings.Select(mapping => mapping.Key), Is.EquivalentTo(neutralKeys), resources.BaseName);
            foreach (var culture in resources.AvailableCultures)
            {
                var keys = resources.GetResourceSet(culture, true, false)!.Cast<DictionaryEntry>().Select(entry => (string)entry.Key);
                Assert.That(keys, Is.SubsetOf(neutralKeys), $"{resources.BaseName}: {culture.Name}");
            }
        }

        foreach (var version in new[] { "075", "095d", "Season6" })
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
            var entities = new[]
            {
                configuration.CharacterClasses.Select(c => ((int)c.Number, c.Name)),
                configuration.Maps.Select(m => ((int)m.Number, m.Name)),
                configuration.Monsters.Where(m => m.MerchantStore is not null).Select(m => ((int)m.Number, m.Designation)),
                configuration.Monsters.Select(m => ((int)m.Number, m.Designation)),
            };
            for (var category = 0; category < categories.Length; category++)
            {
                var (resources, mappings) = categories[category];
                foreach (var (number, key) in mappings)
                {
                    var expected = resources.GetLocalizedString(key);
                    foreach (var (_, actual) in entities[category].Where(e => e.Item1 == number && e.Item2.ValueInNeutralLanguage == expected.ValueInNeutralLanguage))
                    {
                        used[category].Add(key);
                        Assert.That(actual.Value, Is.EqualTo(expected.Value), $"{version}: {resources.BaseName}.{key}");
                    }
                }
            }
        }

        for (var category = 0; category < categories.Length; category++)
        {
            Assert.That(used[category], Is.EquivalentTo(categories[category].Item2.Select(mapping => mapping.Key)), categories[category].ResourceManager.BaseName);
        }
    }

    /// <summary>Adding one regional translation preserves siblings and legacy language-only custom values.</summary>
    [Test]
    public void UpdatePreservesCultureSpecificCustomizations()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var sibling = AddClass("Dark Wizard||zh-TW=自訂繁體||de=Magier");
        var parent = AddClass("Dark Wizard||zh=自定义旧中文");
        var exact = AddClass("Dark Wizard||zh-CN=自定义简体||zh-TW=自訂繁體");
        var parentCopy = AddClass("Dark Wizard||zh=Dark Wizard");
        ConfigurationNameTranslations.Apply(configuration);
        Assert.That(sibling.Name.GetTranslation(CultureInfo.GetCultureInfo("zh-CN"), false), Is.EqualTo("魔法师"));
        Assert.That(sibling.Name.GetTranslation(CultureInfo.GetCultureInfo("zh-TW"), false), Is.EqualTo("自訂繁體"));
        Assert.That(sibling.Name.GetTranslation(CultureInfo.GetCultureInfo("de"), false), Is.EqualTo("Magier"));
        Assert.That(parent.Name.Value, Is.EqualTo("Dark Wizard||zh=自定义旧中文"));
        Assert.That(exact.Name.Value, Is.EqualTo("Dark Wizard||zh-CN=自定义简体||zh-TW=自訂繁體"));
        Assert.That(parentCopy.Name.Value, Does.Contain("||zh-CN=魔法师"));
        var names = configuration.CharacterClasses.Select(c => c.Name.Value).ToArray();
        ConfigurationNameTranslations.Apply(configuration);
        Assert.That(configuration.CharacterClasses.Select(c => c.Name.Value), Is.EqualTo(names));

        CharacterClass AddClass(string name)
        {
            var characterClass = context.CreateNew<CharacterClass>();
            characterClass.Number = 0;
            characterClass.Name = new LocalizedString(name);
            configuration.CharacterClasses.Add(characterClass);
            return characterClass;
        }
    }
}
