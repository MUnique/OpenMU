// <copyright file="MonsterItemDroppedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A killed monster dropped a notable item, e.g. an excellent or ancient item.
/// </summary>
/// <remarks>
/// Names of configuration objects, like maps or monsters, are passed as the full value of a <see cref="LocalizedString"/>,
/// so that a receiver can translate them with <c>new LocalizedString(name).GetTranslation(culture)</c>.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="KillerName">The character name of the player who killed the monster.</param>
/// <param name="MonsterName">The name of the monster.</param>
/// <param name="MapName">The name of the map.</param>
/// <param name="ItemName">The name of the item.</param>
/// <param name="ItemLevel">The level of the item.</param>
/// <param name="IsExcellent">A value indicating whether the item is excellent.</param>
/// <param name="IsAncient">A value indicating whether the item is ancient.</param>
public sealed record MonsterItemDroppedEvent(byte ServerId, DateTime TimestampUtc, string KillerName, string MonsterName, string MapName, string ItemName, byte ItemLevel, bool IsExcellent, bool IsAncient)
    : GameEvent(ServerId, TimestampUtc);
