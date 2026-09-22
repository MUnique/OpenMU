// <copyright file="BotAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Keeps the displayed data of an active server-side bot.
/// </summary>
/// <param name="LoginName">The account login name the bot is driving.</param>
/// <param name="ServerId">The server identifier.</param>
/// <param name="CharacterName">The character which the bot is driving.</param>
/// <param name="StartedAt">The start timestamp of the bot session.</param>
/// <param name="PartyMaster">The character name of the party master, if the bot is in a party.</param>
/// <param name="PartySize">The number of party members, if the bot is in a party.</param>
public record BotAccount(string LoginName, byte ServerId, string? CharacterName, DateTime StartedAt, string? PartyMaster = null, int PartySize = 0) : IPartyGroupedAccount;
