// <copyright file="AttributeRelationshipElementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="AttributeRelationshipElement"/>.
/// </summary>
[TestFixture]
public class AttributeRelationshipElementTests
{
    /// <summary>
    /// Tests if a relationship between two elements with the <see cref="InputOperator.Add"/> is handled correctly.
    /// Both element values should be summed up, and according to the <see cref="InputOperator.Add"/> the inputOperand should be added on top.
    /// </summary>
    [Test]
    public void InputOperatorAdd()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.Add);
        Assert.That(relationshipElement.Value, Is.EqualTo(element1Value + element2Value + inputOperand));
    }

    /// <summary>
    /// Tests if a relationship between two elements with the <see cref="InputOperator.Multiply"/> is handled correctly.
    /// Both element values should be summed up, and according to the <see cref="InputOperator.Multiply"/> the sum should be multiplied with the inputOperand.
    /// </summary>
    [Test]
    public void InputOperatorMultiply()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.Multiply);
        Assert.That(relationshipElement.Value, Is.EqualTo((element1Value + element2Value) * inputOperand));
    }

    /// <summary>
    /// Tests if a relationship between two elements with the <see cref="InputOperator.Exponentiate"/> is handled correctly.
    /// Both element values should be summed up, and according to the <see cref="InputOperator.Exponentiate"/> the sum should be raised by the power of inputOperand.
    /// </summary>
    [Test]
    public void InputOperatorPower()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.Exponentiate);
        Assert.That(relationshipElement.Value, Is.EqualTo(Math.Pow(element1Value + element2Value, inputOperand)));
    }

    /// <summary>
    /// Tests if a relationship between two elements with the <see cref="InputOperator.ExponentiateByAttribute"/> is handled correctly.
    /// Both element values should be summed up, and according to the <see cref="InputOperator.ExponentiateByAttribute"/> the input operand should be raised by the power of the sum.
    /// </summary>
    [Test]
    public void InputOperatorPowerByAttribute()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.ExponentiateByAttribute);
        Assert.That(relationshipElement.Value, Is.EqualTo(Math.Pow(inputOperand, element1Value + element2Value)));
    }

    /// <summary>
    /// Tests if the <see cref="AttributeRelationshipElement.Value"/> is updated correctly when one of the elements value changed.
    /// </summary>
    [Test]
    public void ValueChangesWhenElementChanges()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.Add);
        Assert.That(relationshipElement.Value, Is.EqualTo(element1Value + element2Value + inputOperand));
        const int element1NewValue = 2;
        element1.Value = element1NewValue;
        Assert.That(relationshipElement.Value, Is.EqualTo(element1NewValue + element2Value + inputOperand));
    }

    /// <summary>
    /// Tests if the <see cref="SimpleElement.ValueChanged"/> is called when one of the elements value changed.
    /// </summary>
    [Test]
    public void ValueChangedEventWhenElementChanges()
    {
        const int element1Value = 1;
        const int element2Value = 2;
        const int inputOperand = 10;
        var element1 = new SimpleElement { Value = element1Value };
        var element2 = new SimpleElement { Value = element2Value };
        var operandElement = new ConstantElement(inputOperand);
        var relationshipElement = new AttributeRelationshipElement(new[] { element1, element2 }, operandElement, InputOperator.Add);
        var eventCalled = false;
        relationshipElement.ValueChanged += (sender, e) => eventCalled = true;
        element1.Value = 2;
        Assert.That(eventCalled, Is.True);
    }
}