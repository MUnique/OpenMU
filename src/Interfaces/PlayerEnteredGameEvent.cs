// <copyright file="PlayerEnteredGameEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// A character entered the game.
/// </summary>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="TimestampUtc">The timestamp, in UTC.</param>
/// <param name="CharacterId">The identifier of the character.</param>
/// <param name="CharacterName">The name of the character.</param>
public sealed record PlayerEnteredGameEvent(byte ServerId, DateTime TimestampUtc, Guid CharacterId, string CharacterName)
    : GameEvent(ServerId, TimestampUtc);
