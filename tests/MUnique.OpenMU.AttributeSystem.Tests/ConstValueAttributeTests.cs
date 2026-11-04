// <copyright file="ConstValueAttributeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="ConstValueAttribute"/>.
/// </summary>
[TestFixture]
public class ConstValueAttributeTests
{
    /// <summary>
    /// Tests if the value of the attribute is as defined in the constructor.
    /// </summary>
    [Test]
    public void ValueAsDefined()
    {
        const int constantValue = 999;
        var element = new ConstValueAttribute(constantValue, new AttributeDefinition());
        Assert.That(element.Value, Is.EqualTo(constantValue));
    }

    /// <summary>
    /// Tests if the <see cref="ConstValueAttribute.AggregateType"/> is <see cref="AggregateType.AddRaw"/>.
    /// </summary>
    [Test]
    public void AggregateTypeIsRaw()
    {
        var element = new ConstValueAttribute(999, new AttributeDefinition());
        Assert.That(element.AggregateType, Is.EqualTo(AggregateType.AddRaw));
    }
}