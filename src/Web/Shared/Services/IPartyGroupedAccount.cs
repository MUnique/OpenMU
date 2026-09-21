// <copyright file="IPartyGroupedAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// The data shown in one row of the online-accounts tables which can be grouped by party.
/// </summary>
public interface IPartyGroupedAccount
{
    /// <summary>
    /// Gets the account login name.
    /// </summary>
    string LoginName { get; }

    /// <summary>
    /// Gets the selected character, if it could be resolved.
    /// </summary>
    string? CharacterName { get; }

    /// <summary>
    /// Gets the character name of the party master, if the player is in a party.
    /// </summary>
    string? PartyMaster { get; }
}
