// <copyright file="SelupanSkill.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Raklion;

/// <summary>
/// The skills of Selupan. The values are the monster skill numbers of the client, which shows their animations.
/// </summary>
public enum SelupanSkill : short
{
    /// <summary>
    /// A poison attack on the players around the target.
    /// </summary>
    Poison = 34,

    /// <summary>
    /// An ice storm on the players around the target.
    /// </summary>
    IceStorm = 35,

    /// <summary>
    /// An ice strike on the players around the target.
    /// </summary>
    IceStrike = 36,

    /// <summary>
    /// Selupan falls from the sky when it appears.
    /// </summary>
    Fall = 37,

    /// <summary>
    /// Selupan summons monsters.
    /// </summary>
    Summon = 38,

    /// <summary>
    /// Selupan heals itself.
    /// </summary>
    Heal = 39,

    /// <summary>
    /// Selupan freezes its target, so that it can't move.
    /// </summary>
    Freeze = 40,

    /// <summary>
    /// Selupan teleports to a position nearby.
    /// </summary>
    Teleport = 41,

    /// <summary>
    /// Selupan gets invincible for a short time.
    /// </summary>
    Invincibility = 42,
}
