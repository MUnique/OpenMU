// <copyright file="GensType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The gens (family) of a player.
/// </summary>
/// <remarks>
/// The values are the ones which the game client uses.
/// </remarks>
public enum GensType
{
    /// <summary>
    /// The player is not a member of a gens.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Duprian gens.
    /// </summary>
    Duprian = 1,

    /// <summary>
    /// The Vanert gens.
    /// </summary>
    Vanert = 2,
}
