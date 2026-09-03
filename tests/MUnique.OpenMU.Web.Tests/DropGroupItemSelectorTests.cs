// <copyright file="DropGroupItemSelectorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using Moq;
using MUnique.OpenMU.Web.AdminPanel.Pages;

/// <summary>
/// Tests the category selectors used by the direct drop-group editor.
/// </summary>
[TestFixture]
public class DropGroupItemSelectorTests
{
    /// <summary>
    /// A 380-capable item is selected by its Guardian option and the normal group filter.
    /// </summary>
    [Test]
    public void FindsGuardian380ItemsWithinAGroup()
    {
        var guardianOption = new IncreasableItemOption { OptionType = ItemOptionTypes.GuardianOption };
        var guardianDefinition = new Mock<ItemOptionDefinition>();
        guardianDefinition.SetupGet(definition => definition.PossibleOptions).Returns([guardianOption]);

        var guardianItem = new Mock<ItemDefinition> { CallBase = true };
        guardianItem.Object.Name = new LocalizedString("Dragon Armor");
        guardianItem.Object.Group = 14;
        guardianItem.SetupGet(item => item.PossibleItemOptions).Returns([guardianDefinition.Object]);

        var otherItem = new Mock<ItemDefinition> { CallBase = true };
        otherItem.Object.Name = new LocalizedString("Dragon Helm");
        otherItem.Object.Group = 14;
        otherItem.SetupGet(item => item.PossibleItemOptions).Returns([]);

        var result = DropGroupItemSelector.Filter(
            [guardianItem.Object, otherItem.Object],
            "Dragon",
            14,
            null,
            DropGroupItemCategory.Guardian380);

        Assert.That(result, Is.EqualTo(new[] { guardianItem.Object }));
    }
}