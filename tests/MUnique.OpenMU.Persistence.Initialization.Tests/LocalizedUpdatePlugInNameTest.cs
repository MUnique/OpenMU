// <copyright file="LocalizedUpdatePlugInNameTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Globalization;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests that the names and descriptions of update plugins, which the updates page of the admin panel
/// shows, are localized in the language of the user interface.
/// </summary>
[TestFixture]
public class LocalizedUpdatePlugInNameTest
{
    /// <summary>
    /// Tests the name and description of the update which adds the crafting bonus attribute.
    /// </summary>
    /// <param name="culture">The culture of the user interface.</param>
    /// <param name="expectedName">The expected name.</param>
    [TestCase("en", "Add crafting bonus percentage attribute")]
    [TestCase("de", "Attribut für den Bonus der Erfolgsrate beim Kombinieren hinzufügen")]
    [TestCase("zh-CN", "添加合成成功率加成属性")]
    public void CraftingBonusUpdateIsLocalized(string culture, string expectedName)
    {
        AssertLocalized(new AddCraftingBonusPercentageAttributePlugInSeason6(), culture, expectedName);
    }

    /// <summary>
    /// Tests the name and description of the update which fixes the maximum item level of seed spheres.
    /// </summary>
    /// <param name="culture">The culture of the user interface.</param>
    /// <param name="expectedName">The expected name.</param>
    [TestCase("en", "Fix maximum item level of seed spheres")]
    [TestCase("de", "Maximale Gegenstandsstufe der Seed Spheres korrigieren")]
    [TestCase("zh-CN", "修正种子球的最大物品等级")]
    public void SeedSphereUpdateIsLocalized(string culture, string expectedName)
    {
        AssertLocalized(new FixSeedSphereMaximumItemLevelUpdatePlugIn(), culture, expectedName);
    }

    private static void AssertLocalized(UpdatePlugInBase updatePlugIn, string culture, string expectedName)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            var englishDescription = updatePlugIn.Description;
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

            Assert.That(updatePlugIn.Name, Is.EqualTo(expectedName));
            Assert.That(updatePlugIn.Description, culture == "en" ? Is.EqualTo(englishDescription) : Is.Not.EqualTo(englishDescription));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }
}
