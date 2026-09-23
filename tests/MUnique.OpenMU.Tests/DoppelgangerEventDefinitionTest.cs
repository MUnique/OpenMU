// <copyright file="DoppelgangerEventDefinitionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the <see cref="DoppelgangerEventDefinition"/>.
/// </summary>
[TestFixture]
public class DoppelgangerEventDefinitionTest
{
    private readonly DoppelgangerEventDefinition _definition = DoppelgangerEventDefinition.CreateDefault();

    /// <summary>
    /// Tests that the default definition contains a complete path for each of the four event maps.
    /// </summary>
    [Test]
    public void DefaultPathsAreComplete()
    {
        Assert.That(this._definition.Paths.Select(path => path.MapNumber), Is.EquivalentTo(new short[] { 65, 66, 67, 68 }));
        Assert.That(this._definition.Paths, Has.All.Matches<DoppelgangerPath>(path => path.Areas.Count == IDoppelgangerEventViewPlugIn.MaximumPathPosition + 1));
    }

    /// <summary>
    /// Tests the position index of points on the path of the first map.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="expectedPosition">The expected position index.</param>
    [TestCase(225, 103, 0)]
    [TestCase(197, 27, 22)]
    [TestCase(197, 46, 17)]
    [TestCase(10, 10, 0)]
    public void PathPosition(byte x, byte y, int expectedPosition)
    {
        Assert.That(this._definition.GetPathPosition(65, new Point(x, y)), Is.EqualTo(expectedPosition));
    }

    /// <summary>
    /// Tests that the lower bounds of a path area are exclusive and the upper bounds are inclusive,
    /// as in the original server.
    /// </summary>
    [Test]
    public void PathAreaBounds()
    {
        var area = new DoppelgangerPathArea(10, 20, 15, 25);

        Assert.That(area.Contains(new Point(10, 22)), Is.False);
        Assert.That(area.Contains(new Point(12, 20)), Is.False);
        Assert.That(area.Contains(new Point(15, 25)), Is.True);
        Assert.That(area.Center, Is.EqualTo(new Point(12, 22)));
    }

    /// <summary>
    /// Tests that there is no position on a map without a path.
    /// </summary>
    [Test]
    public void NoPositionOnOtherMaps()
    {
        Assert.That(this._definition.GetPathPosition(0, new Point(225, 103)), Is.Zero);
    }
}
