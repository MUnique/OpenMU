// <copyright file="CrywolfState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The state of the crywolf event, matching the values expected by the client.
/// </summary>
public enum CrywolfState : byte
{
    /// <summary>
    /// The event is not running.
    /// </summary>
    None = 0,

    /// <summary>
    /// The players are notified about the upcoming attack of the army of Balgass.
    /// </summary>
    Notify1 = 1,

    /// <summary>
    /// The fortress prepares for the attack: the common monsters of the map disappear. The client plays the intro of the event.
    /// </summary>
    Notify2 = 2,

    /// <summary>
    /// The army of Balgass appeared and the elves can contract the altars.
    /// </summary>
    Ready = 3,

    /// <summary>
    /// The army of Balgass attacks.
    /// </summary>
    Start = 4,

    /// <summary>
    /// The battle ended and the result is shown.
    /// </summary>
    End = 5,

    /// <summary>
    /// The event is finished: the army disappears and the common monsters of the map return.
    /// </summary>
    EndCycle = 6,
}
