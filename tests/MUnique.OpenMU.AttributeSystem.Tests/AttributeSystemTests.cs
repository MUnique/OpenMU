// <copyright file="AttributeSystemTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="AttributeSystem"/>.
    /// </summary>
    [TestFixture]
    public class AttributeSystemTests
    {
        private readonly AttributeDefinition attributeA = new (Guid.NewGuid(), "A", "The A attribute");
        private readonly AttributeDefinition attributeB = new (Guid.NewGuid(), "B", "The B attribute");
        private readonly AttributeDefinition attributeAplusB = new (Guid.NewGuid(), "A+B", "The A+B attribute");
        private readonly AttributeDefinition attributeAchained = new (Guid.NewGuid(), "A'", "The chained A attribute");

        private List<IAttribute> statAttributes = null!;
        private List<IAttribute> baseAttributes = null!;
        private List<AttributeRelationship> relationShips = null!;

        /// <summary>
        /// Setups each test case.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.statAttributes = new List<IAttribute>();
            this.baseAttributes = new List<IAttribute>();
            this.relationShips = new List<AttributeRelationship>();
        }

        /// <summary>
        /// Tests if a <see cref="StatAttribute"/> is added correctly and it's value is returned.
        /// </summary>
        [Test]
        public void StatAttributeValue()
        {
            const int attributeAValue = 1234;
            this.statAttributes.Add(new StatAttribute(this.attributeA, attributeAValue));
            var system = this.CreateAttributeSystem();
            var value = system[this.attributeA];
            Assert.That(value, Is.EqualTo(attributeAValue));
        }

        /// <summary>
        /// Tests if a attribute is added correctly and it's value is returned.
        /// </summary>
        [Test]
        public void BaseAttributeValue()
        {
            const int attributeAValue = 9999;
            this.baseAttributes.Add(new ConstValueAttribute(attributeAValue, this.attributeA));
            var system = this.CreateAttributeSystem();
            var value = system[this.attributeA];
            Assert.That(value, Is.EqualTo(attributeAValue));
        }

        /// <summary>
        /// Tests if a <see cref="AttributeRelationship"/> is added correctly and the combined (in this case a chained) attribute does return the right value.
        /// </summary>
        [Test]
        public void ChainedAttribute()
        {
            const int attributeAValue = 1234;
            this.statAttributes.Add(new StatAttribute(this.attributeA, attributeAValue));
            this.relationShips.Add(new AttributeRelationship(this.attributeAchained, 1, this.attributeA));
            var system = this.CreateAttributeSystem();
            var value = system[this.attributeAchained];
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
            this.statAttributes.Add(new StatAttribute(this.attributeA, attributeAValue));
            this.relationShips.Add(new AttributeRelationship(this.attributeAchained, multiplier, this.attributeA));
            var system = this.CreateAttributeSystem();
            var value = system[this.attributeAchained];
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
            this.statAttributes.Add(new StatAttribute(this.attributeA, attributeAValue));
            this.statAttributes.Add(new StatAttribute(this.attributeB, attributeBValue));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, 1, this.attributeA));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, attributeBMultiplier, this.attributeB));
            var system = this.CreateAttributeSystem();
            var value = system[this.attributeAplusB];
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
            var statAttributeA = new StatAttribute(this.attributeA, attributeAValue);
            this.statAttributes.Add(statAttributeA);
            this.statAttributes.Add(new StatAttribute(this.attributeB, attributeBValue));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, 1, this.attributeA));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, attributeBMultiplier, this.attributeB));
            var system = this.CreateAttributeSystem();
            const int attributeAnewValue = 1000;
            statAttributeA.Value = attributeAnewValue;
            var value = system[this.attributeAplusB];
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
            var statAttributeA = new StatAttribute(this.attributeA, attributeAValue);
            this.statAttributes.Add(statAttributeA);
            this.statAttributes.Add(new StatAttribute(this.attributeB, attributeBValue));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, 1, this.attributeA));
            this.relationShips.Add(new AttributeRelationship(this.attributeAplusB, attributeBMultiplier, this.attributeB));
            var system = this.CreateAttributeSystem();
            const int addedAttributeValue = 10;
            system.AddElement(new ConstValueAttribute(addedAttributeValue, this.attributeAplusB), this.attributeAplusB);
            var value = system[this.attributeAplusB];
            Assert.That(value, Is.EqualTo(attributeAValue + (attributeBValue * attributeBMultiplier) + addedAttributeValue));
        }

        /// <summary>
        /// Creates the attribute system for testing, initialized with <see cref="statAttributes"/>, <see cref="baseAttributes"/> and <see cref="relationShips"/>.
        /// </summary>
        /// <returns>The acreated attribute system, initialized with <see cref="statAttributes"/>, <see cref="baseAttributes"/> and <see cref="relationShips"/>.</returns>
        private AttributeSystem CreateAttributeSystem()
        {
            return new (this.statAttributes, this.baseAttributes, this.relationShips);
        }
    }
}
