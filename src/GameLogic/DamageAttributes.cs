// <copyright file="DamageAttributes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The attributes of a damage.
/// </summary>
[Flags]
public enum DamageAttributes
{
    /// <summary>
    /// No defined attribute.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The damage is critical (means 100% of the possible damage between minimum and maximum).
    /// </summary>
    Critical = 1,

    /// <summary>
    /// The damage is excellent (20% higher base damage than critical).
    /// </summary>
    Excellent = 2,

    /// <summary>
    /// The damage ignored the defense of the victim.
    /// </summary>
    IgnoreDefense = 4,

    /// <summary>
    /// The damage is caused by poison.
    /// </summary>
    Poison = 8,

    /// <summary>
    /// The damage was doubled.
    /// </summary>
    Double = 16,

    /// <summary>
    /// The damage was tripled (e.g. combo skill).
    /// </summary>
    Triple = 32,

    /// <summary>
    /// The damage was reflected.
    /// </summary>
    Reflected = 64,

    /// <summary>
    /// The damage includes the combo bonus.
    /// </summary>
    Combo = 128,

    /// <summary>
    /// The damage is a non-final hit of a quick sequence from a rage fighter skill.
    /// </summary>
    RageFighterStreakHit = 256,

    /// <summary>
    /// The damage is the final hit of a quick sequence from a rage fighter skill.
    /// </summary>
    RageFighterStreakFinalHit = 512,
}