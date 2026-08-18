// <copyright file="ComposableAttributeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="ComposableAttribute"/>.
/// </summary>
[TestFixture]
public class ComposableAttributeTests
{
    private ComposableAttribute _composableAttribute = null!;

    /// <summary>
    /// Sets up each test case.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        var attributeDefinition = new AttributeDefinition(new Guid("52263EA9-F309-475D-B10B-352D3BFD7650"), "Test attribute", "Test attribute");
        this._composableAttribute = new ComposableAttribute(attributeDefinition);
    }

    /// <summary>
    /// Tests if the value is 0 after creation.
    /// </summary>
    [Test]
    public void ValueIsNullAfterCreation()
    {
        Assert.That(this._composableAttribute.Value, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests if the value is updated after adding an element.
    /// </summary>
    [Test]
    public void ValueAfterAddedElement()
    {
        var element = new ConstantElement(4711);
        this._composableAttribute.AddElement(element);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element.Value));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/> by using <see cref="AggregateType.AddRaw"/>.
    /// </summary>
    [Test]
    public void ValueOfMultipleRawElements()
    {
        var element1 = new ConstantElement(3000);
        var element2 = new ConstantElement(5000);
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element1.Value + element2.Value));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
    /// by using <see cref="AggregateType.AddRaw"/> in the first element and
    /// by using <see cref="AggregateType.Multiplicate"/> in the second element.
    /// </summary>
    [Test]
    public void ValueWithRawAndMultiplierElements()
    {
        var element1 = new ConstantElement(3000);
        var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element1.Value * element2.Value));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
    /// by using <see cref="AggregateType.AddRaw"/> in the first element,
    /// by using <see cref="AggregateType.Multiplicate"/> in the second element and
    /// by using <see cref="AggregateType.AddFinal"/> in the last element.
    /// </summary>
    [Test]
    public void ValueWithRawMultiplierAndFinalElements()
    {
        var element1 = new ConstantElement(3000);
        var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
        var element3 = new SimpleElement { Value = 1000, AggregateType = AggregateType.AddFinal };
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        this._composableAttribute.AddElement(element3);
        Assert.That(this._composableAttribute.Value, Is.EqualTo((element1.Value * element2.Value) + element3.Value));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
    /// by using one <see cref="AggregateType.AddRaw"/> element and
    /// by using several <see cref="AggregateType.Maximum"/> elements.
    /// </summary>
    [Test]
    public void ValueWithRawAndMultipleMaximumElements()
    {
        var element1 = new ConstantElement(3000);
        var element2 = new SimpleElement { Value = 5, AggregateType = AggregateType.Maximum };
        var element3 = new SimpleElement { Value = 1000, AggregateType = AggregateType.Maximum };
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        this._composableAttribute.AddElement(element3);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element1.Value + Math.Max(element2.Value, element3.Value)));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
    /// by using <see cref="AggregateType.Multiplicate"/> elements exclusively.
    /// A <see cref="AggregateType.AddRaw"/> element of value 1 should be assumed.
    /// </summary>
    [Test]
    public void ValueWithMultiplierElementsOnly()
    {
        var element1 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
        var element2 = new SimpleElement { Value = 1000, AggregateType = AggregateType.Multiplicate };
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element1.Value * element2.Value));
    }

    /// <summary>
    /// Tests if the value of multiple elements is combined in <see cref="ComposableAttribute.Value"/>
    /// by using <see cref="AggregateType.Multiplicate"/> in the first element and
    /// by using <see cref="AggregateType.AddFinal"/> in the second element.
    /// </summary>
    [Test]
    public void ValueWithMultiplierAndFinalElements()
    {
        var element1 = new SimpleElement { Value = 5, AggregateType = AggregateType.Multiplicate };
        var element2 = new SimpleElement { Value = 1000, AggregateType = AggregateType.AddFinal };
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(1000));
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
        this._composableAttribute.AddElement(element1);
        this._composableAttribute.AddElement(element2);
        this._composableAttribute.AddElement(element3);
        Assert.That(this._composableAttribute.Value, Is.EqualTo((element1.Value * element2.Value) + element3.Value));
        this._composableAttribute.RemoveElement(element2);
        Assert.That(this._composableAttribute.Value, Is.EqualTo(element1.Value + element3.Value));
    }

    /// <summary>
    /// Tests if the <see cref="BaseAttribute.ValueChanged"/> is called when the depending element value changed.
    /// </summary>
    [Test]
    public void ValueChangedEvent()
    {
        var element = new SimpleElement { Value = 5 };
        this._composableAttribute.AddElement(element);

        var eventCalled = false;
        this._composableAttribute.ValueChanged += (_, _) => eventCalled = true;
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
        this._composableAttribute.ValueChanged += (_, _) => eventCalled = true;
        this._composableAttribute.AddElement(element);
        Assert.That(eventCalled, Is.True);
    }

    /// <summary>
    /// Tests if the <see cref="BaseAttribute.ValueChanged"/> is called when a new depending element was removed.
    /// </summary>
    [Test]
    public void ValueChangedEventWhenElementRemoved()
    {
        var element = new SimpleElement { Value = 5 };
        this._composableAttribute.AddElement(element);

        bool eventCalled = false;
        this._composableAttribute.ValueChanged += (_, _) => eventCalled = true;
        this._composableAttribute.RemoveElement(element);
        Assert.That(eventCalled, Is.True);
    }

    /// <summary>
    /// Tests that reading <see cref="ComposableAttribute.Value"/> on one thread while elements are
    /// added and removed on another thread does not throw. This is a regression test for the race
    /// condition where a magic effect expiring on a timer thread removed an element while the value was
    /// being recalculated, exposing a <c>null</c> list slot to the enumeration and throwing a
    /// <see cref="NullReferenceException"/> (or an <see cref="InvalidOperationException"/>).
    /// </summary>
    /// <returns>The task representing the asynchronous test.</returns>
    [Test]
    public async Task ConcurrentAddRemoveWhileReadingValueDoesNotThrow()
    {
        const int cycles = 20000;
        foreach (var seed in Enumerable.Range(1, 64))
        {
            this._composableAttribute.AddElement(new SimpleElement { Value = seed, AggregateType = AggregateType.AddRaw });
        }

        Exception? readerError = null;

        var writer = Task.Run(() =>
        {
            for (var i = 0; i < cycles; i++)
            {
                // Each cycle mutates the element list twice and invalidates the cached value, mirroring
                // a magic effect that is applied and then expires on a background thread.
                var element = new SimpleElement { Value = 1, AggregateType = AggregateType.AddRaw };
                this._composableAttribute.AddElement(element);
                this._composableAttribute.RemoveElement(element);
            }
        });

        var reader = Task.Run(() =>
        {
            try
            {
                while (!writer.IsCompleted)
                {
                    // The concurrent cache invalidation forces a recompute, which enumerates the list.
                    _ = this._composableAttribute.Value;
                }
            }
            catch (Exception ex)
            {
                readerError = ex;
            }
        });

        await Task.WhenAll(writer, reader).ConfigureAwait(false);

        // The fix guarantees crash-freedom, not value coherency: the value cache stays best-effort under
        // concurrency (a stale or torn read corrects itself on the next recalculation), so this test only
        // asserts that the concurrent reads never threw.
        Assert.That(readerError, Is.Null, $"Reading the value concurrently threw: {readerError}");
    }

    /// <summary>
    /// Tests that recalculating a parent attribute concurrently with mutations of a child attribute it
    /// depends on neither throws nor deadlocks. The parent's value is computed from an
    /// <see cref="AttributeRelationshipElement"/> that wraps the child attribute, so a recalculation
    /// walks into the child while the child's elements are being added and removed on another thread.
    /// This reproduces the nested variant of the race that surfaced through
    /// <see cref="AttributeRelationshipElement"/> in the field, and it fails with a timeout instead of
    /// hanging should the locking ever deadlock.
    /// </summary>
    /// <returns>The task representing the asynchronous test.</returns>
    [Test]
    public async Task ConcurrentRecalculationOfDependentAttributesDoesNotDeadlock()
    {
        const int cycles = 50000;
        var childDefinition = new AttributeDefinition(new Guid("7C2E4C1B-2C0A-4E7A-9C9E-1B0D3F5A6E77"), "Child attribute", "Child attribute");
        var child = new ComposableAttribute(childDefinition);
        child.AddElement(new ConstantElement(10));

        // The parent reads the child attribute as an input element, so recalculating the parent recurses
        // into the child while the child is mutated concurrently.
        this._composableAttribute.AddElement(new AttributeRelationshipElement(new IElement[] { child }, new ConstantElement(0), InputOperator.Add));

        Exception? error = null;

        var writer = Task.Run(() =>
        {
            for (var i = 0; i < cycles; i++)
            {
                var element = new SimpleElement { Value = 1, AggregateType = AggregateType.AddRaw };
                child.AddElement(element);
                child.RemoveElement(element);
            }
        });

        var reader = Task.Run(() =>
        {
            try
            {
                while (!writer.IsCompleted)
                {
                    _ = this._composableAttribute.Value;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });

        await Task.WhenAll(writer, reader).WaitAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

        Assert.That(error, Is.Null, $"Reading the dependent value concurrently threw: {error}");
    }
}