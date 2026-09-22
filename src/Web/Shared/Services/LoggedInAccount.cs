// <copyright file="LoggedInAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Keeps the displayed data of a logged-in account.
/// </summary>
/// <param name="LoginName">The account login name.</param>
/// <param name="Server">The server identifier.</param>
/// <param name="CharacterName">The selected character, if it could be resolved (only available in the all-in-one deployment).</param>
/// <param name="PartyMaster">The character name of the party master, if the player is in a party.</param>
/// <param name="PartySize">The number of party members, if the player is in a party.</param>
public record LoggedInAccount(string LoginName, byte Server, string? CharacterName = null, string? PartyMaster = null, int PartySize = 0) : IPartyGroupedAccount;