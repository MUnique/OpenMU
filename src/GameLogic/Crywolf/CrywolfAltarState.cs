// <copyright file="CrywolfAltarState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The state of an altar of the crywolf event, matching the values expected by the client.
/// </summary>
public enum CrywolfAltarState : byte
{
    /// <summary>
    /// The altar can be contracted.
    /// </summary>
    Free = 0,

    /// <summary>
    /// The altar is contracted by an elf, which protects the statue.
    /// </summary>
    Contracted = 1,

    /// <summary>
    /// An elf is trying to contract the altar.
    /// </summary>
    Attempting = 2,

    /// <summary>
    /// The altar can't be contracted anymore, because all of its contracts were used.
    /// </summary>
    Exhausted = 3,
}
