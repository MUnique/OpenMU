// <copyright file="CrywolfEventDefinitionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the <see cref="CrywolfEventDefinition"/>.
/// </summary>
[TestFixture]
public class CrywolfEventDefinitionTest
{
    /// <summary>
    /// Tests that the rank of a player is determined by its score, like in the original game.
    /// </summary>
    /// <param name="score">The score.</param>
    /// <param name="expectedRank">The expected rank, from 0 (D) to 4 (S).</param>
    [TestCase(0, 0)]
    [TestCase(1000, 0)]
    [TestCase(1001, 1)]
    [TestCase(3001, 2)]
    [TestCase(5000, 2)]
    [TestCase(5001, 3)]
    [TestCase(10001, 4)]
    [TestCase(99999, 4)]
    public void RankByScore(int score, int expectedRank)
    {
        Assert.That(new CrywolfEventDefinition().GetRank(score), Is.EqualTo(expectedRank));
    }

    /// <summary>
    /// Tests that the players get only a part of the experience when the fortress has been occupied.
    /// </summary>
    [Test]
    public void ExperienceByRankAndResult()
    {
        var definition = new CrywolfEventDefinition();

        Assert.That(definition.GetExperience(0, true), Is.Zero);
        Assert.That(definition.GetExperience(4, true), Is.EqualTo(1_800_000));
        Assert.That(definition.GetExperience(4, false), Is.EqualTo(180_000));
        Assert.That(definition.GetExperience(2, false), Is.EqualTo(80_000));
    }

    /// <summary>
    /// Tests that the event only starts on the configured days and times.
    /// </summary>
    [Test]
    public void StartTimes()
    {
        var definition = new CrywolfEventDefinition();

        // 2026-09-30 is a wednesday, 2026-09-29 a tuesday.
        Assert.That(definition.IsStartTime(new DateTime(2026, 09, 30, 20, 30, 2)), Is.True);
        Assert.That(definition.IsStartTime(new DateTime(2026, 09, 30, 20, 31, 0)), Is.False);
        Assert.That(definition.IsStartTime(new DateTime(2026, 09, 29, 20, 30, 2)), Is.False);
    }

    /// <summary>
    /// Tests the scores of the monsters and the durations of the states.
    /// </summary>
    [Test]
    public void ScoresAndDurations()
    {
        var definition = new CrywolfEventDefinition();

        Assert.That(definition.GetScore(349), Is.EqualTo(7000));
        Assert.That(definition.GetScore(340), Is.EqualTo(3000));
        Assert.That(definition.GetScore(310), Is.Zero, "The common monsters of the map don't count.");
        Assert.That(definition.GetDuration(CrywolfState.Start), Is.EqualTo(TimeSpan.FromMinutes(15)));
        Assert.That(definition.GetDuration(CrywolfState.None), Is.EqualTo(TimeSpan.Zero));
        Assert.That(definition.GetAltarIndex(207), Is.EqualTo(2));
        Assert.That(definition.GetAltarIndex(204), Is.EqualTo(-1));
    }

    /// <summary>
    /// Tests that a monster walks along the waypoints to a goal which is far away, and directly to a goal which is near.
    /// </summary>
    [Test]
    public void WalkTargetsAlongWaypoints()
    {
        var goal = new Pathfinding.Point(121, 36);

        Assert.That(CrywolfMonsterIntelligence.GetNextWalkTarget(new Pathfinding.Point(125, 40), goal), Is.EqualTo(goal));

        var start = new Pathfinding.Point(110, 79);
        var waypoint = CrywolfMonsterIntelligence.GetNextWalkTarget(start, goal);
        Assert.That(waypoint, Is.Not.EqualTo(goal));
        Assert.That(waypoint.EuclideanDistanceTo(start), Is.LessThan(20));
        Assert.That(waypoint.EuclideanDistanceTo(goal), Is.LessThan(start.EuclideanDistanceTo(goal)));
    }

    /// <summary>
    /// Tests that Balgass and the army have the skills of the original game by default.
    /// </summary>
    [Test]
    public void DefaultMonsterSkills()
    {
        var skills = new CrywolfEventDefinition().MonsterSkills;

        var balgassSkills = skills.Where(skill => skill.MonsterNumber == 349).ToList();
        Assert.That(balgassSkills.Select(skill => skill.SkillNumber), Is.EquivalentTo(new short[] { 12, 13 }));
        Assert.That(balgassSkills.Select(skill => skill.Radius), Is.All.EqualTo(6));
        Assert.That(skills.Select(skill => skill.MonsterNumber).Distinct(), Is.EquivalentTo(new short[] { 340, 341, 344, 345, 349 }));
    }

    /// <summary>
    /// Tests that a pushed or escaping object moves away from the origin, but not onto blocked tiles or into a safezone.
    /// </summary>
    [Test]
    public void PointAwayFromOriginStopsAtObstacles()
    {
        var terrain = new GameMapTerrain((byte[]?)null);
        var origin = new Point(100, 100);
        var start = new Point(101, 100);
        var direction = origin.GetDirectionTo(start);
        var first = start.CalculateTargetPoint(direction);
        var second = first.CalculateTargetPoint(direction);
        var third = second.CalculateTargetPoint(direction);
        terrain.WalkMap[first.X, first.Y] = true;
        terrain.WalkMap[second.X, second.Y] = true;
        terrain.WalkMap[third.X, third.Y] = true;

        Assert.That(terrain.GetPointAwayFrom(origin, start, 3), Is.EqualTo(third));

        terrain.WalkMap[third.X, third.Y] = false;
        Assert.That(terrain.GetPointAwayFrom(origin, start, 3), Is.EqualTo(second));

        terrain.SafezoneMap[first.X, first.Y] = true;
        Assert.That(terrain.GetPointAwayFrom(origin, start, 3), Is.EqualTo(start));
    }
}
