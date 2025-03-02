// <copyright file="IEventPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for a publisher of events which happen on the game server
/// and might have multiple interested subscribers/services.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Notifies that a player entered the game with a character.
    /// </summary>
    /// <param name="serverId">The identifier of the server on which the player entered.</param>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName);

    /// <summary>
    /// Notifies that a player entered the game with a character.
    /// </summary>
    /// <param name="serverId">The identifier of the server on which the player entered.</param>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="guildId">If the character is in a guild, a guild id can be passed.</param>
    ValueTask PlayerLeftGameAsync(byte serverId, Guid characterId, string characterName, uint guildId = 0);

    /// <summary>
    /// Notifies the guild server that a guild message was sent and maybe needs to be forwarded to the game servers.
    /// </summary>
    /// <param name="guildId">The guild id.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="message">The message.</param>
    ValueTask GuildMessageAsync(uint guildId, string sender, string message);

    /// <summary>
    /// Notifies the guild server that an alliance message was sent and maybe needs to be forwarded to the game servers.
    /// </summary>
    /// <param name="guildId">The guild id.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="message">The message.</param>
    ValueTask AllianceMessageAsync(uint guildId, string sender, string message);

    /// <summary>
    /// Notifies that a client tried to log into an already logged-in account.
    /// The connected player can be notified about that.
    /// </summary>
    /// <param name="serverId">The identifier of the server on which the client tried to enter.</param>
    /// <param name="loginName">The login name.</param>
    ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName);
}