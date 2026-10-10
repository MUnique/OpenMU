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
    /// The German names use the same terms as the German game client.
    /// </summary>
    [Test]
    public void GermanNamesAreResolvedFromSources()
    {
        ConfigurationNameSources.Register();
        var german = CultureInfo.GetCultureInfo("de");
        Assert.That(new LocalizedString("Short Sword").WithSourceKey("ItemNames/ShortSword").GetFromSource()?.GetOwnTranslation(german), Is.EqualTo("Kurzschwert"));
        Assert.That(new LocalizedString("Jewel of Bless").WithSourceKey("ItemNames/JewelOfBless").GetFromSource()?.GetOwnTranslation(german), Is.EqualTo("Juwel des Segens"));
        Assert.That(new LocalizedString("Great Dragon Helm").WithSourceKey("ItemNames/GreatDragonHelm").GetFromSource()?.GetOwnTranslation(german), Is.EqualTo("Großartiger Drachenhelm"));
        Assert.That(new LocalizedString("Kris").WithSourceKey("ItemNames/Kris").GetFromSource()?.GetOwnTranslation(german), Is.Null, "names which are equal in German fall back to the neutral name");
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

        var reference = CaptionLinkReference.Create(configuration);
        Assert.That(ConfigurationCaptions.FindLinkableCaptions(existing, reference), Is.EquivalentTo(new Dictionary<string, int> { { nameof(ItemDefinition), items.Length } }));
        var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(existing, reference);
        Assert.That(linked, Is.EqualTo(items.Length));
        Assert.That(skipped, Is.Zero);
        Assert.That(existing.Items.All(item => item.Name.GetOwnTranslation(culture) is null), Is.True);
        var changes = ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == culture.Name).ToArray();
        Assert.That(changes, Has.Length.EqualTo(items.Length));
        Assert.That(changes.All(change => change.IsRecommended), Is.True);
        ConfigurationCaptions.ApplyChanges(existing, changes.Select(change => change.Id));
        Assert.That(existing.Items.Select(item => item.Name.GetOwnTranslation(culture)), Is.EqualTo(expected));
        Assert.That(ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == culture.Name), Is.Empty);
        Assert.That(ConfigurationCaptions.FindLinkableCaptions(existing, reference), Is.Empty);
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, configuration).Linked, Is.Zero);

        if (version == "Season6")
        {
            var keys = ItemNames.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false)!
                .Cast<DictionaryEntry>().Select(entry => $"ItemNames/{entry.Key}").ToArray();
            Assert.That(referenceNames.Select(name => name.SourceKey).Distinct(), Is.EquivalentTo(keys));
        }
    }
    /// <summary>
    /// Money fallback descriptions retain their neutral text without inheriting item-name metadata.
    /// </summary>
    /// <param name="version">The configuration version.</param>
    /// <param name="cultureName">The culture used during initialization.</param>
    /// <returns>The task.</returns>
    [TestCase("095d", "en-US")]
    [TestCase("095d", "zh-CN")]
    [TestCase("Season6", "en-US")]
    [TestCase("Season6", "zh-CN")]
    public async Task MoneyFallbackDescriptionsAreCultureIndependentAsync(string version, string cultureName)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            var provider = new InMemoryPersistenceContextProvider();
            DataInitializationBase initializer = version == "095d"
                ? new Version095d.DataInitialization(provider, NullLoggerFactory.Instance)
                : new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance);
            await initializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
            using var context = provider.CreateNewContext();
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var expected = new List<(byte Group, short Number, string Name, int MoneyAmount)>
            {
                (14, 11, "Box of Luck", 10000),
            };
            if (version == "Season6")
            {
                expected.AddRange(new (byte, short, string, int)[]
                {
                    (14, 32, "Pink Chocolate Box", 100000),
                    (14, 33, "Red Chocolate Box", 500000),
                    (14, 34, "Blue Chocolate Box", 500000),
                    (12, 32, "Red Ribbon Box", 10000),
                    (12, 33, "Green Ribbon Box", 40000),
                    (12, 34, "Blue Ribbon Box", 80000),
                });
            }

            foreach (var (group, number, name, moneyAmount) in expected)
            {
                var item = configuration.Items.Single(item => item.Group == group && item.Number == number);
                var fallback = item.DropItems.Single(drop => drop.ItemType == SpecialItemType.Money && drop.SourceItemLevel == 0);
                Assert.Multiple(() =>
                {
                    Assert.That(fallback.Description.ValueInNeutralLanguage, Is.EqualTo($"{name} - Money"));
                    Assert.That(fallback.Description.SourceKey, Does.StartWith("ItemDropDescriptions/"));
                    Assert.That(fallback.Description.IsUnchangedSinceSourceStamp, Is.True);
                    Assert.That(fallback.Description.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.Not.Null.And.Not.Empty);
                    Assert.That(fallback.MoneyAmount, Is.EqualTo(moneyAmount));
                    Assert.That(fallback.Chance, Is.EqualTo(1.0));
                });
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

}
