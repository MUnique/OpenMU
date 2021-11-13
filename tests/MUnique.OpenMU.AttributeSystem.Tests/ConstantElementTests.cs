// <copyright file="ConstantElementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="ConstantElement"/>.
/// </summary>
[TestFixture]
public class ConstantElementTests
{
    /// <summary>
    /// Tests if the value of the element is as defined in the constructor.
    /// </summary>
    [Test]
    public void ValueAsDefined()
    {
        const int constantValue = 999;
        var element = new ConstantElement(constantValue);
        Assert.That(element.Value, Is.EqualTo(constantValue));
    }

    /// <summary>
    /// Tests if the <see cref="ConstantElement.AggregateType"/> is <see cref="AggregateType.AddRaw"/>.
    /// </summary>
    [Test]
    public void AggregateTypeIsRaw()
    {
        const int constantValue = 999;
        var element = new ConstantElement(constantValue);
        Assert.That(element.AggregateType, Is.EqualTo(AggregateType.AddRaw));
    }
}