// <copyright file="CombinationBonusCaptionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Captions;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Tests resource-backed combination bonus descriptions and existing configuration updates.</summary>
[TestFixture]
[NonParallelizable]
internal class CombinationBonusCaptionTests
{
    /// <summary>Links old English descriptions, preserves custom text, and applies translations only when selected.</summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ExistingCombinationBonusesCanBeTranslatedAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(provider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var context = provider.CreateNewContext();
        var reference = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var otherProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(otherProvider, NullLoggerFactory.Instance).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        using var otherContext = otherProvider.CreateNewContext();
        var existing = (await otherContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var bonuses = existing.ItemOptionCombinationBonuses.OrderBy(bonus => bonus.Number).ToArray();
        var chinese = CultureInfo.GetCultureInfo("zh-CN");
        var english = new[]
        {
            "Socket package option: Double Damage Chance 3%",
            "Socket package option: Ignore Defense Chance 1%",
            "Black Fenrir Option: Movement Speed",
            "Black Fenrir Option: Underwater Movement Speed",
            "Blue Fenrir Option: Movement Speed",
            "Blue Fenrir Option: Underwater Movement Speed",
            "Gold Fenrir Option: Movement Speed",
            "Gold Fenrir Option: Underwater Movement Speed",
        };
        Assert.That(bonuses.Select(bonus => bonus.Description.ValueInNeutralLanguage), Is.EqualTo(english));
        foreach (var bonus in bonuses)
        {
            Assert.That(bonus.Description.SourceKey, Does.StartWith("ItemOptionDescriptions/"));
            Assert.That(bonus.Description.GetOwnTranslation(chinese), Is.Not.Null.And.Not.Empty);
            bonus.Description = bonus.Description.ValueInNeutralLanguage;
        }

        var customized = bonuses[0];
        customized.Description = customized.Description.WithTranslation(chinese, "自定义组合加成");
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((8, 0)));
        Assert.That(bonuses.Skip(1).All(bonus => bonus.Description.GetOwnTranslation(chinese) is null), Is.True);
        var changes = ConfigurationCaptions.DetermineChanges(existing)
            .Where(change => change.CultureName == chinese.Name && change.IsRecommended).ToArray();
        Assert.That(changes, Has.Length.EqualTo(7));
        Assert.That(ConfigurationCaptions.ApplyChanges(existing, changes.Select(change => change.Id)), Is.EqualTo(7));
        Assert.That(customized.Description.GetOwnTranslation(chinese), Is.EqualTo("自定义组合加成"));
        Assert.That(bonuses[1].Description.GetOwnTranslation(chinese), Is.EqualTo("镶嵌组合属性：无视防御概率 1%"));
        Assert.That(bonuses[3].Description.GetOwnTranslation(chinese), Is.EqualTo("黑色炎狼兽属性：水下移动速度"));
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference).Linked, Is.Zero);
        Assert.That(ConfigurationCaptions.DetermineChanges(existing).Any(change => change.CultureName == chinese.Name && change.IsRecommended), Is.False);
        customized.Description = "Custom combination bonus";
        Assert.That(ConfigurationCaptions.LinkSourceKeys(existing, reference), Is.EqualTo((0, 1)));
        Assert.That(customized.Description.Value, Is.EqualTo("Custom combination bonus"));
    }
}
