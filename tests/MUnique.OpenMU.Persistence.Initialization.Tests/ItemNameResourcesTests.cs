// <copyright file="ItemNameResourcesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Verifies item name resource coverage and the existing caption update workflow.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class ItemNameResourcesTests
{
    /// <summary>
    /// Persisted source keys resolve without initializing a reference configuration.
    /// </summary>
    [Test]
    public void SourcesResolveWithoutInitialization()
    {
        ConfigurationNameSources.Register();
        var name = new LocalizedString("Kris").WithSourceKey("ItemNames/Kris");
        Assert.That(name.GetFromSource()?.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("波刃剑"));
    }

    /// <summary>
    /// Every supported configuration initializes its item names from resources, and existing
    /// neutral names can be linked and translated through the caption workflow.
    /// </summary>
    /// <param name="version">The configuration version.</param>
    /// <returns>The task.</returns>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task ItemNamesUseCaptionWorkflowAsync(string version)
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
        var culture = CultureInfo.GetCultureInfo("zh-CN");
        var items = configuration.Items.Where(item => item.Name.SourceKey?.StartsWith("ItemNames/", StringComparison.Ordinal) == true).ToArray();
        Assert.That(items, Is.Not.Empty);
        Assert.That(items.All(item => !string.IsNullOrEmpty(item.Name.GetOwnTranslation(culture))), Is.True);
        var expected = items.Select(item => item.Name.GetOwnTranslation(culture)).ToArray();
        var referenceNames = items.Select(item => item.Name).ToArray();
        // Keep the initialized configuration as a reference, and remove translations and source keys
        // from a separate configuration, as on an existing installation.
        using var existingContext = provider.CreateNewContext();
        var existing = existingContext.CreateNew<GameConfiguration>();
        foreach (var item in items)
        {
            var copy = existingContext.CreateNew<ItemDefinition>();
            ((IIdentifiable)copy).Id = item.GetId();
            copy.Group = item.Group;
            copy.Number = item.Number;
            copy.Name = new LocalizedString(item.Name.ValueInNeutralLanguage);
            existing.Items.Add(copy);
        }

        var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(existing, configuration);
        Assert.That(linked, Is.EqualTo(items.Length));
        Assert.That(skipped, Is.Zero);
        Assert.That(existing.Items.All(item => item.Name.GetOwnTranslation(culture) is null), Is.True);
        var changes = ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == culture.Name).ToArray();
        Assert.That(changes, Has.Length.EqualTo(items.Length));
        Assert.That(changes.All(change => change.IsRecommended), Is.True);
        ConfigurationCaptions.ApplyChanges(existing, changes.Select(change => change.Id));
        Assert.That(existing.Items.Select(item => item.Name.GetOwnTranslation(culture)), Is.EqualTo(expected));
        Assert.That(ConfigurationCaptions.DetermineChanges(existing), Is.Empty);
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, configuration).Linked, Is.Zero);

        if (version == "Season6")
        {
            var keys = ItemNames.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false)!
                .Cast<DictionaryEntry>().Select(entry => $"ItemNames/{entry.Key}").ToArray();
            Assert.That(referenceNames.Select(name => name.SourceKey).Distinct(), Is.EquivalentTo(keys));
        }
    }
}
