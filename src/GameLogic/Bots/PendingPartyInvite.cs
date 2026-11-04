// <copyright file="PendingPartyInvite.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// A party invitation from a player which a bot accepted, waiting for the human-like delay to pass
/// before the party is actually formed (see <see cref="BotPartyHandler"/>).
/// </summary>
/// <param name="Requester">The player who invited the bot.</param>
/// <param name="AcceptAtUtc">When the bot answers the invitation.</param>
internal sealed record PendingPartyInvite(Player Requester, DateTime AcceptAtUtc);
