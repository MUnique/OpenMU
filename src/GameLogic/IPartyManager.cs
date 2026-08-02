// <copyright file="IPartyManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Manages party creation and tracks party membership for member reconnection.
/// </summary>
public interface IPartyManager
{
    /// <summary>
    /// Creates a new party with the configured maximum party size.
    /// </summary>
    /// <returns>The newly created party.</returns>
    Party CreateParty();

    /// <summary>
    /// Called when a party member reconnects. Restores the live player into their previous party,
    /// replacing the <see cref="OfflinePartyMember"/> snapshot that was created on disconnect.
    /// </summary>
    /// <param name="member">The reconnected member.</param>
    ValueTask OnMemberReconnectedAsync(IPartyMember member);

    /// <summary>
    /// Registers that a character belongs to a party. Called by <see cref="Party"/> internally
    /// when members are added, replaced, or removed.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    /// <param name="party">The party.</param>
    internal void TrackMembership(string characterName, Party party);

    /// <summary>
    /// Removes the party tracking for a character. Called by <see cref="Party"/> internally
    /// when members leave or are replaced.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    internal void UntrackMembership(string characterName);
}