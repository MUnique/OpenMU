// <copyright file="CrywolfOccupationState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The occupation state of the crywolf fortress, matching the values expected by the client.
/// </summary>
public enum CrywolfOccupationState : byte
{
    /// <summary>
    /// The fortress has been defended.
    /// </summary>
    Peace = 0,

    /// <summary>
    /// The fortress has been occupied by Balgass. The common NPCs of the map are hidden.
    /// </summary>
    Occupied = 1,

    /// <summary>
    /// The fortress is under attack.
    /// </summary>
    War = 2,
}
