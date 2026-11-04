// <copyright file="CharacterMoveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the <see cref="PointExtensions"/>.
/// </summary>
[TestFixture]
internal class PointExtensionsTest
{
    /// <summary>
    /// Gets the angle degree of 180 °.
    /// </summary>
    [Test]
    public void GetAngleDegreeTo_180Degree()
    {
        var start = new Point(100, 100);
        var end = new Point(100, 101);

        var degree = start.GetAngleDegreeTo(end);
        Assert.That(degree, Is.EqualTo(180.0));
    }

    /// <summary>
    /// Gets the angle degree of 0 °.
    /// </summary>
    [Test]
    public void GetAngleDegreeTo_0Degree()
    {
        var start = new Point(100, 100);
        var end = new Point(100, 99);

        var degree = start.GetAngleDegreeTo(end);
        Assert.That(degree, Is.EqualTo(0.0));
    }

    /// <summary>
    /// Gets the angle degree of 90 °.
    /// </summary>
    [Test]
    public void GetAngleDegreeTo_90Degree()
    {
        var start = new Point(100, 100);
        var end = new Point(101, 100);

        var degree = start.GetAngleDegreeTo(end);
        Assert.That(degree, Is.EqualTo(90.0));
    }

    /// <summary>
    /// Gets the angle degree of 270 °.
    /// </summary>
    [Test]
    public void GetAngleDegreeTo_270Degree()
    {
        var start = new Point(100, 100);
        var end = new Point(99, 100);

        var degree = start.GetAngleDegreeTo(end);
        Assert.That(degree, Is.EqualTo(270.0));
    }
}
