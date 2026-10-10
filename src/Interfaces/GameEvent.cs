// <copyright file="GameEvent.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Text.Json.Serialization;

/// <summary>
/// The base class of an event of the game which is published to the whole server,
/// e.g. to notify external systems like Discord.
/// </summary>
/// <remarks>
/// The events are published with <see cref="IEventPublisher.GameEventAsync"/>.
/// They are serialized as JSON when they are published between processes, so they only contain
/// simple values and no references to game objects. The type of the event is part of the JSON.
/// </remarks>
/// <param name="ServerId">The identifier of the game server on which the event happened.</param>
/// <param name="TimestampUtc">The timestamp, in UTC, when the event happened.</param>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MiniGameEntranceOpenedEvent), nameof(MiniGameEntranceOpenedEvent))]
[JsonDerivedType(typeof(MiniGameStartedEvent), nameof(MiniGameStartedEvent))]
[JsonDerivedType(typeof(MiniGameEndedEvent), nameof(MiniGameEndedEvent))]
[JsonDerivedType(typeof(InvasionStartedEvent), nameof(InvasionStartedEvent))]
[JsonDerivedType(typeof(InvasionEndedEvent), nameof(InvasionEndedEvent))]
[JsonDerivedType(typeof(CastleSiegeStateChangedEvent), nameof(CastleSiegeStateChangedEvent))]
[JsonDerivedType(typeof(MonsterItemDroppedEvent), nameof(MonsterItemDroppedEvent))]
[JsonDerivedType(typeof(GlobalNoticeEvent), nameof(GlobalNoticeEvent))]
[JsonDerivedType(typeof(BossKilledEvent), nameof(BossKilledEvent))]
[JsonDerivedType(typeof(CharacterLevelMilestoneEvent), nameof(CharacterLevelMilestoneEvent))]
public abstract record GameEvent(byte ServerId, DateTime TimestampUtc);
