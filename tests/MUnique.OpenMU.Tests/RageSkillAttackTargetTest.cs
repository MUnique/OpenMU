// <copyright file="RageSkillAttackTargetTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the target selection of the rage fighter area skills (dark side).
/// </summary>
[TestFixture]
internal class RageSkillAttackTargetTest
{
    /// <summary>
    /// Tests that the explicit target comes first and the other targets are the
    /// nearest ones to the search center (the explicit target), not to the player.
    /// </summary>
    [Test]
    public void ExplicitTargetFirstAndNearestToCenter()
    {
        var player = CreateAttackable(10, 10);
        var explicitTarget = CreateAttackable(14, 10);
        var nearTarget = CreateAttackable(15, 10);
        var farFromTarget = CreateAttackable(11, 10);

        var targets = RageSkillAttackAction.SelectTargets(
            player,
            explicitTarget,
            explicitTarget.Position,
            new[] { farFromTarget, player, explicitTarget, nearTarget });

        Assert.That(targets, Is.EqualTo(new[] { explicitTarget, nearTarget, farFromTarget }));
    }

    /// <summary>
    /// Tests that not more than the maximum number of targets are selected.
    /// </summary>
    [Test]
    public void LimitedToMaximumTargets()
    {
        var player = CreateAttackable(10, 10);
        var candidates = Enumerable.Range(0, 8).Select(i => CreateAttackable((byte)(11 + i), 10)).ToList();

        var targets = RageSkillAttackAction.SelectTargets(player, null, player.Position, candidates);

        Assert.That(targets, Is.EqualTo(candidates.Take(RageSkillAttackAction.MaximumTargetsPerAttack)));
    }

    private static IAttackable CreateAttackable(byte x, byte y)
    {
        var mock = new Mock<IAttackable>();
        mock.Setup(a => a.Position).Returns(new Point(x, y));
        return mock.Object;
    }
}
