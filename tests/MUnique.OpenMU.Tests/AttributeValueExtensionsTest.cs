// <copyright file="AttributeValueExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView;

/// <summary>
/// Tests for <see cref="AttributeValueExtensions"/>.
/// </summary>
[TestFixture]
public class AttributeValueExtensionsTest
{
    /// <summary>
    /// Tests that attribute values are clamped to the range of <see cref="ushort"/> instead of wrapping around.
    /// </summary>
    /// <param name="value">The attribute value.</param>
    /// <param name="expected">The expected packet value.</param>
    [TestCase(-5f, (ushort)0)]
    [TestCase(0f, (ushort)0)]
    [TestCase(47505f, (ushort)47505)]
    [TestCase(65535f, ushort.MaxValue)]
    [TestCase(290505f, ushort.MaxValue)]
    public void ToUInt16Clamped(float value, ushort expected)
    {
        Assert.That(value.ToUInt16Clamped(), Is.EqualTo(expected));
    }
}
