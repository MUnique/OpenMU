// <copyright file="IPartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Manages party creation, cache persistence, and offline-member lifecycle.
/// </summary>
public interface IPartyManager
{
    /// <summary>
    /// Creates a new party using the configured maximum party size.
    /// </summary>
    /// <returns>The newly created party.</returns>
    Party CreateParty();

    /// <summary>
    /// Persists the party for the specified character identifier.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="party">The party to persist.</param>
    void SaveParty(Guid characterId, Party party);

    /// <summary>
    /// Removes the persisted party for the specified character identifier.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    void RemoveParty(Guid characterId);

    /// <summary>
    /// Called when a party member disconnects. Replaces the live member with an
    /// <see cref="OfflinePartyMember"/> snapshot so the party stays intact.
    /// </summary>
    /// <param name="member">The member who disconnected.</param>
    ValueTask OnMemberDisconnectedAsync(IPartyMember member);

    /// <summary>
    /// Called when a previously offline party member reconnects. Swaps the
    /// <see cref="OfflinePartyMember"/> snapshot back to the live player.
    /// </summary>
    /// <param name="member">The member who reconnected.</param>
    ValueTask OnMemberReconnectedAsync(IPartyMember member);
}