// <copyright file="KanturuRequiredItemHelperTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Tests for <see cref="KanturuRequiredItemHelper"/>.
/// </summary>
[TestFixture]
public class KanturuRequiredItemHelperTests
{
    private static Item CreateItemWithPowerUp(AttributeDefinition targetAttribute)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>
        {
            new() { TargetAttribute = targetAttribute },
        });

        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.Definition).Returns(definitionMock.Object);
        return itemMock.Object;
    }

    /// <summary>
    /// Tests that an item with a matching power-up attribute is returned.
    /// </summary>
    [Test]
    public void GetRequiredItems_MatchingPowerUp_ReturnsItem()
    {
        var items = new[] { CreateItemWithPowerUp(Stats.CanFly) };
        var requirements = new List<AttributeRequirement> { new() { Attribute = Stats.CanFly } };

        var result = KanturuRequiredItemHelper.GetRequiredItems(items, requirements);

        Assert.That(result, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that items without a matching power-up attribute are filtered out.
    /// </summary>
    [Test]
    public void GetRequiredItems_OtherPowerUp_ReturnsEmpty()
    {
        var items = new[] { CreateItemWithPowerUp(Stats.CanFly) };
        var requirements = new List<AttributeRequirement> { new() { Attribute = Stats.MaximumHealth } };

        var result = KanturuRequiredItemHelper.GetRequiredItems(items, requirements);

        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Tests that null items return an empty result.
    /// </summary>
    [Test]
    public void GetRequiredItems_NullItems_ReturnsEmpty()
    {
        var requirements = new List<AttributeRequirement> { new() { Attribute = Stats.CanFly } };

        Assert.That(KanturuRequiredItemHelper.GetRequiredItems((IEnumerable<Item>?)null, requirements), Is.Empty);
    }
}
