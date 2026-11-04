// <copyright file="AccountLoginBlockedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Somebody tried to log into an account with the correct password, while the account was already logged in.
/// </summary>
/// <remarks>
/// Logins with a wrong password don't cause it; they don't reach the point where the account is known to be logged in.
/// </remarks>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="TimestampUtc">The timestamp, in UTC.</param>
/// <param name="LoginName">The login name of the account.</param>
public sealed record AccountLoginBlockedEvent(byte ServerId, DateTime TimestampUtc, string LoginName)
    : GameEvent(ServerId, TimestampUtc);
