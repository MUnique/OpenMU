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
    /// Tests the base count of monsters of a herd, which grows with the elapsed game time.
    /// </summary>
    /// <param name="elapsedSeconds">The elapsed game time in seconds.</param>
    /// <param name="expectedCount">The expected base count.</param>
    [TestCase(0, 1)]
    [TestCase(179, 1)]
    [TestCase(180, 2)]
    [TestCase(359, 2)]
    [TestCase(360, 3)]
    [TestCase(600, 3)]
    public void HerdBaseCount(int elapsedSeconds, int expectedCount)
    {
        Assert.That(this._definition.GetHerdBaseCount(TimeSpan.FromSeconds(elapsedSeconds)), Is.EqualTo(expectedCount));
    }

    /// <summary>
    /// Tests that the multipliers of the monsters are chosen by the highest player level, rounded up to the next
    /// range of ten levels, and that levels above the last range use the last one.
    /// </summary>
    /// <param name="playerLevel">The highest player level, including the master level.</param>
    /// <param name="expectedMaximumPlayerLevel">The expected maximum player level of the chosen multipliers.</param>
    [TestCase(1, 10)]
    [TestCase(10, 10)]
    [TestCase(11, 20)]
    [TestCase(400, 400)]
    [TestCase(401, 410)]
    [TestCase(600, 600)]
    [TestCase(601, 600)]
    [TestCase(800, 600)]
    public void MonsterScalingByPlayerLevel(int playerLevel, int expectedMaximumPlayerLevel)
    {
        Assert.That(this._definition.GetMonsterScaling(playerLevel)?.MaximumPlayerLevel, Is.EqualTo(expectedMaximumPlayerLevel));
    }

    /// <summary>
    /// Tests that the default multipliers contain a value for one to five players.
    /// </summary>
    [Test]
    public void DefaultMonsterScalingsAreComplete()
    {
        Assert.That(this._definition.MonsterScalings, Has.Count.EqualTo(60));
        Assert.That(
            this._definition.MonsterScalings,
            Has.All.Matches<DoppelgangerMonsterScaling>(scaling =>
                scaling.LevelMultipliers.Count == 5
                && scaling.HealthMultipliers.Count == 5
                && scaling.DamageMultipliers.Count == 5
                && scaling.DefenseMultipliers.Count == 5));
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
