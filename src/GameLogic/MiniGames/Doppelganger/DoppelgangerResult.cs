// <copyright file="DoppelgangerResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <summary>
/// The result of the doppelganger event for a player, matching the values expected by the client.
/// </summary>
public enum DoppelgangerResult : byte
{
    /// <summary>
    /// The party successfully defended the magic circle.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The player failed, e.g. because the character died or left the event map.
    /// </summary>
    Failed = 1,

    /// <summary>
    /// The defense failed, because too many monsters reached the magic circle.
    /// </summary>
    MonstersReachedMagicCircle = 2,
}
