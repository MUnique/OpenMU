// <copyright file="FrustumBasedTargetFilterTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the <see cref="FrustumBasedTargetFilter"/>.
/// </summary>
[TestFixture]
internal class FrustumBasedTargetFilterTest
{
    /// <summary>
    /// Tests that a single projectile can hit a target in the center of the frustum.
    /// </summary>
    [Test]
    public void SingleProjectile_TargetInCenter_CanHit()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 1);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 105); // Directly in front (positive Y)

        // Rotation 128 points in +Y direction (180 degrees in 0-255 system)
        var result = filter.IsTargetWithinBounds(attacker, target, 128, 0);

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that with triple shot, a target directly in front can be hit by the all projectiles.
    /// </summary>
    [Test]
    public void TripleShot_TargetNear_CanBeHitByAllProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 101);

        // Rotation 128 points in +Y direction
        // Check if all projectiles can hit
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.True);
    }

    /// <summary>
    /// Tests that with triple shot, a target in the center can be hit by the center projectile.
    /// </summary>
    [Test]
    public void TripleShot_TargetInCenter_CanBeHitByCenterProjectile()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 105); // Directly in front (positive Y)

        // Rotation 128 points in +Y direction
        // Check if the center projectile (index 1) can hit
        var result = filter.IsTargetWithinBounds(attacker, target, 128, 1);

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that with triple shot, a target on the left side can be hit by the left projectile.
    /// </summary>
    [Test]
    public void TripleShot_TargetOnLeft_CanBeHitByLeftProjectile()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(98, 105); // To the left and in front (2 units left, within frustum)

        // Rotation 128 points in +Y direction
        // Left projectile should be able to hit (index 0)
        var leftResult = filter.IsTargetWithinBounds(attacker, target, 128, 0);
        Assert.That(leftResult, Is.True);

        // Right projectile should NOT be able to hit (index 2)
        var rightResult = filter.IsTargetWithinBounds(attacker, target, 128, 2);
        Assert.That(rightResult, Is.False);
    }

    /// <summary>
    /// Tests that with triple shot, a target on the right side can be hit by the right projectile.
    /// </summary>
    [Test]
    public void TripleShot_TargetOnRight_CanBeHitByRightProjectile()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(102, 105); // To the right and in front (2 units right, within frustum)

        // Rotation 128 points in +Y direction
        // Right projectile should be able to hit (index 2)
        var rightResult = filter.IsTargetWithinBounds(attacker, target, 128, 2);
        Assert.That(rightResult, Is.True);

        // Left projectile should NOT be able to hit (index 0)
        var leftResult = filter.IsTargetWithinBounds(attacker, target, 128, 0);
        Assert.That(leftResult, Is.False);
    }

    /// <summary>
    /// Tests that a target outside the frustum cannot be hit by any projectile.
    /// </summary>
    [Test]
    public void TripleShot_TargetOutsideFrustum_CannotBeHit()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(110, 105); // Far to the right, outside frustum

        // Rotation 128 points in +Y direction
        // No projectile should be able to hit
        for (int i = 0; i < 3; i++)
        {
            var result = filter.IsTargetWithinBounds(attacker, target, 128, i);
            Assert.That(result, Is.False, $"Projectile {i} should not hit target outside frustum");
        }
    }

    /// <summary>
    /// Tests that the old IsTargetWithinBounds method still works for backward compatibility.
    /// </summary>
    [Test]
    public void IsTargetWithinBounds_TargetInFrustum_ReturnsTrue()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 105); // Directly in front

        // Rotation 128 points in +Y direction
        var result = filter.IsTargetWithinBounds(attacker, target, 128);

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the old IsTargetWithinBounds method returns false for targets outside the frustum.
    /// </summary>
    [Test]
    public void IsTargetWithinBounds_TargetOutsideFrustum_ReturnsFalse()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(110, 105); // Far to the right, outside frustum

        // Rotation 128 points in +Y direction
        var result = filter.IsTargetWithinBounds(attacker, target, 128);

        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that with triple shot, a target two tiles ahead in the center is only covered by the center projectile.
    /// </summary>
    [Test]
    public void TripleShot_TargetTwoTilesAhead_CenterLaneOnly()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 102); // Directly in front, distance 2

        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.False);
    }

    /// <summary>
    /// Tests that with triple shot, a target three tiles ahead and slightly off-center is covered by two projectiles.
    /// </summary>
    [Test]
    public void TripleShot_TargetSlightlyOffCenter_CoveredByTwoProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(101, 103); // Slightly right of center, distance 3

        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.True);
    }

    /// <summary>
    /// Tests that with triple shot, a diagonally adjacent locked target is covered by all three projectiles.
    /// </summary>
    [Test]
    public void TripleShot_DiagonalAdjacentLockedTarget_CoveredByAllProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 3);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(101, 101);

        // Same rotation correction as CorrectRotationForLockedTarget (rounded, RotationByteScale 255).
        var angle = GetAngleDegreeTo(attacker.Position, target.Position);
        var rotation = (byte)Math.Round(angle / 360.0 * 255.0, MidpointRounding.AwayFromZero);

        Assert.That(filter.IsTargetWithinBounds(attacker, target, rotation, 0), Is.True, "left projectile");
        Assert.That(filter.IsTargetWithinBounds(attacker, target, rotation, 1), Is.True, "center projectile");
        Assert.That(filter.IsTargetWithinBounds(attacker, target, rotation, 2), Is.True, "right projectile");
    }

    private static double GetAngleDegreeTo(Point start, Point target)
    {
        var diffX = start.X - target.X;
        var diffY = start.Y - target.Y;
        var radian = Math.Atan2(diffY, diffX);
        var result = (radian * 180 / Math.PI) - 90;
        if (result < 0)
        {
            result += 360;
        }

        return result;
    }

    /// <summary>
    /// Tests that with multi shot, an adjacent centered target is covered by all five projectiles:
    /// one tile out the arrows haven't diverged yet.
    /// </summary>
    [Test]
    public void MultiShot_TargetAdjacent_CoveredByAllProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 6f, 7f, 5);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 101);

        for (int i = 0; i < 5; i++)
        {
            Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, i), Is.True, $"Projectile {i} should hit the adjacent target");
        }
    }

    /// <summary>
    /// Tests that with multi shot, a centered target two tiles out is covered by the inner three projectiles.
    /// </summary>
    [Test]
    public void MultiShot_TargetTwoTilesOut_CoveredByInnerThreeProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 6f, 7f, 5);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 102);

        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 3), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 4), Is.False);
    }

    /// <summary>
    /// Tests that with multi shot, a centered target four tiles out is only covered by the center projectile.
    /// </summary>
    [Test]
    public void MultiShot_TargetFourTilesOut_CoveredByCenterProjectileOnly()
    {
        var filter = new FrustumBasedTargetFilter(1f, 6f, 7f, 5);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 104);

        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 3), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 4), Is.False);
    }

    /// <summary>
    /// Tests that with four projectiles (e.g. triple shot with a bow), an adjacent centered
    /// target is covered by all four projectiles: one tile out the arrows haven't diverged yet.
    /// </summary>
    [Test]
    public void TripleShotWithExtraProjectile_TargetAdjacent_CoveredByAllProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 4);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 101);

        for (int i = 0; i < 4; i++)
        {
            Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, i), Is.True, $"Projectile {i} should hit the adjacent target");
        }
    }

    /// <summary>
    /// Tests that with four projectiles, a centered target two tiles out is covered
    /// by the inner two projectiles only.
    /// </summary>
    [Test]
    public void TripleShotWithExtraProjectile_TargetTwoTilesOut_CoveredByInnerTwoProjectiles()
    {
        var filter = new FrustumBasedTargetFilter(1f, 4.5f, 7f, 4);
        var attacker = CreateLocateable(100, 100);
        var target = CreateLocateable(100, 102);

        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 0), Is.False);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 1), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 2), Is.True);
        Assert.That(filter.IsTargetWithinBounds(attacker, target, 128, 3), Is.False);
    }

    private static ILocateable CreateLocateable(byte x, byte y)
    {
        var mock = new Mock<ILocateable>();
        mock.Setup(l => l.Position).Returns(new Point(x, y));
        return mock.Object;
    }
}
