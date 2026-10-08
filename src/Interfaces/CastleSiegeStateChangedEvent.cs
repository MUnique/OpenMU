// <copyright file="CastleSiegeStateChangedEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The state of the castle siege changed, e.g. the battle started.
/// </summary>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
/// <param name="PreviousState">The name of the previous state, e.g. <c>Ready</c>.</param>
/// <param name="State">The name of the new state, e.g. <c>Start</c>.</param>
/// <param name="StateEndsAtUtc">The time, in UTC, when the new state ends.</param>
/// <param name="OwnerGuildId">
/// The persistent identifier of the guild which owns the castle, if any. After the battle, it's the winner.
/// The name can be retrieved with <see cref="IGuildServer.GetPersistentGuildNameAsync"/>.
/// </param>
public sealed record CastleSiegeStateChangedEvent(byte ServerId, DateTime TimestampUtc, string PreviousState, string State, DateTime StateEndsAtUtc, Guid? OwnerGuildId)
    : GameEvent(ServerId, TimestampUtc);
