// <copyright file="GameEventWebhookMessage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The body of a request of the <see cref="GameEventWebhookSender"/>.
/// </summary>
/// <param name="Id">The identifier of the delivery, which stays the same for its retries.</param>
/// <param name="Event">The game event, serialized with its type in <c>$type</c>.</param>
public sealed record GameEventWebhookMessage(Guid Id, GameEvent Event);
