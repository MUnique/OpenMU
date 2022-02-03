using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.GuildServer;

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

    void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status);
}