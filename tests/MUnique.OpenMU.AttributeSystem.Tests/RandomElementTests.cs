// <copyright file="RandomElementTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="RandomElementTests"/>.
/// </summary>
[TestFixture]
public class RandomElementTests
{
    /// <summary>
    /// Tests if the value of the element is as defined in the constructor.
    /// </summary>
    [Test]
    public void ValueAsDefined()
    {
        const int minValue = 4;
        const int maxValue = 10;
        var element = new RandomElement(minValue, maxValue);
        var randomValue = element.Value;
        Assert.That(randomValue, Is.GreaterThanOrEqualTo(minValue));
        Assert.That(randomValue, Is.LessThanOrEqualTo(maxValue));
    }
}