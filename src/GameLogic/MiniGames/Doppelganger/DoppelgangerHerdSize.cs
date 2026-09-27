// <copyright file="DoppelgangerHerdSize.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

/// <summary>
/// The size of the herds of the doppelganger event, starting at a specific game time.
/// </summary>
public class DoppelgangerHerdSize
{
    /// <summary>
    /// Gets or sets the elapsed game time after which this size applies.
    /// </summary>
    public TimeSpan StartsAfter { get; set; }

    /// <summary>
    /// Gets or sets the base count of monsters of a herd.
    /// </summary>
    public int BaseCount { get; set; }
}
