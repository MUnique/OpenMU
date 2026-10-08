// <copyright file="ArmorSetCaptionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Tests translations of full armor set bonuses and their existing database workflow.</summary>
[TestFixture]
[NonParallelizable]
internal class ArmorSetCaptionTests
{
    /// <summary>Random identifiers and repeated English names do not prevent linking the correct armor translations.</summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ExistingArmorSetsCanBeLinkedAndTranslatedAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var reference = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var otherProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(otherProvider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var otherContext = otherProvider.CreateNewContext();
        var existing = (await otherContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var sets = existing.ItemSetGroups.Where(set => set.AlwaysApplies).ToArray();
        var chinese = CultureInfo.GetCultureInfo("zh-CN");
        Assert.That(sets, Has.Length.EqualTo(399));
        Assert.That(sets.All(set => set.Name.SourceKey?.StartsWith("ArmorSetNames/", StringComparison.Ordinal) == true), Is.True);
        foreach (var set in sets)
        {
            var prefix = set.Items.First().ItemDefinition!.Name.ValueInNeutralLanguage.Split(' ')[0];
            var expected = set.SetLevel == 0 ? prefix + " Defense Rate Bonus" : $"{prefix} Defense Bonus (Level {set.SetLevel})";
            Assert.That(set.Name.ValueInNeutralLanguage, Is.EqualTo(expected));
            Assert.That(set.Name.GetOwnTranslation(chinese), Is.Not.Null.And.Not.Empty);
            set.Name = expected;
        }

        var customized = sets.First();
        customized.Name = customized.Name.WithTranslation(chinese, "自定义套装");
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((399, 0)));
        var changes = ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == chinese.Name && change.IsRecommended).ToArray();
        Assert.That(changes, Has.Length.EqualTo(398));
        Assert.That(ConfigurationCaptions.ApplyChanges(existing, changes.Select(change => change.Id)), Is.EqualTo(398));
        Assert.That(customized.Name.GetOwnTranslation(chinese), Is.EqualTo("自定义套装"));
        var darkSets = sets.Where(set => set.Name.ValueInNeutralLanguage == "Dark Defense Rate Bonus").ToArray();
        Assert.That(darkSets.Select(set => set.Name.GetOwnTranslation(chinese)), Is.Unique);
        Assert.That(darkSets.Select(set => set.Name.GetOwnTranslation(chinese)), Does.Contain("黑凤凰套装防御成功率加成"));
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.Zero);
        Assert.That(ConfigurationCaptions.DetermineChanges(existing).Any(change => change.CultureName == chinese.Name && change.IsRecommended), Is.False);

        customized.Name = "Custom Armor Set";
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((0, 1)));
        Assert.That(customized.Name.Value, Is.EqualTo("Custom Armor Set"));

        var originalName = reference.ItemSetGroups.Single(set => set.AlwaysApplies
            && set.SetLevel == customized.SetLevel
            && set.Items.First().ItemDefinition!.Number == customized.Items.First().ItemDefinition!.Number).Name.ValueInNeutralLanguage;
        customized.Name = originalName;
        var ambiguous = otherContext.CreateNew<ItemSetGroup>();
        ambiguous.Name = originalName;
        ambiguous.AlwaysApplies = true;
        ambiguous.SetLevel = customized.SetLevel;
        foreach (var item in customized.Items)
        {
            ambiguous.Items.Add(item);
        }

        existing.ItemSetGroups.Add(ambiguous);
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((0, 0)));
        Assert.That(customized.Name.SourceKey, Is.Null);
        Assert.That(ambiguous.Name.SourceKey, Is.Null);
    }
}
