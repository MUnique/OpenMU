// <copyright file="RaklionState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// The state of the raklion event, matching the values expected by the client.
/// </summary>
public enum RaklionState : byte
{
    /// <summary>
    /// The spider eggs are alive and the hatchery gate is open.
    /// </summary>
    Idle = 0,

    /// <summary>
    /// Only a few spider eggs are left.
    /// </summary>
    Notify1 = 1,

    /// <summary>
    /// All spider eggs are destroyed and Selupan is about to appear.
    /// </summary>
    Standby = 2,

    /// <summary>
    /// Selupan appeared, the hatchery gate closes soon.
    /// </summary>
    Notify2 = 3,

    /// <summary>
    /// Selupan rises.
    /// </summary>
    Ready = 4,

    /// <summary>
    /// The battle against Selupan is running and players can still enter the hatchery.
    /// </summary>
    StartBattle = 5,

    /// <summary>
    /// The hatchery gate is about to close.
    /// </summary>
    Notify3 = 6,

    /// <summary>
    /// The hatchery gate is closed, nobody can enter the hatchery anymore.
    /// </summary>
    CloseDoor = 7,

    /// <summary>
    /// All players of the battle died or left.
    /// </summary>
    AllUserDie = 8,

    /// <summary>
    /// The battle ended, the hatchery gate opens soon.
    /// </summary>
    Notify4 = 9,

    /// <summary>
    /// The event ended, the spider eggs appear again.
    /// </summary>
    End = 10,
}
