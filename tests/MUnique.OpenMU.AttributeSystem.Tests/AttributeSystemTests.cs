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
    private readonly AttributeDefinition _attributeAplusB = new (Guid.NewGuid(), "A+B", "The A+B attribute");
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
    /// Creates the attribute system for testing, initialized with <see cref="_statAttributes"/>, <see cref="_baseAttributes"/> and <see cref="_relationShips"/>.
    /// </summary>
    /// <returns>The acreated attribute system, initialized with <see cref="_statAttributes"/>, <see cref="_baseAttributes"/> and <see cref="_relationShips"/>.</returns>
    private AttributeSystem CreateAttributeSystem()
    {
        return new (this._statAttributes, this._baseAttributes, this._relationShips);
    }
}