// <copyright file="AttributeSystemTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="AttributeSystem"/>.
/// </summary>
[TestFixture]
public class AttributeSystemTests
{
    private readonly AttributeDefinition _attributeA = new (Guid.NewGuid(), "A", "The A attribute");
    private readonly AttributeDefinition _attributeB = new (Guid.NewGuid(), "B", "The B attribute");
    private readonly AttributeDefinition _attributeC = new(Guid.NewGuid(), "C", "The C attribute");
    private readonly AttributeDefinition _attributeD = new(Guid.NewGuid(), "D", "The D attribute");
    private readonly AttributeDefinition _attributeAplusB = new (Guid.NewGuid(), "A+B", "The A+B attribute");
    private readonly AttributeDefinition _attributeAtimesB = new(Guid.NewGuid(), "A*B", "The A*B attribute");
    private readonly AttributeDefinition _attributeAchained = new (Guid.NewGuid(), "A'", "The chained A attribute");

    private List<IAttribute> _statAttributes = null!;
    private List<IAttribute> _baseAttributes = null!;
    private List<AttributeRelationship> _relationShips = null!;

    /// <summary>
    /// Setups each test case.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._statAttributes = new List<IAttribute>();
        this._baseAttributes = new List<IAttribute>();
        this._relationShips = new List<AttributeRelationship>();
    }

    /// <summary>
    /// Tests if a <see cref="StatAttribute"/> is added correctly and it's value is returned.
    /// </summary>
    [Test]
    public void StatAttributeValue()
    {
        const int attributeAValue = 1234;
        this._statAttributes.Add(new StatAttribute(this._attributeA, attributeAValue));
        var system = this.CreateAttributeSystem();
        var value = system[this._attributeA];
        Assert.That(value, Is.EqualTo(attributeAValue));
    }

    /// <summary>
    /// Tests if a attribute is added correctly and it's value is returned.
    /// </summary>
    [Test]
    public void BaseAttributeValue()
    {
        const int attributeAValue = 9999;
        this._baseAttributes.Add(new ConstValueAttribute(attributeAValue, this._attributeA));
        var system = this.CreateAttributeSystem();
        var value = system[this._attributeA];
        Assert.That(value, Is.EqualTo(attributeAValue));
    }

    /// <summary>
    /// Tests if a <see cref="AttributeRelationship"/> is added correctly and the combined (in this case a chained) attribute does return the right value.
    /// </summary>
    [Test]
    public void ChainedAttribute()
    {
        const int attributeAValue = 1234;
        this._statAttributes.Add(new StatAttribute(this._attributeA, attributeAValue));
        this._relationShips.Add(new AttributeRelationship(this._attributeAchained, 1, this._attributeA));
        var system = this.CreateAttributeSystem();
        var value = system[this._attributeAchained];
        Assert.That(value, Is.EqualTo(attributeAValue));
    }

    /// <summary>
    /// Tests if a <see cref="AttributeRelationship"/> is added correctly and the multiplied attribute does return the right multiplied value.
    /// </summary>
    [Test]
    public void MultipliedAttribute()
    {
        const int attributeAValue = 1234;
        const int multiplier = 3;
        this._statAttributes.Add(new StatAttribute(this._attributeA, attributeAValue));
        this._relationShips.Add(new AttributeRelationship(this._attributeAchained, multiplier, this._attributeA));
        var system = this.CreateAttributeSystem();
        var value = system[this._attributeAchained];
        Assert.That(value, Is.EqualTo(attributeAValue * multiplier));
    }

    /// <summary>
    /// Tests if a combination of <see cref="AttributeRelationship"/>s is added and handled correctly.
    /// Two attributes are combined to one target attribute.
    /// It tests if the resulting value is calculated correctly.
    /// </summary>
    [Test]
    public void CombinedRelationship()
    {
        const int attributeAValue = 1234;
        const int attributeBValue = 4938;
        const int attributeBMultiplier = 2;
        this._statAttributes.Add(new StatAttribute(this._attributeA, attributeAValue));
        this._statAttributes.Add(new StatAttribute(this._attributeB, attributeBValue));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, 1, this._attributeA));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, attributeBMultiplier, this._attributeB));
        var system = this.CreateAttributeSystem();
        var value = system[this._attributeAplusB];
        Assert.That(value, Is.EqualTo(attributeAValue + (attributeBValue * attributeBMultiplier)));
    }

    /// <summary>
    /// Tests if a combination of <see cref="AttributeRelationship"/>s is added and handled correctly.
    /// Two attributes are combined to one target attribute.
    /// It tests if the resulting value is calculated correctly after one of the depending attributes changed their value.
    /// </summary>
    [Test]
    public void CombinedRelationshipChangedValue()
    {
        const int attributeAValue = 1234;
        const int attributeBValue = 4938;
        const int attributeBMultiplier = 2;
        var statAttributeA = new StatAttribute(this._attributeA, attributeAValue);
        this._statAttributes.Add(statAttributeA);
        this._statAttributes.Add(new StatAttribute(this._attributeB, attributeBValue));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, 1, this._attributeA));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, attributeBMultiplier, this._attributeB));
        var system = this.CreateAttributeSystem();
        const int attributeAnewValue = 1000;
        statAttributeA.Value = attributeAnewValue;
        var value = system[this._attributeAplusB];
        Assert.That(value, Is.EqualTo(attributeAnewValue + (attributeBValue * attributeBMultiplier)));
    }

    /// <summary>
    /// Tests if adding additional elements results in a updated correct value.
    /// </summary>
    [Test]
    public void CombinedRelationshipAddedElement()
    {
        const int attributeAValue = 1234;
        const int attributeBValue = 4938;
        const int attributeBMultiplier = 2;
        var statAttributeA = new StatAttribute(this._attributeA, attributeAValue);
        this._statAttributes.Add(statAttributeA);
        this._statAttributes.Add(new StatAttribute(this._attributeB, attributeBValue));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, 1, this._attributeA));
        this._relationShips.Add(new AttributeRelationship(this._attributeAplusB, attributeBMultiplier, this._attributeB));
        var system = this.CreateAttributeSystem();
        const int addedAttributeValue = 10;
        system.AddElement(new ConstValueAttribute(addedAttributeValue, this._attributeAplusB), this._attributeAplusB);
        var value = system[this._attributeAplusB];
        Assert.That(value, Is.EqualTo(attributeAValue + (attributeBValue * attributeBMultiplier) + addedAttributeValue));
    }

    /// <summary>
    /// Tests if using another attribute as operand results in a correct value.
    /// </summary>
    [Test]
    public void MultipliedAttributes()
    {
        const int attributeAValue = 1234;
        const int attributeBValue = 2;

        var statAttributeA = new StatAttribute(this._attributeA, attributeAValue);
        var statAttributeB = new StatAttribute(this._attributeB, attributeBValue);
        this._statAttributes.Add(statAttributeA);
        this._statAttributes.Add(statAttributeB);

        this._relationShips.Add(new AttributeRelationship(this._attributeAtimesB, this._attributeB, this._attributeA));
        var system = this.CreateAttributeSystem();

        var value = system[this._attributeAtimesB];
        Assert.That(value, Is.EqualTo(attributeAValue * attributeBValue));
    }

    /// <summary>
    /// Tests if using another attribute as operand results in a correct value.
    /// </summary>
    [TestCase(true, 2)]
    [TestCase(false, 0)]
    public void ConditionalAttributes(bool conditionMet, float expected)
    {
        const int bonusValue = 2;
        var targetAttribute = this._attributeAplusB;
        var bonusIfConditionMet = new StatAttribute(this._attributeB, bonusValue);

        var conditionalAttribute = new StatAttribute(this._attributeC, conditionMet ? 1 : 0);

        this._statAttributes.Add(bonusIfConditionMet);
        this._statAttributes.Add(conditionalAttribute);

        this._relationShips.Add(new AttributeRelationship(targetAttribute, conditionalAttribute.Definition, bonusIfConditionMet.Definition));

        var system = this.CreateAttributeSystem();
        var value = system[targetAttribute];
        Assert.That(value, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the use case of multiplying a base value with a conditional bonus multiplier.
    /// This is how Stats.DefenseIncreaseWithEquippedShield is intended to work.
    /// </summary>
    [TestCase(true, 105)]
    [TestCase(false, 100)]
    public void ConditionalAttributes_Multiply(bool conditionMet, float expected)
    {
        const float bonusMultiplier = 0.05f;
        const float baseValue = 100f;
        var targetAttribute = this._attributeAplusB;
        var baseAttribute = new StatAttribute(this._attributeA, baseValue);
        var bonusIfConditionMet = new StatAttribute(this._attributeB, bonusMultiplier);
        var conditionalAttribute = new StatAttribute(this._attributeC, conditionMet ? 1 : 0);

        var tempAttribute = this._attributeD;

        this._statAttributes.Add(baseAttribute);
        this._statAttributes.Add(bonusIfConditionMet);
        this._statAttributes.Add(conditionalAttribute);

        // First, we copy our base value to the target
        this._relationShips.Add(new AttributeRelationship(targetAttribute, 1, baseAttribute.Definition));

        // Then, we calculate the bonus into a temporary attribute, depending on the condition
        this._relationShips.Add(new AttributeRelationship(tempAttribute, conditionalAttribute.Definition, bonusIfConditionMet.Definition));

        // Finally, we apply the temporary attribute as multiplier for the base value and add it to the target
        this._relationShips.Add(new AttributeRelationship(targetAttribute, tempAttribute, baseAttribute.Definition));

        var system = this.CreateAttributeSystem();
        var value = system[targetAttribute];
        Assert.That(value, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests if a changed maximum value of an attribute is considered when requesting it from the AttributeSystem.
    /// </summary>
    [Test]
    public void MaximumValueIsRespectedFromStoredDefinition()
    {
        // Create an attribute definition with a maximum value
        var attackSpeedDefinition = new AttributeDefinition(Guid.NewGuid(), "AttackSpeed", "Attack speed attribute")
        {
            MaximumValue = 300,
        };

        // Add a stat attribute with a very high value (exceeding the maximum)
        const int highValue = 5000;
        this._statAttributes.Add(new StatAttribute(attackSpeedDefinition, highValue));

        var system = this.CreateAttributeSystem();

        // Create a different instance of the attribute definition (simulating what happens when
        // the definition is retrieved from a different source, like a database)
        var differentInstanceOfDefinition = new AttributeDefinition(attackSpeedDefinition.Id, "AttackSpeed", "Attack speed attribute");

        // The system should return the maximum value from the stored definition, not exceed it
        var value = system[differentInstanceOfDefinition];
        Assert.That(value, Is.EqualTo(300));
    }

    /// <summary>
    /// Tests if a changed maximum value is respected for a ComposableAttribute when requesting it from the AttributeSystem.
    /// </summary>
    [Test]
    public void MaximumValueIsRespectedFromStoredDefinitionForComposableAttribute()
    {
        // Create an attribute definition with a maximum value
        var attackSpeedDefinition = new AttributeDefinition(Guid.NewGuid(), "AttackSpeed", "Attack speed attribute")
        {
            MaximumValue = 300,
        };

        // Add a base attribute which will create a ComposableAttribute
        const int highValue = 5000;
        this._baseAttributes.Add(new ConstValueAttribute(highValue, attackSpeedDefinition));

        var system = this.CreateAttributeSystem();

        // Create a different instance of the attribute definition
        var differentInstanceOfDefinition = new AttributeDefinition(attackSpeedDefinition.Id, "AttackSpeed", "Attack speed attribute");

        // The system should return the maximum value from the stored definition, not exceed it
        var value = system[differentInstanceOfDefinition];
        Assert.That(value, Is.EqualTo(300));
    }

    /// <summary>
    /// Creates the attribute system for testing, initialized with <see cref="_statAttributes"/>, <see cref="_baseAttributes"/> and <see cref="_relationShips"/>.
    /// </summary>
    /// <returns>The created attribute system, initialized with <see cref="_statAttributes"/>, <see cref="_baseAttributes"/> and <see cref="_relationShips"/>.</returns>
    private AttributeSystem CreateAttributeSystem()
    {
        return new(this._statAttributes, this._baseAttributes, this._relationShips);
    }
}