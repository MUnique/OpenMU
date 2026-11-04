// <copyright file="MapPowerUpsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Tests for the <see cref="MapPowerUps"/>.
/// </summary>
[TestFixture]
public class MapPowerUpsTest
{
    private const float MapBonus = 15;

    /// <summary>
    /// Tests if the power-ups of the map are applied to the attribute system.
    /// </summary>
    [Test]
    public void ApplyAddsPowerUpsOfMap()
    {
        var attributes = CreateAttributeSystem();
        using var mapPowerUps = new MapPowerUps();

        mapPowerUps.Apply(CreateMap(new SimpleElement(MapBonus, AggregateType.AddRaw)), attributes);

        Assert.That(attributes[Stats.DefenseBase], Is.EqualTo(MapBonus));
    }

    /// <summary>
    /// Tests if removing the power-ups detaches them from the map configuration, so that the configuration
    /// neither changes the attribute system anymore, nor keeps it alive.
    /// </summary>
    [Test]
    public void RemoveDetachesPowerUpsFromMapConfiguration()
    {
        var configurationElement = new SimpleElement(MapBonus, AggregateType.AddRaw);
        var attributes = CreateAttributeSystem();
        using var mapPowerUps = new MapPowerUps();
        mapPowerUps.Apply(CreateMap(configurationElement), attributes);

        mapPowerUps.Remove();
        configurationElement.Value = MapBonus * 2;

        Assert.That(attributes[Stats.DefenseBase], Is.Zero);
    }

    /// <summary>
    /// Tests if applying the power-ups of another map replaces the ones of the previous map.
    /// </summary>
    [Test]
    public void ApplyReplacesPowerUpsOfPreviousMap()
    {
        var attributes = CreateAttributeSystem();
        using var mapPowerUps = new MapPowerUps();
        mapPowerUps.Apply(CreateMap(new SimpleElement(MapBonus, AggregateType.AddRaw)), attributes);

        mapPowerUps.Apply(CreateMap(new SimpleElement(MapBonus * 2, AggregateType.AddRaw)), attributes);

        Assert.That(attributes[Stats.DefenseBase], Is.EqualTo(MapBonus * 2));
    }

    private static AttributeSystem CreateAttributeSystem() => new([], [], []);

    private static GameMapDefinition CreateMap(SimpleElement bonus)
    {
        var map = new Mock<GameMapDefinition>();
        map.Setup(m => m.CharacterPowerUpDefinitions).Returns(new List<PowerUpDefinition>
        {
            new() { TargetAttribute = Stats.DefenseBase, Boost = new TestPowerUpDefinitionValue(bonus) },
        });
        return map.Object;
    }

    private sealed class TestPowerUpDefinitionValue : PowerUpDefinitionValue
    {
        public TestPowerUpDefinitionValue(SimpleElement constantValue)
        {
            this.ConstantValue = constantValue;
        }
    }
}
