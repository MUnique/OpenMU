// <copyright file="OfflineAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Keeps the displayed data of an active offline session.
/// </summary>
/// <param name="LoginName">The account login name.</param>
/// <param name="ServerId">The server identifier.</param>
/// <param name="StartedAt">The start timestamp of the offline session.</param>
/// <param name="CharacterName">The character which keeps leveling.</param>
/// <param name="PartyMaster">The character name of the party master, if the player is in a party.</param>
/// <param name="PartySize">The number of party members, if the player is in a party.</param>
public record OfflineAccount(string LoginName, byte ServerId, DateTime StartedAt, string? CharacterName = null, string? PartyMaster = null, int PartySize = 0) : IPartyGroupedAccount;
