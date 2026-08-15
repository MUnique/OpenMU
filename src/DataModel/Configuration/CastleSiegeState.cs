// <copyright file="CastleSiegeState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The state of the castle siege event cycle.
/// </summary>
public enum CastleSiegeState : byte
{
    /// <summary>
    /// Idle state before guild registration opens.
    /// </summary>
    Idle1 = 0,

    /// <summary>
    /// Guilds may register for the siege.
    /// </summary>
    RegisterGuild = 1,

    /// <summary>
    /// Idle state after guild registration.
    /// </summary>
    Idle2 = 2,

    /// <summary>
    /// Guilds may register emblems (Marks of Lord) to determine the attacking guilds.
    /// </summary>
    RegisterMark = 3,

    /// <summary>
    /// Idle state after mark registration.
    /// </summary>
    Idle3 = 4,

    /// <summary>
    /// Players are notified that the siege is about to start.
    /// </summary>
    Notify = 5,

    /// <summary>
    /// The siege map is prepared and entry is allowed.
    /// </summary>
    Ready = 6,

    /// <summary>
    /// The siege battle is in progress.
    /// </summary>
    Start = 7,

    /// <summary>
    /// The siege battle has ended and results are being processed.
    /// </summary>
    End = 8,

    /// <summary>
    /// The full siege cycle has completed.
    /// </summary>
    EndCycle = 9,
}
