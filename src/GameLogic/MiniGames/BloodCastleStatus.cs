// <copyright file="BloodCastleStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// The status of a blood castle event.
/// </summary>
public enum BloodCastleStatus
{
    /// <summary>
    /// The event has just started and is running.
    /// </summary>
    Started,

    /// <summary>
    /// The event is running, but the gate is not destroyed.
    /// </summary>
    GateNotDestroyed,

    /// <summary>
    /// The event is running and the gate is destroyed.
    /// </summary>
    GateDestroyed,

    /// <summary>
    /// The event has ended.
    /// </summary>
    Ended,
}
