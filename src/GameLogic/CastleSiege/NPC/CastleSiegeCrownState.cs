// <copyright file="CastleSiegeCrownState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Defines the state of the Castle Siege Crown.
/// </summary>
public enum CastleSiegeCrownState : byte
{
    /// <summary>
    /// The Crown is idle.
    /// </summary>
    Idle,

    /// <summary>
    /// The Crown is locked.
    /// </summary>
    Locked,

    /// <summary>
    /// The Crown has been captured.
    /// </summary>
    Captured,
}
