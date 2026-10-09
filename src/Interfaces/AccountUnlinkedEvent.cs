// <copyright file="AccountUnlinkedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The link of an account to a user of an external service, e.g. Discord, was removed in the game.
/// </summary>
/// <param name="ServerId">The identifier of the game server.</param>
/// <param name="TimestampUtc">The timestamp, in UTC.</param>
/// <param name="Provider">The name of the external service, e.g. <c>discord</c>.</param>
/// <param name="ExternalUserId">The identifier of the user in the external service.</param>
public sealed record AccountUnlinkedEvent(byte ServerId, DateTime TimestampUtc, string Provider, string ExternalUserId)
    : GameEvent(ServerId, TimestampUtc);
