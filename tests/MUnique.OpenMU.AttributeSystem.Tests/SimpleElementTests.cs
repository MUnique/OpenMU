// <copyright file="SimpleElementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="SimpleElement"/>.
/// </summary>
[TestFixture]
public class SimpleElementTests
{
    /// <summary>
    /// Tests if the value is 0 after creation.
    /// </summary>
    [Test]
    public void ValueIsNullAfterCreation()
    {
        var element = new SimpleElement();
        Assert.That(element.Value, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if the value returns the same as what has been set before.
    /// </summary>
    [Test]
    public void ValueIsSet()
    {
        const int elementValue = 4711;
        var element = new SimpleElement { Value = elementValue };
        Assert.That(element.Value, Is.EqualTo(elementValue));
    }

    /// <summary>
    /// Tests if the <see cref="SimpleElement.ValueChanged"/> is called when the <see cref="SimpleElement.Value"/> has been changed.
    /// </summary>
    [Test]
    public void ValueChangedEventWhenValueChanges()
    {
        var element = new SimpleElement();
        bool eventCalled = false;
        element.ValueChanged += (sender, e) => eventCalled = true;
        element.Value = 6;

        Assert.That(eventCalled, Is.True);
    }

    /// <summary>
    /// Tests if the <see cref="SimpleElement.ValueChanged"/> is called when the <see cref="SimpleElement.AggregateType"/> has been changed.
    /// </summary>
    [Test]
    public void ValueChangedEventWhenAggregateTypeChanges()
    {
        var element = new SimpleElement { Value = 0 };
        bool eventCalled = false;
        element.ValueChanged += (sender, e) => eventCalled = true;
        element.AggregateType = AggregateType.AddFinal;

        Assert.That(eventCalled, Is.True);
    }
}