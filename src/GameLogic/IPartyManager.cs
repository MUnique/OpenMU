// <copyright file="IPartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Manages party creation and tracks character-to-party membership.
/// </summary>
public interface IPartyManager
{
    /// <summary>
    /// Creates a new party using the configured maximum party size.
    /// </summary>
    /// <returns>The newly created party.</returns>
    Party CreateParty();

    /// <summary>
    /// Tracks that a character belongs to a party.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="party">The party.</param>
    void TrackPartyMembership(string characterName, Party party);

    /// <summary>
    /// Removes the party membership tracking for a character.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    void RemovePartyMembership(string characterName);

    /// <summary>
    /// Called when a party member disconnects. Replaces the live member with an
    /// <see cref="OfflinePartyMember"/> snapshot so the party stays intact.
    /// </summary>
    /// <param name="member">The member who disconnected.</param>
    ValueTask OnMemberDisconnectedAsync(IPartyMember member);

    /// <summary>
    /// Called when a party member reconnects. Replaces the old member reference in the party with the new player.
    /// </summary>
    /// <param name="member">The member who reconnected.</param>
    ValueTask OnMemberReconnectedAsync(IPartyMember member);
}