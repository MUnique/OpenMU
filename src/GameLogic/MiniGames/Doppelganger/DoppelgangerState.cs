// <copyright file="DoppelgangerState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <summary>
/// The state of the doppelganger event, matching the values expected by the client.
/// </summary>
public enum DoppelgangerState : byte
{
    /// <summary>
    /// The event is waiting for the party members to enter.
    /// </summary>
    Waiting = 0,

    /// <summary>
    /// The entrance is closed and the event is about to start.
    /// </summary>
    Ready = 1,

    /// <summary>
    /// The event is running.
    /// </summary>
    Playing = 2,

    /// <summary>
    /// The event has ended.
    /// </summary>
    Ended = 3,
}
