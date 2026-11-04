// <copyright file="BotPartyOffer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// A bot's answer to a player's party search (see <see cref="BotPartyRequest"/>): the bot whispered
/// that it would come and now waits for the player to whisper back where to meet.
/// </summary>
/// <param name="Requester">The player who searched for party members.</param>
/// <param name="ExpiresUtc">Until when the bot waits for the player's answer.</param>
internal sealed record BotPartyOffer(Player Requester, DateTime ExpiresUtc);
