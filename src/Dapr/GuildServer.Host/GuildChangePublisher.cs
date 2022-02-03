using Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

namespace MUnique.OpenMU.GuildServer.Host;

public class GuildChangePublisher : IGuildChangePublisher
{
    private readonly DaprClient _daprClient;

    public GuildChangePublisher(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public void GuildPlayerKicked(string playerName)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildPlayerKicked), playerName);
    }

    public void GuildDeleted(uint guildId)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildDeleted), guildId);
    }

    public void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status)
    {
        this._daprClient.InvokeMethodAsync($"gameServer{serverId + 1}", nameof(IGameServer.AssignGuildToPlayer), new GuildMemberAssignArguments(characterName, status));
    }
}