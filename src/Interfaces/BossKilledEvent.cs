// <copyright file="BossKilledEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A boss monster got killed.
/// </summary>
/// <remarks>
/// Names of configuration objects, like maps or monsters, are passed as the full value of a <see cref="LocalizedString"/>,
/// so that a receiver can translate them with <c>new LocalizedString(name).GetTranslation(culture)</c>.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="KillerName">The character name of the player who killed the boss.</param>
/// <param name="MonsterName">The name of the boss.</param>
/// <param name="MapName">The name of the map.</param>
public sealed record BossKilledEvent(byte ServerId, DateTime TimestampUtc, string KillerName, string MonsterName, string MapName)
    : GameEvent(ServerId, TimestampUtc);
