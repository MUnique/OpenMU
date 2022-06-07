// <copyright file="IGuildChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Interface for a publisher of guild related events.
/// </summary>
public interface IGuildChangePublisher
{
    /// <summary>
    /// Notifies the game server that a guild member got removed from a guild.
    /// </summary>
    /// <param name="playerName">Name of the player which got removed from a guild.</param>
    void GuildPlayerKicked(string playerName);

    /// <summary>
    /// Notifies the game server that a guild got deleted.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    void GuildDeleted(uint guildId);

    /// <summary>
    /// Assigns a guild to a player.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="status">The status.</param>
    void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status);
}