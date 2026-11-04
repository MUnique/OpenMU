// <copyright file="ItemOptionCaptionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Tests resource-backed option and set captions for new and existing configurations.</summary>
[TestFixture]
[NonParallelizable]
public class ItemOptionCaptionTests
{
    /// <summary>Exact-name fallback never guesses between duplicate or customized option names.</summary>
    /// <param name="duplicateReference">Whether the reference name is ambiguous.</param>
    /// <param name="duplicateTarget">Whether the existing name is ambiguous.</param>
    /// <param name="customized">Whether the existing neutral name was customized.</param>
    /// <param name="expectedLinks">The expected number of links.</param>
    [TestCase(false, false, false, 1)]
    [TestCase(true, false, false, 0)]
    [TestCase(false, true, false, 0)]
    [TestCase(false, false, true, 0)]
    public void RandomOptionIdsRequireUniqueUnchangedNames(bool duplicateReference, bool duplicateTarget, bool customized, int expectedLinks)
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var reference = context.CreateNew<GameConfiguration>();
        var existing = context.CreateNew<GameConfiguration>();
        var sourceName = LocalizedString.FromResource(() => ItemOptionNames.WingsOfElfOptions);
        var source = context.CreateNew<ItemOptionDefinition>();
        source.Name = sourceName;
        reference.ItemOptions.Add(source);
        var target = context.CreateNew<ItemOptionDefinition>();
        target.Name = customized ? "Custom wing options" : sourceName.ValueInNeutralLanguage;
        existing.ItemOptions.Add(target);
        if (duplicateReference)
        {
            var duplicate = context.CreateNew<ItemOptionDefinition>();
            duplicate.Name = sourceName;
            reference.ItemOptions.Add(duplicate);
        }

        if (duplicateTarget)
        {
            var duplicate = context.CreateNew<ItemOptionDefinition>();
            duplicate.Name = target.Name;
            existing.ItemOptions.Add(duplicate);
        }

        var storedName = target.Name.Value;
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.EqualTo(expectedLinks));
        if (expectedLinks == 0)
        {
            Assert.That(target.Name.Value, Is.EqualTo(storedName));
        }
        else
        {
            Assert.That(target.Name.SourceKey, Is.EqualTo(sourceName.SourceKey));
        }
    }

    /// <summary>Existing captions can be linked and updated without replacing custom translations.</summary>
    /// <param name="version">The initialization version.</param>
    /// <param name="cultureName">The initialization culture.</param>
    /// <returns>The task.</returns>
    [TestCase("075", "en-US")]
    [TestCase("095d", "en-US")]
    [TestCase("Season6", "en-US")]
    [TestCase("Season6", "zh-CN")]
    public async Task CaptionsUseConfigurationNameWorkflowAsync(string version, string cultureName)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
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
            var referenceProvider = new InMemoryPersistenceContextProvider();
            DataInitializationBase referenceInitializer = version switch
            {
                "075" => new Version075.DataInitialization(referenceProvider, NullLoggerFactory.Instance),
                "095d" => new Version095d.DataInitialization(referenceProvider, NullLoggerFactory.Instance),
                _ => new VersionSeasonSix.DataInitialization(referenceProvider, NullLoggerFactory.Instance),
            };
            await referenceInitializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
            using var referenceContext = referenceProvider.CreateNewContext();
            var reference = CaptionLinkReference.Create((await referenceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single());
            var captions = new List<(Func<LocalizedString> Get, Action<LocalizedString> Set)>();
            foreach (var type in configuration.ItemOptionTypes)
            {
                Assert.That(type.ToString(), Is.EqualTo(type.Name.GetTranslation(CultureInfo.CurrentCulture)));
                captions.Add((() => type.Name, value => type.Name = value));
                if (type.Description.SourceKey is not null)
                {
                    captions.Add((() => type.Description, value => type.Description = value));
                }
            }

            foreach (var option in configuration.ItemOptions.Distinct())
            {
                Assert.That(option.ToString(), Is.EqualTo(option.Name.GetTranslation(CultureInfo.CurrentCulture)));
                captions.Add((() => option.Name, value => option.Name = value));
            }

            foreach (var set in configuration.ItemSetGroups)
            {
                captions.Add((() => set.Name, value => set.Name = value));
            }

            var chinese = CultureInfo.GetCultureInfo("zh-CN");
            var expected = captions.Select(caption => caption.Get()).ToArray();
            Assert.That(expected.All(name => name.SourceKey is not null && name.IsUnchangedSinceSourceStamp), Is.True);
            Assert.That(expected.All(name => !string.IsNullOrEmpty(name.GetOwnTranslation(chinese))), Is.True);
            Assert.That(expected.All(name => name.ValueInNeutralLanguage.All(character => character < 128)), Is.True);
            foreach (var caption in captions)
            {
                caption.Set(caption.Get().ValueInNeutralLanguage);
            }

            captions[0].Set(captions[0].Get().WithTranslation(chinese, "自定义属性"));
            Assert.That(ConfigurationCaptions.LinkSourceKeys(configuration, reference), Is.EqualTo((captions.Count, 0)));
            var changes = ConfigurationCaptions.DetermineChanges(configuration).Where(change => change.CultureName == chinese.Name && change.IsRecommended).ToArray();
            Assert.That(changes, Has.Length.EqualTo(captions.Count - 1));
            Assert.That(ConfigurationCaptions.ApplyChanges(configuration, changes.Select(change => change.Id)), Is.EqualTo(changes.Length));
            for (var index = 1; index < captions.Count; index++)
            {
                Assert.That(captions[index].Get().ValueInNeutralLanguage, Is.EqualTo(expected[index].ValueInNeutralLanguage));
                Assert.That(captions[index].Get().GetOwnTranslation(chinese), Is.EqualTo(expected[index].GetOwnTranslation(chinese)));
            }

            Assert.That(captions[0].Get().GetOwnTranslation(chinese), Is.EqualTo("自定义属性"));
            Assert.That(ConfigurationCaptions.LinkSourceKeys(configuration, reference).Linked, Is.Zero);
            Assert.That(ConfigurationCaptions.DetermineChanges(configuration).Any(change => change.CultureName == chinese.Name && change.IsRecommended), Is.False);
            captions[0].Set("Custom option type");
            Assert.That(ConfigurationCaptions.LinkSourceKeys(configuration, reference), Is.EqualTo((0, 1)));
            Assert.That(captions[0].Get().Value, Is.EqualTo("Custom option type"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }
}
