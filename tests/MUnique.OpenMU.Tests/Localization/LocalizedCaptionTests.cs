// <copyright file="LocalizedCaptionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Localization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests that captions which contain a <see cref="LocalizedString"/> show its translation
/// for the current culture instead of its serialized value with all translations.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LocalizedCaptionTests
{
    private const string ItemName = "Spirit||de=Geist||@src=ItemNames/Spirit";

    private CultureInfo _originalCulture = null!;

    /// <summary>
    /// Sets the current culture to german.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de");
    }

    /// <summary>
    /// Restores the current culture.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        CultureInfo.CurrentCulture = this._originalCulture;
    }

    /// <summary>
    /// Tests the caption of an <see cref="ItemCraftingResultItem"/>.
    /// </summary>
    [Test]
    public void CraftingResultItemShowsTranslatedItemName()
    {
        var resultItem = new ItemCraftingResultItem { ItemDefinition = new ItemDefinition { Name = ItemName } };

        Assert.That(resultItem.ToString(), Does.StartWith("Geist").And.Not.Contain(LocalizedString.Separator));
    }

    /// <summary>
    /// Tests the caption of a <see cref="MiniGameReward"/> with an item reward.
    /// </summary>
    [Test]
    public void MiniGameRewardShowsTranslatedDescription()
    {
        var reward = new MiniGameReward { ItemReward = new DropItemGroup { Description = ItemName } };

        Assert.That(reward.ToString(), Does.EndWith(" Geist"));
    }
}
