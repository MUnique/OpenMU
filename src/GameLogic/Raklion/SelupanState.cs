// <copyright file="SelupanState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// The state of Selupan, matching the values expected by the client.
/// The patterns depend on the remaining health; the higher the pattern, the stronger Selupan gets.
/// </summary>
public enum SelupanState : byte
{
    /// <summary>
    /// Selupan isn't present.
    /// </summary>
    None = 0,

    /// <summary>
    /// Selupan appeared.
    /// </summary>
    Standby = 1,

    /// <summary>
    /// The first pattern.
    /// </summary>
    Pattern1 = 2,

    /// <summary>
    /// The second pattern.
    /// </summary>
    Pattern2 = 3,

    /// <summary>
    /// The third pattern.
    /// </summary>
    Pattern3 = 4,

    /// <summary>
    /// The fourth pattern.
    /// </summary>
    Pattern4 = 5,

    /// <summary>
    /// The fifth pattern.
    /// </summary>
    Pattern5 = 6,

    /// <summary>
    /// The sixth pattern.
    /// </summary>
    Pattern6 = 7,

    /// <summary>
    /// The seventh pattern.
    /// </summary>
    Pattern7 = 8,

    /// <summary>
    /// Selupan is dead.
    /// </summary>
    Dead = 9,
}
