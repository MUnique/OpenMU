// <copyright file="CastleSiegeJoinSide.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines the side (defending or attacking) a guild or NPC belongs to in the castle siege.
/// </summary>
public enum CastleSiegeJoinSide : byte
{
    /// <summary>
    /// No side assigned.
    /// </summary>
    None = 0,

    /// <summary>
    /// The defending guild side.
    /// </summary>
    Defense = 1,

    /// <summary>
    /// The first attacking alliance slot.
    /// </summary>
    Attack1 = 2,

    /// <summary>
    /// The second attacking alliance slot.
    /// </summary>
    Attack2 = 3,

    /// <summary>
    /// The third attacking alliance slot.
    /// </summary>
    Attack3 = 4,
}
