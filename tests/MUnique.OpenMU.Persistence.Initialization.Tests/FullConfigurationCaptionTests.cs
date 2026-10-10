// <copyright file="FullConfigurationCaptionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Persistence.Initialization.Captions;

/// <summary>Checks full-configuration captions in fresh and independently initialized existing databases.</summary>
[TestFixture]
[NonParallelizable]
internal class FullConfigurationCaptionTests
{
    /// <summary>All visible captions have sources and Chinese translations, and old English data can be linked.</summary>
    /// <param name="version">The configuration version.</param>
    /// <returns>The task.</returns>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task AllCaptionsCanBeTranslatedAsync(string version)
    {
        var referenceProvider = new InMemoryPersistenceContextProvider();
        await InitializeAsync(referenceProvider, version).ConfigureAwait(false);
        using var referenceContext = referenceProvider.CreateNewContext();
        var reference = (await referenceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var existingProvider = new InMemoryPersistenceContextProvider();
        var previousCulture = CultureInfo.CurrentUICulture;
        var previousFormattingCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture;
            await InitializeAsync(existingProvider, version).ConfigureAwait(false);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
            CultureInfo.CurrentCulture = previousFormattingCulture;
        }
        using var existingContext = existingProvider.CreateNewContext();
        var existing = (await existingContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var chinese = CultureInfo.GetCultureInfo("zh-CN");
        var captions = LocalizedCaption.FindAll(existing).Where(caption => !string.IsNullOrWhiteSpace(caption.Value.ValueInNeutralLanguage)).ToArray();
        Assert.That(captions.Select(caption => $"{caption.Owner.GetType().Name}/{caption.Property.Name}/{caption.Value.ValueInNeutralLanguage}"),
            Is.EquivalentTo(LocalizedCaption.FindAll(reference).Where(caption => !string.IsNullOrWhiteSpace(caption.Value.ValueInNeutralLanguage))
                .Select(caption => $"{caption.Owner.GetType().Name}/{caption.Property.Name}/{caption.Value.ValueInNeutralLanguage}")));
        foreach (var caption in captions)
        {
            Assert.That(caption.Value.SourceKey, Is.Not.Null, caption.Value.ValueInNeutralLanguage);
            Assert.That(caption.Value.GetOwnTranslation(chinese), Is.Not.Null.And.Not.Empty, caption.Value.SourceKey);
            caption.SetValue(caption.Value.ValueInNeutralLanguage);
        }

        var custom = existing.Skills.First();
        custom.Name = custom.Name.WithTranslation(chinese, "自定义技能");
        var result = ConfigurationCaptions.LinkSourceKeys(existing, reference);
        var unlinked = LocalizedCaption.FindAll(existing).Where(caption => !string.IsNullOrWhiteSpace(caption.Value.ValueInNeutralLanguage) && caption.Value.SourceKey is null).ToArray();
        Assert.That(unlinked.Select(caption => $"{caption.Owner.GetType().Name}.{caption.Property.Name}: {caption.Value.ValueInNeutralLanguage}"), Is.Empty, string.Join("\n", unlinked.Select(caption => $"{caption.Owner.GetType().Name}.{caption.Property.Name}: {caption.Value.ValueInNeutralLanguage}")));
        Assert.That(result.Linked, Is.EqualTo(captions.Length));
        Assert.That(result.SkippedBecauseOfCustomizedNeutralText, Is.Zero);
        Assert.That(custom.Name.GetOwnTranslation(chinese), Is.EqualTo("自定义技能"));
        var changes = ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == chinese.Name && change.IsRecommended).ToArray();
        Assert.That(changes, Has.Length.EqualTo(captions.Length - 1));
        Assert.That(ConfigurationCaptions.ApplyChanges(existing, changes.Select(change => change.Id)), Is.EqualTo(changes.Length));
        Assert.That(custom.Name.GetOwnTranslation(chinese), Is.EqualTo("自定义技能"));
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.Zero);
        Assert.That(ConfigurationCaptions.DetermineChanges(existing).Any(change => change.CultureName == chinese.Name && change.IsRecommended), Is.False);
        foreach (var caption in LocalizedCaption.FindAll(existing).Where(caption => !string.IsNullOrWhiteSpace(caption.Value.ValueInNeutralLanguage)))
        {
            Assert.That(caption.Value.GetOwnTranslation(chinese), Is.Not.Null.And.Not.Empty, caption.Value.SourceKey);
        }
    }

    /// <summary>Repeated drop descriptions can share a source, but conflicting sources and renamed text are never guessed.</summary>
    /// <param name="differentSources">Whether identical reference text has conflicting sources.</param>
    /// <param name="customized">Whether the target text was renamed.</param>
    /// <param name="expectedLinks">The expected number of linked target captions.</param>
    [TestCase(false, false, 2)]
    [TestCase(true, false, 0)]
    [TestCase(false, true, 0)]
    public void RepeatedDescriptionsRequireAnUnambiguousSource(bool differentSources, bool customized, int expectedLinks)
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var reference = context.CreateNew<GameConfiguration>();
        var existing = context.CreateNew<GameConfiguration>();
        for (var index = 0; index < 2; index++)
        {
            var source = context.CreateNew<DropItemGroup>();
            source.Description = new LocalizedString("Shared description").WithSourceKey(differentSources ? $"Test/Source{index}" : "Test/Shared");
            reference.DropItemGroups.Add(source);
            var target = context.CreateNew<DropItemGroup>();
            target.Description = customized ? "Custom description" : "Shared description";
            existing.DropItemGroups.Add(target);
        }

        var result = ConfigurationCaptions.LinkSourceKeys(existing, reference);
        Assert.That(result.Linked, Is.EqualTo(expectedLinks));
        Assert.That(existing.DropItemGroups.All(group => group.Description.ValueInNeutralLanguage == (customized ? "Custom description" : "Shared description")), Is.True);
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.Zero);
    }

    private static async Task InitializeAsync(InMemoryPersistenceContextProvider provider, string version)
    {
        DataInitializationBase initializer = version switch
        {
            "075" => new Version075.DataInitialization(provider, NullLoggerFactory.Instance),
            "095d" => new Version095d.DataInitialization(provider, NullLoggerFactory.Instance),
            _ => new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance),
        };
        await initializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
    }
}
