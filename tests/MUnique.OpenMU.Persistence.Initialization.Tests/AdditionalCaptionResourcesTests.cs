// <copyright file="AdditionalCaptionResourcesTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests source-backed names of imperial events, Fenrir material drops and Selupan skills.
/// </summary>
[TestFixture]
[NonParallelizable]
internal class AdditionalCaptionResourcesTests
{
    /// <summary>
    /// Existing English captions can be linked and translated without changing a customized caption.
    /// </summary>
    /// <param name="cultureName">The culture used during initialization.</param>
    /// <returns>The task.</returns>
    [TestCase("en-US")]
    [TestCase("zh-CN")]
    public async Task MissingCaptionsUseSourceWorkflowAsync(string cultureName)
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            var provider = new InMemoryPersistenceContextProvider();
            var initializer = new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance);
            await initializer.CreateInitialDataAsync(1, false).ConfigureAwait(false);
            using var context = provider.CreateNewContext();
            var reference = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var existing = context.CreateNew<GameConfiguration>();
            var captions = new List<(object Owner, PropertyInfo Property, LocalizedString Expected)>();
            var culture = CultureInfo.GetCultureInfo("zh-CN");
            foreach (var source in reference.MiniGameDefinitions.Where(definition => definition.Type == MiniGameType.ImperialGuardian))
            {
                var day = (ImperialGuardianDay)source.GameLevel;
                Assert.That(source.Name.ValueInNeutralLanguage, Is.EqualTo($"Imperial Guardian {day}"));
                Assert.That(source.Description.ValueInNeutralLanguage, Is.EqualTo($"Event definition for the imperial guardian event on {day}, which takes place on map {source.Entrance!.Map!.Number}."));
                var copy = context.CreateNew<MiniGameDefinition>();
                ((IIdentifiable)copy).Id = source.GetId();
                existing.MiniGameDefinitions.Add(copy);
                AddCaption(copy, nameof(MiniGameDefinition.Name), source.Name);
                AddCaption(copy, nameof(MiniGameDefinition.Description), source.Description);
            }

            foreach (var source in reference.DropItemGroups.Where(group => group.Description.SourceKey is "ItemNames/SplinterOfArmor" or "ItemNames/BlessOfGuardian" or "ItemNames/ClawOfBeast"))
            {
                Assert.That(source.Description.Value, Is.EqualTo(source.PossibleItems.Single().Name.Value));
                var copy = context.CreateNew<DropItemGroup>();
                ((IIdentifiable)copy).Id = source.GetId();
                existing.DropItemGroups.Add(copy);
                AddCaption(copy, nameof(DropItemGroup.Description), source.Description);
            }

            foreach (var source in reference.Skills.Where(skill => skill.Number is >= 250 and <= 253))
            {
                var neutralName = (SkillNumber)source.Number switch
                {
                    SkillNumber.SelupanPoison => "Selupan Poison",
                    SkillNumber.SelupanIceStorm => "Selupan Ice Storm",
                    SkillNumber.SelupanIceStrike => "Selupan Ice Strike",
                    SkillNumber.SelupanFall => "Selupan Fall",
                    _ => throw new InvalidOperationException(),
                };
                Assert.That(source.Name.ValueInNeutralLanguage, Is.EqualTo(neutralName));
                var copy = context.CreateNew<Skill>();
                ((IIdentifiable)copy).Id = source.GetId();
                copy.Number = source.Number;
                existing.Skills.Add(copy);
                AddCaption(copy, nameof(Skill.Name), source.Name);
            }

            Assert.That(captions, Has.Count.EqualTo(21));
            Assert.That(captions.All(caption => caption.Expected.SourceKey is not null && caption.Expected.IsUnchangedSinceSourceStamp), Is.True);
            Assert.That(captions.All(caption => !string.IsNullOrEmpty(caption.Expected.GetOwnTranslation(culture))), Is.True);
            var customized = existing.Skills.Single(skill => skill.Number == 250);
            customized.Name = customized.Name.WithTranslation(culture, "自定义毒液技能");
            var (linked, skipped) = ConfigurationCaptions.LinkSourceKeys(existing, reference);
            Assert.That(linked, Is.EqualTo(21));
            Assert.That(skipped, Is.Zero);
            Assert.That(customized.Name.GetOwnTranslation(culture), Is.EqualTo("自定义毒液技能"));
            var changes = ConfigurationCaptions.DetermineChanges(existing).Where(change => change.CultureName == culture.Name).ToArray();
            Assert.That(changes, Has.Length.EqualTo(21));
            Assert.That(changes.Count(change => change.IsRecommended), Is.EqualTo(20));
            Assert.That(ConfigurationCaptions.ApplyChanges(existing, changes.Where(change => change.IsRecommended).Select(change => change.Id)), Is.EqualTo(20));
            foreach (var (owner, property, expected) in captions.Where(caption => !ReferenceEquals(caption.Owner, customized)))
            {
                Assert.That(((LocalizedString)property.GetValue(owner)!).GetOwnTranslation(culture), Is.EqualTo(expected.GetOwnTranslation(culture)));
            }

            Assert.That(customized.Name.GetOwnTranslation(culture), Is.EqualTo("自定义毒液技能"));
            Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.Zero);
            Assert.That(ConfigurationCaptions.DetermineChanges(existing).Any(change => change.IsRecommended), Is.False);

            var renamed = existing.Skills.Single(skill => skill.Number == (short)SkillNumber.SelupanIceStorm);
            renamed.Name = "Custom Ice Storm||zh-CN=自定义冰风暴";
            Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((0, 1)));
            Assert.That(renamed.Name.Value, Is.EqualTo("Custom Ice Storm||zh-CN=自定义冰风暴"));

            void AddCaption(object owner, string propertyName, LocalizedString expected)
            {
                var property = owner.GetType().GetProperty(propertyName)!;
                captions.Add((owner, property, expected));
                property.SetValue(owner, new LocalizedString(expected.ValueInNeutralLanguage));
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }
}
