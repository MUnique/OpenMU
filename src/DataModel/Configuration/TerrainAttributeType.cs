// <copyright file="TerrainAttributeType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines the attribute which should be set/unset.
/// </summary>
public enum TerrainAttributeType
{
    /// <summary>
    /// The coordinate is a safezone.
    /// </summary>
    Safezone = 1,

    /// <summary>
    /// The coordinate is occupied by a character.
    /// </summary>
    Character = 2,

    /// <summary>
    /// The coordinate is blocked and can’t be passed by a character.
    /// </summary>
    Blocked = 4,

    /// <summary>
    /// The coordinate is blocked, because there is no ground and can’t be passed by a character.
    /// </summary>
    NoGround = 8,

    /// <summary>
    /// The coordinate is blocked by water and can’t be passed by a character.
    /// </summary>
    Water = 16,
}