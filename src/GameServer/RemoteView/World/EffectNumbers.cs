// <copyright file="EffectNumbers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

/// <summary>
/// Skill effect flags which were used in earlier versions, like 0.75.
/// </summary>
internal static class EffectNumbers
{
    /// <summary>
    /// The undefined effect. No effect.
    /// </summary>
    public const int Undefined = 0;

    /// <summary>
    /// The object has a damage buff.
    /// </summary>
    public const int DamageBuff = 0x01;

    /// <summary>
    /// The object has a defense buff.
    /// </summary>
    public const int DefenseBuff = 0x02;

    /// <summary>
    /// The object is poisoned.
    /// </summary>
    public const int Poisoned = 0x37;

    /// <summary>
    /// The object is iced.
    /// </summary>
    public const int Iced = 0x38;

    /// <summary>
    /// Shows the health bar for duel spectators.
    /// </summary>
    public const int DuelSpectatorHealthBar = 0x62;
}