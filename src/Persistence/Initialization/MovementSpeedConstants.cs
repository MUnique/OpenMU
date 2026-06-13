// <copyright file="MovementSpeedConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

/// <summary>
/// Movement speed constants used by configuration initializers and update plugins.
/// </summary>
internal static class MovementSpeedConstants
{
    internal const int RunningGearMinimumLevel = 5;
    internal const float RunningGearMovementSpeed = 15f;
    internal const float DefaultWingMovementSpeed = 15f;
    internal const float FastWingMovementSpeed = 16f;
    internal const float BasicMountMovementSpeed = 15f;
    internal const float HorseOrFenrirMovementSpeed = 17f;
    internal const float UpgradedFenrirMovementSpeed = 19f;
    internal const float IcedMovementSpeedFactor = 0.5f;
    internal const float ColdMovementSpeedFactor = 0.33f;
}
