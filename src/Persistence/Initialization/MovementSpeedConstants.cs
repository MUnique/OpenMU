// <copyright file="MovementSpeedConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// Movement speed constants used by configuration initializers and update plugins.
/// </summary>
internal static class MovementSpeedConstants
{
    /// <summary>
    /// Gets the minimum level to use running gear.
    /// </summary>
    internal const int RunningGearMinimumLevel = 5;

    /// <summary>
    /// Gets the movement speed of running gear.
    /// </summary>
    internal const float RunningGearMovementSpeed = 15f;

    /// <summary>
    /// Gets the default movement speed of wings.
    /// </summary>
    internal const float DefaultWingMovementSpeed = 15f;

    /// <summary>
    /// Gets the movement speed of fast wings.
    /// </summary>
    internal const float FastWingMovementSpeed = 16f;

    /// <summary>
    /// Gets the basic movement speed of mounts.
    /// </summary>
    internal const float BasicMountMovementSpeed = 15f;

    /// <summary>
    /// Gets the movement speed of horse or fenrir.
    /// </summary>
    internal const float HorseOrFenrirMovementSpeed = 17f;

    /// <summary>
    /// Gets the movement speed of upgraded fenrir.
    /// </summary>
    internal const float UpgradedFenrirMovementSpeed = 19f;

    /// <summary>
    /// Gets the movement speed factor when iced.
    /// </summary>
    internal const float IcedMovementSpeedFactor = 0.5f;

    /// <summary>
    /// Gets the movement speed factor when slowed by cold.
    /// </summary>
    internal const float ColdMovementSpeedFactor = 0.33f;
}
