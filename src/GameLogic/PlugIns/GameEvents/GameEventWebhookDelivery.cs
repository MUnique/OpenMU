// <copyright file="GameEventWebhookDelivery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A game event which waits to be posted to the webhook.
/// </summary>
/// <param name="Url">The URL of the webhook.</param>
/// <param name="Secret">The shared secret with which the request is signed.</param>
/// <param name="Id">The identifier of the delivery, which stays the same for its retries, so the receiver can drop duplicates.</param>
/// <param name="Event">The game event.</param>
public sealed record GameEventWebhookDelivery(Uri Url, string Secret, Guid Id, GameEvent Event);
