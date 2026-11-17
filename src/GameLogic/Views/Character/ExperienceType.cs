// <copyright file="ExperienceType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Defines the type of experience which is added.
/// </summary>
public enum ExperienceType : byte
{
    /// <summary>
    /// The type is not defined.
    /// </summary>
    Undefined,

    /// <summary>
    /// The normal experience is added.
    /// </summary>
    Normal,

    /// <summary>
    /// The master experience is added.
    /// </summary>
    Master,

    /// <summary>
    /// The maximum level has been reached, no experience is added.
    /// </summary>
    MaxLevelReached,

    /// <summary>
    /// The maximum master level has been reached, no master experience is added.
    /// </summary>
    MaxMasterLevelReached,

    /// <summary>
    /// The monster level is too low for master experience, no master experience is added.
    /// </summary>
    MonsterLevelTooLowForMasterExperience,
}
