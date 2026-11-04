// <copyright file="BasicModelConverterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.Persistence.EntityFramework.Model;
using AggregateType = MUnique.OpenMU.AttributeSystem.AggregateType;

/// <summary>
/// Tests for the generated conversion of the entity framework model to the basic model.
/// </summary>
[TestFixture]
public class BasicModelConverterTests
{
    /// <summary>
    /// Tests that objects which are referenced multiple times in the source graph
    /// are converted to one object which is referenced multiple times in the target graph.
    /// </summary>
    [Test]
    public void ReferencesArePreserved()
    {
        var itemDefinition = new ItemDefinition { Name = "Item" };
        var configuration = new GameConfiguration();
        configuration.Items.Add(itemDefinition);
        var dropGroup = new DropItemGroup();
        dropGroup.PossibleItems.Add(itemDefinition);
        configuration.DropItemGroups.Add(dropGroup);

        var converted = Convert<BasicModel.GameConfiguration>(configuration);

        Assert.That(converted.Items.Single(), Is.InstanceOf<BasicModel.ItemDefinition>());
        Assert.That(converted.Items.Single().Name.ToString(), Is.EqualTo("Item"));
        Assert.That(converted.DropItemGroups.Single().PossibleItems.Single(), Is.SameAs(converted.Items.Single()));
    }

    /// <summary>
    /// Tests that the ids of the objects are kept.
    /// </summary>
    [Test]
    public void IdsAreKept()
    {
        var configuration = new GameConfiguration { Id = Guid.NewGuid() };
        var map = new GameMapDefinition { Id = Guid.NewGuid(), Number = 7 };
        configuration.Maps.Add(map);

        var converted = Convert<BasicModel.GameConfiguration>(configuration);

        Assert.That(converted.Id, Is.EqualTo(configuration.Id));
        Assert.That(((BasicModel.GameMapDefinition)converted.Maps.Single()).Id, Is.EqualTo(map.Id));
        Assert.That(converted.Maps.Single().Number, Is.EqualTo(7));
    }

    /// <summary>
    /// Tests that objects of types which have to be created by a constructor with parameters are converted completely.
    /// </summary>
    [Test]
    public void ObjectsWithConstructorParametersAreConverted()
    {
        var target = new AttributeDefinition { Id = Guid.NewGuid(), Designation = "Target" };
        var input = new AttributeDefinition { Id = Guid.NewGuid(), Designation = "Input" };
        var configuration = new GameConfiguration();
        configuration.Attributes.Add(target);
        configuration.Attributes.Add(input);
        var characterClass = new CharacterClass();
        characterClass.AttributeCombinations.Add(new AttributeRelationship(target, 2.5f, input, AggregateType.Multiplicate));
        configuration.CharacterClasses.Add(characterClass);

        var converted = Convert<BasicModel.GameConfiguration>(configuration);

        var relationship = converted.CharacterClasses.Single().AttributeCombinations.Single();
        Assert.That(relationship, Is.InstanceOf<BasicModel.AttributeRelationship>());
        Assert.That(relationship.TargetAttribute, Is.SameAs(converted.Attributes.Single(a => a.Designation == "Target")));
        Assert.That(relationship.InputAttribute, Is.SameAs(converted.Attributes.Single(a => a.Designation == "Input")));
        Assert.That(relationship.InputOperand, Is.EqualTo(2.5f));
        Assert.That(relationship.AggregateType, Is.EqualTo(AggregateType.Multiplicate));
    }

    /// <summary>
    /// Tests that properties with non-public setters are converted.
    /// </summary>
    [Test]
    public void PropertiesWithNonPublicSettersAreConverted()
    {
        var powerUp = new PowerUpDefinition { Boost = new PowerUpDefinitionValue { Value = 42, AggregateType = AggregateType.Multiplicate } };

        var converted = BasicModelConverter.Convert<BasicModel.PowerUpDefinition>(powerUp);

        Assert.That(converted.Boost, Is.InstanceOf<BasicModel.PowerUpDefinitionValue>());
        Assert.That(converted.Boost, Is.Not.SameAs(powerUp.Boost));
        Assert.That(converted.Boost!.ConstantValue.Value, Is.EqualTo(42));
        Assert.That(converted.Boost.ConstantValue.AggregateType, Is.EqualTo(AggregateType.Multiplicate));
    }

    private static T Convert<T>(object source)
        where T : class
    {
        return ((Json.IConvertibleTo<T>)source).Convert();
    }
}
