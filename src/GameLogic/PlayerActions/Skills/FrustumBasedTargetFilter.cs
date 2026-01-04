// <copyright file="FrustumBasedTargetFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Numerics;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A target filter which will be executed when an area skill is about to hit its targets.
/// It allows to filter out targets which are out of range.
/// </summary>
public record FrustumBasedTargetFilter
{
    private const double DistanceEpsilon = 0.001;
    private readonly Vector2[][] _rotationVectors;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrustumBasedTargetFilter"/> class.
    /// </summary>
    /// <param name="startWidth">The width of the frustum at the start.</param>
    /// <param name="endWidth">The width of the frustum at the end.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="projectileCount">The number of projectiles. Default is 1.</param>
    public FrustumBasedTargetFilter(float startWidth, float endWidth, float distance, int projectileCount = 1)
    {
        this.EndWidth = endWidth;
        this.Distance = distance;
        this.StartWidth = startWidth;
        this.ProjectileCount = projectileCount;
        this._rotationVectors = this.CalculateRotationVectors();
    }

    /// <summary>
    /// Gets the end width.
    /// </summary>
    public float EndWidth { get; }

    /// <summary>
    /// Gets the distance.
    /// </summary>
    public float Distance { get; }

    /// <summary>
    /// Gets the start width.
    /// </summary>
    public float StartWidth { get; }

    /// <summary>
    /// Gets the number of projectiles/arrows.
    /// </summary>
    public int ProjectileCount { get; }

    /// <summary>
    /// Determines whether the target is within the hit bounds.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="target">The target.</param>
    /// <param name="rotation">The rotation.</param>
    /// <returns><c>true</c> if the target is within hit bounds; otherwise, <c>false</c>.</returns>
    public bool IsTargetWithinBounds(ILocateable attacker, ILocateable target, byte rotation)
    {
        if (attacker.Position == target.Position)
        {
            return true;
        }

        var frustum = this.GetFrustum(attacker.Position, rotation);
        return IsWithinFrustum(frustum, target.Position);
    }

    /// <summary>
    /// Determines whether the target is within the hit bounds for a specific projectile.
    /// When multiple projectiles are used, they are evenly distributed within the frustum.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="target">The target.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="projectileIndex">The zero-based index of the projectile (0 to ProjectileCount-1).</param>
    /// <returns><c>true</c> if the target is within hit bounds for the specified projectile; otherwise, <c>false</c>.</returns>
    public bool IsTargetWithinBounds(ILocateable attacker, ILocateable target, byte rotation, int projectileIndex)
    {
        if (this.ProjectileCount <= 1)
        {
            // For single projectile, use the simple frustum check
            return this.IsTargetWithinBounds(attacker, target, rotation);
        }

        if (projectileIndex < 0 || projectileIndex >= this.ProjectileCount)
        {
            return false;
        }

        // First check if target is within the overall frustum
        if (!this.IsTargetWithinBounds(attacker, target, rotation))
        {
            return false;
        }

        // Calculate the relative position of the target within the frustum
        var relativePosition = this.CalculateRelativePositionInFrustum(attacker.Position, target.Position, rotation);

        // Divide the frustum into sections (-1 to 1 range)
        // For 3 projectiles: left (-1 to -0.33), center (-0.33 to 0.33), right (0.33 to 1)
        var sectionWidth = 2.0 / this.ProjectileCount;
        var sectionStart = -1.0 + (projectileIndex * sectionWidth);
        var sectionEnd = sectionStart + sectionWidth;

        // Add overlap so targets near boundaries can be hit by adjacent projectiles
        var overlap = this.GetOverlap(attacker.Position, target.Position);
        sectionStart -= overlap;
        sectionEnd += overlap;

        return relativePosition >= sectionStart && relativePosition <= sectionEnd;
    }

    private double GetOverlap(Point attackerPos, Point targetPos)
    {
        var distance = attackerPos.EuclideanDistanceTo(targetPos);
        if (distance == 0)
        {
            return 1;
        }

        // The overlap decreases over higher distance
        return (1.0 / Math.Floor(distance)) / this.ProjectileCount;
    }

    private double CalculateRelativePositionInFrustum(Point attackerPos, Point targetPos, byte rotation)
    {
        // Calculate the vector from attacker to target
        var dx = targetPos.X - attackerPos.X;
        var dy = targetPos.Y - attackerPos.Y;

        // Calculate the rotation angle (same logic as in CalculateRotationVectors)
        var rotationAngle = (rotation * 360.0 / 256.0) + 180; // Add 180 offset as in the frustum calculation
        var rotationRad = rotationAngle * Math.PI / 180.0;

        // Rotate the vector to align with the frustum's coordinate system
        // The frustum has Y pointing forward and X pointing right
        var cos = Math.Cos(-rotationRad);
        var sin = Math.Sin(-rotationRad);
        var rotatedX = dx * cos - dy * sin;
        var rotatedY = dx * sin + dy * cos;

        // If target is behind us or too close, return 0
        if (rotatedY <= 0)
        {
            return 0;
        }

        // Calculate the frustum width at the target's distance
        // Linear interpolation between start and end width
        var distanceRatio = Math.Min(rotatedY / this.Distance, 1.0);
        var frustumWidthAtDistance = this.StartWidth + (this.EndWidth - this.StartWidth) * distanceRatio;

        // Normalize the X position by the frustum width at that distance
        // Result will be in range [-1, 1] where -1 is left edge, 0 is center, 1 is right edge
        if (Math.Abs(frustumWidthAtDistance) < DistanceEpsilon)
        {
            return 0;
        }

        var normalizedX = rotatedX / frustumWidthAtDistance;
        return Math.Clamp(normalizedX, -1.0, 1.0);
    }

    private static bool IsWithinFrustum((Vector4 X, Vector4 Y) frustum, Point target)
    {
        var isOutOfRange = (((frustum.X.X - target.X) * (frustum.Y.W - target.Y)) - ((frustum.X.W - target.X) * (frustum.Y.X - target.Y))) < 0.0f
                           || (((frustum.X.Y - target.X) * (frustum.Y.X - target.Y)) - ((frustum.X.X - target.X) * (frustum.Y.Y - target.Y))) < 0.0f
                           || (((frustum.X.Z - target.X) * (frustum.Y.Y - target.Y)) - ((frustum.X.Y - target.X) * (frustum.Y.Z - target.Y))) < 0.0f
                           || (((frustum.X.W - target.X) * (frustum.Y.Z - target.Y)) - ((frustum.X.Z - target.X) * (frustum.Y.W - target.Y))) < 0.0f;

        return !isOutOfRange;
    }

    private static Vector2 VectorRotate(Vector3 angleVector, Matrix4x4 angleMatrix)
    {
        return new Vector2(
            Vector3.Dot(angleVector, new Vector3(angleMatrix.M11, angleMatrix.M12, angleMatrix.M13)),
            Vector3.Dot(angleVector, new Vector3(angleMatrix.M21, angleMatrix.M22, angleMatrix.M23)));
    }

    private static Matrix4x4 CreateAngleMatrix(double angleInDegrees)
    {
        var radian = angleInDegrees * (Math.PI / 180);

        var sy = (float)Math.Sin(radian);
        var cy = (float)Math.Cos(radian);

        var sp = 0.0f;
        var cp = 1.0f;

        var sr = 0.0f;
        var cr = 1.0f;

        Matrix4x4 matrix = default;

        matrix.M11 = cp * cy;
        matrix.M21 = cp * sy;
        matrix.M31 = -sp;
        matrix.M12 = (sr * sp * cy) + (cr * -sy);
        matrix.M22 = (sr * sp * sy) + (cr * cy);
        matrix.M32 = sr * cp;
        matrix.M13 = (cr * sp * cy) + (-sr * -sy);
        matrix.M23 = (cr * sp * sy) + (-sr * cy);
        matrix.M33 = cr * cp;

        return matrix;
    }

    private Vector2[][] CalculateRotationVectors()
    {
        const int degreeOffset = 180;
        const float distanceOffset = 0.99f; // we always start in front of the characters coordinates.

        var result = new Vector2[byte.MaxValue + 1][];

        var temp = new Vector3[4];
        temp[0] = new Vector3(-this.EndWidth, this.Distance, 0);
        temp[1] = new Vector3(this.EndWidth, this.Distance, 0);
        temp[2] = new Vector3(this.StartWidth, distanceOffset, 0);
        temp[3] = new Vector3(-this.StartWidth, distanceOffset, 0);

        for (int rotation = 0; rotation <= byte.MaxValue; rotation++)
        {
            var degrees = (rotation * 360.0) / byte.MaxValue;
            degrees = (degrees + degreeOffset) % 360;
            var angleMatrix = CreateAngleMatrix(degrees);
            var vectorsOfAngle = new Vector2[4];

            for (int i = 0; i < temp.Length; i++)
            {
                vectorsOfAngle[i] = VectorRotate(temp[i], angleMatrix);
            }

            result[rotation] = vectorsOfAngle;
        }

        return result;
    }

    private (Vector4 X, Vector4 Y) GetFrustum(Point attackerPosition, byte rotation)
    {
        var rotationVectors = this._rotationVectors[rotation];

        Vector4 resultX = default;
        Vector4 resultY = default;
        resultX.X = (int)rotationVectors[0].X + attackerPosition.X;
        resultY.X = (int)rotationVectors[0].Y + attackerPosition.Y;

        resultX.Y = (int)rotationVectors[1].X + attackerPosition.X;
        resultY.Y = (int)rotationVectors[1].Y + attackerPosition.Y;

        resultX.Z = (int)rotationVectors[2].X + attackerPosition.X;
        resultY.Z = (int)rotationVectors[2].Y + attackerPosition.Y;

        resultX.W = (int)rotationVectors[3].X + attackerPosition.X;
        resultY.W = (int)rotationVectors[3].Y + attackerPosition.Y;

        return (resultX, resultY);
    }
}