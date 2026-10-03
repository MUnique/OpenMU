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

/// <summary>Checks the configuration name resources and their usage by the initializations.</summary>
[TestFixture]
[NonParallelizable]
internal class ConfigurationNameResourcesTests
{
    private static readonly (string Name, ResourceManager ResourceManager)[] Sources =
    [
        (nameof(CharacterClassNames), CharacterClassNames.ResourceManager),
        (nameof(MapNames), MapNames.ResourceManager),
        (nameof(MerchantNames), MerchantNames.ResourceManager),
        (nameof(MonsterNames), MonsterNames.ResourceManager),
    ];

    /// <summary>The resources are registered as sources, so source keys can be resolved without running an initialization.</summary>
    [Test]
    public void ResourcesAreRegisteredAsSources()
    {
        ConfigurationNameSources.Register();
        foreach (var (name, resourceManager) in Sources)
        {
            Assert.That(LocalizedStringResources.TryGetName(resourceManager, out var registeredName), Is.True, name);
            Assert.That(registeredName, Is.EqualTo(name));
        }

        Assert.That(new LocalizedString("Lorencia").WithSourceKey("MapNames/Lorencia").GetFromSource()?.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("勇者大陆"));
    }

    /// <summary>Every key of a satellite resource exists in the neutral resource.</summary>
    [Test]
    public void SatelliteKeysExistInNeutralResources()
    {
        foreach (var (name, resourceManager) in Sources)
        {
            var neutralKeys = GetKeys(resourceManager, CultureInfo.InvariantCulture);
            Assert.That(resourceManager.AvailableCultures, Is.Not.Empty, name);
            foreach (var culture in resourceManager.AvailableCultures)
            {
                Assert.That(GetKeys(resourceManager, culture), Is.SubsetOf(neutralKeys), $"{name}: {culture.Name}");
            }
        }
    }

    /// <summary>
    /// The built-in names of all initializations are taken from the resources, including their source keys,
    /// and every resource key is used by at least one initialization.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ResourceKeysMatchInitializedEntitiesAsync()
    {
        var usedSourceKeys = new HashSet<string>(StringComparer.Ordinal);
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
            var names = configuration.CharacterClasses.Select(c => c.Name)
                .Concat(configuration.Maps.Select(m => m.Name))
                .Concat(configuration.Monsters.Select(m => m.Designation));
            foreach (var name in names.Where(n => n.SourceKey is not null))
            {
                usedSourceKeys.Add(name.SourceKey!);
                Assert.That(name.GetFromSource()?.Value, Is.EqualTo(name.Value), $"{version}: {name.SourceKey}");
                Assert.That(name.IsUnchangedSinceSourceStamp, Is.True, $"{version}: {name.SourceKey}");
            }
        }

        var allSourceKeys = Sources.SelectMany(source => GetKeys(source.ResourceManager, CultureInfo.InvariantCulture)
            .Select(key => LocalizedStringResources.CreateSourceKey(source.Name, key)));
        Assert.That(usedSourceKeys, Is.EquivalentTo(allSourceKeys));
    }

    private static IReadOnlyList<string> GetKeys(ResourceManager resourceManager, CultureInfo culture)
    {
        return resourceManager.GetResourceSet(culture, true, false)!
            .Cast<DictionaryEntry>()
            .Select(entry => (string)entry.Key)
            .ToList();
    }
}
