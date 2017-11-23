// <copyright file="ComposableAttributeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="ComposableAttribute"/>.
    /// </summary>
    [TestFixture]
    public class ComposableAttributeTests
    {
        private ComposableAttribute composableAttribute;

        /// <summary>
        /// Sets up each test case.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var attributeDefinition = new AttributeDefinition(new Guid("52263EA9-F309-475D-B10B-352D3BFD7650"), "Test attribute", "Test attribute");
            this.composableAttribute = new ComposableAttribute(attributeDefinition);
        }

        /// <summary>
        /// Tests if the value is 0 after creation.
        /// </summary>
        [Test]
        public void ValueIsNullAfterCreation()
        {
            Assert.That(this.composableAttribute.Value, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests if the value is updated after adding an element.
        /// </summary>
        [Test]
        public void ValueAfterAddedElement()
        {
            var element = new ConstantElement(4711);
            this.composableAttribute.AddElement(element);
            Assert.That(this.composableAttribute.Value, Is.EqualTo(element.Value));
        }

        /// <summary>
        /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/> by using <see cref="AggregateType.AddRaw"/>.
        /// </summary>
        [Test]
        public void ValueOfMultipleElements()
        {
            var element1 = new ConstantElement(3000);
            var element2 = new ConstantElement(5000);
            this.composableAttribute.AddElement(element1);
            this.composableAttribute.AddElement(element2);
            Assert.That(this.composableAttribute.Value, Is.EqualTo(element1.Value + element2.Value));
        }

        /// <summary>
        /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/> by using <see cref="AggregateType.Multiplicate"/> in the second element.
        /// </summary>
        [Test]
        public void ValueWithMultiplierElement()
        {
            var element1 = new ConstantElement(3000);
            var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
            this.composableAttribute.AddElement(element1);
            this.composableAttribute.AddElement(element2);
            Assert.That(this.composableAttribute.Value, Is.EqualTo(element1.Value * element2.Value));
        }

        /// <summary>
        /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
        /// by using <see cref="AggregateType.Multiplicate"/> in the second element and
        /// by using <see cref="AggregateType.AddFinal"/> in the last element.
        /// </summary>
        [Test]
        public void ValueWithMultiplierAndFinalElement()
        {
            var element1 = new ConstantElement(3000);
            var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
            var element3 = new SimpleElement { Value = 1000, AggregateType = AggregateType.AddFinal };
            this.composableAttribute.AddElement(element1);
            this.composableAttribute.AddElement(element2);
            this.composableAttribute.AddElement(element3);
            Assert.That(this.composableAttribute.Value, Is.EqualTo((element1.Value * element2.Value) + element3.Value));
        }

        /// <summary>
        /// Tests if the updated correctly after an element got removed.
        /// </summary>
        [Test]
        public void ValueCorrectAfterElementRemoved()
        {
            var element1 = new ConstantElement(3000);
            var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
            var element3 = new SimpleElement { Value = 1000, AggregateType = AggregateType.AddFinal };
            this.composableAttribute.AddElement(element1);
            this.composableAttribute.AddElement(element2);
            this.composableAttribute.AddElement(element3);
            Assert.That(this.composableAttribute.Value, Is.EqualTo((element1.Value * element2.Value) + element3.Value));
            this.composableAttribute.RemoveElement(element2);
            Assert.That(this.composableAttribute.Value, Is.EqualTo(element1.Value + element3.Value));
        }

        /// <summary>
        /// Tests if the <see cref="BaseAttribute.ValueChanged"/> is called when the depending element value changed.
        /// </summary>
        [Test]
        public void ValueChangedEvent()
        {
            var element = new SimpleElement { Value = 5 };
            this.composableAttribute.AddElement(element);

            bool eventCalled = false;
            this.composableAttribute.ValueChanged += (sender, e) => eventCalled = true;
            element.Value = 6;

            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="BaseAttribute.ValueChanged"/> is called when a new depending element was added.
        /// </summary>
        [Test]
        public void ValueChangedEventWhenElementAdded()
        {
            var element = new SimpleElement { Value = 5 };
            bool eventCalled = false;
            this.composableAttribute.ValueChanged += (sender, e) => eventCalled = true;
            this.composableAttribute.AddElement(element);
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="BaseAttribute.ValueChanged"/> is called when a new depending element was removed.
        /// </summary>
        [Test]
        public void ValueChangedEventWhenElementRemoved()
        {
            var element = new SimpleElement { Value = 5 };
            this.composableAttribute.AddElement(element);

            bool eventCalled = false;
            this.composableAttribute.ValueChanged += (sender, e) => eventCalled = true;
            this.composableAttribute.RemoveElement(element);
            Assert.That(eventCalled, Is.True);
        }
    }
}
