namespace MUnique.OpenMU.GuildServer;

using MUnique.OpenMU.Interfaces;

public class GuildChangeToGameServerPublisher : IGuildChangePublisher
{
    private readonly IDictionary<int, IGameServer> _gameServers;

    public GuildChangeToGameServerPublisher(IDictionary<int, IGameServer> gameServers)
    {
        this._gameServers = gameServers;
    }

    public void GuildPlayerKicked(string playerName)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            gameServer.GuildPlayerKicked(playerName);
        }
    }

    public void GuildDeleted(uint guildId)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            gameServer.GuildDeleted(guildId);
        }
    }

    public void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            gameServer.AssignGuildToPlayer(characterName, status);
        }
    }
}